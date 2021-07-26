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
        selectors.validationError = '.field-validation-error';
        selectors.userId = '#UserId';
        selectors.btnSave = '#btnSave';
        selectors.startDate = 'input#StartDate';
        selectors.entryDeadline = 'input#EntryDeadline';
        selectors.votingEndDate = 'input#VotingEndDate';
        selectors.resultDate = 'input#ResultDate';

        selectors.imageDivElement = '.newimageupload';

        selectors.divJudges = '#divJudges';
        selectors.btnJudgeDelete = '.btn-judge-delete';
        selectors.judge = '.judge:not(.template)';
        selectors.judgeTemplate = '.judge.template';
        selectors.ddlVoters = '#Voters';
        selectors.ddlSearchUsers = '#ddlSearchUsers';
        selectors.alertNoJudges = '#alertnojudges';
        selectors.numberOnly = '.numberonly';
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
        objs.divJudges = $(selectors.divJudges);
        objs.ddlVoters = $(selectors.ddlVoters);
        objs.ddlSearchUsers = $(selectors.ddlSearchUsers);
        objs.alertNoJudges = $(selectors.alertNoJudges);
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
        bindChangeVoters();
        bindBtnJudgeDelete();
        bindSelect2();
        bundNumbersOnly();

        WYSIWYGEDITOR.BindEditors('.wysiwygeditor');

        IMAGEMANIPULAION.Cropper.BindCropper(selectors.imageDivElement);

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

    function bundNumbersOnly() {
        objs.container.on('keypress', selectors.numberOnly, function (e) {
            var code = (event.keyCode ? event.keyCode : event.which);

            if (!(code >= 48 && code <= 57)) {
                e.preventDefault();
                return false;
            }
        });
    }

    function bindBtnSaveForm() {
        objs.container.on('click', selectors.btnSave, function () {
            var btn = $(this);

            updateEditors();

            var valid = objs.form.valid();

            if (valid && canInteract && !btn.hasClass('disabled')) {
                MAINMODULE.Common.DisableButton(btn).ready(() => {
                    IMAGEMANIPULAION.Cropper.UploadCroppedImages(selectors.imageDivElement, function () {
                        submitForm(btn);
                    });
                });
            }
            else {
                scrollToFirstError();
            }

            return false;
        });
    }

    function bindChangeVoters() {
        objs.container.on('change', selectors.ddlVoters, function () {
            var ddl = $(this);
            var judgeCount = $(selectors.judge).length;

            if (judgeCount === 0 && ddl.val() === '1') {
                objs.alertNoJudges.show();
            }
            else {
                objs.alertNoJudges.hide();
            }

            return false;
        });
    }

    function bindBtnJudgeDelete() {
        objs.container.on('click', selectors.btnJudgeDelete, function () {
            var btn = $(this);
            var msg = btn.data('deleteerrormsg');
            var currentJudges = $(selectors.judge);
            var judge = btn.closest(selectors.judge);
            var votersSelection = objs.ddlVoters.val();

            var canDeleteJudge = !(votersSelection === '1' && currentJudges.length === 1) || votersSelection !== '1';

            if (judge && canDeleteJudge) {
                judge.remove();

                MAINMODULE.Common.RenameInputs(objs.divJudges, selectors.judge, 'Judges');
            }
            else {
                ALERTSYSTEM.Toastr.ShowInfo(msg)
            }

            return false;
        });
    }

    function bindSelect2() {
        objs.ddlSearchUsers.select2({
            width: 'resolve',
            minimumInputLength: 2,
            templateResult: select2FormatResult
        });

        objs.ddlSearchUsers.on('select2:select', function (e) {
            var data = e.params.data;
            $(this).val(null).trigger('change');

            addNewJudge(data);
        });
    }

    function select2FormatResult(result) {
        if (!result.id) {
            return result.text;
        }

        return $('<span><img class="rounded-circle lazyload avatar" data-src="' + result.profileImageUrl + '" src="/images/placeholders/developer.png" alt="meh"> ' + result.text + '</span>');
    }

    function addNewJudge(data) {
        var newJudgeObj = $(selectors.judgeTemplate).first().clone();
        var hdn = newJudgeObj.find('input[type=hidden]');
        var name = newJudgeObj.find('.widget-user-username');
        var location = newJudgeObj.find('.location');
        var sincedate = newJudgeObj.find('.sincedate');
        var avatarImg = newJudgeObj.find('.widget-user-image img');
        var coverImg = newJudgeObj.find('.card-img');

        hdn.val(data.id);
        name.text(data.text);
        location.text(data.location);
        sincedate.text(data.createDateText);
        coverImg.css('background-image', `url(${data.coverImageUrl})`);
        avatarImg.attr('data-src', data.profileImageUrl);

        newJudgeObj.removeClass('template').removeAttr('aria-hidden');

        newJudgeObj.appendTo(selectors.divJudges);

        objs.alertNoJudges.hide();

        MAINMODULE.Common.RenameInputs(objs.divJudges, selectors.judge, 'Judges');
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

    function scrollToFirstError() {
        var elementToScroll = $(selectors.form).first();

        MAINMODULE.Utils.ScrollTo(elementToScroll.parent());
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