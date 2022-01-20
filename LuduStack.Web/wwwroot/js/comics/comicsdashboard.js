var COMICSDASHBOARD = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var urlComics;

    function setSelectors() {
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

        urlComics = objs.urls.data('urlList');

        loadItems(urlComics);

        COMICSCOMMON.Callback.DeleteEntity = handleResponse;
    }

    function bindAll() {
        bindDuplicateComics();
    }

    function bindDuplicateComics() {
        objs.container.on('click', selectors.btnDuplicate, function (e) {
            e.preventDefault();

            var btn = $(this);

            MAINMODULE.Common.PostWithoutConfirmation(btn, 'POST', handleResponse);

            return false;
        });
    }

    function loadItems(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.list);
    }

    function handleResponse(response) {
        if (response.success) {
            loadItems(urlComics);
        }
    }

    return {
        Init: init
    };
}());

$(function () {
    COMICSDASHBOARD.Init();
});