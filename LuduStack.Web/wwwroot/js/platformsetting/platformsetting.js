var PLATFORMSETTING = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var urlList;

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.containerList = '#containerlist';
        selectors.list = '#divList';
        selectors.btnToggle = '.btn-toggle';
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

        PLATFORMSETTINGCOMMON.Callback.ResetEntity = deleteCallback;

        bindAll();
    }

    function bindAll() {
        bindToggle();
    }

    function bindToggle() {
        console.log("bindToggle");
        objs.container.on('click', selectors.btnToggle, function (e) {
            console.log('clicked');
            e.preventDefault();

            if (MAINMODULE.CanInteract()) {
                var btn = $(this);
                var url = btn.attr('href');

                MAINMODULE.Ajax.Post(url, null, null, toggleCallback);
            }

            return false;
        });
    }

    function loadItems(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.list);
    }

    function toggleCallback(response) {
        if (response.success) {
            loadItems(urlList);
        }
        else {
            MAINMODULE.Ajax.HandleErrorResponse(response);
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
    PLATFORMSETTING.Init();
});