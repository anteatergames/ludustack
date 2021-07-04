var GAMEJAMDETAILS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.btnJoin = '.btn-join';
        selectors.btnCantJoin = '.btn-cantjoin';
    }

    function cacheObjs() {
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.containerDetails = $(selectors.containerDetails);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        canInteract = MAINMODULE.CanInteract();

        FX.StartCountDown('#SecondsToCountDown');
    }

    function bindAll() {
        bindBtnJoin();
        bindBtnCantJoin();
    }

    function bindBtnJoin() {
        objs.container.on('click', selectors.btnJoin, function () {
            var btn = $(this);

            MAINMODULE.Common.PostWithoutConfirmation(btn, handleResponse);

            return false;
        });
    }

    function bindBtnCantJoin() {
        objs.container.on('click', selectors.btnCantJoin, function () {
            var btn = $(this);
            var msg = btn.data('msg');

            ALERTSYSTEM.Toastr.ShowWarning(msg);

            return false;
        });
    }

    function handleResponse(response) {
        if (response.success) {
            MAINMODULE.Ajax.HandleUrlResponse(response);
        }
    }

    return {
        Init: init
    };
}());

$(function () {
    GAMEJAMDETAILS.Init();
});