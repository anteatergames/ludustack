var IMAGEMANIPULAION = (function () {
    "use strict";

    var cropped = [];
    var cropper = [];
    var canvas = [];
    var initialUrl = [];

    var dropzones = [];
    var random = [];

    function init() {
        console.log('IMAGEMANIPULAION init');
    }

    function bindCropper(index, modalCrop) {
        var imgFeaturedImage = document.getElementById('imgFeaturedImage');
        var image = document.getElementById('image');
        var input = document.getElementById('featuredimage');

        input.addEventListener('change', function (e) {
            if (cropper[index]) {
                cropper[index].destroy();
            }

            var files = e.target.files;
            var done = function (url2) {
                input.value = '';

                image.src = url2;

                modalCrop.modal('show');
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

        modalCrop.on('shown.bs.modal', function () {
            cropper[index] = new Cropper(image, {
                aspectRatio: 16 / 9,
                viewMode: 3,
                autoCropArea: 1
            });
        });

        document.getElementById('crop').addEventListener('click', function () {
            cropped[index] = true;
            if (cropper[index]) {
                canvas[index] = cropper[index].getCroppedCanvas({
                    width: 1200,
                    minWidth: 1200
                });
                initialUrl[index] = imgFeaturedImage.src;
                imgFeaturedImage.src = canvas[index].toDataURL();
            }

            modalCrop.modal('hide');
        });
    }

    function uploadCroppedImage(index, inputImage, userId, callback) {
        console.log(cropper[index]);
        if (cropper[index]) {
            if (canvas[index]) {
                canvas[index].toBlob(function (blob) {
                    var formData = new FormData();

                    formData.append('userId', userId);

                    formData.append('featuredimage', blob);

                    $.ajax('/storage/uploadfeaturedimage', {
                        method: "POST",
                        data: formData,
                        processData: false,
                        contentType: false,
                        success: function (response) {
                            inputImage.val(response.imageUrl);
                            if (callback) {
                                callback();

                                cropper[index].destroy();
                                cropper[index] = null;
                            }
                        },
                        error: function (response) {
                            imgFeaturedImage.src = initialUrl[index];
                        }
                    });
                });
            }
        }
    }

    function instantiateDropZone(index, imagesToUploadCount, el, r) {
        if (dropzones[index]) {
            dropzones[index].destroy();
            dropzones[index] = null;
        }

        random[index] = r;

        dropzones[index] = new Dropzone(el, {
            url: '/storage/uploadcontentimage',
            paramName: 'upload',
            addRemoveLinks: true,
            autoProcessQueue: false,
            maxFiles: imagesToUploadCount,
            parallelUploads: 2,
            resizeWidth: 720,
            resizeMethod: 'crop'
        });

        dropzones[index].on("processing", function () {
            this.options.autoProcessQueue = true;
        });

        dropzones[index].on("sending", function (file, xhr, formData) {
            if (random[index]) {
                formData.append("randomName", random[index]);
            }
        });

        dropzones[index].on("queuecomplete", function () {
            console.log('done from imagemanipulation');
            dropzones[index].options.autoProcessQueue = false;
        });

        dropzones[index].on('addedfile', function (file) {
            if (this.files.length > imagesToUploadCount) {
                this.removeFile(this.files[this.files.length - 1]);
                if (imagesToUploadCount === 0) {
                    ALERTSYSTEM.ShowWarningMessage(`You cannot upload more images.`);
                }
                if (imagesToUploadCount === 1) {
                    ALERTSYSTEM.ShowWarningMessage(`You can only add ${imagesToUploadCount} more file.`);
                }
                else {
                    ALERTSYSTEM.ShowWarningMessage(`You can only add ${imagesToUploadCount} more files.`);
                }
            }
        });
    }

    function getDropZone(index) {
        return dropzones[index];
    }

    return {
        Init: init,
        Cropped: cropped,
        BindCropper: bindCropper,
        UploadCroppedImage: uploadCroppedImage,
        Dropzone: {
            Initialize: instantiateDropZone,
            Get: getDropZone
        }
    };
}());



$(function () {
    IMAGEMANIPULAION.Init();
});