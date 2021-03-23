var CONTENTDETAILS = (function () {
    "use strict";

    function init() {
        console.log('CONTENTDETAILS.init');
    }

    return {
        Init: init
    };
}());

$(function () {
    CONTENTDETAILS.Init();
});