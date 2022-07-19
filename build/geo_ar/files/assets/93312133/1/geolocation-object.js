var GeolocationObject = pc.createScript('geolocationObject');

GeolocationObject.attributes.add('geoCam', {
    type: "entity"
});

GeolocationObject.attributes.add('coordinates', {
    type: "json",
    schema: [{
        name: 'latitude',
        type: 'number',
        default: 50,
        precision: 15

    }, {
        name: 'longitude',
        type: 'number',
        default: 13,
        precision: 15
    }]
});

// initialize code called once per entity
GeolocationObject.prototype.initialize = function () {
    this.first = true;

    var lat = getURLParameter("lat");
    var lon = getURLParameter("lon");
    var rot = getURLParameter("rot");
    this.height = getURLParameter("height");
    var scale = getURLParameter("scale");

    if (scale) {
        scale = parseFloat(scale);
        this.entity.setLocalScale(scale, scale, scale);
    }

    if (lat !== null && lon !== null) {
        this.coordinates.latitude = parseFloat(lat);
        this.coordinates.longitude = parseFloat(lon);
    }

    if (rot) {
        this.entity.setLocalEulerAngles(0, parseFloat(rot), 0);
    }

    if (this.height) {
        var pos = this.entity.getPosition();
        this.entity.setPosition(pos.x, parseFloat(this.height), pos.y);
        console.log(this.entity.getPosition());
    }

    console.log(this.coordinates);
    this.geolocationAR = this.geoCam.script.geolocationAR;
    console.log(this.geolocationAR);
};

// update code called every frame
GeolocationObject.prototype.update = function (dt) {
    var position = this.geolocationAR.updatePosition(this.coordinates);

    if (position) {
        if (this.height) {
            //console.log("adjusting height");
            position.y = parseFloat(this.height);

        }

        //console.log(position);

        if (this.first) {
            this.first = false;
            this.entity.setPosition(position);
        } else {
            var offset = position.sub(this.entity.getPosition());
            //this.entity.setPosition(position);
            this.entity.translate(offset.mulScalar(dt));
        }

    }

};

// swap method called for script hot-reloading
// inherit your script state here
// GeolocationObject.prototype.swap = function(old) { };

// to learn more about script anatomy, please read:
// https://developer.playcanvas.com/en/user-manual/scripting/