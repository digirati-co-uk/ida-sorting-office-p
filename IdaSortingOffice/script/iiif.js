var IIIF = function () {

    function jsonLdTidy(obj, propNames) {
        propNames.forEach(function (pn) {
            Object.defineProperty(obj, pn, {
                get: function () {
                    return this["@" + pn];
                },
                set: function (value) {
                    this["@" + pn] = value;
                }
            });
        });
    }

    function wrap(rawObj) {
        if (!!rawObj && typeof (rawObj) === "object") {
            //if (rawObj["@type"] && rawObj["@type"].indexOf("sc:") === 0) {
            if (rawObj.constructor !== Array) {
                jsonLdTidy(rawObj, ["id", "type", "context"]);
                rawObj.getThumbnail = getThumbnail;
            }
            // traverse:
            for (var obj in rawObj) {
                if (rawObj.hasOwnProperty(obj)) {
                    wrap(rawObj[obj]);
                }
            }
            //}
        }
    }

    // todo - Size (different w,h) rather than square confinement
    // will not follow links
    function getThumbnail(required, min, max) {
        if (!max) max = 3 * (min || 100);
        if (this.hasOwnProperty("thumbnail")) {
            if (typeof this.thumbnail === "string") {
                // A thumbnail has been supplied but we have no idea how big it is
                return {
                    url: this.thumbnail
                };
            }
            if (this.thumbnail.service && this.thumbnail.service.sizes) {
                var bestFromSizes = getThumbnailFromServiceSizes(this.thumbnail.service, required, min, max);
                if (bestFromSizes) return bestFromSizes;
            }
            return {
                url: this.thumbnail.id,
                width: this.thumbnail.width,
                height: this.thumbnail.height
            };
        } else {
            // does not have a thumbnail property
            return null; // for now
        }
    }

    function getThumbnailFromServiceSizes(service, required, min, max) {
        // this will return a thumbnail between min and max
        var sizes = service.sizes;
        sizes.sort(function (a, b) { return a.width - b.width; });
        var best = null;
        for (var i = sizes.length - 1; i >= 0; i--) {
            // start with the biggest; see if each one matches criteria.
            var size = sizes[i];
            if (size.width >= min || size.height >= min && size.width <= max && size.height <= max) {
                best = size;
            } else {
                if(best) break;
            }
        }
        if (best) {
            var scale = getScale(required, best.width, best.height);
            return {
                url: getCanonicalUri(service, best.width, best.height),
                width: Math.round(scale * best.width),
                height: Math.round(scale * best.height),
                actualWidth: best.width,
                actualHeight: best.height
            };
        }
        return null;
    }

    function getScale(box, width, height) {
        var scaleW = box / width;
        var scaleH = box / height;
        return Math.min(scaleW, scaleH);
    }

    function getCanonicalUri(imageService, width, height) {
        // TODO - this is not correct, it's a placeholder...
        return imageService.id + "/full/" + width + "," + height + "/0/default.jpg";
    }

    return {
        wrap: function (obj){wrap(obj)}
    };
}();