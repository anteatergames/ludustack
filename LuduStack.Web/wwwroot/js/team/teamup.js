var TEAMUP = (function () {
    "use strict";

    var rootUrl = '/team';
    var avatarBaseUrl = "https://res.cloudinary.com/ludustack/image/upload/f_auto,q_auto/v1/";

    var propPrefix = 'Members';

    var selectors = {};
    var objs = {};
    var isAjax = false;
    var canInteract = false;
    var isList = false;

    function setSelectors() {
        selectors.container = '#teamcontainer';
        selectors.isAjax = '#isajax';
        selectors.Id = '#Id';
        selectors.divListTeams = '#divListTeams';
        selectors.divListMyTeams = '#divListMyTeams';
        selectors.btnTeamNew = '.btn-team-new';
        selectors.teamMemberTemplate = '.team-member.template';
        selectors.formSaveTeam = '#frmTeamSave';
        selectors.btnSaveTeam = '#btnSaveTeam';
        selectors.divteamItem = '.team-item';
        selectors.divMembers = '#divMembers';
        selectors.teamMember = '.team-member';
        selectors.teamMemberUserId = '.team-member:not(.template) .team-member-userid';
        selectors.teamMemberIsLeader = '.team-member-isleader';
        selectors.teamMemberName = '.team-member-name';
        selectors.ddlSearchMembers = '#ddlSearchMembers';
        selectors.divDetails = '.div-details';
        selectors.divInvitation = '#divInvitation';
        selectors.txtMyQuote = '#txtMyQuote';
        selectors.btnAcceptInvitation = '#btnAcceptInvitation';
        selectors.btnRejectInvitation = '#btnRejectInvitation';
        selectors.btnEditTeam = '.btnEditTeam';
        selectors.btnDeleteTeam = '.btnDeleteTeam';
        selectors.btnDeleteMember = '.btnDeleteMember';
        selectors.divGames = '#divGames';
        selectors.divGamesList = '#divGamesList';
        selectors.divApplication = '#divApplication';
        selectors.btnCandidateApply = '#btnCandidateApply';
        selectors.btnAcceptCandidate = '.btnAcceptCandidate';
        selectors.btnRejectCandidate = '.btnRejectCandidate';
    }

    function cacheObjects() {
        objs.container = $(selectors.container);
        objs.Id = $(selectors.Id);
        objs.divListTeams = $(selectors.divListTeams);
        objs.divListMyTeams = $(selectors.divListMyTeams);
        objs.divInvitation = $(selectors.divInvitation);
        objs.divGames = $(selectors.divGames);
        objs.divGamesList = $(selectors.divGamesList);
        objs.divApplication = $(selectors.divApplication);

        if (!isAjax) {
            cacheAjaxObjs();
            objs.txtMyQuote = $(selectors.txtMyQuote);
        }
    }

    function cacheObjectsDetails() {
        objs.btnAcceptCandidate = $(selectors.btnAcceptCandidate);
        objs.btnRejectCandidate = $(selectors.btnRejectCandidate);
    }

    function init() {
        setSelectors();
        cacheObjects();

        isList = objs.Id.val() === undefined;

        canInteract = MAINMODULE.CanInteract();
        isAjax = $(selectors.container).find(selectors.isAjax).val();

        bindAll();

        if (isList) {
            loadTeams();
            loadMyTeams();
        }
        else {
            cacheObjectsDetails();
            if (objs.divGames.length > 0) {
                loadTeamGames();
            }
        }

        bindSelect2();
    }

    function bindAll() {
        bindBtnSave();

        if (isList) {
            bindBtnNew();
            bindEditTeam();
            bindDeleteTeam();
        } else {
            bindAcceptInvitation();
            bindRejectInvitation();
            bindCandidateApply();
            bindAcceptCandidate();
            bindRejectCandidate();
        }
        bindDeleteMember();
    }

    function bindSelect2() {
        $('select.select2').each(function () {
            const hasSelect2 = $(this).hasClass("select2-hidden-accessible");

            if (!hasSelect2) {
                selectTwoBinder(this);
            }
        });
    }

    function selectTwoBinder(element) {
        if ($(element).data('select2') === undefined) {
            $(element).select2({
                width: 'element'
            });
        }
    }

    function bindSelect2Search() {
        objs.ddlSearchMembers.select2({
            minimumInputLength: 2,
            templateResult: select2FormatResult
        });

        objs.ddlSearchMembers.on('select2:select', function (e) {
            var data = e.params.data;
            $(this).val(null).trigger('change');

            selectNewMemberCallBack(data);
        });
    }

    function bindBtnNew() {
        objs.container.on('click', selectors.btnTeamNew, function () {
            if (canInteract) {
                loadNewForm();
            }
        });
    }

    function bindBtnSave() {
        objs.container.on('click', selectors.btnSaveTeam, function (e) {
            e.preventDefault();
            var btn = $(this);

            var valid = objs.form.valid();
            if (valid && canInteract && !btn.hasClass('disabled')) {
                MAINMODULE.Common.DisableButton(btn).ready(() => {
                    submitForm(btn);
                });
            }

            return false;
        });
    }

    function bindAcceptInvitation() {
        objs.divMembers.on('click', selectors.btnAcceptInvitation, function () {
            var btn = $(this);
            var url = $(this).data('url');
            var myQuote = objs.txtMyQuote.val();

            var data = {
                quote: myQuote
            };

            $.post(url, data)
                .done(function (response) {
                    MAINMODULE.Common.HandlePointsEarned(response);

                    var quote = btn.closest(selectors.divDetails).find('.quote');
                    quote.text(myQuote);
                    quote.removeClass('d-none');
                    objs.divInvitation.remove();
                });
        });
    }

    function bindRejectInvitation() {
        objs.divMembers.on('click', selectors.btnRejectInvitation, function () {
            postReject(this);
        });
    }

    function bindCandidateApply() {
        objs.divApplication.on('click', selectors.btnCandidateApply, function () {
            var btn = $(this);
            var url = btn.data('url');

            var data = objs.divApplication.find(':input').serialize();

            $.post(url, data)
                .done(function (response) {
                    if (!response.success) {
                        ALERTSYSTEM.ShowWarningMessage(response.message);
                    }
                    else {
                        MAINMODULE.Common.HandlePointsEarned(response);

                        MAINMODULE.Ajax.HandleUrlResponse(response);
                    }
                });
        });
    }

    function bindAcceptCandidate() {
        objs.divMembers.on('click', selectors.btnAcceptCandidate, function () {
            var btn = $(this);
            var url = btn.data('url');

            $.post(url)
                .done(function (response) {
                    if (!response.success) {
                        ALERTSYSTEM.ShowWarningMessage(response.message);
                    }
                    else {
                        MAINMODULE.Ajax.HandleUrlResponse(response);
                    }
                });
        });
    }

    function bindRejectCandidate() {
        objs.divMembers.on('click', selectors.btnRejectCandidate, function () {
            postReject(this);
        });
    }

    function bindEditTeam() {
        objs.divListTeams.on('click', selectors.btnEditTeam, function (e) {
            e.preventDefault();
            var url = $(this).data('url');

            if (canInteract) {
                loadEditForm(url);
            }
        });
    }

    function bindDeleteTeam() {
        objs.divListTeams.on('click', selectors.btnDeleteTeam, function (e) {
            e.preventDefault();

            var btn = $(this);
            var msg = btn.data('confirmationmessage');
            var confirmationTitle = btn.data('confirmationtitle');
            var confirmationButtonText = btn.data('confirmationbuttontext');
            var cancelButtonText = btn.data('cancelbuttontext');

            ALERTSYSTEM.ShowConfirmMessage(confirmationTitle, msg, confirmationButtonText, cancelButtonText, function () {
                deleteTeam(btn);
            });
        });
    }

    function postReject(element) {
        var btn = $(element);
        var url = $(element).data('url');

        $.post(url)
            .done(function () {
                btn.closest(selectors.teamMember).remove();
            });
    }

    function deleteTeam(btn) {
        var url = btn.data('url');
        $.ajax({
            url: url,
            type: 'DELETE'
        }).done(function (response) {
            deleteTeamCallback(response, btn);
        });
    }

    function deleteTeamCallback(response, btn) {
        if (response.success) {
            btn.closest(selectors.divteamItem).parent().remove();
            loadMyTeams();

            MAINMODULE.Ajax.HandleUrlResponse(response);
        }
        else {
            ALERTSYSTEM.ShowWarningMessage(response.message);
        }
    }

    function bindDeleteMember() {
        objs.container.on('click', selectors.btnDeleteMember, function (e) {
            e.preventDefault();

            var btn = $(this);
            var url = $(this).data('url');
            var msg = btn.data('confirmationmessage');
            var confirmationTitle = btn.data('confirmationtitle');
            var confirmationButtonText = btn.data('confirmationbuttontext');
            var cancelButtonText = btn.data('cancelbuttontext');

            ALERTSYSTEM.ShowConfirmMessage(confirmationTitle, msg, confirmationButtonText, cancelButtonText, function () {
                $.ajax({
                    url: url,
                    type: 'DELETE'
                }).done(function (response) {
                    if (response.success) {
                        btn.closest(selectors.teamMember).remove();
                        MAINMODULE.Common.RenameInputs(objs.divMembers, selectors.teamMember, propPrefix);

                        if (response.message) {
                            ALERTSYSTEM.ShowSuccessMessage(response.message);
                        }
                    }
                    else {
                        ALERTSYSTEM.ShowWarningMessage(response.message);
                    }
                });
            });
        });
    }

    function loadTeams() {
        MAINMODULE.Ajax.LoadHtml('/team/list/', objs.divListTeams).then(() => {
            MAINMODULE.Common.BindPopOvers(true);
        });
    }

    function loadMyTeams() {
        MAINMODULE.Ajax.LoadHtml('/team/list/mine', objs.divListMyTeams);
    }

    function loadNewForm() {
        MAINMODULE.Ajax.LoadHtml(rootUrl + "/new", objs.container).then(() => {
            cacheAjaxObjs();

            bindSelect2();
            bindSelect2Search();

            $.validator.unobtrusive.parse(objs.form);
        });
    }

    function loadEditForm(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.container).then(() => {
            MAINMODULE.Common.RenameInputs(objs.divMembers, selectors.teamMember, propPrefix);

            cacheAjaxObjs();

            bindSelect2();
            bindSelect2Search();

            $.validator.unobtrusive.parse(objs.form);
        });
    }

    function loadTeamGames() {
        var id = objs.divGames.data('id');

        MAINMODULE.Ajax.LoadHtml('/game/byteam/' + id, objs.divGamesList);
    }

    function selectNewMemberCallBack(data) {
        var newMemberObj = $(selectors.teamMemberTemplate).first().clone();

        var existingUserIds = [];
        $(selectors.teamMemberUserId).each(function () {
            existingUserIds.push(this.value);
        });

        var alreadyAdded = existingUserIds.indexOf(data.id) > -1;

        if (alreadyAdded) {
            ALERTSYSTEM.ShowWarningMessage("The user you selected is already added to this team!");
            return;
        }

        var userId = newMemberObj.find('[id$=__UserId]');
        userId.val(data.id);

        var avatar = newMemberObj.find('.avatar');
        avatar.attr('src', avatarBaseUrl + data.id + '/profileimage_' + data.id + "_Personal");
        var isleader = newMemberObj.find(selectors.teamMemberIsLeader);
        isleader.remove();
        var name = newMemberObj.find(selectors.teamMemberName);
        name.text(data.text);
        var nameHidden = newMemberObj.find('.team-member-name-hidden');
        nameHidden.val(data.text);

        newMemberObj.removeClass('template');

        newMemberObj.appendTo(selectors.divMembers);

        MAINMODULE.Common.RenameInputs(objs.divMembers, selectors.teamMember, propPrefix);

        bindSelect2();
    }

    function select2FormatResult(result) {
        if (!result.id) {
            return result.text;
        }

        return $('<span><img class="rounded-circle lazyload avatar" data-src="' + avatarBaseUrl + result.id + '/profileimage_' + result.id + '_Personal' + '" src="/images/placeholders/developer.png" alt="meh"> ' + result.text + '</span>');
    }

    function cacheAjaxObjs() {
        objs.form = $(selectors.formSaveTeam);
        objs.ddlSearchMembers = $(selectors.ddlSearchMembers);
        objs.divMembers = $(selectors.divMembers);
        objs.teamMember = $(selectors.teamMember);
    }

    function submitForm(btn) {
        var url = objs.form.attr('action');

        var data = objs.form.serialize();

        $.post(url, data).done(function (response) {
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
    TEAMUP.Init();
});