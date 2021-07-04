var JOBPROFILESELECTOR = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;

    function setSelectors() {
        selectors.container = '.content-wrapper';
        selectors.btnSetJobProfile = '.btnsetjobprofile';
    }

    function cacheObjects() {
        objs.container = $(selectors.container);
    }

    function init() {
        setSelectors();
        cacheObjects();

        canInteract = MAINMODULE.CanInteract();

        bindAll();
    }

    function bindAll() {
        bindBtnSetJobProfile();
    }

    function bindBtnSetJobProfile() {
        objs.container.on('click', selectors.btnSetJobProfile, function (e) {
            e.preventDefault();

            var btn = $(this);
            var url = btn.prop('href');

            if (canInteract) {
                setJobProfile(url);
            }

            return false;
        });
    }

    function setJobProfile(url, callback) {
        $.post(url).done(function (response) {
            if (response.success === true) {
                if (callback) {
                    callback(response);
                }

                MAINMODULE.Ajax.HandleUrlResponse(response);
            }
            else {
                ALERTSYSTEM.ShowWarningMessage(response.message);
            }
        });
    }

    return {
        Init: init
    };
}());

$(function () {
    JOBPROFILESELECTOR.Init();
});