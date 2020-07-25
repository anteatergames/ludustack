var GIVEAWAYYOUAREIN = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    const selection = window.getSelection();

    var sharedOnFacebook = false;

    function setSelectors() {
        selectors.status = '#Status';
        selectors.container = '#featurecontainer';
        selectors.urlInput = '#ShareUrl';
        selectors.copyIcon = '.copy-icon';
        selectors.btnDaily = '#btn-daily';
        selectors.entryoption = '.entryoption';
        selectors.btnPoints = '.btn-points';
        selectors.entryCount = '#entry-count';
        selectors.btnFacebookShare = '#btn-facebookshare';
        selectors.btnEmailConfirmed = '#btn-emailconfirmed';
        selectors.btnReferralCode = '#btn-referralcode';
    }

    function cacheObjs() {
        objs.container = $(selectors.container);
        objs.status = $(selectors.status);
        objs.urlInput = $(selectors.urlInput);
        objs.entryCount = $(selectors.entryCount);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        var userIsIn = $(selectors.emailInput).length === 0 && (objs.status.val() === 'OpenForEntries' || objs.status.val() === 'Ended');

        if (userIsIn) {
            FX.Poof();
        }
    }

    function bindAll() {
        bindBtnCopy();

        bindBtnEmailConfirmed();

        bindBtnReferralCode();

        bindBtnDaily();

        bindBtnFacebookShare();

        FX.BindKonamiCode();
    }

    function bindBtnCopy() {
        objs.container.on('click', selectors.copyIcon, function (e) {
            e.preventDefault();
            var btn = $(this);

            copyToClipboard(selectors.urlInput);

            ALERTSYSTEM.Toastr.ShowInfo(MESSAGES.Translation['msgUrlCopied'])

            return false;
        });
    }

    function bindBtnEmailConfirmed() {
        objs.container.on('click', selectors.btnEmailConfirmed, function (e) {
            e.preventDefault();

            ALERTSYSTEM.ShowWarningMessage(MESSAGES.Translation['msgEmailConfirmedClick']);

            return false;
        });
    }

    function bindBtnReferralCode() {
        objs.container.on('click', selectors.btnReferralCode, function (e) {
            e.preventDefault();

            ALERTSYSTEM.ShowWarningMessage(MESSAGES.Translation['msgReferralCodeClick']);

            return false;
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
                    registerNewEntry(btn, response.value);
                }
                else {
                    ALERTSYSTEM.ShowWarningMessage(response.message);
                }
            });

            return false;
        });
    }

    function bindBtnFacebookShare() {
        objs.container.on('click', selectors.btnFacebookShare, function (e) {
            e.preventDefault();
            var btn = $(this);

            if (!sharedOnFacebook) {
                facebookShare(btn, objs.urlInput.val());
                sharedOnFacebook = true;
            }
            else {
                ALERTSYSTEM.ShowWarningMessage(MESSAGES.Translation['msgFacebookShareAlreadyClicked']);
            }

            return false;
        });
    }

    function registerNewEntry(btn, points) {
        if (!points || points === 0) {
            points = 1;
        }

        var entryOption = checkEntry(btn);
        var btnPoints = entryOption.find(selectors.btnPoints);

        btnPoints.html(points);

        objs.entryCount.html(objs.entryCount.data('count') + 1);
    }

    function checkEntry(btn) {
        var entryOption = btn.closest(selectors.entryoption);
        entryOption.addClass('checked');
        return entryOption;
    }

    function copyToClipboard(selector) {
        selection.removeAllRanges();

        var copyText = document.querySelector(selector);

        copyText.select();
        copyText.setSelectionRange(0, 99999); /*For mobile devices*/
        document.execCommand("copy");
    }

    function facebookShare(btn, url) {
        url += '?source=FacebookShare';
        FB.ui({
            method: 'share',
            href: url
        }, function (response) {
            if (response) {
                checkEntry(btn);
                ALERTSYSTEM.Toastr.ShowInfo(MESSAGES.Translation['msgFacebookShareResponse'])
            }
        });
    }

    return {
        Init: init
    };
}());

$(function () {
    GIVEAWAYYOUAREIN.Init();
});