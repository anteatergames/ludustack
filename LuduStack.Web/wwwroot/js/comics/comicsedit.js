var COMICSEDIT = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;
    var isNew = false;

    var cropper;
    var canvas;
    var initialUrl;
    var cropped = false;

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
        selectors.dropzoneImages = '#dropzoneImages';
        selectors.inputImageListItem = 'input.imagelistitem';
        selectors.imageListItem = 'img.imagelistitem';
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
        bindCropper();
        bindDateTimePickers();
        bindBtnSaveForm();
    }

    function bindCropper() {
        var imgFeaturedImage = document.getElementById('imgFeaturedImage');
        var image = document.getElementById('image');
        var input = document.getElementById('featuredimage');

        input.addEventListener('change', function (e) {
            if (cropper) {
                cropper.destroy();
            }

            var files = e.target.files;
            var done = function (url2) {
                input.value = '';

                image.src = url2;

                objs.modalCrop.modal('show');
            };
            var reader;
            var file;

            if (files && files.length > 0) {
                file = files[0];

                if (URL) {
                    done(URL.createObjectURL(file));
                } else if (FileReader) {
                    reader = new FileReader();
                    reader.onloadend = function (e2) {
                        done(reader.result);
                    };
                    reader.readAsDataURL(file);
                }
            }
        });

        objs.modalCrop.on('shown.bs.modal', function () {
            cropper = new Cropper(image, {
                aspectRatio: 40 / 21,
                viewMode: 3,
                autoCropArea: 1
            });
        });

        document.getElementById('crop').addEventListener('click', function () {
            cropped = true;
            if (cropper) {
                canvas = cropper.getCroppedCanvas({
                    width: 1200,
                    minWidth: 1200
                });
                initialUrl = imgFeaturedImage.src;
                imgFeaturedImage.src = canvas.toDataURL();
            }

            objs.modalCrop.modal('hide');
        });
    }

    function uploadCroppedImage(callback) {
        if (cropper) {
            if (canvas) {
                canvas.toBlob(function (blob) {
                    var formData = new FormData();

                    formData.append('userId', objs.userId.val());

                    formData.append('featuredimage', blob);

                    $.ajax('/storage/uploadfeaturedimage', {
                        method: "POST",
                        data: formData,
                        processData: false,
                        contentType: false,
                        success: function (response) {
                            $('#FeaturedImage').val(response.imageUrl);
                            if (callback) {
                                callback();

                                cropper.destroy();
                                cropper = null;
                            }
                        },
                        error: function (response) {
                            imgFeaturedImage.src = initialUrl;
                        }
                    });
                });
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

                console.log(cropped);
                if (cropped) {
                    uploadCroppedImage(function () {
                        submitForm(btn);
                    });
                }
                else {
                    submitForm(btn);
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