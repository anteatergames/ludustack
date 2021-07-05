var GAMEJAMDETAILS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.Id = '#Id';
        selectors.btnJoin = '.btn-join';
        selectors.btnCantJoin = '.btn-cantjoin';
    }

    function cacheObjs() {
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.containerDetails = $(selectors.containerDetails);
        objs.Id = $(selectors.Id);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        canInteract = MAINMODULE.CanInteract();

        FX.StartCountDown('#SecondsToCountDown');

        var hash = location.hash.replace(/^#/, '');
        if (hash) {
            $('.nav-tabs a[href="#' + hash + '"]').tab('show');
        }
    }

    function bindAll() {
        bindBtnJoin();
        bindBtnCantJoin();
        bindTabs();

        MAINMODULE.Common.BindPopOvers();
    }

    function bindBtnJoin() {
        objs.container.on('click', selectors.btnJoin, function () {
            var btn = $(this);

            MAINMODULE.Common.PostWithoutConfirmation(btn, handleResponse);

            return false;
        });
    }

    function bindBtnCantJoin() {
        objs.container.on('click', selectors.btnCantJoin, function () {
            var btn = $(this);
            var msg = btn.data('msg');

            ALERTSYSTEM.Toastr.ShowWarning(msg);

            return false;
        });
    }

    function bindTabs() {
        $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            e.preventDefault();

            window.location.hash = e.target.hash;

            var url = $(this).data('url');

            var tabDestination = e.target.getAttribute('href');

            if (tabDestination === '#taboverview') {
                history.replaceState({}, document.title, ".");
            }
            else if (url !== undefined) {
                loadTab($(tabDestination), url);
            }

            return false;
        });
    }

    function loadTab(tab, url) {
        tab.html(MAINMODULE.Default.Spinner);

        MAINMODULE.Ajax.LoadHtml(url, tab);
    }

    function handleResponse(response) {
        if (response.success) {
            MAINMODULE.Ajax.HandleUrlResponse(response);
        }
    }

    return {
        Init: init
    };
}());

$(function () {
    GAMEJAMDETAILS.Init();
});