var GAMEJAMMANAGEMENT = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;

    var urlList;

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.containerList = '#containerlist';
        selectors.list = '#divList';
        selectors.btnCalculateResults = '.btn-results-calculate';
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

        canInteract = MAINMODULE.CanInteract();

        urlList = objs.urls.data('urlList');

        loadItems(urlList);

        GAMEJAMCOMMON.Callback.DeleteEntity = postActionCallback;
    }

    function bindAll() {
        bindCalculateResults();
    }

    function bindCalculateResults() {
        objs.container.on('click', selectors.btnCalculateResults, function (e) {
            e.preventDefault();

            var btn = $(this);

            if (canInteract) {
                MAINMODULE.Common.PostWithConfirmation(btn, postActionCallback);
            }

            return false;
        });
    }

    function loadItems(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.list);
    }

    function postActionCallback(response) {
        if (response.success) {
            loadItems(urlList);
        }
    }

    return {
        Init: init
    };
}());

$(function () {
    GAMEJAMMANAGEMENT.Init();
});