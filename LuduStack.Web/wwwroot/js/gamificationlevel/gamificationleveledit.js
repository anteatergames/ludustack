var GAMIFICATIONLEVELEDIT = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;
    var isNew = false;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.canInteract = '#caninteract';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.form = '#frmSave';
        selectors.userId = '#UserId';
        selectors.btnSave = '#btnSave';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.form = $(selectors.form);
        objs.userId = $(selectors.userId);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        canInteract = $(selectors.canInteract).val();
        isNew = window.location.href.indexOf('add') > -1;

        if (isNew) {
            console.log('is New');
        }
    }

    function bindAll() {
        bindBtnSaveForm();
    }

    function bindBtnSaveForm() {
        objs.container.on('click', selectors.btnSave, function () {
            var btn = $(this);
            var valid = objs.form.valid();

            if (valid && canInteract) {
                MAINMODULE.Common.DisableButton(btn);

                submitForm(btn);
            }
        });
    }

    function submitForm(btn) {
        var url = objs.form.attr('action');

        var data = objs.form.serializeObject();

        return $.post(url, data).done(function (response) {
            if (response.success === true) {
                MAINMODULE.Common.PostSaveCallback(response, btn);

                ALERTSYSTEM.ShowSuccessMessage("Awesome!", function () {
                    window.location = response.url;
                });
            }
            else {
                ALERTSYSTEM.ShowWarningMessage("An error occurred! Check the console!");
            }
        });
    }

    return {
        Init: init
    };
}());

$(function () {
    GAMIFICATIONLEVELEDIT.Init();
});