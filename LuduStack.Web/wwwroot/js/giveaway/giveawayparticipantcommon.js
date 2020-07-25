var GIVEAWAYPARTICIPANTCOMMON = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    function setSelectors() {
        selectors.secondsToEnd = '#SecondsToEnd';
        selectors.container = '#featurecontainer';
        selectors.giveawayContainer = '.giveaway-container';
        selectors.termsContainer = '.terms-container';
        selectors.btnShowTerms = '.btn-terms';
        selectors.footer = 'footer';
    }

    function cacheObjs() {
        objs.container = $(selectors.container);
        objs.giveawayContainer = $(selectors.giveawayContainer);
        objs.termsContainer = $(selectors.termsContainer);
        objs.footer = $(selectors.footer);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        FX.StartCountDown(selectors.secondsToEnd);
    }

    function bindAll() {
        bindBtnShowTerms();

        FX.BindKonamiCode();
    }

    function bindBtnShowTerms() {
        objs.container.on('click', selectors.btnShowTerms, function (e) {
            e.preventDefault();
            var btn = $(this);

            if (objs.giveawayContainer.hasClass('d-none')) {
                objs.termsContainer.addClass('d-none');
                objs.giveawayContainer.removeClass('d-none');
                objs.footer.removeClass('d-none');
            }
            else {
                objs.termsContainer.removeClass('d-none');
                objs.giveawayContainer.addClass('d-none');
                objs.footer.addClass('d-none')
            }

            return false;
        });
    }

    return {
        Init: init
    };
}());

$(function () {
    GIVEAWAYPARTICIPANTCOMMON.Init();
});