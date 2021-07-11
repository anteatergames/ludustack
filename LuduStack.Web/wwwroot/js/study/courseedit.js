var COURSEEDIT = (function () {
    "use strict";

    var selectors = {};
    var objs = {};
    var canInteract = false;
    var isNew = false;

    var propPrefix = 'Plans';

    var croppers = [];
    var imagesProcessed = 0;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.form = '#frmCourseSave';
        selectors.userId = '#UserId';
        selectors.btnSave = '#btnSaveCourse';
        selectors.txtAreaDescription = '#Description';
        selectors.sortablePlanning = 'divPlans';
        selectors.divPlans = '#divPlans';
        selectors.planItem = '.studyplan';
        selectors.template = selectors.planItem + '.template';
        selectors.btnAddPlan = '#btn-course-plan-add';
        selectors.btnDeletePlan = '.btn-plan-delete';
        selectors.planCounter = '#planCounter';
        selectors.divNoItems = '#divNoItems';
        selectors.btnCollapse = '.btn-collapse';
        selectors.btnSavePlans = '#btn-course-plans-save';
        selectors.rangeSlider = 'input[type="range"]';
        selectors.inputImageListItem = 'input.image-upload';
        selectors.imageListItem = 'img.image-upload';
        selectors.modalCrop = '#modalCrop';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.form = $(selectors.form);
        objs.userId = $(selectors.userId);
        objs.txtAreaDescription = $(selectors.txtAreaDescription);
        objs.sortablePlanning = document.getElementById(selectors.sortablePlanning);
        objs.divPlans = $(selectors.divPlans);
        objs.divNoItems = $(selectors.divNoItems);
        objs.inputImageListItem = $(selectors.inputImageListItem);
        objs.modalCrop = $(selectors.modalCrop);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        canInteract = MAINMODULE.CanInteract();
        isNew = COMMONEDIT.IsNew();

        if (isNew) {
            console.log('new course');
        }
        else {
            var urlPlans = objs.urls.data('urlListplansforedit');
            loadPlans(urlPlans);
        }

        MAINMODULE.Common.BindPopOvers();
    }

    function bindAll() {
        bindSelect2();
        bindBtnSaveForm();
        bindBtnAddPlan();
        bindBtnDeletePlan();
        bindBtnCollapse();
        bindBtnSavePlans();

        bindChangeImage();

        bindCropper();
    }

    function bindSelect2() {
        $('select.select2').each(function () {
            if ($(this).data('select2') === undefined) {
                $(this).select2({
                    width: 'element'
                });
            }
        });
    }

    function bindBtnSaveForm() {
        objs.container.on('click', selectors.btnSave, function () {
            var btn = $(this);
            var valid = objs.form.valid();

            if (valid && canInteract && !btn.hasClass('disabled')) {
                MAINMODULE.Common.DisableButton(btn).ready(() => {
                    uploadCroppedImages(function () {
                        submitForm(btn);
                    });
                });
            }
        });
    }

    function bindBtnAddPlan() {
        objs.container.on('click', selectors.btnAddPlan, function (e) {
            e.preventDefault();

            addNewPlan();

            return false;
        });
    }

    function bindBtnDeletePlan() {
        objs.container.on('click', selectors.btnDeletePlan, function (e) {
            e.preventDefault();

            var btn = $(this);

            deletePlan(btn);

            return false;
        });
    }

    function bindBtnCollapse() {
        objs.container.on('click', selectors.btnCollapse, function (e) {
            e.preventDefault();

            var btn = $(this);
            var icon = btn.find('i');

            var target = btn.closest('.studyplan').find('.collapse').first();
            var alternateIcon = icon.data('icon-alternate');
            var temp = icon.attr('class');
            icon.attr('class', alternateIcon);
            icon.data('icon-alternate', temp);

            $(target).slideToggle();

            return false;
        });
    }

    function bindBtnSavePlans() {
        objs.container.on('click', selectors.btnSavePlans, function (e) {
            e.preventDefault();
            var btn = $(this);

            MAINMODULE.Common.DisableButton(btn);

            var valid = objs.form.valid();

            if (valid && canInteract && !btn.hasClass('disabled')) {
                savePlans(btn, function (response) {
                    if (response.message) {
                        ALERTSYSTEM.Toastr.ShowWarning(response.message);
                    }
                });
            }
            else {
                MAINMODULE.Common.RemoveErrorFromButton(btn);
            }

            return false;
        });
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

    var changeDone = function (url2, element, image) {
        element.value = '';

        croppers[image.dataset.cropperIndex].replace(url2);

        image.src = url2;

        element.dataset.changed = true;
    };

    function bindCropper() {
        var images = document.querySelectorAll(selectors.imageListItem);
        var i;

        for (i = 0; i < images.length; i++) {
            croppers.push(new Cropper(images[i], {
                aspectRatio: 40 / 21,
                viewMode: 3,
                autoCropArea: 1,
                zoomOnWheel: false
            }));

            images[i].dataset.cropperIndex = i;
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

    function initSortable() {
        if (objs.sortablePlanning) {
            new Sortable(objs.sortablePlanning, {
                handle: '.handle',
                animation: 150,
                ghostClass: 'blue-background-class',
                onEnd: (e) => {
                    MAINMODULE.Common.RenameInputs(objs.divPlans, selectors.planItem, propPrefix);
                    reOrder(objs.divPlans, selectors.planItem, propPrefix);
                }
            });
        }
    }

    const initRangeSlider = (context) => {
        $(selectors.rangeSlider, context).each(function () {
            var slider = $(this);
            var handle;

            slider.rangeslider({
                polyfill: false,
                onInit: function (position, value) {
                    handle = setSliderHandler(handle, this.$range, this.value);
                },
                onSlide: function (position, value) {
                    handle.html(this.value);
                }
            }).on('input', function () {
                handle = setSliderHandler(handle, this.$range, this.value);
            });
        });
    };

    function setSliderHandler(handle, range, value) {
        if (handle === undefined) {
            handle = $('.rangeslider__handle', range);
        }

        handle.html(value);

        return handle;
    }

    function submitForm(btn, callback) {
        var url = objs.form.attr('action');

        var data = objs.form.serializeObject();

        $.post(url, data).done(function (response) {
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

    function loadPlans(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.divPlans).then(() => {
            objs.divNoItems = $(selectors.divNoItems);

            MAINMODULE.Common.RenameInputs(objs.divPlans, selectors.planItem, propPrefix);

            initSortable();

            checkNoItems(selectors.planItem, selectors.planCounter, objs.divNoItems);

            initRangeSlider(selectors.container);
        });
    }

    function addNewPlan() {
        var newPlanObj = $(selectors.template).first().clone();

        newPlanObj.find(':input').val('');

        newPlanObj.removeClass('template');

        newPlanObj.appendTo(selectors.divPlans);

        newPlanObj.find('input.form-control').first().focus();

        MAINMODULE.Common.RenameInputs(objs.divPlans, selectors.planItem, propPrefix);

        checkNoItems(selectors.planItem, selectors.planCounter, objs.divNoItems);

        MAINMODULE.Common.BindPopOvers();

        COMMONEDIT.ResetValidator(objs.form);

        initSortable();

        reOrder(objs.divPlans, selectors.planItem, propPrefix);

        initRangeSlider(newPlanObj);
    }

    function deletePlan(btn) {
        var plan = btn.closest(selectors.planItem);

        plan.remove();

        MAINMODULE.Common.RenameInputs(objs.divPlans, selectors.planItem, propPrefix);

        checkNoItems(selectors.planItem, selectors.planCounter, objs.divNoItems);

        COMMONEDIT.ResetValidator(objs.form);
    }

    function savePlans(btn, callback) {
        var url = objs.urls.data('urlPlansSave');

        var plans = $(selectors.planItem + ':not(.template)');
        var data = plans.find(':input, :hidden').serializeObject();

        $.post(url, data).done(function (response) {
            if (response.success === true) {
                MAINMODULE.Common.PostSaveCallback(response, btn);

                MAINMODULE.Common.HandleSuccessDefault(response, callback, function (result) {
                    MAINMODULE.Common.RemoveErrorFromButton(btn);

                    loadPlans(objs.urls.data('urlListplansforedit'));
                });
            }
            else {
                MAINMODULE.Ajax.HandleErrorResponse(response);
            }
        });
    }

    function checkNoItems(itemSelector, counterSelector, noItemObject) {
        var count = $(itemSelector + ':not(.template)').length;

        $(counterSelector).html(count);

        if (count === 0) {
            noItemObject.show();
        }
        else {
            noItemObject.hide();
        }
    }

    function reOrder(objContainer, itemSelector, propPreffix) {
        var count = 0;

        objContainer.find(itemSelector + ' .order').each(function (index2, element2) {
            $(this).val(count);
            count++;
        });
    }

    return {
        Init: init
    };
}());

$(function () {
    COURSEEDIT.Init();
});