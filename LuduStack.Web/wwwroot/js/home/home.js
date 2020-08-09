var HOMEPAGE = (function () {
    "use strict";

    var postModalActive = false;
    var divPostGameActive = false;
    var divPostImagesActive = false;
    var divPostPollActive = false;

    var postImagesDropZone = null;

    var minheight = 147;
    var defaultCommentBoxHeight = minheight;
    var defaultTxtPostContentHeight = 0;
    var selectors = {};
    var objs = {};

    function setSelectors() {
        selectors.divCounters = '#divCounters';
        selectors.divGameIdea = '#divGameIdea';
        selectors.divLatestGames = '#divLatestGames';
        selectors.divActivityFeed = '#divActivityFeed';
        selectors.divPostGame = 'div#divPostGame';
        selectors.divPostImages = 'div#divPostImages';
        selectors.divPostPoll = 'div#divPostPoll';
        selectors.txtPostContent = '#txtPostContent';
        selectors.commentBox = '.commentmodal';
        selectors.commentModal = '.commentmodal .modal';
        selectors.postImages = '#txtPostImages';
        selectors.commentModalBody = undefined;
        selectors.ddlPostGameId = '#ddlPostGameId';
        selectors.btnPostAddGame = '#btnPostAddGame';
        selectors.btnPostAddImage = '#btnPostAddImage';
        selectors.btnPostAddPoll = '#btnPostAddPoll';
    }

    function cacheObjects() {
        objs.divActivityFeed = $(selectors.divActivityFeed);
        objs.divPostGame = $(selectors.divPostGame);
        objs.divPostImages = $(selectors.divPostImages);
        objs.divPostPoll = $(selectors.divPostPoll);
        objs.txtPostContent = $(selectors.txtPostContent);
        objs.commentBox = $(selectors.commentBox);
        objs.commentModal = $(selectors.commentModal);
        objs.postImages = $(selectors.postImages);
        objs.ddlPostGameId = $(selectors.ddlPostGameId);
        objs.btnPostAddGame = $(selectors.btnPostAddGame);
        objs.btnPostAddImage = $(selectors.btnPostAddImage);
        objs.btnPostAddPoll = $(selectors.btnPostAddPoll);
    }

    function init() {
        setSelectors();
        cacheObjects();

        bindAll();

        loadCounters();

        loadGameIdea();

        loadLatestGames();

        //defaultCommentBoxHeight = Math.ceil(objs.commentBox.outerHeight());

        defaultTxtPostContentHeight = Math.ceil(objs.txtPostContent.height());

        POLLS.Events.PostAddOption = resizePostBox;

        ACTIVITYFEED.Init(objs.divActivityFeed, FEEDTYPE.HOME);

        ACTIVITYFEED.Methods.LoadActivityFeed();

        setStickyElements();
    }

    function bindAll() {
        bindSendSimpleContent();
        bindShowCommentModal();
        bindBtnPostAddGameBtn();
        bindPostAddImageBtn();
        bindPostTextArea();
        bindBtnPostAddPollBtn();
        bindPostModalHide();
    }

    function bindPostTextArea() {
        $('.content').on('keydown', 'textarea.postbox', function (e) {
            var txtArea = $(this);
            if ((e.keyCode === 10 || e.keyCode === 13) && e.ctrlKey) {
                var text = txtArea.val();

                if (text.length > 0) {
                    // send
                }
            }

            autosize(this);
        });
    }

    function bindShowCommentModal() {
        $('.content').on('click', '.posttextarea', function () {
            if (postModalActive === false) {
                showPostModal();
                selectors.commentModalBody = $('.commentmodal .modal .modal-body');
            }
            $(this).focus();
            resizePostBox();
            objs.commentModal.animate({ scrollTop: 0 }, "fast");
        });
    }

    function bindBtnPostAddGameBtn() {
        $('.content').on('click', selectors.btnPostAddGame, function () {
            if (postModalActive === false) {
                showPostModal();
            }

            if (divPostGameActive) {
                objs.btnPostAddGame.removeClass('btn-warning');
                hideGameAdd();
            } else {
                objs.btnPostAddGame.addClass('btn-warning');
                showGameAdd();
            }

            resizePostBox();
        });
    }

    function bindPostAddImageBtn() {
        Dropzone.autoDiscover = false;
        $('.content').on('click', selectors.btnPostAddImage, function () {
            if (postModalActive === false) {
                showPostModal();
            }

            if (divPostImagesActive) {
                objs.btnPostAddImage.removeClass('btn-warning');
                hideImageAdd();

                if (postImagesDropZone) {
                    postImagesDropZone.destroy();
                    postImagesDropZone = null;
                }
            }
            else {
                objs.btnPostAddImage.addClass('btn-warning');
                divPostImagesActive = true;
                if (!postImagesDropZone) {
                    instantiateDropZone();

                    postImagesDropZone.on("addedfile", function (file) {
                        resizePostBox();
                        console.log(file);
                    });
                }

                objs.divPostImages.show();
            }

            resizePostBox();
        });
    }

    function bindBtnPostAddPollBtn() {
        $('.content').on('click', selectors.btnPostAddPoll, function () {
            if (postModalActive === false) {
                showPostModal();
            }

            if (divPostPollActive) {
                objs.btnPostAddPoll.removeClass('btn-warning');
                hidePollAdd();
                POLLS.Methods.ClearOptions();
            } else {
                objs.btnPostAddPoll.addClass('btn-warning');
                showPollAdd();
            }

            resizePostBox();
        });
    }

    function bindSendSimpleContent() {
        $('.content').on('click', '#btnSendSimpleContent', function () {
            var btn = $(this);
            var txtArea = btn.closest('.simplecontentpostarea').find('.posttextarea');
            var text = txtArea.val().replace(/\n/g, '<br>\n');
            if (text.length === 0) {
                ALERTSYSTEM.ShowWarningMessage("You must type a text to post!");
                return false;
            }

            var languageSelect = objs.commentBox.find('#postlanguage');
            var language = languageSelect.val();
            var gameId = objs.ddlPostGameId.val();

            var pollOptions = document.getElementsByClassName("polloptioninput");

            var options = $(pollOptions).map(function () {
                var imageBtn = $(this).next().children();
                var img = imageBtn.data('image');
                return this.value ? {
                    text: this.value,
                    image: img
                } : null;
            }).get();

            if (!postImagesDropZone || postImagesDropZone.getQueuedFiles().length === 0) {
                var images = objs.postImages.val();
                var json = { text: text, gameId: gameId, images: images, pollOptions: options, language: language };

                sendSimpleContent(json).done(function (response) {
                    if (response.success) {
                        sendSimpleContentCallback(response, txtArea);
                    }
                });
            }
            else {
                postImagesDropZone.processQueue();

                var success = false;

                postImagesDropZone.on("success", function (file) {
                    var response = JSON.parse(file.xhr.response);
                    if (response.uploaded) {
                        success = true;
                        objs.postImages.val(objs.postImages.val() + '|' + response.url);
                    }
                    else {
                        if (response.error) {
                            ALERTSYSTEM.ShowWarningMessage(response.error);
                        }
                    }
                });

                postImagesDropZone.on("queuecomplete", function (file) {
                    if (success === true) {
                        var images2 = objs.postImages.val();
                        var json2 = { text: text, gameId: gameId, images: images2, pollOptions: options };
                        sendSimpleContent(json2).done(function (response) {
                            sendSimpleContentCallback(response, txtArea);

                            if (postImagesDropZone) {
                                postImagesDropZone.destroy();
                                postImagesDropZone = null;
                            }

                            instantiateDropZone();
                        });
                        console.log(file);
                    }
                });
            }
        });
    }

    function bindPostModalHide() {
        $('#modalPost').on('hidden.bs.modal', function () {
            hideGameAdd();
            hideImageAdd();
            hidePollAdd();

            if (postModalActive === true) {
                objs.txtPostContent.val('');

                hidePostModal();
            }
            else {
                resizePostBox();
            }
            POLLS.Methods.ClearOptions();
        });
    }

    function instantiateDropZone() {
        postImagesDropZone = new Dropzone("div#divPostImages", {
            url: '/storage/uploadcontentimage',
            paramName: 'upload',
            addRemoveLinks: true,
            autoProcessQueue: false,
            maxFiles: 1
            //resizeWidth
        });
    }

    function showPostModal() {
        postModalActive = true;
        $('#modalPost').addClass('modal');
        $('#modalPost').modal('show');
        objs.commentBox.css('min-height', minheight + 'px');
        objs.commentModal.css('padding-right', '');
        $('.commentmodal .modal-header').removeClass('d-none');
        $('.commentmodal .modal-footer').removeClass('d-none');
        $('.commentmodal .modal-header .close').show();
        $('.modal-backdrop').css('height', window.innerHeight + 'px');
        $('.modal-backdrop').css('top', window.pageYOffset + 'px');
    }

    function resizePostBox() {
        var divPostGameHeight = objs.divPostGame.outerHeight();
        var divPostImagesHeight = objs.divPostImages.outerHeight();
        var divPostPollHeight = objs.divPostPoll.outerHeight();

        var h = Math.floor(objs.txtPostContent.height());
        var txtPostContentHeight = h === defaultTxtPostContentHeight ? 0 : h - defaultTxtPostContentHeight;

        var height = defaultCommentBoxHeight + txtPostContentHeight;

        var extra = 0;
        if (divPostGameActive) {
            height += divPostGameHeight;
            extra += 15;
        }

        if (divPostImagesActive) {
            height += divPostImagesHeight;
            extra += 10;
        }

        if (divPostPollActive) {
            height += divPostPollHeight;
            extra += 10;
        }

        height += extra;

        selectors.commentModalBody.height(objs.txtPostContent.height());

        objs.commentModal.height(height);

        objs.commentBox.height(height);
    }

    function hidePostModal() {
        postModalActive = false;
        $('.commentmodal .modal').css('padding-right', '');
        $('#modalPost').modal('hide');
        $('#modalPost').css('display', '');
        $('#modalPost').removeClass('modal');
        $('.commentmodal .modal-header .close').hide();

        $('.commentmodal .modal-header').addClass('d-none');
        $('.commentmodal .modal-footer').addClass('d-none');

        $('.commentmodal').css('height', '');
        $('.commentmodal').css('min-height', '');

        selectors.commentModalBody.height('38px');

        resizeTextArea(objs.txtPostContent[0]);
    }

    function hideGameAdd() {
        divPostGameActive = false;
        objs.divPostGame.hide();
        objs.ddlPostGameId.val('');
    }

    function hideImageAdd() {
        divPostImagesActive = false;
        objs.divPostImages.hide();
    }

    function hidePollAdd() {
        divPostPollActive = false;
        objs.divPostPoll.hide();
    }

    function showGameAdd() {
        divPostGameActive = true;
        objs.divPostGame.show();
    }

    function showPollAdd() {
        divPostPollActive = true;
        objs.divPostPoll.show();
    }

    function sendSimpleContent(json) {
        return $.post("/content/post", json)
            .done(function (response) {
                if (!response.success) {
                    ALERTSYSTEM.ShowWarningMessage(response.message);
                }
            });
    }
    function sendSimpleContentCallback(response, txtArea) {
        hidePostModal();
        hideImageAdd();

        if (postImagesDropZone) {
            postImagesDropZone.removeAllFiles();
        }

        if (response.success === true) {
            txtArea.val('');
            CONTENTACTIONS.AutosizeTextArea(txtArea[0]);
            ACTIVITYFEED.Methods.LoadActivityFeed();
            if (postImagesDropZone) {
                postImagesDropZone.disable();
            }

            objs.postImages.val('');

            MAINMODULE.Common.HandlePointsEarned(response);
        }
    }

    function loadCounters() {
        MAINMODULE.Ajax.LoadHtml("/home/counters", selectors.divCounters);
    }

    function loadGameIdea() {
        MAINMODULE.Ajax.LoadHtml("/home/gameidea", selectors.divGameIdea).then(() => {
            GAMEIDEA.Init();
        });
    }

    function loadLatestGames() {
        MAINMODULE.Ajax.LoadHtml("/game/latest", selectors.divLatestGames).then(() => {
            //lazyLoadInstance.update();
        });
    }

    function autosize(el) {
        setTimeout(function () {
            resizeTextArea(el);

            resizePostBox();
        }, 0);
    }

    function resizeTextArea(el) {
        el.style.cssText = 'height:auto;';
        el.style.cssText = 'height:' + (el.scrollHeight + 2) + 'px';
    }

    function setStickyElements() {
        var isLg = window.matchMedia('screen and (min-width: 992px)').matches;
        var isXl = window.matchMedia('screen and (min-width: 1200px)').matches;

        if (isLg) {
            MAINMODULE.Layout.SetStickyElement('#infobox');
        }

        if (isXl) {
            MAINMODULE.Layout.SetStickyElement('#stickyLeft', 60, '#leftColumn');
        }

        window.addEventListener('resize', function (e) {
            $('#stickyLeft').sticky('update');
            isLg = window.matchMedia('screen and (min-width: 992px)').matches;
            isXl = window.matchMedia('screen and (min-width: 1200px)').matches;

            if (isXl) {
                MAINMODULE.Layout.SetStickyElement('#stickyLeft', 60, '#leftColumn');
            }
            else {
                $('#stickyLeft').unstick();
            }
        });
    }

    return {
        Init: init
    };
}());

$(function () {
    HOMEPAGE.Init();
});

if (typeof Embedo === 'function') {
    new Embedo({
        youtube: true,
        facebook: {
            appId: $('meta[property="fb:app_id"]').attr('content'), // Enable facebook SDK
            version: 'v3.2',
            width: "100%"
        }
    });
}

Dropzone.autoDiscover = false;