var DestroyOnevent = pc.createScript('destroyOnevent');

DestroyOnevent.attributes.add('srcEntity', {
    title: 'Source Entity',
    description: 'Source Entity to listen on.',
    type: 'entity'
});

DestroyOnevent.attributes.add('eventName', {
    title: 'Event Name',
    description: 'Event to listen for.',
    type: 'string'
});

// initialize code called once per entity
DestroyOnevent.prototype.initialize = function () {
    this.eventTarget = this.srcEntity ? this.targetEntity : this.app;
    this.eventTarget.on(this.eventName, () => {
        this.entity.destroy();
    });

    this.entity.on("destroy", () => {
        this.entity.off();
    }, this);
};

// update code called every frame
DestroyOnevent.prototype.update = function (dt) {

};

// swap method called for script hot-reloading
// inherit your script state here
// DestroyOnevent.prototype.swap = function(old) { };

// to learn more about script anatomy, please read:
// https://developer.playcanvas.com/en/user-manual/scripting/