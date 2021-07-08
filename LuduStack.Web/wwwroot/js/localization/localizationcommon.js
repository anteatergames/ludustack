var LOCALIZATIONCOMMON = (function () {
    "use strict";

    var selectors = {};
    var objs = {};
    var canInteract = false;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';

        selectors.btnDelete = '.btnDeleteTranslationProject';
    }

    function cacheObjsTranslate() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
    }

    function init() {
        setSelectors();
        cacheObjsTranslate();

        canInteract = MAINMODULE.CanInteract();

        bindTranslate();

        setStickyElements();
    }

    function bindTranslate() {
        bindDeleteProject();
        CONTENTACTIONS.BindShareContent();
        MAINMODULE.Common.BindPopOvers();
    }

    function bindDeleteProject() {
        objs.container.on('click', selectors.btnDelete, function (e) {
            e.preventDefault();

            var btn = $(this);

            if (canInteract) {
                deleteProject(btn);
            }

            return false;
        });
    }

    function deleteProject(btn, callback) {
        var url = btn.data('url');

        var msgs = MAINMODULE.Common.GetPostConfirmationMessages(btn);

        ALERTSYSTEM.ShowConfirmMessage(msgs.confirmationTitle, msgs.msg, msgs.confirmationButtonText, msgs.cancelButtonText, function () {
            $.ajax({
                url: url,
                type: 'DELETE'
            }).done(function (response) {
                if (response.success) {
                    if (callback) {
                        callback(response);
                    }

                    MAINMODULE.Common.HandleSuccessDefault(response);
                }
                else {
                    ALERTSYSTEM.ShowWarningMessage(response.message);
                }
            });
        });
    }

    function setStickyElements() {
        MAINMODULE.Layout.SetStickyElement('#divTranslationSelector', 50, '#divTranslationSelector');
    }

    return {
        Init: init
    };
}());

$(function () {
    LOCALIZATIONCOMMON.Init();
});