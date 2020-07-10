var GIVEAWAYDETAILS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    const selection = window.getSelection();

    function setSelectors() {
        selectors.canInteract = '#caninteract';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.containerList = '#containerlist';
        selectors.form = '#frmEnterGiveaway';
        selectors.emailInput = '#Enter_Email';
        selectors.btnEnter = '#btnEnterGiveaway';
        selectors.urlInput = '#ShareUrl';
        selectors.copyIcon = '.copy-icon';
    }

    function cacheObjs() {
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.form = $(selectors.form);
        objs.emailInput = $(selectors.emailInput);
        objs.urlInput = $(selectors.urlInput);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        FX.StartCountDown('#SecondsToEnd');

        var userIsIn = $(selectors.emailInput).length === 0;

        if (userIsIn) {
            FX.Poof();
        }
    }

    function bindAll() {
        bindBtnEnter();
        bindBtnCopy();

        FX.BindKonamiCode();
    }

    function bindBtnEnter() {
        objs.container.on('click', selectors.btnEnter, function (e) {
            e.preventDefault();
            var btn = $(this);

            var valid = objs.emailInput.val().length > 0;

            if (valid) {
                submitForm(btn);
            }
            else {
                ALERTSYSTEM.ShowWarningMessage("You must type your email to participate!");
            }

            return false;
        });
    }

    function bindBtnCopy() {
        objs.container.on('click', selectors.copyIcon, function (e) {
            e.preventDefault();
            var btn = $(this);

            copyToClipboard(selectors.urlInput);

            return false;
        });
    }

    function submitForm(btn, callback) {
        btn.addClass('disabled');
        var url = objs.form.attr('action');

        var data = objs.form.serialize();

        $.post(url, data).done(function (response) {
            if (response.success === true) {
                if (callback) {
                    callback();
                }

                window.location = response.url;
            }
            else {
                ALERTSYSTEM.ShowWarningMessage("Unable to join the gvieaway.");
                btn.removeClass('disabled');
            }
        });
    }

    function copyToClipboard(selector) {
        selection.removeAllRanges();

        var copyText = document.querySelector(selector);

        copyText.select();
        copyText.setSelectionRange(0, 99999); /*For mobile devices*/
        document.execCommand("copy");
    }

    return {
        Init: init
    };
}());

$(function () {
    GIVEAWAYDETAILS.Init();
});