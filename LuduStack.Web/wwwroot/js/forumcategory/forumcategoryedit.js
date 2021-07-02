var FORUMCATEGORYEDIT = (function () {
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
        selectors.btnSave = '#btnSave';
        selectors.inputImageListItem = 'input.featuredimageinput';
        selectors.imageListItem = 'img.featuredimage';
        selectors.featuredImage = '#FeaturedImage';
        selectors.modalCrop = '#modalCrop';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.form = $(selectors.form);
        objs.userId = $(selectors.userId);
        objs.inputImageListItem = $(selectors.inputImageListItem);
        objs.featuredImage = $(selectors.featuredImage);
        objs.modalCrop = $(selectors.modalCrop);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        canInteract = MAINMODULE.CanInteract();
    }

    function bindAll() {
        IMAGEMANIPULAION.Cropper.BindCropper(selectors.imageListItem, objs.inputImageListItem);

        bindBtnSaveForm();
    }

    function bindBtnSaveForm() {
        objs.container.on('click', selectors.btnSave, function () {
            var btn = $(this);
            var valid = objs.form.valid();

            if (valid && canInteract) {
                MAINMODULE.Common.DisableButton(btn).ready(() => {
                    IMAGEMANIPULAION.Cropper.UploadCroppedImages(objs.inputImageListItem, function () {
                        submitForm(btn);
                    });
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
            }
        });
    }

    return {
        Init: init
    };
}());

$(function () {
    FORUMCATEGORYEDIT.Init();
});