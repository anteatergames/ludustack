var COMICSEDIT = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;
    var isNew = false;

    var croppers = [];

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
        selectors.form = '#frmComicsSave';
        selectors.userId = '#UserId';
        selectors.publishDate = 'input#PublishDate';
        selectors.btnSave = '#btnSaveComics';
        selectors.dropzone = '.dropzone';
        selectors.inputImageListItem = 'input.comicstripimageinput';
        selectors.imageListItem = 'img.comicstripimage';
        selectors.btnImageDelete = '.btn-image-delete';
        selectors.featuredLabel = '.featuredlabel';
        selectors.featuredImage = '#FeaturedImage';
        selectors.modalCrop = '#modalCrop';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.form = $(selectors.form);
        objs.userId = $(selectors.userId);
        objs.publishDate = $(selectors.publishDate);
        objs.txtAreaDescription = $(selectors.txtAreaDescription);
        objs.sortablePlanning = document.getElementById(selectors.sortablePlanning);
        objs.divPlans = $(selectors.divPlans);
        objs.divNoItems = $(selectors.divNoItems);
        objs.inputImageListItem = $(selectors.inputImageListItem);
        objs.imageListItem = $(selectors.imageListItem);
        objs.featuredLabel = $(selectors.featuredLabel);
        objs.featuredImage = $(selectors.featuredImage);
        objs.modalCrop = $(selectors.modalCrop);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        canInteract = $(selectors.canInteract).val();
        isNew = window.location.href.indexOf('add') > -1;

        if (isNew) {
            console.log('new comic strip');
        }

        MAINMODULE.Common.BindPopOvers();
    }

    function bindAll() {
        bindDateTimePickers();
        bindBtnSaveForm();

        bindChangeImage();

        bindCropper();
    }

    function bindChangeImage() {
        for (var i = 0; i < objs.inputImageListItem.length; i++) {
            var element = objs.inputImageListItem[i];

            element.addEventListener('change', function (e) {
                var image = document.getElementById(e.target.dataset.targetImg);

                var files = e.target.files

                var done = function (url2) {
                    element.value = '';

                    croppers[image.dataset.cropperIndex].replace(url2);

                    image.src = url2;

                    e.target.dataset.changed = true;
                };

                MAINMODULE.Utils.GetSelectedFileUrl(files, done);
            });
        }
    }

    function bindCropper() {
        var images = document.querySelectorAll(selectors.imageListItem);
        var i;

        for (i = 0; i < images.length; i++) {
            croppers.push(new Cropper(images[i], {
                aspectRatio: 16 / 9,
                viewMode: 3,
                autoCropArea: 1,
                zoomOnWheel: false
            }));

            images[i].dataset.cropperIndex = i;
        }
    }

    function uploadCroppedImages(callback) {
        var imagesProcessed = 0;

        var imagesChanged = objs.inputImageListItem.filter(function (index) {
            return objs.inputImageListItem[index].dataset.changed === 'true';
        });

        var imagesToProcessCount = imagesChanged.length;

        if (imagesChanged.length > 0) {
            for (var i = 0; i < imagesToProcessCount; i++) {
                var element = imagesChanged[i];
                var changed = element.dataset.changed === 'true';

                if (!changed) {
                    console.log('skipping...');
                    imagesProcessed++;
                    continue;
                }
                console.log('uploading...');

                var image = document.getElementById(element.dataset.targetImg);
                var hidden = document.getElementById(element.dataset.targetHidden);

                var cropper = croppers[image.dataset.cropperIndex];

                var canvas = cropper.getCroppedCanvas();

                var dataUri = canvas.toDataURL();

                var blob = MAINMODULE.Utils.DataURItoBlob(dataUri);

                var formData = new FormData();
                formData.append('userId', objs.userId.val());

                formData.append('upload', blob);

                formData.append("randomName", true);

                $.ajax('/storage/uploadcontentimage', {
                    method: "POST",
                    data: formData,
                    async: false,
                    processData: false,
                    contentType: false,
                    success: function (response) {
                        imagesProcessed++;
                        hidden.value = response.url;

                        console.log(imagesToProcessCount);
                        console.log(imagesProcessed);

                        if (imagesProcessed === imagesToProcessCount) {
                            if (callback) {
                                callback();
                            }
                        }
                    },
                    error: function (response) {
                        imgFeaturedImage.src = initialUrl;
                    }
                });
            }
        }
        else {
            if (callback) {
                callback();
            }
        }
    }

    function bindDateTimePickers() {
        var defaultLocale = MAINMODULE.GetLocale() || window.navigator.userLanguage || window.navigator.language;

        var locale = moment.locale(defaultLocale);

        var publishDateOptions = {
            icons: datetimePickerIcons,
            locale: locale,
            sideBySide: true
        };

        var sd = moment(objs.publishDate.val(), 'L LT');
        publishDateOptions.date = sd;

        objs.publishDate.keypress(function (e) {
            e.preventDefault();
            ALERTSYSTEM.Toastr.ShowInfo('You need to select the date using<br>the button on the side.');
        }).datetimepicker(publishDateOptions);

        objs.publishDate.datetimepicker('defaultDate', sd);
    }

    function bindBtnSaveForm() {
        objs.container.on('click', selectors.btnSave, function () {
            var btn = $(this);
            var valid = objs.form.valid();

            if (valid && canInteract) {
                MAINMODULE.Common.DisableButton(btn);

                uploadCroppedImages(function () {
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
    COMICSEDIT.Init();
});