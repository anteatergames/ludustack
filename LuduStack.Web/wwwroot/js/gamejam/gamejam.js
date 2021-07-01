var GAMEJAM = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var urlList;

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.containerList = '#containerlist';
        selectors.list = '#divList';
        selectors.items = '.gamejamitem';
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

        GAMEJAMCOMMON.Callback.DeleteEntity = deleteCallback;
    }

    function bindCounters() {
        objs.items.each(function (index, element) {
            var dateElement = $(element).find('.hiddendate');

            if (dateElement) {
                FX.StartCountDown(`#${dateElement.attr('id')}`, `#${element.id}`);
            }
        });
    }

    function loadItems(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.list).then(() => {
            objs.items = $(selectors.items);
            bindCounters();

            MAINMODULE.Common.BindPopOvers();
        });
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
    GAMEJAM.Init();
});