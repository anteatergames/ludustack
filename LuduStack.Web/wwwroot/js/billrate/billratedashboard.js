var BILLRATEDASHBOARD = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var urlBillRate;

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.containerList = '#containerlist';
        selectors.list = '#divList';
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

        urlBillRate = objs.urls.data('urlList');

        loadItems(urlBillRate);

        BILLRATECOMMON.Callback.DeleteEntity = handleResponse;
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