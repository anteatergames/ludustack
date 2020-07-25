var MESSAGES = (function () {
    "use strict";

    var selectors = {};
    var objs = {};
    var getTranslatedMessage = {};

    function init() {
        setSelectors();
        cacheObjects();

        bindAll();

        loadTranslatedMessages();

        showMessage();
    }

    function setSelectors() {
        selectors.locale = '#locale';
        selectors.spanMessage = "#spanMessage";
        selectors.translatedJavascriptMessages = "#translatedJavascriptMessages";
    }

    function cacheObjects() {
        objs.locale = $(selectors.locale);
        objs.spanMessage = $(selectors.spanMessage);
        objs.translatedJavascriptMessages = $(selectors.translatedJavascriptMessages);
    }

    function bindAll() {
        bindNotImplemented();
        bindYouNeedToLogIn();
    }

    function loadTranslatedMessages() {
        objs.translatedJavascriptMessages.find('.msg').each(function () {
            var msgId = $(this).data('msgId');
            var text = $(this).text();

            getTranslatedMessage[msgId] = text;
        });
    }

    function showMessage() {
        var msg = objs.spanMessage.text();
        if (msg !== undefined && msg.length > 0) {
            ALERTSYSTEM.Toastr.ShowWarning(msg);
            history.replaceState({}, null, window.location.href.split('?')[0]);
        }
    }

    function bindNotImplemented() {
        $('body').on('click', '.notimplemented', function (e) {
            e.preventDefault();

            var msg = getTranslatedMessage['msgNotImplementedYet'];

            ALERTSYSTEM.Toastr.ShowWarning(msg);

            return false;
        });
    }

    function bindYouNeedToLogIn() {
        $('body').on('click', '.needlogin', function (e) {
            e.preventDefault();
            var msgId = $(this).data('msgId');

            var msg = getTranslatedMessage[msgId];

            ALERTSYSTEM.Toastr.ShowWarning(msg);

            return false;
        });
    }

    function getLocale() {
        if (!objs.locale) {
            objs.locale = $(selectors.locale);
        }

        return objs.locale.val();
    }

    return {
        Init: init,
        GetLocale: getLocale,
        Translation: getTranslatedMessage
    };
}());

MESSAGES.Init();