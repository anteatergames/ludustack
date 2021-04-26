var BILLRATEDASHBOARD = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var urlBillRate;

    function setSelectors() {
        selectors.canInteract = '#caninteract';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.containerList = '#containerlist';
        selectors.list = '#divList';
        selectors.btnDuplicate = '.btn-billrate-duplicate';
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

        urlBillRate = objs.urls.data('urlList');

        loadItems(urlBillRate);

        BILLRATECOMMON.Callback.DeleteEntity = handleResponse;
    }

    function bindAll() {
        bindDuplicateBillRate();
    }

    function bindDuplicateBillRate() {
        objs.container.on('click', selectors.btnDuplicate, function (e) {
            e.preventDefault();

            var btn = $(this);

            MAINMODULE.Common.PostWithoutConfirmation(btn, handleResponse);

            return false;
        });
    }

    function loadItems(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.list);
    }

    function handleResponse(response) {
        if (response.success) {
            loadItems(urlBillRate);
        }
    }

    return {
        Init: init
    };
}());

$(function () {
    BILLRATEDASHBOARD.Init();
});