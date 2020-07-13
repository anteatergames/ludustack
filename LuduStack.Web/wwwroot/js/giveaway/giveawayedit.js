var GIVEAWAYEDIT = (function () {
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
        selectors.form = '#frmGiveawaySave';
        selectors.userId = '#userId';
        selectors.btnSave = '#btnSaveGiveaway';
        selectors.modalCrop = '#modalCrop';
        selectors.inputFeaturedImage = '#FeaturedImage';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.form = $(selectors.form);
        objs.userId = $(selectors.userId);
        objs.txtAreaDescription = $(selectors.txtAreaDescription);
        objs.sortablePlanning = document.getElementById(selectors.sortablePlanning);
        objs.divPlans = $(selectors.divPlans);
        objs.divNoItems = $(selectors.divNoItems);
        objs.modalCrop = $(selectors.modalCrop);
        objs.inputFeaturedImage = $(selectors.inputFeaturedImage);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        canInteract = $(selectors.canInteract).val();
        isNew = window.location.href.indexOf('add') > -1;

        if (isNew) {
            console.log('new giveaway');
        }

        MAINMODULE.Common.BindPopOvers();
    }

    function bindAll() {
        bindBtnSaveForm();

        IMAGECROP.BindCropper(0, objs.modalCrop);
    }

    function bindBtnSaveForm() {
        objs.container.on('click', selectors.btnSave, function () {
            var btn = $(this);
            var valid = objs.form.valid();

            if (valid && canInteract) {
                MAINMODULE.Common.DisableButton(btn);

                if (IMAGECROP.Cropped[0]) {
                    IMAGECROP.UploadCroppedImage(0, objs.inputFeaturedImage, objs.userId.val(),function () {
                        submitForm(btn);
                    });
                }
                else {
                    submitForm(btn);
                }
            }
        });
    }

    function submitForm(btn, callback) {
        var url = objs.form.attr('action');

        var data = objs.form.serializeObject();

        $.post(url, data).done(function (response) {
            if (response.success === true) {
                MAINMODULE.Common.PostSaveCallback(response, btn);

                if (callback) {
                    callback();
                }

                ALERTSYSTEM.ShowSuccessMessage("Awesome!", function (isConfirm) {
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
    GIVEAWAYEDIT.Init();
});