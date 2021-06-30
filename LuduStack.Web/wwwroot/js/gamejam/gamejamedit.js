var GAMEJAMEDIT = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;
    var isNew = false;

    var datetimePickerIcons = {
        time: "fa fa-clock",
        date: "fa fa-calendar",
        up: "fa fa-arrow-up",
        down: "fa fa-arrow-down"
    }

    var croppers = [];
    var imagesProcessed = 0;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.canInteract = '#caninteract';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.form = '#frmSave';
        selectors.userId = '#UserId';
        selectors.btnSave = '#btnSave';
        selectors.startDate = 'input#StartDate';
        selectors.entryDeadline = 'input#EntryDeadline';
        selectors.votingEndDate = 'input#VotingEndDate';
        selectors.resultDate = 'input#ResultDate';
        selectors.inputImageListItem = 'input.imageinput';
        selectors.imageListItem = 'img.uploadimage';
        selectors.featuredImage = '#FeaturedImage';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.form = $(selectors.form);
        objs.userId = $(selectors.userId);
        objs.startDate = $(selectors.startDate);
        objs.entryDeadline = $(selectors.entryDeadline);
        objs.votingEndDate = $(selectors.votingEndDate);
        objs.resultDate = $(selectors.resultDate);
        objs.inputImageListItem = $(selectors.inputImageListItem);
        objs.featuredImage = $(selectors.featuredImage);
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

        MAINMODULE.Common.BindPopOvers();
    }

    function bindAll() {
        bindDateTimePickers();

        WYSIWYGEDITOR.BindEditors('.wysiwygeditor');

        bindCropper();
        bindChangeImage();

        bindBtnSaveForm();
    }

    function bindDateTimePickers() {
        var defaultLocale = MAINMODULE.GetLocale() || window.navigator.userLanguage || window.navigator.language;

        var locale = moment.locale(defaultLocale);

        var options = {
            icons: datetimePickerIcons,
            locale: locale,
            sideBySide: true
        };

        bindDateTimePicker(objs.startDate, options);
        bindDateTimePicker(objs.entryDeadline, options);
        bindDateTimePicker(objs.votingEndDate, options);
        bindDateTimePicker(objs.resultDate, options);
    }

    function bindDateTimePicker(obj, options) {
        var pd = moment(obj.val(), 'L LT');
        options.date = pd;

        obj.keypress(function (e) {
            e.preventDefault();
            ALERTSYSTEM.Toastr.ShowInfo('You need to select the date using<br>the calendar button on the side.');
        }).datetimepicker(options);

        obj.off('change.datetimepicker').on('change.datetimepicker', function (e) {
            e.preventDefault();

            if (typeof e.oldDate !== 'undefined' && typeof e.date !== 'undefined' && (obj.val() === e.date._i || e.oldDate._i === e.date._i)) {

                var selectedDate = moment(obj.val(), 'L LT');

                var maxDateDateTarget = $(this).attr('data-maxdate');
                var maxDateDateElement = $(maxDateDateTarget);

                maxDateDateElement.datetimepicker('minDate', selectedDate.add(1, 'd'));
            }

            return false;
        });

        obj.datetimepicker('defaultDate', pd);
    }

    function bindCropper() {
        var images = document.querySelectorAll(selectors.imageListItem);

        for (var i = 0; i < images.length; i++) {

            var ratioValue = NaN;
            var ratio = images[i].dataset.aspectratio?.replace(' ', '').split('/');

            if (ratio !== undefined) {
                ratioValue = parseInt(ratio[0]) / parseInt(ratio[1]);
            }
            var cropper = new Cropper(images[i], {
                aspectRatio: ratioValue,
                viewMode: 0,
                autoCropArea: 1,
                zoomOnWheel: false,
                modal: false,
                dragMode: 'move'
            });

            cropper.disabled = true;

            croppers.push(cropper);

            images[i].dataset.cropperIndex = i;
        }
    }

    function bindChangeImage() {
        objs.inputImageListItem.on('change', function (e) {
            var image = document.getElementById(e.target.dataset.targetImg);
            var cropper = croppers[image.dataset.cropperIndex];
            var extension = $(this).val().split('.').pop().toLowerCase();
            var isGif = extension === 'gif';

            if (isGif) {
                image.dataset.isgif = true;
                //cropper.destroy();
            }

            var files = e.target.files;

            MAINMODULE.Utils.GetSelectedFileUrl(files, function (url2) {
                changeDone(url2, e.target, image, isGif);
            });
        });
    }

    function bindBtnSaveForm() {
        objs.container.on('click', selectors.btnSave, function () {
            var btn = $(this);

            updateEditors();

            var valid = objs.form.valid();

            if (valid && canInteract) {
                MAINMODULE.Common.DisableButton(btn);

                uploadCroppedImages(function () {
                    submitForm(btn);
                });
            }
        });
    }

    function updateEditors() {
        $('.wysiwygeditor').each((index, element) => {
            var editorId = $(element).attr('data-editor-id');
            WYSIWYGEDITOR.UpdateSourceElement(editorId);
        });
    }

    function changeDone(blobUrl, inputElement, image, isGif) {
        //inputElement.value = '';
        image.src = blobUrl;

        inputElement.dataset.changed = true;

        var cropper = croppers[image.dataset.cropperIndex];
        cropper.disabled = false;

        cropper.replace(blobUrl);

        if (isGif) {
            cropper.disabled = true;
        }
    }

    function uploadCroppedImages(callback) {
        var imagesChanged = objs.inputImageListItem.filter(function (index) {
            return objs.inputImageListItem[index].dataset.changed === 'true';
        });

        var imagesToProcessCount = imagesChanged.length;

        if (imagesChanged.length > 0) {
            processImages(imagesChanged, imagesToProcessCount, callback);
        }
        else {
            if (callback) {
                callback();
            }
        }
    }

    function processImages(imagesChanged, imagesToProcessCount, callback) {
        imagesProcessed = 0;

        for (var i = 0; i < imagesToProcessCount; i++) {
            var inputElement = imagesChanged[i];
            var changed = inputElement.dataset.changed === 'true';

            if (!changed) {
                console.log('skipping...');
                imagesProcessed++;
                continue;
            }

            var image = document.getElementById(inputElement.dataset.targetImg);
            var hidden = document.getElementById(inputElement.dataset.targetHidden);

            var cropper = croppers[image.dataset.cropperIndex];

            var uploadValue = inputElement.files[0];

            if (image.dataset.isgif !== 'true') {
                var canvas = cropper.getCroppedCanvas();

                var dataUri = canvas.toDataURL();

                uploadValue = MAINMODULE.Utils.DataURItoBlob(dataUri);
            }

            var formData = new FormData();
            formData.append('userId', objs.userId.val());

            formData.append('upload', uploadValue);

            formData.append("randomName", true);

            uploadImage(formData, imagesToProcessCount, hidden, callback);
        }
    }

    function uploadImage(formData, imagesToProcessCount, hidden, callback) {
        $.ajax('/storage/uploadmedia', {
            method: "POST",
            data: formData,
            async: false,
            processData: false,
            contentType: false,
            success: function (response) {
                imagesProcessed++;
                hidden.value = response.filename;

                if (imagesProcessed === imagesToProcessCount) {
                    if (callback) {
                        callback();
                    }
                }
            },
            error: function (response) {
                console.log(response);
                imgFeaturedImage.src = initialUrl;
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
    GAMEJAMEDIT.Init();
});

$.validator.setDefaults({
    ignore: ":hidden:not(.wysiwygeditor)"
});