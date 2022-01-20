var GAMEIDEAEDIT = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.form = '#frmSave';
        selectors.userId = '#UserId';
        selectors.txtDescription = '#Description';
        selectors.btnSave = '#btnSave';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.form = $(selectors.form);
        objs.userId = $(selectors.userId);
        objs.btnSave = $(selectors.btnSave);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        canInteract = MAINMODULE.CanInteract();
    }

    function bindAll() {
        bindBtnSaveForm();
        bindEnterPress();
    }

    function bindBtnSaveForm() {
        objs.container.on('click', selectors.btnSave, function (e) {
            var btn = $(this);
            var valid = objs.form.valid();

            if (valid && canInteract && !btn.hasClass('disabled')) {
                MAINMODULE.Common.DisableButton(btn).ready(() => {
                    submitForm(btn);
                });
            }
        });
    }

    function submitForm(btn) {
        var url = objs.form.attr('action');

        var data = objs.form.serializeObject();

        return $.post(url, data).done(function (response) {
            if (response.success === true) {
                MAINMODULE.Common.PostSaveCallback(response, btn);

                MAINMODULE.Ajax.HandleUrlResponse(response);
            }
            else {
                MAINMODULE.Ajax.HandleErrorResponse(response);
                MAINMODULE.Common.EnableButton(btn)
            }
        });
    }

    function bindEnterPress() {
        objs.container.on('keypress', selectors.txtDescription, function (e) {
            if (e.which === 13) {
                objs.btnSave.click();
            }
        });
    }

    return {
        Init: init
    };
}());

$(function () {
    GAMEIDEAEDIT.Init();
});