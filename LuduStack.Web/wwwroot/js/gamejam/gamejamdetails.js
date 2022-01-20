var GAMEJAMDETAILS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.Id = '#Id';
        selectors.btnJoin = '.btn-join';
        selectors.btnCantJoin = '.btn-cantjoin';
        selectors.scoreRating = '.criteria-rating';
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
        bindWindowResize();

        MAINMODULE.Common.BindPopOvers();
    }

    function bindBtnJoin() {
        objs.container.on('click', selectors.btnJoin, function () {
            var btn = $(this);

            MAINMODULE.Common.PostWithoutConfirmation(btn, 'POST', handleResponse);

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
                loadTab($(tabDestination), url).then(() => {
                    if (tabDestination === '#tabsubmissions' || tabDestination === '#tabresults') {
                        bindRatings();
                    }
                });
            }

            return false;
        });
    }

    function bindRatings() {
        $(selectors.scoreRating).rating({
            theme: 'krajee-fas',
            showClear: false,
            showCaption: false,
            hoverEnabled: false,
            displayOnly: true,
            size: getDesiredStarRatingBreakpoint(),
            animate: false,
            step: 0.5,
            filledStar: '<i class="fas fa-gamepad tilt-20"></i>',
            emptyStar: '<i class="fas fa-gamepad tilt-20"></i>',
            starCaptions: {
                0.5: 'Score: 0.5',
                1: 'Score: 1',
                1.5: 'Score: 1.5',
                2: 'Score: 2',
                2.5: 'Score: 2.5',
                3: 'Score: 3',
                3.5: 'Score: 3.5',
                4: 'Score: 4',
                4.5: 'Score: 4.5',
                5: 'Score: 5'
            }
        });

        objs.container.on('rating:change', selectors.scoreRating, function (event, value, caption) {
            var url = $(this).data('url');

            var data = { score: value };

            MAINMODULE.Ajax.Post(url, data);
        });
    }

    function bindWindowResize() {
        window.onresize = handleWindowResize
    }

    function handleWindowResize() {
        var starRatingBreakpoint = getDesiredStarRatingBreakpoint();

        $(selectors.scoreRating).rating('refresh', {
            size: starRatingBreakpoint,
        });
    }

    function getDesiredStarRatingBreakpoint() {
        var starRatingBreakpoint = 'xs';

        switch (MAINMODULE.Layout.GetCurrentBreakpoint()) {
            case 'xs':
                starRatingBreakpoint = 'md';
                break;
            case 'sm':
                starRatingBreakpoint = 'lg';
                break;
            case 'md':
                starRatingBreakpoint = 'md';
                break;
            case 'lg':
                starRatingBreakpoint = 'sm';
                break;
            case 'xl':
                starRatingBreakpoint = 'sm';
                break;
        }

        return starRatingBreakpoint;
    }

    function loadTab(tab, url) {
        tab.html(MAINMODULE.Default.Spinner);

        return MAINMODULE.Ajax.LoadHtml(url, tab);
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