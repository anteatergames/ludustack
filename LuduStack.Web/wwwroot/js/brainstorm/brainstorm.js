﻿var BRAINSTORM = (function () {
    "use strict";

    var rootUrl = '/brainstorm';

    var selectors = {};
    var objs = {};
    var canInteract = false;
    var newIdea = false;
    var details = false;

    function init() {
        setSelectors();
        cacheObjects();

        bindAll();

        canInteract = $(selectors.canInteract).val() === 'true';
        newIdea = window.location.href.indexOf('newidea') > -1;
        details = window.location.href.indexOf('details') > -1;

        if (!newIdea && !details) {
            loadSession();
        }

        objs.ddlStatus.removeClass('invisible').show();
    }

    function setSelectors() {
        selectors.container = '#contentwrapper';
        selectors.canInteract = '#caninteract';
        selectors.toolbar = $("#divToolbar");
        selectors.list = $("#divList");
        selectors.btnPostVotingItem = $("#btnPostVotingItem");
        selectors.btnSave = '.btn-save';
        selectors.form = $("#frmBrainstormIdeaSave");
        selectors.ddlStatus = '#ddlStatus';
    }

    function cacheObjects() {
        objs.container = $(selectors.container);
        objs.ddlStatus = $(selectors.ddlStatus);
    }

    function bindAll() {
        bindBtnNewIdea();
        bindBtnNewSession();
        bindBtnSave();
        bindBtnVote();
        bindStatusChange();
    }

    function bindStatusChange() {
        objs.container.on('change', selectors.ddlStatus, function (e) {
            var selectedStatus = $(this).val();
            var url = $(this).data('url');
            var ideaId = $(this).data('id');

            var data = {
                selectedStatus: selectedStatus,
                ideaId: ideaId
            };

            $.post(url, data).done(function (response) {
                if (response.success === true) {
                    ALERTSYSTEM.ShowSuccessMessage("Awesome!", function () {
                        MAINMODULE.Ajax.HandleUrlResponse(response);
                    });
                }
                else {
                    MAINMODULE.Ajax.HandleErrorResponse(response);
                }
            });
        });
    }

    function bindBtnNewIdea() {
        objs.container.on('click', '.btn-idea-new', function () {
            if (canInteract) {
                loadNewForm();
            }
        });
    }

    function bindBtnNewSession() {
        objs.container.on('click', '.btn-session-new', function () {
            if (canInteract) {
                loadNewSessionForm();
            }
        });
    }

    function bindBtnSave() {
        objs.container.on('click', selectors.btnSave, function (e) {
            e.preventDefault();

            var btn = $(this);

            var valid = selectors.form.valid();
            if (valid && canInteract) {
                MAINMODULE.Common.DisableButton(btn);

                submitForm(btn);
            }

            return false;
        });
    }

    function bindBtnVote() {
        objs.container.on('click', '.brainstorm-button', function () {
            var btn = $(this);
            var item = btn.closest('.brainstorm-item');
            var id = item.data('id');
            var vote = btn.data('vote');
            var sameVote = item.data('currentuservote') === vote;

            if (canInteract === true && !sameVote) {
                var url = rootUrl + "/vote";

                return $.post(url, { votingItemId: id, voteValue: vote }).then(function (response) {
                    if (response.success === true) {
                        location.reload();
                    }
                    else {
                        MAINMODULE.Ajax.HandleErrorResponse(response);
                    }
                });
            }
        });
    }

    function loadSession() {
        selectors.list.html(MAINMODULE.Default.Spinner);

        var url = "/list";

        var sessionId = $('#Id').val();

        if (sessionId) {
            url += '/' + sessionId;
        }

        MAINMODULE.Ajax.LoadHtml(rootUrl + url, selectors.list);
    }

    function loadNewForm() {
        var sessionId = $('#brainstormcontainer #Id').val();

        MAINMODULE.Ajax.LoadHtml(rootUrl + "/" + sessionId + "/newidea", objs.container).then((response) => {
            selectors.form = $("#frmBrainstormIdeaSave");

            $.validator.unobtrusive.parse(selectors.form);
        });
    }

    function loadNewSessionForm() {
        MAINMODULE.Ajax.LoadHtml(rootUrl + "/newsession", objs.container).then((response) => {
            selectors.form = $("#frmBrainstormSessionSave");

            $.validator.unobtrusive.parse(selectors.form);
        });
    }

    function submitForm(btn, callback) {
        var url = selectors.form.attr('action');

        var data = selectors.form.serialize();

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

    return {
        Init: init
    };
}());

$(function () {
    BRAINSTORM.Init();
});