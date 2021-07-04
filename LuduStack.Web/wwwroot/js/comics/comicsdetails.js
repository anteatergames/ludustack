var COMICSDETAILS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;

    var ratingCaptions = {
        0.5: 'meh',
        1: '1.0',
        1.5: '1.5',
        2: '2.0',
        2.5: '2.5',
        3: '3.0',
        3.5: '3.5',
        4: '4.0',
        4.5: '4.5',
        5: 'AWESOME!'
    };

    function setSelectors() {
        selectors.container = '.container';
        selectors.urls = '#urls';
        selectors.btnLanguage = '.btn-changelanguage';
        selectors.imgMultilanguage = '.img-multilanguage';
        selectors.totalRating = '#inputTotalRating';
        selectors.userRating = '#inputUserRating';
        selectors.ratingCaption = '#myrating';
    }

    function cacheObjs() {
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.imgMultilanguage = $(selectors.imgMultilanguage);
        objs.totalRating = $(selectors.totalRating);
        objs.userRating = $(selectors.userRating);
        objs.ratingCaption = $(selectors.ratingCaption);
    }

    function init() {
        setSelectors();
        cacheObjs();

        canInteract = MAINMODULE.CanInteract();

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
        objs.totalRating.rating({
            theme: 'krajee-fas',
            showClear: false,
            size: 'sm',
            animate: false,
            showCaption: false,
            disabled: true,
            readonly: true,
            step: 0.5,
            filledStar: '<i class="fas fa-gamepad tilt-20"></i>',
            emptyStar: '<i class="fas fa-gamepad tilt-20"></i>',
            starCaptions: ratingCaptions
        });

        objs.userRating.data('ratedValue', objs.userRating.val());

        objs.userRating.rating({
            theme: 'krajee-fas',
            showClear: false,
            size: 'sm',
            animate: false,
            showCaption: false,
            disabled: !canInteract,
            readonly: !canInteract,
            step: 0.5,
            filledStar: '<i class="fas fa-gamepad tilt-20"></i>',
            emptyStar: '<i class="fas fa-gamepad tilt-20"></i>',
            starCaptions: ratingCaptions
        }).on('rating:hover', function (event, value, caption, target) {
            console.log(event);
            console.log(target);
            console.log(value);
            var txt = caption.replace(new RegExp(/<span (.+)>(.+)<\/span>/, 'g'), '$2');
            objs.ratingCaption.html(txt);
        }).on('rating:hoverleave', function (event, target) {
            console.log(event);
            console.log(target);
            var stars = objs.userRating.data('ratedValue');
            objs.ratingCaption.html(stars);
        }).on('rating:change', selectors.rating, function (event, value, caption) {
            var url = $(this).data('url');

            console.log(event);
            console.log(caption);

            var data = { score: value };

            MAINMODULE.Ajax.Post(url, data);

            objs.userRating.data('ratedValue', value);
        });
    }

    return {
        Init: init
    };
}());

$(function () {
    COMICSDETAILS.Init();
});