var ORDER = (function () {
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
        selectors.btnSyncOrder = '.btn-sync-order';
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

        urlList = objs.urls.data('urlList');

        loadItems(urlList);

        ORDERCOMMON.Callback.DeleteEntity = deleteCallback;

        canInteract = MAINMODULE.CanInteract();

        bindAll();
    }

    function bindAll() {
        bindSyncOrder();
    }

    function bindSyncOrder() {
        objs.container.on('click', selectors.btnSyncOrder, function (e) {
            e.preventDefault();

            var btn = $(this);
            var url = btn.attr('href');

            if (canInteract && !btn.hasClass('disabled')) {
                MAINMODULE.Ajax.Post(url, null, null, syncCallback);
            }

            return false;
        });
    }

    function loadItems(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.list);
    }

    function syncCallback(response) {
        if (response.success) {
            loadItems(urlList);
        }
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
    ORDER.Init();
});