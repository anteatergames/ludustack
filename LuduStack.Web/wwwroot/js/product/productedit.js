var PRODUCTEDIT = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.form = '#frmSave';
        selectors.userId = '#UserId';
        selectors.btnSave = '#btnSave';
        selectors.owner = '.owner:not(.template)';
        selectors.ownerTemplate = '.owner.template';
        selectors.divOwners = '#divOwners';
        selectors.btnOwnerDelete = '.btn-owner-delete';
        selectors.ddlSearchUsers = '#ddlSearchUsers';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.form = $(selectors.form);
        objs.userId = $(selectors.userId);
        objs.divOwners = $(selectors.divOwners);
        objs.ddlSearchUsers = $(selectors.ddlSearchUsers);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        canInteract = MAINMODULE.CanInteract();

        MAINMODULE.Common.BindPopOvers();
    }

    function bindAll() {
        bindSelect2();
        bindBtnOwnerDelete();
        bindBtnSaveForm();
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

            addNewOwner(data);
        });
    }

    function select2FormatResult(result) {
        if (!result.id) {
            return result.text;
        }

        return $('<span><img class="rounded-circle lazyload avatar" data-src="' + result.profileImageUrl + '" src="/images/placeholders/developer.png" alt="meh"> ' + result.text + '</span>');
    }

    function addNewOwner(data) {
        var newOwnerObj = $(selectors.ownerTemplate).first().clone();
        var hdn = newOwnerObj.find('input[type=hidden]');
        var name = newOwnerObj.find('.widget-user-username');
        var location = newOwnerObj.find('.location');
        var sincedate = newOwnerObj.find('.sincedate');
        var avatarImg = newOwnerObj.find('.widget-user-image img');
        var coverImg = newOwnerObj.find('.card-img');

        var existingOwner = $(`${selectors.owner} .ownerid[value=${data.id}]`);

        if (existingOwner.length > 0) {
            ALERTSYSTEM.Toastr.ShowInfo(MESSAGES.Translation['msgAlreadyMember']);
        }
        else {
            hdn.val(data.id);
            name.text(data.text);
            location.text(data.location);
            sincedate.text(data.createDateTimeText);
            coverImg.css('background-image', `url(${data.coverImageUrl})`);
            avatarImg.attr('data-src', data.profileImageUrl);

            newOwnerObj.removeClass('template').removeAttr('aria-hidden');

            newOwnerObj.prependTo(selectors.divOwners);

            MAINMODULE.Common.RenameInputs(objs.divOwners, selectors.owner, 'Owners');
        }
    }

    function bindBtnOwnerDelete() {
        objs.container.on('click', selectors.btnOwnerDelete, function () {
            var btn = $(this);
            var owner = btn.closest(selectors.owner);

            if (owner) {
                owner.remove();

                MAINMODULE.Common.RenameInputs(objs.divOwners, selectors.owner, 'owners');
            }

            return false;
        });
    }

    function bindBtnSaveForm() {
        objs.container.on('click', selectors.btnSave, function () {
            var btn = $(this);
            var valid = objs.form.valid();

            if (valid && canInteract && !btn.hasClass('disabled')) {
                MAINMODULE.Common.DisableButton(btn).ready(() => {
                    submitForm(btn);
                });
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
    PRODUCTEDIT.Init();
});