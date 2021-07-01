var GAMEJAMDETAILS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.containerList = '#containerlist';
    }

    function cacheObjs() {
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.containerDetails = $(selectors.containerDetails);
        objs.containerList = $(selectors.containerList);
    }

    function init() {
        setSelectors();
        cacheObjs();


        FX.StartCountDown('#SecondsToCountDown');
    }

    return {
        Init: init
    };
}());

$(function () {
    GAMEJAMDETAILS.Init();
});