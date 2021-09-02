var BRAINSTORM = (function () {
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

        canInteract = MAINMODULE.CanInteract();
        newIdea = window.location.href.indexOf('newidea') > -1;
        details = window.location.href.indexOf('details') > -1;

        if (!newIdea && !details) {
            loadSession(0);
        }

        objs.ddlStatus.removeClass('invisible').show();
    }

    function setSelectors() {
        selectors.container = '#contentwrapper';
        selectors.toolbar = $("#divToolbar");
        selectors.list = $("#divList");
        selectors.btnPostVotingItem = $("#btnPostVotingItem");
        selectors.btnSave = '.btn-save';
        selectors.form = $("#frmBrainstormIdeaSave");
        selectors.ddlStatus = '#ddlStatus';
        selectors.ddlFilter = '#ddlFilter';
    }

    function cacheObjects() {
        objs.container = $(selectors.container);
        objs.ddlStatus = $(selectors.ddlStatus);
        objs.ddlFilter = $(selectors.ddlFilter);
    }

    function bindAll() {
        bindBtnNewIdea();
        bindBtnNewSession();
        bindBtnSave();
        bindBtnVote();
        bindStatusChange();
        bindfilterChange();
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
                    MAINMODULE.Ajax.HandleUrlResponse(response);
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
            if (valid && canInteract && !btn.hasClass('disabled')) {
                MAINMODULE.Common.DisableButton(btn).ready(() => {
                    submitForm(btn);
                });
            }

            return false;
        });
    }

    function bindBtnVote() {
        objs.container.on('click', '.brainstorm-button', function () {
            var btn = $(this);
            var item = btn.closest('.brainstorm-item');
            var ideaId = item.data('id');
            var vote = btn.data('vote');
            var sameVote = item.data('currentuservote') === vote;

            if (canInteract === true && !sameVote) {
                var url = rootUrl + "/vote";

                return $.post(url, { ideaId, vote }).then(function (response) {
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

    function bindfilterChange() {
        objs.container.on('change', selectors.ddlFilter, function () {
            loadSession($(this).val());
        });
    }

    function loadSession(filter) {
        selectors.list.html(MAINMODULE.Default.Spinner);

        var url = "/list";

        var sessionId = $('#Id').val();

        if (sessionId) {
            url += '/' + sessionId;
        }

        if (filter !== null && filter !== undefined) {
            url += '?filter=' + filter;
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