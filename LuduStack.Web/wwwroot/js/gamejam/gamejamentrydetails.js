var GAMEJAMENTRYDETAILS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.participationType = '#GameJam_ParticipationType';
        selectors.form = '#frmSubmitGame';
        selectors.userId = '#UserId';
        selectors.btnSaveTeam = '#btnSaveTeam';
        selectors.btnSubmitGame = '#btnSubmitGame';
        selectors.scoreRating = '.criteria-rating';
        selectors.divTeamMembers = '#divTeamMembers';
        selectors.btnTeamMemberDelete = '.btn-teammember-delete';
        selectors.teamMember = '.teammember:not(.template)';
        selectors.teamMemberTemplate = '.teammember.template';
        selectors.ddlSearchUsers = '#ddlSearchUsers';
    }

    function cacheObjs() {
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.participationType = $(selectors.participationType);
        objs.form = $(selectors.form);
        objs.userId = $(selectors.userId);
        objs.divTeamMembers = $(selectors.divTeamMembers);
        objs.ddlSearchUsers = $(selectors.ddlSearchUsers);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        canInteract = MAINMODULE.CanInteract();

        FX.StartCountDown('#SecondsToCountDown');

        MAINMODULE.Common.BindPopOvers();
    }

    function bindAll() {
        bindBtnTeamMemberDelete();
        bindSelect2();
        bindBtnSaveTeam();
        bindBtnSubmitGame();
        bindRatings();
    }

    function bindBtnTeamMemberDelete() {
        objs.container.on('click', selectors.btnTeamMemberDelete, function () {
            var btn = $(this);
            var teamMember = btn.closest(selectors.teamMember);
            var teamMemberUserId = teamMember.find('.teammemberid');
            var userId = objs.userId.val();
            var isMe = teamMemberUserId.val() === userId;
            var participationType = objs.participationType.val();
            var currentMemberCount = $(selectors.teamMember).length;
            var canDeleteOneMore = participationType === 'TeamsOnly' ? currentMemberCount > 2 : true;

            if (teamMember) {
                if (isMe) {
                    ALERTSYSTEM.Toastr.ShowInfo(MESSAGES.Translation['msgDeleteYourself']);
                }
                else if (!canDeleteOneMore) {
                    ALERTSYSTEM.Toastr.ShowInfo(MESSAGES.Translation['msgTeamMinimumMember']);
                }
                else {
                    teamMember.remove();

                    MAINMODULE.Common.RenameInputs(objs.divTeamMembers, selectors.teamMember, 'TeamMembers');
                }
            }

            return false;
        });
    }

    function bindSelect2() {
        objs.ddlSearchUsers.select2({
            width: 'element',
            minimumInputLength: 2,
            templateResult: select2FormatResult
        });

        objs.ddlSearchUsers.on('select2:select', function (e) {
            var data = e.params.data;
            $(this).val(null).trigger('change');

            addNewTeamMember(data);
        });
    }

    function select2FormatResult(result) {
        if (!result.id) {
            return result.text;
        }

        return $('<span><img class="rounded-circle lazyload avatar" data-src="' + result.profileImageUrl + '" src="/images/placeholders/developer.png" alt="meh"> ' + result.text + '</span>');
    }

    function addNewTeamMember(data) {
        var newTeamMemberObj = $(selectors.teamMemberTemplate).first().clone();
        var hdn = newTeamMemberObj.find('input[type=hidden]');
        var name = newTeamMemberObj.find('.widget-user-username');
        var location = newTeamMemberObj.find('.location');
        var sincedate = newTeamMemberObj.find('.sincedate');
        var avatarImg = newTeamMemberObj.find('.widget-user-image img');
        var coverImg = newTeamMemberObj.find('.card-img');

        var existingTeamMember = $(`${selectors.teamMember} .teammemberid[value=${data.id}]`);

        if (existingTeamMember.length > 0) {
            ALERTSYSTEM.Toastr.ShowInfo(MESSAGES.Translation['msgAlreadyMember']);
        }
        else {
            hdn.val(data.id);
            name.text(data.text);
            location.text(data.location);
            sincedate.text(data.createDateText);
            coverImg.css('background-image', `url(${data.coverImageUrl})`);
            avatarImg.attr('data-src', data.profileImageUrl);

            newTeamMemberObj.removeClass('template').removeAttr('aria-hidden');

            newTeamMemberObj.prependTo(selectors.divTeamMembers);

            MAINMODULE.Common.RenameInputs(objs.divTeamMembers, selectors.teamMember, 'TeamMembers');
        }
    }

    function bindBtnSaveTeam() {
        objs.container.on('click', selectors.btnSaveTeam, function () {
            var btn = $(this);

            validateTeamBeforeSave(() => {
                if (canInteract && !btn.hasClass('disabled')) {
                    MAINMODULE.Common.DisableButton(btn).ready(() => {
                        saveTeam(btn);
                    });
                }
            });

            return false;
        });
    }

    function bindBtnSubmitGame() {
        objs.container.on('click', selectors.btnSubmitGame, function () {
            var btn = $(this);

            validateTeamBeforeSave(() => {
                var valid = objs.form.valid();

                if (valid && canInteract && !btn.hasClass('disabled')) {
                    var msgs = MAINMODULE.Common.GetPostConfirmationMessages(btn);

                    ALERTSYSTEM.ShowConfirmMessage(msgs.confirmationTitle, msgs.msg, msgs.confirmationButtonText, msgs.cancelButtonText, function () {
                        MAINMODULE.Common.DisableButton($(selectors.btnSaveTeam)).ready(() => {
                            MAINMODULE.Common.DisableButton(btn).ready(() => {
                                submitForm(btn);
                            });
                        });
                    });
                }
            });

            return false;
        });
    }

    function validateTeamBeforeSave(callback) {
        var participationType = objs.participationType.val();
        var currentMemberCount = $(selectors.teamMember).length;
        var canSaveTeamCount = participationType === 'TeamsOnly' ? currentMemberCount > 1 : true;

        if (!canSaveTeamCount) {
            ALERTSYSTEM.Toastr.ShowInfo(MESSAGES.Translation['msgTeamMinimumMember']);
        }
        else {
            if (callback) {
                callback();
            }
        }
    }

    function bindRatings() {
        var readOnly = $(selectors.scoreRating).data('readonly') === true;

        $(selectors.scoreRating).rating({
            theme: 'krajee-fas',
            showClear: false,
            showCaption: false,
            displayOnly: readOnly,
            size: 'md',
            animate: false,
            step: 0.5,
            filledStar: '<i class="fas fa-gamepad tilt-20"></i>',
            emptyStar: '<i class="fas fa-gamepad tilt-20"></i>',
            starCaptions: {
                0.5: 'Rating: 0.5',
                1: 'Rating: 1',
                1.5: 'Rating: 1.5',
                2: 'Rating: 2',
                2.5: 'Rating: 2.5',
                3: 'Rating: 3',
                3.5: 'Rating: 3.5',
                4: 'Rating: 4',
                4.5: 'Rating: 4.5',
                5: 'Rating: 5'
            }
        });

        objs.container.on('rating:change', selectors.scoreRating, function (event, value, caption) {
            var url = $(this).data('url');

            var data = { score: value };

            MAINMODULE.Ajax.Post(url, data);
        });
    }

    function saveTeam(btn) {
        var url = btn.data('url');

        var inputs = $(`${selectors.teamMember} :input:not(.btn)`);
        var data = inputs.serializeObject();

        return $.post(url, data).done(function (response) {
            if (response.success === true) {
                MAINMODULE.Common.PostSaveCallback(response, btn);

                MAINMODULE.Common.HandleSuccessDefault(response, () => MAINMODULE.Common.EnableButton(btn));
            }
            else {
                MAINMODULE.Ajax.HandleErrorResponse(response);
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
    GAMEJAMENTRYDETAILS.Init();
});