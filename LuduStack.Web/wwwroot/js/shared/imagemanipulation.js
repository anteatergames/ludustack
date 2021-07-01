var IMAGEMANIPULAION = (function () {
    "use strict";

    var croppers = [];
    var imagesProcessed = 0;

    var dropzones = [];
    var random = [];

    function init() {
        console.log('IMAGEMANIPULAION init');
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
    function bindCroppers(imageListItemSelector, objs) {
        var images = document.querySelectorAll(imageListItemSelector);

        for (var i = 0; i < images.length; i++) {
            var ratioValue = NaN;
            if (images[i].dataset.aspectratio !== undefined) {
                var ratio = images[i].dataset.aspectratio.replace(' ', '').split('/');

                if (ratio !== undefined) {
                    ratioValue = parseInt(ratio[0]) / parseInt(ratio[1]);
                }
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

        bindChangeImage(objs)
    }

    function bindChangeImage(objs) {
        objs.on('change', function (e) {
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

    function uploadCroppedImages(objs, callback) {
        var imagesChanged = objs.filter(function (index) {
            return objs[index].dataset.changed === 'true';
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
            formData.append('userId', $('#UserId').val());

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