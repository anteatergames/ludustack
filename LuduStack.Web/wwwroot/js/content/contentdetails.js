var CONTENTDETAILS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    function setSelectors() {
        selectors.gallery = '#postgallery';
    }

    function setObjs() {
        objs.gallery = $(selectors.gallery);
    }

    function init() {
        setSelectors();
        setObjs();

        bindAll();
    }

    function bindAll() {
        bindGallery();
    }

    function bindGallery() {
        if (objs.gallery.data('hasitems') === true && objs.gallery.data('isgallery') === true) {
            setTimeout(function () {
                GALLERY.Load(selectors.gallery, 2);
            }, 10);
        }
    }

    return {
        Init: init
    };
}());

$(function () {
    CONTENTDETAILS.Init();
});