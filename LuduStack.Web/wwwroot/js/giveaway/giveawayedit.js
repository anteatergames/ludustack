var GIVEAWAYEDIT = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;
    var isNew = false;

    var imagesToUploadCount = 5;

    var allImagesUploaded = false;

    var datetimePickerIcons = {
        time: "fa fa-clock",
        date: "fa fa-calendar",
        up: "fa fa-arrow-up",
        down: "fa fa-arrow-down"
    }

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.canInteract = '#caninteract';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.form = '#frmGiveawaySave';
        selectors.userId = '#UserId';
        selectors.startDate = 'input#StartDate';
        selectors.endDate = 'input#EndDate';
        selectors.btnClearEndDate = '#btnClearEndDate';
        selectors.btnSave = '#btnSaveGiveaway';
        selectors.dropzoneImages = '#dropzoneImages';
        selectors.inputImageListItem = 'input.imagelistitem';
        selectors.imageListItem = 'img.imagelistitem';
        selectors.btnImageDelete = '.btn-image-delete';
        selectors.featuredLabel = '.featuredlabel';
        selectors.featuredImage = '#FeaturedImage';
        selectors.btnDuplicate = '.btn-giveaway-duplicate';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.form = $(selectors.form);
        objs.userId = $(selectors.userId);
        objs.startDate = $(selectors.startDate);
        objs.endDate = $(selectors.endDate);
        objs.btnClearEndDate = $(selectors.btnClearEndDate);
        objs.txtAreaDescription = $(selectors.txtAreaDescription);
        objs.sortablePlanning = document.getElementById(selectors.sortablePlanning);
        objs.divPlans = $(selectors.divPlans);
        objs.divNoItems = $(selectors.divNoItems);
        objs.inputImageListItem = $(selectors.inputImageListItem);
        objs.imageListItem = $(selectors.imageListItem);
        objs.featuredLabel = $(selectors.featuredLabel);
        objs.featuredImage = $(selectors.featuredImage);
    }

    function init() {
        setSelectors();
        cacheObjs();

        calculateImagesToUpload();

        initFeaturedImage();

        bindAll();

        canInteract = $(selectors.canInteract).val();
        isNew = window.location.href.indexOf('add') > -1;

        if (isNew) {
            console.log('new giveaway');
        }

        MAINMODULE.Common.BindPopOvers();
    }

    function initFeaturedImage() {
        $.map(objs.imageListItem, function (obj, index) {
            var $this = $(obj);
            var src = $this.attr('src');

            var x = src.indexOf(objs.featuredImage.val());

            if (x > -1) {
                setImageFeatured($this);
            }
        });
    }

    function bindAll() {
        bindDuplicateGiveaway();
        bindDateTimePickers();
        bindImageClick();
        bindImageDeleteClick();
        bindBtnSaveForm();

        IMAGEMANIPULAION.Dropzone.Initialize(0, imagesToUploadCount, selectors.dropzoneImages, true, 'giveaway');

        bindDropZoneSuccess();
    }

    function bindDuplicateGiveaway() {
        objs.container.on('click', selectors.btnDuplicate, function (e) {
            e.preventDefault();

            var btn = $(this);

            MAINMODULE.Common.PostWithoutConfirmation(btn);

            return false;
        });
    }

    function bindDateTimePickers() {
        var defaultLocale = MAINMODULE.GetLocale() || window.navigator.userLanguage || window.navigator.language;

        var locale = moment.locale(defaultLocale);

        var startDateOptions = {
            icons: datetimePickerIcons,
            locale: locale,
            sideBySide: true
        };

        var endDateOptions = {
            icons: datetimePickerIcons,
            locale: locale,
            sideBySide: true
        };

        var sd = moment(objs.startDate.val(), 'L LT');
        var ed = sd.clone().add(1, 'd');
        startDateOptions.date = sd;

        objs.startDate.keypress(function (e) {
            e.preventDefault();
            ALERTSYSTEM.Toastr.ShowInfo('You need to select the date using<br>the button on the side.');
        }).datetimepicker(startDateOptions);

        objs.startDate.datetimepicker('defaultDate', sd);
        if (objs.endDate.val()) {
            endDateOptions.date = moment(objs.endDate.val(), 'L LT');
        }
        else {
            endDateOptions.minDate = ed;
        }

        objs.endDate.keypress(function (e) {
            e.preventDefault();
            ALERTSYSTEM.Toastr.ShowInfo('You need to select the date using<br>the button on the side.');
        }).datetimepicker(endDateOptions);

        if (objs.endDate.val()) {
            ed = moment(objs.endDate.val(), 'L LT');
            endDateOptions.minDate = ed;
            objs.endDate.datetimepicker(endDateOptions);
        }

        objs.startDate.on("change.datetimepicker", function (e) {
            objs.endDate.datetimepicker('minDate', e.date.clone().add(1, 'hours'));
        });

        objs.container.on('click', selectors.btnClearEndDate, function (e) {
            e.preventDefault();

            objs.endDate.val('');

            return false;
        });
    }

    function bindImageClick() {
        objs.container.on('click', selectors.imageListItem, function () {
            var img = $(this);

            setImageFeatured(img);
        });
    }

    function bindImageDeleteClick() {
        objs.container.on('click', selectors.btnImageDelete, function (e) {
            e.preventDefault();
            var btn = $(this);

            var img = btn.parent().find('img');

            if (img.hasClass('featured')) {
                objs.featuredImage.val('');
                var label = btn.parent().find(selectors.featuredLabel);
                label.addClass('d-none');
            }
            img.prop('src', img.data('placeholder'));
            img.removeClass('featured').addClass('default');

            var input = btn.parent().find('input[type=hidden]');
            input.val(img.data('placeholder'));
            input.addClass('default');

            btn.addClass('d-none');

            calculateImagesToUpload();

            return false;
        });
    }

    function bindDropZoneSuccess() {
        IMAGEMANIPULAION.Dropzone.Get(0).on("success", function (file) {
            var response = JSON.parse(file.xhr.response);
            if (response.uploaded) {
                var newUrl = objs.urls.data('urlImage').replace(/xpto/g, objs.userId.val());

                placeUploadedImage(newUrl + '/' + response.url);

                calculateImagesToUpload();
            }
            else {
                if (response.error) {
                    ALERTSYSTEM.ShowWarningMessage(response.error);
                }
            }
        });
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

                    IMAGEMANIPULAION.Dropzone.Get(0).on("success", function (file) {
                        var response = JSON.parse(file.xhr.response);
                        if (response.uploaded) {
                            allImagesUploaded = true;

                            placeUploadedImage(response.url);
                        }
                        else {
                            if (response.error) {
                                ALERTSYSTEM.ShowWarningMessage(response.error);
                            }
                        }
                    });

                    IMAGEMANIPULAION.Dropzone.Get(0).on("queuecomplete", function (file) {
                        if (allImagesUploaded === true) {
                            submitForm(btn).done(function () {
                                IMAGEMANIPULAION.Dropzone.Initialize(0, imagesToUploadCount, selectors.dropzoneImages, true);
                            });

                            console.log(file);
                        }
                    });
                }
            }
        });
    }

    function placeUploadedImage(imageUrl) {
        var freeSlot = objs.inputImageListItem.filter('.default').first();

        freeSlot.val(imageUrl);
        freeSlot.removeClass('default');

        var img = freeSlot.parent().find('img');
        img.attr('src', imageUrl);
        img.removeClass('default');

        if (img.hasClass('featured')) {
            objs.featuredImage.val(img.attr('src'));
        }

        var btnDelete = freeSlot.parent().find(selectors.btnImageDelete);
        btnDelete.removeClass('d-none');
    }

    function calculateImagesToUpload() {
        imagesToUploadCount = 0;
        objs.imageListItem.each(function () {
            var img = $(this);
            var placeholder = img.data('placeholder');
            var src = img.attr('src');

            if (src === placeholder) {
                imagesToUploadCount++;
            }

            if (IMAGEMANIPULAION.Dropzone.Get(0)) {
                IMAGEMANIPULAION.Dropzone.Get(0).options.maxFiles = imagesToUploadCount;
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

    function setImageFeatured(img) {
        objs.imageListItem.removeClass('featured');
        objs.featuredLabel.addClass('d-none');

        if (!img.hasClass('featured')) {
            img.addClass('featured');
            var label = img.parent().find('.featuredlabel');
            label.removeClass('d-none');
            objs.featuredImage.val(img.attr('src'));
        }
    }

    return {
        Init: init
    };
}());

$(function () {
    GIVEAWAYEDIT.Init();
});

Dropzone.autoDiscover = false;