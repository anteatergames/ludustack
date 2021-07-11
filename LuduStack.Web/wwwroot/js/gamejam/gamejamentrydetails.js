var GAMEJAMENTRYDETAILS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.form = '#frmSubmitGame';
        selectors.userId = '#UserId';
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
        objs.containerDetails = $(selectors.containerDetails);
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
        bindBtnSubmitGame();
        bindRatings();
    }

    function bindBtnTeamMemberDelete() {
        objs.container.on('click', selectors.btnTeamMemberDelete, function () {
            var btn = $(this);
            var msg = btn.data('deleteerrormsg');
            var teamMember = btn.closest(selectors.teamMember);
            var teamMemberUserId = teamMember.find('.teammemberid');
            var userId = objs.userId.val();
            var canDelete = teamMemberUserId.val() !== userId;

            console.log(teamMember);

            console.log(canDelete);

            if (teamMember && canDelete) {
                teamMember.remove();

                MAINMODULE.Common.RenameInputs(objs.divTeamMembers, selectors.teamMember, 'TeamMembers');
            }
            else {
                ALERTSYSTEM.Toastr.ShowInfo(msg);
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

        hdn.val(data.id);
        name.text(data.text);
        location.text(data.location);
        sincedate.text(data.createDateText);
        coverImg.css('background-image', `url(${data.coverImageUrl})`);
        avatarImg.attr('data-src', data.profileImageUrl);

        newTeamMemberObj.removeClass('template').removeAttr('aria-hidden');

        newTeamMemberObj.appendTo(selectors.divTeamMembers);

        MAINMODULE.Common.RenameInputs(objs.divTeamMembers, selectors.teamMember, 'TeamMembers');
    }

    function bindBtnSubmitGame() {
        objs.container.on('click', selectors.btnSubmitGame, function () {
            var btn = $(this);

            var valid = objs.form.valid();

            if (valid && canInteract) {
                MAINMODULE.Common.DisableButton(btn).ready(() => {
                    submitForm(btn);
                });
            }

            return false;
        });
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