var PAGINATION = (function () {
    "use strict";

    var selectors = {};
    var objs = {};
    var canInteract = false;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.canInteract = '#caninteract';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
    }

    function cacheObjs() {
        objs.canInteract = $(selectors.canInteract);
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.ajaxContainer = $(selectors.ajaxContainer);
    }

    function init(btnPageSelector, ajaxContainerSelector) {
        setSelectors();

        selectors.btnPage = btnPageSelector;
        selectors.ajaxContainer = ajaxContainerSelector;

        cacheObjs();

        canInteract = objs.canInteract.val() === 'true';

        bindAll();
    }

    function bindAll() {
        bindPagination();
    }

    function bindPagination() {
        if (!selectors.btnPage) {
            console.log('paging button selector is undefined');
        }
        else {
            objs.container.on('click', selectors.btnPage, function (e) {
                e.preventDefault();

                var btn = $(this);
                var url = btn.attr('href');

                loadItems(url);

                return false;
            });
        }
    }

    function loadItems(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.ajaxContainer);
    }

    return {
        Init: init,
    };
}());