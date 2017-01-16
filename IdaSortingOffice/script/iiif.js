var IIIF = function () {

    function makeAtProp(name) {
        return {
            get: function () {
                return this["@" + name];
            },
            set: function (value) {
                this["@" + name] = value;
            }
        }
    }

    function IiifBase() { }
    Object.defineProperty(IiifBase.prototype, "id", makeAtProp("id"));
    Object.defineProperty(IiifBase.prototype, "type", makeAtProp("type"));
    Object.defineProperty(IiifBase.prototype, "context", makeAtProp("context"));

    function Manifest() { }
    Manifest.prototype = new IiifBase();

    function Load() {

    }

    return {
        Manifest: Manifest,
    };
}();