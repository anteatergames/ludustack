var MODULETEMPLATE = (function () {
    "use strict";

    var selectors = {};
    var objs = {};
    var canInteract = false;
    var isIndex = false;
    var isNew = false;
    var isDetails = false;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.containerDetails = '#containerdetails';
        selectors.containerList = '#containerlist';
        selectors.list = '#divList';
        selectors.divListItem = '.feature-item';
        selectors.btnNew = '#btn-new';
        selectors.form = '#frmID';
        selectors.btnSave = '#btnID';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.containerDetails = $(selectors.containerDetails);
        objs.containerList = $(selectors.containerList);
        objs.list = $(selectors.list);
    }

    function cacheObjectsCreateEdit() {
        // cache objects exclusive to the created/edit mode
    }

    function setCreateEdit() {
        cacheObjectsCreateEdit();
    }

    function init() {
        setSelectors();
        cacheObjs();

        canInteract = MAINMODULE.CanInteract();
        isNew = COMMONEDIT.IsNew();
        isDetails = window.location.href.indexOf('details') > -1;
        isIndex = !isNew && !isDetails;

        bindAll();

        if (isIndex) {
            var url = objs.urls.data('urlList');
            loadItems(false, url);
        }
        else if (isDetails) {
            bindDetails();
        }
    }

    function bindAll() {
        bindBtnNew();
        bindBtnSaveForm();
    }

    function bindBtnNew() {
        objs.container.on('click', selectors.btnNew, function () {
            var url = $(this).data('url');
            if (canInteract) {
                loadNewForm(url);
            }
        });
    }

    function bindDetails() {
        CONTENTACTIONS.BindShareContent();
    }

    function bindBtnSaveForm() {
        objs.containerDetails.on('click', selectors.btnSave, function () {
            var btn = $(this);
            var valid = objs.form.valid();

            if (valid && canInteract && !btn.hasClass('disabled')) {
                MAINMODULE.Common.DisableButton(btn).ready(() => {
                    submitForm(btn);
                });
            }
        });
    }

    function loadItems(fromControlSidebar, url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.list).then(() => {
            cacheObjects();
        });
    }

    function loadNewForm(url) {
        objs.containerList.hide();
        MAINMODULE.Ajax.LoadHtml(url, objs.containerDetails).then(() => {
            objs.containerDetails.show();

            objs.form = $(selectors.form);

            $.validator.unobtrusive.parse(selectors.form);
            setCreateEdit();
        });
    }

    function submitForm(btn, callback) {
        var url = objs.form.attr('action');

        var data = objs.form.serialize();

        $.post(url, data).done(function (response) {
            if (response.success === true) {
                MAINMODULE.Common.PostSaveCallback(response, btn);

                if (callback) {
                    callback();
                }

                MAINMODULE.Ajax.HandleUrlResponse(response);
            }
            else {
                MAINMODULE.Ajax.HandleErrorResponse(response);
            }
        });
    }

    return {
        Init: init
    };
}());

$(function () {
    MODULETEMPLATE.Init();
});