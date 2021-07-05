var GAMEJAMENTRYDETAILS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.form = '#frmSubmitGame';
        selectors.btnSubmitGame = '#btnSubmitGame';
        selectors.scoreRating = '.criteria-rating';
    }

    function cacheObjs() {
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.containerDetails = $(selectors.containerDetails);
        objs.form = $(selectors.form);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        canInteract = MAINMODULE.CanInteract();

        FX.StartCountDown('#SecondsToCountDown');
    }

    function bindAll() {
        bindBtnSubmitGame();
        bindRatings();
    }

    function bindBtnSubmitGame() {
        objs.container.on('click', selectors.btnSubmitGame, function () {
            var btn = $(this);

            var valid = objs.form.valid();

            if (valid && canInteract) {
                MAINMODULE.Common.DisableButton(btn).ready(() => {
                    submitForm(btn);
                });
            }

            return false;
        });
    }

    function bindRatings() {
        $(selectors.scoreRating).rating({
            theme: 'krajee-fas',
            showClear: false,
            showCaption: false,
            size: 'md',
            animate: false,
            step: 0.5,
            filledStar: '<i class="fas fa-gamepad tilt-20"></i>',
            emptyStar: '<i class="fas fa-gamepad tilt-20"></i>',
            starCaptions: {
                0.5: '0.5',
                1: '1.0',
                1.5: '1.5',
                2: '2.0',
                2.5: '2.5',
                3: '3.0',
                3.5: '3.5',
                4: '4.0',
                4.5: '4.5',
                5: '5.0'
            }
        });

        objs.container.on('rating:change', selectors.scoreRating, function (event, value, caption) {
            var url = $(this).data('url');

            var data = { score: value };

            MAINMODULE.Ajax.Post(url, data);
        });
    }

    function submitForm(btn) {
        var url = objs.form.attr('action');

        var data = objs.form.serializeObject();

        return $.post(url, data).done(function (response) {
            if (response.success === true) {
                MAINMODULE.Common.PostSaveCallback(response, btn);

                MAINMODULE.Ajax.HandleUrlResponse(response);
            }
            else {
                MAINMODULE.Ajax.HandleErrorResponse(response);
            }
        });
    }

    return {
        Init: init
    };
}());

$(function () {
    GAMEJAMENTRYDETAILS.Init();
});