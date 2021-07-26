var IMAGEMANIPULAION = (function () {
    "use strict";

    var croppers = [];
    var imagesProcessed = 0;

    var dropzones = [];
    var random = [];

    var selectors = {};

    function init() {
        console.log('IMAGEMANIPULAION init');

        selectors.inputImageListItem = 'input.imageinput';
        selectors.imageListItem = 'img.uploadimage';
    }

    // dropzone
    function instantiateDropZone(index, imagesToUploadCount, el, r, tag) {
        if (dropzones[index]) {
            dropzones[index].destroy();
            dropzones[index] = null;
        }

        random[index] = r;

        dropzones[index] = new Dropzone(el, {
            url: '/storage/uploadmedia',
            paramName: 'upload',
            addRemoveLinks: true,
            autoProcessQueue: true,
            maxFiles: imagesToUploadCount,
            parallelUploads: 2,
            resizeWidth: 720,
            resizeMethod: 'crop',
            acceptedFiles: 'image/*'
            //transformFile: transformFile
        });

        dropzones[index].on("processing", function () {
            this.options.autoProcessQueue = true;
        });

        dropzones[index].on("sending", function (file, xhr, formData) {
            if (random[index]) {
                formData.append("randomName", random[index]);
            }
            if (tag) {
                formData.append("tag", tag);
            }
        });

        dropzones[index].on('addedfile', function (file) {
            imagesToUploadCount = dropzones[index].options.maxFiles;

            if (file.type.indexOf('image') < 0) {
                ALERTSYSTEM.ShowWarningMessage(`You can only upload images!`);
            }
            else if (this.files.length > imagesToUploadCount) {
                this.removeFile(this.files[this.files.length - 1]);
                if (imagesToUploadCount === 0) {
                    ALERTSYSTEM.ShowWarningMessage(`You cannot upload more images.`);
                }
                else if (imagesToUploadCount === 1) {
                    ALERTSYSTEM.ShowWarningMessage(`You can only add ${imagesToUploadCount} more file.`);
                }
                else {
                    ALERTSYSTEM.ShowWarningMessage(`You can only add ${imagesToUploadCount} more files.`);
                }
            }
        });

        dropzones[index].on("complete", function (file) {
            dropzones[index].removeFile(file);
        });
    }

    function getDropZone(index) {
        return dropzones[index];
    }

    // cropper
    function bindCroppers(imageDivSelector) {
        var imageDivElements = document.querySelectorAll(`${imageDivSelector}`);

        for (var i = 0; i < imageDivElements.length; i++) {
            var imageElements = imageDivElements[i].querySelectorAll(`${selectors.imageListItem}`);
            var inputElements = imageDivElements[i].querySelectorAll(`${selectors.inputImageListItem}`);

            var ratioValue = NaN;
            if (imageElements[0].dataset.aspectratio !== undefined) {
                var ratio = imageElements[0].dataset.aspectratio.replace(' ', '').split('/');

                if (ratio !== undefined) {
                    ratioValue = parseInt(ratio[0]) / parseInt(ratio[1]);
                }
            }

            var cropper = new Cropper(imageElements[0], {
                aspectRatio: ratioValue,
                viewMode: 0,
                autoCropArea: 1,
                zoomOnWheel: false,
                modal: false,
                dragMode: 'move'
            });

            cropper.disabled = true;

            croppers.push(cropper);

            imageElements[0].dataset.cropperIndex = i;

            var parent = imageElements[0].closest('.newimageupload');
            var removeBtn = $(parent).find('.btn-remove-image');

            bindChangeImage(inputElements, removeBtn);

            bindRemoveImage(removeBtn);
        }
    }

    function bindChangeImage(inputElement, removeButtonObj) {
        $(inputElement).on('change', function (e) {
            var image = document.getElementById(e.target.dataset.targetImg);
            var extension = $(this).val().split('.').pop().toLowerCase();
            var isGif = extension === 'gif';

            if (isGif) {
                image.dataset.isgif = true;
            }

            var files = e.target.files;

            MAINMODULE.Utils.GetSelectedFileUrl(files, function (url2) {
                changeDone(url2, e.target, image, isGif);

                setRemoveHiddenInput(removeButtonObj, false);
            });
        });
    }

    function bindRemoveImage(removeButtonObj) {
        removeButtonObj.off('click');
        removeButtonObj.on('click', function (e) {
            var targetImgId = removeButtonObj.data('targetImg');
            var image = document.getElementById(targetImgId);
            if (image) {
                image.src = image.dataset.defaultImg;

                var cropper = croppers[image.dataset.cropperIndex];
                cropper.disabled = false;

                cropper.replace(image.dataset.defaultImg);
                cropper.disabled = true;

                setRemoveHiddenInput(removeButtonObj, true);
            }
        });
    }

    function setRemoveHiddenInput(removeButtonObj, booleanValue) {
        var booleanInput = removeButtonObj.find('.removeimage');
        booleanInput.val(booleanValue ? 'True' : 'False');
    }

    function changeDone(blobUrl, inputElement, image, isGif) {
        image.src = blobUrl;

        inputElement.dataset.changed = true;

        var cropper = croppers[image.dataset.cropperIndex];
        cropper.disabled = false;

        cropper.replace(blobUrl);

        if (isGif) {
            cropper.disabled = true;
        }
    }

    function uploadCroppedImages(imageDivSelector, callback) {
        imagesProcessed = 0;
        var imageDivElements = document.querySelectorAll(`${imageDivSelector}`);
        var changedInputs = Array.from(document.querySelectorAll(`${selectors.inputImageListItem}`)).filter(x => x.dataset.changed === 'true');
        console.log(changedInputs);

        var totalImageCont = imageDivElements.length;

        for (var i = 0; i < imageDivElements.length; i++) {
            var inputElements = imageDivElements[i].querySelectorAll(`${selectors.inputImageListItem}`);
            var element = inputElements[0];

            processImage(element, totalImageCont, callback);
        }
    }

    function processImage(inputElement, totalImageCont, callback) {
        var changed = inputElement.dataset.changed === 'true';

        if (!changed) {
            imagesProcessed++;
            console.log('skipping...');

            if (totalImageCont === imagesProcessed && callback) {
                callback();
            }
        }
        else {
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
            formData.append('userId', $('#UserId').val());

            formData.append('upload', uploadValue);

            formData.append("randomName", true);

            uploadImage(formData, totalImageCont, hidden, callback);
        }
    }

    function uploadImage(formData, totalImageCont, hidden, callback) {
        return $.ajax('/storage/uploadmedia', {
            method: "POST",
            data: formData,
            async: true,
            processData: false,
            contentType: false,
            success: function (response) {
                imagesProcessed++;
                hidden.value = response.filename;

                if (totalImageCont === imagesProcessed && callback) {
                    callback();
                }
            },
            error: function (response) {
                console.log(response);
                imgFeaturedImage.src = initialUrl;
            }
        });
    }

    return {
        Init: init,
        Cropper: {
            BindCropper: bindCroppers,
            UploadCroppedImages: uploadCroppedImages
        },
        Dropzone: {
            Initialize: instantiateDropZone,
            Get: getDropZone
        }
    };
}());

$(function () {
    IMAGEMANIPULAION.Init();
});