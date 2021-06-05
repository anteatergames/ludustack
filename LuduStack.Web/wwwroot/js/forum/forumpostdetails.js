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
        selectors.btnSaveAnswer = '.btn-post-save-answer';
        selectors.btnEditPost = '.btn-post-edit';
        selectors.btnEditCancel = '.btn-edit-cancel';
        selectors.postItem = '.postitem';
        selectors.postItemContainer = '.postitemcontainer';
        selectors.postItemContainerEdit = '.postitemcontaineredit';
        selectors.txtAnswer = '.txtAnswer';
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
        bindEditors();
        bindBtnSaveAnswer();
        bindBtnSavePost();
        bindBtnEdit();
        bindBtnEditCancel();
    }


    function bindEditors() {
        $(selectors.txtAnswer).each((index, element) => {
            var id = element.id;

            WYSIWYGEDITOR.BindEditor(`#${id}`).then((editorId) => {
                $(element).attr('data-editor-id', editorId);
            });
        });
    }

    function bindEditor(selector) {
        var element = document.querySelector(selector);

        return WYSIWYGEDITOR.BindEditor(selector).then((editorId) => {
            $(element).attr('data-editor-id', editorId);
            return editorId;
        });
    }

    function bindBtnSaveAnswer() {
        objs.container.on('click', selectors.btnSaveAnswer, function (e) {
            var btn = $(this);
            var form = btn.closest('form');
            var valid = form.valid();

            if (valid && canInteract) {
                MAINMODULE.Common.DisableButton(btn);

                saveAnswer(btn);
            }

            e.preventDefault();
            return false;
        });
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
        MAINMODULE.Ajax.LoadHtml(url, objs.answers).then(() => {
            objs.answers.hide();

            objs.answers.slideDown();
        });
    }

    function saveAnswer(btn) {
        var form = btn.closest('form');
        var url = form.attr('action');
        var txtArea = form.find(selectors.txtAnswer);
        var editorId = txtArea.attr('id');

        WYSIWYGEDITOR.UpdateSourceElement(editorId);

        $.validator.unobtrusive.parse(form);

        var valid = form.valid();

        if (valid && canInteract) {
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
    }

    function savePost(btn) {
        var form = btn.closest('form');
        var url = form.attr('action');
        var txtArea = form.find(selectors.txtAnswer);
        var editorId = txtArea.attr('id');

        WYSIWYGEDITOR.UpdateSourceElement(editorId);

        $.validator.unobtrusive.parse(form);

        var valid = form.valid();

        if (valid && canInteract) {
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
    }

    function edit(btn) {
        var urlEdit = btn.attr('href');

        var postDiv = btn.closest(selectors.postItem);

        var viewDiv = postDiv.find(selectors.postItemContainer);
        var editDiv = postDiv.find(selectors.postItemContainerEdit);

        editDiv.hide();

        MAINMODULE.Common.DisableButton(btn);

        postDiv.css('height', postDiv.css('height'));

        MAINMODULE.Ajax.LoadHtml(urlEdit, editDiv).then(() => {
            var txtArea = editDiv.find(selectors.txtAnswer);

            bindEditor(`#${txtArea.attr('id')}`).then(() => {

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
    }

    function editCancel(btn) {
        var postDiv = btn.closest(selectors.postItem);

        var viewDiv = postDiv.find(selectors.postItemContainer);
        var editDiv = postDiv.find(selectors.postItemContainerEdit);

        postDiv.css('height', postDiv.css('height'));
        editDiv.fadeOut("slow", function () {
            var txtArea = editDiv.find(selectors.txtAnswer);
            var editorId = txtArea.attr('id');

            WYSIWYGEDITOR.DestroyEditor(editorId);

            editDiv.html('');

            postDiv.animate({ 'height': viewDiv.css('height') },
                {
                    complete: () => {
                        viewDiv.addClass('d-flex').fadeIn();

                        postDiv.css('height', '');
                    }
                });
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