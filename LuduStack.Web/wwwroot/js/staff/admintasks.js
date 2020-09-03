var ADMINTASKS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    function init() {
        setSelectors();
        cacheObjects();

        bindAll();
    }

    function setSelectors() {
        selectors.container = '#contentwrapper';
        selectors.btnAction = '.btn-action';
        selectors.divResult = '#divResult';
    }

    function cacheObjects() {
        objs.container = $(selectors.container);
        objs.divResult = $(selectors.divResult);
    }

    function bindAll() {
        bindBtnAction();
    }

    function bindBtnAction() {
        objs.container.on('click', selectors.btnAction, function (e) {
            var btn = $(this);
            var url = btn.data('url');

            executeAction(url);
        });
    }

    function executeAction(url) {
        objs.divResult.html(MAINMODULE.Default.SpinnerTop);
        $.post(url)
            .done(function (response) {
                objs.divResult.html(response);
            });
    }

    return {
        Init: init
    };
}());

$(function () {
    ADMINTASKS.Init();
});