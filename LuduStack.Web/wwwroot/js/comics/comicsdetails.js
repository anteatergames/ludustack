var COMICSDETAILS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    function setSelectors() {
        selectors.container = '.container';
        selectors.canInteract = '#caninteract';
        selectors.urls = '#urls';
        selectors.btnLanguage = '.btn-changelanguage';
        selectors.imgMultilanguage = '.img-multilanguage';
    }

    function cacheObjs() {
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.imgMultilanguage = $(selectors.imgMultilanguage);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();
    }

    function bindAll() {
        bindChangeLanguage();
    }

    function bindChangeLanguage() {
        objs.container.on('click', selectors.btnLanguage, function (e) {
            e.preventDefault();

            var btn = $(this);

            objs.imgMultilanguage.addClass('d-none');

            var target = btn.data('target');

            var targetToEnable = objs.imgMultilanguage.filter('.' + target);

            targetToEnable.removeClass('d-none');

            return false;
        });
    }

    return {
        Init: init
    };
}());

$(function () {
    COMICSDETAILS.Init();
});