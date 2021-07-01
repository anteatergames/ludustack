var NOTIFICATIONDASHBOARD = (function () {
    "use strict";

    var selectors = {};
    var objs = {};
    var canInteract = false;

    var urlList;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.list = '#divList';
        selectors.btnDelete = '.btn-notification-delete';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.list = $(selectors.list);
    }

    function init() {
        setSelectors();
        cacheObjs();

        canInteract = MAINMODULE.CanInteract();

        bindAll();

        urlList = objs.urls.data('urlList');

        loadItems(urlList);

        NOTIFICATIONDASHBOARD.Callback.DeleteEntity = deleteCallback;
    }

    function bindAll() {
        bindBtnDelete();
    }

    function bindBtnDelete() {
        objs.container.on('click', selectors.btnDelete, function (e) {
            e.preventDefault();

            var btn = $(this);

            if (canInteract) {
                MAINMODULE.Common.DeleteEntity(btn, NOTIFICATIONDASHBOARD.Callback.DeleteEntity);
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

    return {
        Init: init,
        CanInteract: canInteract,
        Callback: {
            DeleteEntity: function () {
                console.log('empty NOTIFICATIONDASHBOARD.Callback.Delete');
            }
        }
    };
}());

$(function () {
    NOTIFICATIONDASHBOARD.Init();
});