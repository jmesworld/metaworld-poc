var GeolocationAR = pc.createScript('geolocationAR');

GeolocationAR.attributes.add("debugText", {
    type: 'entity'
});

GeolocationAR.attributes.add('startEvent', {
    title: 'Start Event Name',
    description: 'Event to listen for.',
    type: 'string'
});

// initialize code called once per entity
GeolocationAR.prototype.initialize = function () {
    this.app.on(this.startEvent, () => {
        let options = {
            enableHighAccuracy: true,
            maximumAge: 0,
            timeout: 27000
        };
        navigator.geolocation.watchPosition((position) => {
            var text = "latitude: " + position.coords.latitude +
                "\n" + "longitude: " + position.coords.longitude +
                "\n" + "altitude: " + position.coords.altitude +
                "\n" + "accuracy: " + position.coords.accuracy;
            this.debugText.element.text = text;
            this.originCoords = position.coords;
            console.log(text);
        }, (e) => console.log(e), options);

        var constraints = {
            audio: false,
            video: {
                // Prefer the rear camera
                facingMode: "environment",
                advanced: [{
                    zoom: 2.0,
                    focusMode: "continuous"
                }]
            }
        };
        var self = this;
        navigator.mediaDevices.getUserMedia(constraints).then(function (stream) {
            self.videoPlaying = false;

            // Create the video element to receive the camera stream
            var video = document.createElement('video');

            video.setAttribute('autoplay', '');
            video.setAttribute('muted', '');
            // This is critical for iOS or the video initially goes fullscreen
            video.setAttribute('playsinline', '');
            video.srcObject = stream;

            self.video = video;

            // Check for both video and canvas resizing
            // Changing screen orientation on mobile can change both!
            self.app.graphicsDevice.on('resizecanvas', function () {
                //self.onResize();
            });
            video.addEventListener('resize', function () {
                //self.onResize();
            });

            // Only play the video when it's actually ready
            video.addEventListener('canplay', function () {
                if (!self.videoPlaying) {
                    console.log("Starting Video...");
                    self.startVideo();
                    self.videoPlaying = true;
                }
            });

            // iOS needs a user action to start the video
            if (pc.platform.mobile) {
                window.addEventListener('touchstart', function (e) {
                    e.preventDefault();
                    if (!self.videoPlaying) {
                        self.startVideo();
                        self.videoPlaying = true;
                    }
                });
            }
        }).catch(function (e) {
            if (error) error("ERROR: Unable to acquire camera stream");
        });
    }, this);


};

// update code called every frame
GeolocationAR.prototype.update = function (dt) {

};

GeolocationAR.prototype.startVideo = function () {
    // Create a video element that is full tab and centered
    // CCS taken from: https://slicejack.com/fullscreen-html5-video-background-css/
    var style = this.video.style;
    style.position = 'absolute';
    style.top = '50%';
    style.left = '50%';
    style.width = 'auto';
    style.height = 'auto';
    style.minWidth = '100%';
    style.minHeight = '100%';
    style.backgroundSize = 'cover';
    style.overflow = 'hidden';
    style.transform = 'translate(-50%, -50%)';
    style.zIndex = '0';
    document.body.appendChild(this.video);

    // Z-order for page is:
    //   0: Video DOM element
    //   1: PlayCanvas canvas element
    //   2: ARToolkit debug canvas
    this.app.graphicsDevice.canvas.style.zIndex = '1';

};
GeolocationAR.prototype.updatePosition = function (coords) {
    if (this.originCoords === undefined)
        return;

    var position = new pc.Vec3(0, 0, 0);

    // update position.x
    var dstCoords = {
        longitude: coords.longitude,
        latitude: this.originCoords.latitude,
    };

    position.x = this.computeDistanceMeters(this.originCoords, dstCoords, true);
    this._positionXDebug = position.x;
    position.x *= coords.longitude > this.originCoords.longitude ? 1 : -1;

    // update position.z
    dstCoords = {
        longitude: this.originCoords.longitude,
        latitude: coords.latitude,
    };

    position.z = this.computeDistanceMeters(this.originCoords, dstCoords, true);
    position.z *= coords.latitude > this.originCoords.latitude ? -1 : 1;

    // return position in 3D world
    return position;
};

GeolocationAR.prototype.computeDistanceMeters = function (src, dest) {
    var dlongitude = pc.math.DEG_TO_RAD * (dest.longitude - src.longitude);
    var dlatitude = pc.math.DEG_TO_RAD * (dest.latitude - src.latitude);
    var a = (Math.sin(dlatitude / 2) * Math.sin(dlatitude / 2)) + Math.cos(pc.math.DEG_TO_RAD * (src.latitude)) * Math.cos(pc.math.DEG_TO_RAD * (dest.latitude)) * (Math.sin(dlongitude / 2) * Math.sin(dlongitude / 2));
    var angle = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var distance = angle * 6378160;

    return distance;
};

// swap method called for script hot-reloading
// inherit your script state here
// GeolocationAR.prototype.swap = function(old) { };

// to learn more about script anatomy, please read:
// https://developer.playcanvas.com/en/user-manual/scripting/