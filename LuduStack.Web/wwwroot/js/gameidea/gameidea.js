var GAMEIDEA = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var urlList;

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.containerList = '#containerlist';
        selectors.list = '#divList';
        selectors.filterLanguage = '#ddlLanguage';
    }

    function cacheObjs() {
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.containerDetails = $(selectors.containerDetails);
        objs.containerList = $(selectors.containerList);
        objs.list = $(selectors.list);
        objs.filterLanguage = $(selectors.filterLanguage);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        loadItems();

        GAMEIDEACOMMON.Callback.DeleteEntity = deleteCallback;
    }

    function bindAll() {
        bindLanguageChange();
    }

    function bindLanguageChange() {
        objs.container.on('change', selectors.filterLanguage, languageChange);
    }

    function loadItems() {
        urlList = objs.urls.data('urlList');

        urlList += objs.filterLanguage.val();

        MAINMODULE.Ajax.LoadHtml(urlList, objs.list);
    }

    function deleteCallback(response, btn) {
        if (response.success) {
            btn.remove();
        }
    }

    function languageChange(value) {
        loadItems();
    }

    return {
        Init: init
    };
}());

$(function () {
    GAMEIDEA.Init();
});