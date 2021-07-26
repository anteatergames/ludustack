var FORUMPOSTDETAILS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;
    var urlReplies;

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.replies = '#divReplies';
        selectors.userId = '#UserId';
        selectors.btnSavePost = '.btn-post-save';
        selectors.btnSaveReply = '.btn-post-save-reply';
        selectors.btnEditPost = '.btn-post-edit';
        selectors.btnEditCancel = '.btn-edit-cancel';
        selectors.btnReply = '.btn-reply';
        selectors.postItem = '.postitem';
        selectors.postReply = '.postreply';
        selectors.postItemContainer = '.postitemcontainer';
        selectors.postItemContainerEdit = '.postitemcontaineredit';
        selectors.txtReply = '.txtReply';
        selectors.hdnReplyPostId = '#hdnReplyPostId';
        selectors.hdnReplyUserId = '#hdnReplyUserId';
        selectors.postReplyAlert = '#postreplyalert';
        selectors.postReplyAuthorName = '#postreplyauthorname';
        selectors.btnCloseReplyAlert = '#btnclosereplyalert';
        selectors.btnPage = 'a.page-link';
        selectors.latest = '#hdnLatest';
        selectors.btnScrollTo = '.btn-scrollto';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.replies = $(selectors.replies);
        objs.userId = $(selectors.userId);
        objs.latest = $(selectors.latest);
    }

    function init() {
        setSelectors();
        cacheObjs();

        canInteract = MAINMODULE.CanInteract();
        urlReplies = objs.urls.data('urlReplies');

        bindAll();

        loadItems(urlReplies);

        FORUMCOMMON.Callback.DeleteEntity = deleteCallback;

        PAGINATION.Init(selectors.btnPage, selectors.replies);

        hljs.highlightAll();
    }

    function bindAll() {
        WYSIWYGEDITOR.BindEditors('.wysiwygeditor');
        bindBtnSaveReply();
        bindBtnSavePost();
        bindBtnEdit();
        bindBtnEditCancel();
        bindBtnReply();
        bindBtnCloseReplyAlert();
        bindBtnScrollTo();
    }

    function bindBtnSaveReply() {
        objs.container.on('click', selectors.btnSaveReply, function (e) {
            var btn = $(this);

            if (canInteract && !btn.hasClass('disabled')) {
                MAINMODULE.Common.DisableButton(btn).ready(() => {
                    saveReply(btn);
                });
            }

            e.preventDefault();
            return false;
        });
    }

    function bindBtnSavePost() {
        objs.container.on('click', selectors.btnSavePost, function (e) {
            var btn = $(this);

            if (canInteract && !btn.hasClass('disabled')) {
                MAINMODULE.Common.DisableButton(btn).ready(() => {
                    savePost(btn);
                });
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

    function bindBtnReply() {
        objs.container.on('click', selectors.btnReply, function (e) {
            var btn = $(this);
            var postItem = btn.closest(selectors.postItem);
            var replyPostId = postItem.data('postid');
            var replyUserId = postItem.data('userid');
            var authorName = postItem.data('authorname');
            var postReply = $(selectors.postReply);
            var replyHiddenPostId = postReply.find(selectors.hdnReplyPostId);
            var replyHiddenUserId = postReply.find(selectors.hdnReplyUserId);
            var replyAuthorName = $(selectors.postReplyAuthorName);
            var postReplyAlert = $(selectors.postReplyAlert);

            replyHiddenPostId.val(replyPostId);
            replyHiddenUserId.val(replyUserId);
            replyAuthorName.text(authorName);

            postReplyAlert.hide().removeClass('d-none').slideDown();

            var txtArea = $(selectors.txtReply);
            var editorId = txtArea.attr('data-editor-id');

            var complete = false;
            $('html, body').animate({
                scrollTop: postReply.offset().top
            }, {
                complete: () => {
                    if (!complete) {
                        complete = true;
                        WYSIWYGEDITOR.GetEditor(editorId).editor.editing.view.focus();
                    }
                }
            }, 1000);

            e.preventDefault();
            return false;
        });
    }

    function bindBtnCloseReplyAlert() {
        objs.container.on('click', selectors.btnCloseReplyAlert, function (e) {
            e.preventDefault();

            var btn = $(this);
            var postReply = $(selectors.postReply);
            var replyHiddenPostId = postReply.find(selectors.hdnReplyPostId);
            var replyHiddenUserId = postReply.find(selectors.hdnReplyUserId);

            replyHiddenPostId.val('');
            replyHiddenUserId.val('');

            btn.closest('.alert').slideUp();

            return false;
        });
    }

    function bindBtnScrollTo() {
        objs.container.on('click', selectors.btnScrollTo, function (e) {
            e.preventDefault();

            var btn = $(this);

            var toBottom = btn.hasClass('btn-scrollto-bottom');

            if (toBottom) {
                scrollToLatest();
            }
            else {
                MAINMODULE.Utils.ScrollToTop();
            }

            return false;
        });
    }

    function loadItems(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.replies).then(() => {
            objs.replies.hide();

            objs.replies.slideDown('fast', () => {
                setTimeout(function () {
                    if (objs.latest.length > 0) {
                        scrollToLatest();
                    }
                }, 100);
            });

            hljs.highlightAll();
        });
    }

    function saveReply(btn) {
        var form = btn.closest('form');
        var url = form.attr('action');
        var txtArea = form.find(selectors.txtReply);
        var editorId = txtArea.attr('data-editor-id');

        WYSIWYGEDITOR.UpdateSourceElement(editorId);

        var data = form.serializeObject();

        return $.post(url, data).done(function (response) {
            if (response.success === true) {
                MAINMODULE.Common.PostSaveCallback(response, btn);

                MAINMODULE.Ajax.HandleUrlResponse(response);
            }
            else {
                MAINMODULE.Common.EnableButton(btn);
                MAINMODULE.Ajax.HandleErrorResponse(response);
            }
        });
    }

    function savePost(btn) {
        var form = btn.closest('form');
        var url = form.attr('action');
        var txtArea = form.find(selectors.txtReply);
        var editorId = txtArea.attr('data-editor-id');

        WYSIWYGEDITOR.UpdateSourceElement(editorId);

        var data = form.serializeObject();

        return $.post(url, data).done(function (response) {
            if (response.success === true) {
                MAINMODULE.Common.PostSaveCallback(response, btn);

                var detailsContainer = btn.closest(selectors.postItem).find(selectors.postItemContainer);

                MAINMODULE.Ajax.LoadHtml(response.url, detailsContainer).then(() => editCancel(btn));
            }
            else {
                MAINMODULE.Common.EnableButton(btn);
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

        MAINMODULE.Common.DisableButton(btn).ready(() => {
            postDiv.css('height', postDiv.css('height'));

            MAINMODULE.Ajax.LoadHtml(urlEdit, editDiv).then(() => {
                var txtArea = editDiv.find(selectors.txtReply);

                WYSIWYGEDITOR.BindEditor(`#${txtArea.attr('id')}`).then(() => {
                    viewDiv.removeClass('d-flex').fadeOut("slow", function () {
                        postDiv.animate({ 'height': editDiv.css('height') },
                            {
                                complete: () => {
                                    editDiv.fadeIn("slow");

                                    MAINMODULE.Common.EnableButton(btn);
                                    postDiv.css('height', '');
                                }
                            });
                    });
                });
            });
        });
    }

    function editCancel(btn) {
        var postDiv = btn.closest(selectors.postItem);

        var viewDiv = postDiv.find(selectors.postItemContainer);
        var editDiv = postDiv.find(selectors.postItemContainerEdit);

        postDiv.css('height', postDiv.css('height'));
        editDiv.fadeOut("slow", function () {
            var txtArea = editDiv.find(selectors.txtReply);
            var editorId = txtArea.attr('data-editor-id');

            WYSIWYGEDITOR.DestroyEditor(editorId);

            editDiv.html('');

            postDiv.animate({ 'height': viewDiv.css('height') },
                {
                    complete: () => {
                        viewDiv.addClass('d-flex').fadeIn();

                        postDiv.css('height', '');

                        hljs.highlightAll();
                    }
                });
        });
    }

    function scrollToLatest() {
        var elementToScroll = $(selectors.postItem).last();

        MAINMODULE.Utils.ScrollTo(elementToScroll);
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