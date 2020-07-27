var GIVEAWAYDETAILS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    const selection = window.getSelection();

    var spinnerBtn = '<span class="spinner-border" role="status" aria-hidden="true"></span>';
    var saveBtnOriginalText = '';

    function setSelectors() {
        selectors.canInteract = '#caninteract';
        selectors.secondsToEnd = '#SecondsToEnd';
        selectors.status = '#Status';
        selectors.container = '#featurecontainer';
        selectors.giveawayContainer = '.giveaway-container';
        selectors.termsContainer = '.terms-container';
        selectors.containerList = '#containerlist';
        selectors.form = '#frmEnterGiveaway';
        selectors.emailInput = '#Enter_Email';
        selectors.btnEnter = '#btnEnterGiveaway';
    }

    function cacheObjs() {
        objs.container = $(selectors.container);
        objs.giveawayContainer = $(selectors.giveawayContainer);
        objs.termsContainer = $(selectors.termsContainer);
        objs.status = $(selectors.status);
        objs.form = $(selectors.form);
        objs.emailInput = $(selectors.emailInput);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();
    }

    function bindAll() {
        bindBtnEnter();
    }

    function bindBtnEnter() {
        objs.container.on('click', selectors.btnEnter, function (e) {
            e.preventDefault();
            var btn = $(this);

            var valid = objs.form.valid();

            if (valid) {
                submitForm(btn);
            }

            return false;
        });
    }

    function submitForm(btn, callback) {
        var url = objs.form.attr('action');

        var data = objs.form.serialize();

        disableButton(btn);
        $.post(url, data).done(function (response) {
            if (response.success === true) {
                if (callback) {
                    callback();
                }

                window.location = response.url;
            }
            else {
                if (response.message) {
                    ALERTSYSTEM.ShowWarningMessage(response.message);
                }
                enableButton(btn);
            }
        });
    }

    function disableButton(btn) {
        btn.addClass('disabled');
        saveBtnOriginalText = btn.html();
        btn.html(spinnerBtn);
    }

    function enableButton(btn) {
        btn.html(saveBtnOriginalText);
        btn.removeClass('disabled');
    }

    return {
        Init: init
    };
}());

$(function () {
    GIVEAWAYDETAILS.Init();
});