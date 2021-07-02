var GAMEJAMDETAILS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.btnJoin = '.btn-join';
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

        FX.StartCountDown('#SecondsToCountDown');
    }

    function bindAll() {
        bindBtnJoin();
    }

    function bindBtnJoin() {
        objs.container.on('click', selectors.btnJoin, function () {
            var btn = $(this);

            MAINMODULE.Common.PostWithoutConfirmation(btn, handleResponse);

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