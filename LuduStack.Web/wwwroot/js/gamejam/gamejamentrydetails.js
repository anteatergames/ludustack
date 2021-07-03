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