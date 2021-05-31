var FORUMPOSTDETAILS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;
    var urlAnswers;

    function setSelectors() {
        selectors.canInteract = '#caninteract';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.answers = '#divAnswers';
        selectors.userId = '#UserId';
        selectors.btnSavePost = '.btn-post-save';
        selectors.btnEditPost = '.btn-post-edit';
        selectors.btnEditCancel = '.btn-edit-cancel';
        selectors.postItem = '.postitem';
        selectors.postItemContainer = '.postitemcontainer';
        selectors.postItemContainerEdit = '.postitemcontaineredit';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.answers = $(selectors.answers);
        objs.userId = $(selectors.userId);
    }

    function init() {
        setSelectors();
        cacheObjs();

        canInteract = $(selectors.canInteract).val();
        urlAnswers = objs.urls.data('urlAnswers');

        bindAll();

        loadItems(urlAnswers);

        FORUMCOMMON.Callback.DeleteEntity = deleteCallback;
    }

    function bindAll() {
        bindBtnSavePost();
        bindBtnEdit();
        bindBtnEditCancel();
    }

    function bindBtnSavePost() {
        objs.container.on('click', selectors.btnSavePost, function (e) {
            var btn = $(this);
            var form = btn.closest('form');
            var valid = form.valid();

            if (valid && canInteract) {
                MAINMODULE.Common.DisableButton(btn);

                savePost(btn);
            }

            e.preventDefault();
            return false;
        });
    }

    function bindBtnEdit() {
        objs.container.on('click', selectors.btnEditPost, function (e) {
            var btn = $(this);

            edit(btn);

            e.preventDefault();
            return false;
        });
    }

    function bindBtnEditCancel() {
        objs.container.on('click', selectors.btnEditCancel, function (e) {
            var btn = $(this);

            editCancel(btn);

            e.preventDefault();
            return false;
        });
    }

    function loadItems(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.answers);
    }

    function savePost(btn) {
        var form = btn.closest('form');
        var url = form.attr('action');

        var data = form.serializeObject();

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

    function edit(btn) {
        var urlEdit = btn.attr('href');

        var postDiv = btn.closest(selectors.postItem);

        var viewDiv = postDiv.find(selectors.postItemContainer);
        var editDiv = postDiv.find(selectors.postItemContainerEdit);
        editDiv.hide();

        MAINMODULE.Common.DisableButton(btn);

        MAINMODULE.Ajax.LoadHtml(urlEdit, editDiv).then(() => {
            var newHeight = editDiv.css('height');

            postDiv.animate({ 'height': newHeight });

            viewDiv.removeClass('d-flex').fadeOut("slow", function () {
                editDiv.fadeIn("slow");

                MAINMODULE.Common.EnableButton(btn);
            });
        });
    }

    function editCancel(btn) {
        var postDiv = btn.closest(selectors.postItem);

        var viewDiv = postDiv.find(selectors.postItemContainer);
        var editDiv = postDiv.find(selectors.postItemContainerEdit);

        var newHeight = viewDiv.css('height');

        postDiv.css('height', postDiv.css('height'));
        editDiv.fadeOut("slow", function () {
            postDiv.animate({ 'height': newHeight });
            viewDiv.addClass('d-flex').fadeIn();
        });
    }

    function deleteCallback(response, btn) {
        if (response.success) {
            btn.closest(selectors.postItem).remove();
            btn.closest(selectors.postItem).parent().remove();
        }
    }

    return {
        Init: init
    };
}());

$(function () {
    FORUMPOSTDETAILS.Init();
});