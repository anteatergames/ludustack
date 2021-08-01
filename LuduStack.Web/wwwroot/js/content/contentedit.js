﻿var CONTENTEDIT = (function () {
    "use strict";

    var selectors = {};

    var cropper;
    var canvas;
    var initialUrl;
    var cropped = false;

    function init() {
        cacheSelectors();

        bindAll();

        $('#frmContentSave').validate().settings.ignore = ":hidden:not(.wysiwygeditor), .ck-editor__editable";
    }

    function cacheSelectors() {
        selectors.Content = $('#Content');
        selectors.UserId = $('#UserId');
        selectors.modalCrop = $('#modalCrop');
    }

    function bindAll() {
        bindSelect2();

        WYSIWYGEDITOR.BindEditors('.wysiwygeditor');

        bindCropper();

        bindSave();
    }

    function bindSelect2() {
        $('.select2').select2();
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

                selectors.modalCrop.modal('show');
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

        selectors.modalCrop.on('shown.bs.modal', function () {
            cropper = new Cropper(image, {
                aspectRatio: 16 / 9,
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

            selectors.modalCrop.modal('hide');
        });
    }

    function uploadCroppedImage(callback) {
        if (cropper) {
            if (canvas) {
                canvas.toBlob(function (blob) {
                    var formData = new FormData();

                    formData.append('userId', selectors.UserId.val());

                    formData.append('featuredimage', blob);

                    $.ajax('/storage/uploadfeaturedimage', {
                        method: "POST",
                        data: formData,
                        processData: false,
                        contentType: false,
                        success: function (response) {
                            $('#FeaturedImage').val(response.filename);
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

    function bindSave() {
        $('#frmContentSave').on('click', '#btnSave', function (e) {
            e.preventDefault();

            var btn = $(this);
            var form = btn.closest('form');
            btn.button('loading');
            var icon = btn.find('i');

            icon.removeClass('fa-save');
            icon.addClass('fa-circle-notch fa-spin');

            var txtArea = form.find(selectors.Content);
            var editorId = txtArea.attr('data-editor-id');

            WYSIWYGEDITOR.UpdateSourceElement(editorId);

            var valid = form.valid();
            if (valid) {
                var templateEmpty = "<p>&nbsp;</p>";
                if (selectors.Content.val().length === 0 || selectors.Content.val() === templateEmpty) {
                    ALERTSYSTEM.ShowWarningMessage("The Content is empty!! You cannot post an empty article!");
                    return false;
                }
                else {
                    if (cropped) {
                        uploadCroppedImage(function () {
                            submitForm(btn, function () {
                                icon.removeClass('fa-circle-notch fa-spin');
                                icon.addClass('fa-save');
                            });
                        });
                    }
                    else {
                        submitForm(btn, function () {
                            icon.removeClass('fa-circle-notch fa-spin');
                            icon.addClass('fa-save');
                        });
                    }
                }
            }

            return false;
        });
    }

    function submitForm(btn, callback) {
        var form = $('#frmContentSave');
        var url = form.attr('action');

        var data = form.serialize();

        $.ajax({
            type: "POST",
            url: url,
            data: data,
            enctype: 'multipart/form-data'
        }).done(function (response) {
            if (response.success === true) {
                MAINMODULE.Common.PostSaveCallback(response, btn);

                if (callback) {
                    callback();
                }

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
    CONTENTEDIT.Init();
});