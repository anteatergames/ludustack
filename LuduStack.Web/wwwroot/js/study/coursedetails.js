var COURSEDETAILS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};
    var canInteract = false;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.listPlans = '#divPlans';
        selectors.btnEnroll = '#btnEnroll';
        selectors.btnLeaveCourse = '#btnLeaveCourse';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.listPlans = $(selectors.listPlans);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        canInteract = MAINMODULE.CanInteract();

        var urlPlans = objs.urls.data('urlListplans');
        listPlans(urlPlans);
    }

    function bindAll() {
        bindBtnEnroll();
        bindBtnLeave()
    }

    function bindBtnEnroll() {
        objs.container.on('click', selectors.btnEnroll, studentAction);
    }

    function bindBtnLeave() {
        objs.container.on('click', selectors.btnLeaveCourse, studentAction);
    }

    function studentAction(e) {
        e.preventDefault();

        var btn = $(this);
        var url = btn.data('url');

        if (canInteract) {
            MAINMODULE.Ajax.Post(url);
        }

        return false;
    }

    function listPlans(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.listPlans);
    }

    return {
        Init: init
    };
}());

$(function () {
    COURSEDETAILS.Init();
});