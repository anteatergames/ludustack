﻿var GAMEJAMENTRYDETAILS = (function () {
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
                0.5: 'Your Rating: 0.5',
                1: 'Your Rating: 1',
                1.5: 'Your Rating: 1.5',
                2: 'Your Rating: 2',
                2.5: 'Your Rating: 2.5',
                3: 'Your Rating: 3',
                3.5: 'Your Rating: 3.5',
                4: 'Your Rating: 4',
                4.5: 'Your Rating: 4.5',
                5: 'Your Rating: 5'
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