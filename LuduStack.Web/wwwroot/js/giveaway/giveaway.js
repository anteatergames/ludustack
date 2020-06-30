var GIVEAWAY = (function () {
    "use strict";

    var selectors = {};
    var objs = {};
    var canInteract = false;

    function setSelectors() {
        selectors.canInteract = '#caninteract';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.containerList = '#containerlist';
        selectors.list = '#divList';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.containerDetails = $(selectors.containerDetails);
        objs.containerList = $(selectors.containerList);
        objs.list = $(selectors.list);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        canInteract = objs.container.find(selectors.canInteract).val();

        var url = objs.urls.data('urlList');
        loadItems(url);
    }

    function bindAll() {
    }

    function loadItems(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.listCourses);
    }

    return {
        Init: init
    };
}());

$(function () {
    GIVEAWAY.Init();
});