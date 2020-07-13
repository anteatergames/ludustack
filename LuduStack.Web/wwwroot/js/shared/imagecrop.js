var IMAGECROP = (function () {
    "use strict";

    var cropped = [];
    var cropper = [];
    var canvas = [];
    var initialUrl = [];

    function init() {
        console.log('imagecrop init');
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


    return {
        Init: init,
        Cropped: cropped,
        BindCropper: bindCropper,
        UploadCroppedImage: uploadCroppedImage
    };
}());



$(function () {
    IMAGECROP.Init();
});