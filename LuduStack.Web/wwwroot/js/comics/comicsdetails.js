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
        selectors.rating = '.comics-rating';
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
        bindRatings();
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

    function bindRatings() {
        $(selectors.rating).rating({
            theme: 'krajee-fas',
            showClear: false,
            size: 'md',
            animate: false,
            showCaption: false,
            step: 0.5,
            filledStar: '<i class="fas fa-gamepad tilt-20"></i>',
            emptyStar: '<i class="fas fa-gamepad tilt-20"></i>',
            starCaptions: {
                0.5: 'Padawan',
                1: '1',
                1.5: '1.5',
                2: '2',
                2.5: '2.5',
                3: '3',
                3.5: '3.5',
                4: '4',
                4.5: '4.5',
                5: 'Jedi'
            }
        });

        objs.container.on('rating:change', selectors.rating, function (event, value, caption) {
            var url = $(this).data('url');

            console.log(event);
            console.log(caption);

            var data = { score: value };

            MAINMODULE.Ajax.Post(url, data);
        });
    }

    return {
        Init: init
    };
}());

$(function () {
    COMICSDETAILS.Init();
});