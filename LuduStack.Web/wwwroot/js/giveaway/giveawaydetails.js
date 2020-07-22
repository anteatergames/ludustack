var GIVEAWAYDETAILS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    const selection = window.getSelection();

    var spinnerBtn = '<span class="spinner-border" role="status" aria-hidden="true"></span>';
    var saveBtnOriginalText = '';

    function setSelectors() {
        selectors.canInteract = '#caninteract';
        selectors.urls = '#urls';
        selectors.secondsToEnd = '#SecondsToEnd';
        selectors.status = '#Status';
        selectors.container = '#featurecontainer';
        selectors.giveawayContainer = '.giveaway-container';
        selectors.termsContainer = '.terms-container';
        selectors.containerList = '#containerlist';
        selectors.form = '#frmEnterGiveaway';
        selectors.emailInput = '#Enter_Email';
        selectors.btnEnter = '#btnEnterGiveaway';
        selectors.btnShowTerms = '.btn-terms';
        selectors.urlInput = '#ShareUrl';
        selectors.copyIcon = '.copy-icon';
        selectors.footer = 'footer';
        selectors.btnDaily = '#btn-daily';
        selectors.entryoption = '.entryoption';
        selectors.btnPoints = '.btn-points';
        selectors.entryCount = '#entry-count';
    }

    function cacheObjs() {
        objs.container = $(selectors.container);
        objs.giveawayContainer = $(selectors.giveawayContainer);
        objs.termsContainer = $(selectors.termsContainer);
        objs.urls = $(selectors.urls);
        objs.status = $(selectors.status);
        objs.form = $(selectors.form);
        objs.emailInput = $(selectors.emailInput);
        objs.urlInput = $(selectors.urlInput);
        objs.footer = $(selectors.footer);
        objs.entryCount = $(selectors.entryCount);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        FX.StartCountDown(selectors.secondsToEnd);

        var userIsIn = $(selectors.emailInput).length === 0 && (objs.status.val() === 'OpenForEntries' || objs.status.val() === 'Ended');

        if (userIsIn) {
            FX.Poof();
        }
    }

    function bindAll() {
        bindBtnEnter();
        bindBtnShowTerms();
        bindBtnCopy();

        bindBtnDaily();

        FX.BindKonamiCode();
    }

    function bindBtnEnter() {
        objs.container.on('click', selectors.btnEnter, function (e) {
            e.preventDefault();
            var btn = $(this);

            var valid = objs.form.valid();;

            if (valid) {
                submitForm(btn);
            }

            return false;
        });
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

    function bindBtnCopy() {
        objs.container.on('click', selectors.copyIcon, function (e) {
            e.preventDefault();
            var btn = $(this);

            copyToClipboard(selectors.urlInput);

            ALERTSYSTEM.Toastr.ShowInfo("URL copied to the clipboard.")

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

    function bindBtnDaily() {
        objs.container.on('click', selectors.btnDaily, function (e) {
            e.preventDefault();
            var btn = $(this);

            var url = btn.data('url');

            $.ajax({
                url: url,
                type: 'POST'
            }).done(function (response) {
                if (response.success) {
                    ALERTSYSTEM.Toastr.ShowInfo(response.message);
                    var entryOption = btn.closest(selectors.entryoption);
                    var btnPoints = entryOption.find(selectors.btnPoints);

                    entryOption.addClass('checked');
                    btnPoints.html(response.value);

                    objs.entryCount.html(objs.entryCount.data('count') + 1);
                }
                else {
                    ALERTSYSTEM.ShowWarningMessage(response.message);
                }
            });

            return false;
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