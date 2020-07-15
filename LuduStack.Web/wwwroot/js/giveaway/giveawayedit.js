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
        selectors.inputImages = '#Images';
        selectors.dropzoneImages = '#dropzoneImages';
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
        objs.inputImages = $(selectors.inputImages);
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

        IMAGEMANIPULAION.Dropzone.Initialize(0, selectors.dropzoneImages);
    }

    function bindBtnSaveForm() {
        objs.container.on('click', selectors.btnSave, function () {
            var btn = $(this);
            var valid = objs.form.valid();

            if (valid && canInteract) {
                MAINMODULE.Common.DisableButton(btn);

                if (!IMAGEMANIPULAION.Dropzone.Get(0) || IMAGEMANIPULAION.Dropzone.Get(0).getQueuedFiles().length === 0) {
                    submitForm(btn);
                }
                else {
                    IMAGEMANIPULAION.Dropzone.Get(0).processQueue();

                    var success = false;

                    IMAGEMANIPULAION.Dropzone.Get(0).on("success", function (file) {
                        var response = JSON.parse(file.xhr.response);
                        if (response.uploaded) {
                            success = true;
                            objs.inputImages.val(objs.inputImages.val() + '|' + response.url);
                        }
                        else {
                            if (response.error) {
                                ALERTSYSTEM.ShowWarningMessage(response.error);
                            }
                        }
                    });

                    IMAGEMANIPULAION.Dropzone.Get(0).on("queuecomplete", function (file) {
                        if (success === true) {
                            submitForm(btn).done(function (response) {
                                console.log('done');

                                IMAGEMANIPULAION.Dropzone.Initialize(0, selectors.dropzoneImages);
                            });

                            console.log(file);
                        }
                    });
                }
            }
        });
    }

    function submitForm(btn) {
        var url = objs.form.attr('action');

        var data = objs.form.serializeObject();

        return $.post(url, data).done(function (response) {
            if (response.success === true) {
                MAINMODULE.Common.PostSaveCallback(response, btn);

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

Dropzone.autoDiscover = false;