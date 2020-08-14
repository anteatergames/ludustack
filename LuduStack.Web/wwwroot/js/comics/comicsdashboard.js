var COMICSDASHBOARD = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var urlComicss;

    function setSelectors() {
        selectors.canInteract = '#caninteract';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.containerList = '#containerlist';
        selectors.list = '#divList';
        selectors.btnDuplicate = '.btn-comics-duplicate';
    }

    function cacheObjs() {
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

        urlComicss = objs.urls.data('urlList');

        loadItems(urlComicss);

        COMICSCOMMON.Callback.DeleteEntity = deleteCallback;
    }

    function bindAll() {
        bindDuplicateComics();
    }

    function bindDuplicateComics() {
        objs.container.on('click', selectors.btnDuplicate, function (e) {
            e.preventDefault();

            var btn = $(this);

            MAINMODULE.Common.PostWithoutConfirmation(btn, function (response) {
                if (response.success) {
                    loadItems(urlComicss);
                }
            });

            return false;
        });
    }

    function loadItems(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.list);
    }

    function deleteCallback(response) {
        if (response.success) {
            loadItems(urlComicss);
        }
    }

    return {
        Init: init
    };
}());

$(function () {
    COMICSDASHBOARD.Init();
});