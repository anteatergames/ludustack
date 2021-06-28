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
        bindBtnSaveForm();
        WYSIWYGEDITOR.BindEditors('.wysiwygeditor');

        bindDateTimePickers();
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

        obj.datetimepicker('defaultDate', pd);
    }

    function bindBtnSaveForm() {
        objs.container.on('click', selectors.btnSave, function () {
            var btn = $(this);
            var valid = objs.form.valid();

            if (valid && canInteract) {
                MAINMODULE.Common.DisableButton(btn);

                submitForm(btn);
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

        updateEditors();

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