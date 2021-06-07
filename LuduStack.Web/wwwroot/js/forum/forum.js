var FORUM = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var urlList;

    function setSelectors() {
        selectors.canInteract = '#caninteract';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.containerList = '#containerlist';
        selectors.list = '#divList';
        selectors.btnPage = 'a.page-link';
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

        urlList = objs.urls.data('urlList');

        loadItems(urlList);

        FORUMCOMMON.Callback.DeleteEntity = deleteCallback;
    }

    function bindAll() {
        bindPagination();
    }

    function bindPagination() {
        objs.container.on('click', selectors.btnPage, function (e) {
            e.preventDefault();

            var btn = $(this);
            urlList = btn.attr('href');

            console.log(urlList);

            loadItems(urlList);

            return false;
        });
    }

    function loadItems(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.list);
    }

    function deleteCallback(response) {
        if (response.success) {
            loadItems(urlList);
        }
    }

    return {
        Init: init
    };
}());

$(function () {
    FORUM.Init();
});