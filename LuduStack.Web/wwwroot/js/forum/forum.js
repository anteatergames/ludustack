var FORUM = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var urlList;

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.containerList = '#containerlist';
        selectors.list = '#divList';
        selectors.btnPage = 'a.page-link';
        selectors.btnScrollTo = '.btn-scrollto';
        selectors.listItem = '.forumlistitem';
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

        PAGINATION.Init(selectors.btnPage, selectors.list);
    }

    function bindAll() {
        bindBtnScrollTo();
    }

    function bindBtnScrollTo() {
        objs.container.on('click', selectors.btnScrollTo, function (e) {
            e.preventDefault();

            var btn = $(this);

            var toBottom = btn.hasClass('btn-scrollto-bottom');

            if (toBottom) {
                scrollToLatest();
            }
            else {
                MAINMODULE.Utils.ScrollToTop();
            }

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

    function scrollToLatest() {
        var elementToScroll = $(selectors.listItem).last();

        MAINMODULE.Utils.ScrollTo(elementToScroll);
    }

    return {
        Init: init
    };
}());

$(function () {
    FORUM.Init();
});