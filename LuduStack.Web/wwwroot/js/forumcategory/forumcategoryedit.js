var FORUMCATEGORYEDIT = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;
    var isNew = false;

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

        canInteract = $(selectors.canInteract).val();
        isNew = window.location.href.indexOf('add') > -1;

        if (isNew) {
            console.log('is New');
        }
    }

    function bindAll() {
        bindCropper();
        bindChangeImage();
        bindBtnSaveForm();
    }

    function bindCropper() {
        var images = document.querySelectorAll(selectors.imageListItem);

        for (var i = 0; i < images.length; i++) {
            croppers.push(new Cropper(images[i], {
                aspectRatio: 40 / 21,
                viewMode: 3,
                autoCropArea: 1,
                zoomOnWheel: false
            }));

            images[i].dataset.cropperIndex = i;
        }
    }

    function bindChangeImage() {
        objs.inputImageListItem.on('change', function (e) {
            var image = document.getElementById(e.target.dataset.targetImg);

            var files = e.target.files;

            MAINMODULE.Utils.GetSelectedFileUrl(files, function (url2) {
                changeDone(url2, e.target, image);
            });
        });
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

    function changeDone(url2, element, image) {
        element.value = '';

        croppers[image.dataset.cropperIndex].replace(url2);

        image.src = url2;

        element.dataset.changed = true;
    };

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
            var element = imagesChanged[i];
            var changed = element.dataset.changed === 'true';

            if (!changed) {
                console.log('skipping...');
                imagesProcessed++;
                continue;
            }

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
    FORUMCATEGORYEDIT.Init();
});