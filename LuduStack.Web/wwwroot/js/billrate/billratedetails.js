var BILLRATEDETAILS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    function setSelectors() {
        selectors.container = '.container';
    }

    function cacheObjs() {
        objs.container = $(selectors.container);
    }

    function init() {
        setSelectors();
        cacheObjs();
    }

    return {
        Init: init
    };
}());

$(function () {
    BILLRATEDETAILS.Init();
});