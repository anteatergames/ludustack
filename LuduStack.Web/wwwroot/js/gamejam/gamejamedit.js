var GAMEJAMEDIT = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;

    var datetimePickerIcons = {
        time: "fa fa-clock",
        date: "fa fa-calendar",
        up: "fa fa-arrow-up",
        down: "fa fa-arrow-down"
    };

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
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
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        canInteract = MAINMODULE.CanInteract();

        MAINMODULE.Common.BindPopOvers();
    }

    function bindAll() {
        bindDateTimePickers();

        WYSIWYGEDITOR.BindEditors('.wysiwygeditor');

        IMAGEMANIPULAION.Cropper.BindCropper(selectors.imageListItem, objs.inputImageListItem);

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

    function bindBtnSaveForm() {
        objs.container.on('click', selectors.btnSave, function () {
            var btn = $(this);

            updateEditors();

            var valid = objs.form.valid();

            if (valid && canInteract) {
                MAINMODULE.Common.DisableButton(btn);

                IMAGEMANIPULAION.Cropper.UploadCroppedImages(objs.inputImageListItem, function () {
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


// this must be outside the module
WYSIWYGEDITOR.SetValidatorDefaults();