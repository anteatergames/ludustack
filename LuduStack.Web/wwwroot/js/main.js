﻿var MAINMODULE = (function () {
    "use strict";

    var spinnerCenter = '<div class="spinner flex-square rectangle bg-transparent"><div class="flex-square-inner"><div class="flex-square-inner-content text-dark"><i class="fa fa-spinner fa-3x fa-spin"></i></div></div></div>';

    var spinnerTop = '<div class="bg-transparent text-center"><div class="flex-square-inner"><div class="flex-square-inner-content text-dark"><i class="fa fa-spinner fa-3x fa-spin"></i></div></div></div>';

    var spinnerBtn = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>';

    var doneBtn = '<i class="fas fa-check"></i>';

    var selectors = {};
    var objs = {};
    var translatedMessages = {};
    var saveBtnOriginalText = '';

    function init() {
        setSelectors();
        cacheObjects();

        bindAll();

        setGlobalAjax();

        loadTranslatedMessages();

        loadNotifications();

        $('[data-toggle="tooltip"]').tooltip();

        showMessage();
    }

    function setSelectors() {
        selectors.notificationsMenu = "#notificationsMenu";
        selectors.locale = '#locale';
        selectors.spanMessage = "#spanMessage";
        selectors.translatedJavascriptMessages = "#translatedJavascriptMessages";
        selectors.sharePopup = '.share-popup';
        selectors.btnInteractionShare = '.btn-interaction-share';
    }

    function cacheObjects() {
        objs.notificationsMenu = $(selectors.notificationsMenu);
        objs.locale = $(selectors.locale);
        objs.spanMessage = $(selectors.spanMessage);
        objs.translatedJavascriptMessages = $(selectors.translatedJavascriptMessages);
    }

    function bindAll() {
        bindMenu();
        bindNotImplemented();
        bindYouNeedToLogIn();
    }

    function bindMenu() {
        $('body').on('click', '#mainmenu li', function () {
            var clickedMenu = $(this);
            var siblings = clickedMenu.siblings();

            siblings.removeClass('active');

            clickedMenu.addClass('active');
        });
    }
    function loadTranslatedMessages() {
        objs.translatedJavascriptMessages.find('.msg').each(function () {
            var msgId = $(this).data('msgId');
            var text = $(this).text();

            translatedMessages[msgId] = text;
        });
    }

    function showMessage() {
        var msg = objs.spanMessage.text();
        if (msg !== undefined && msg.length > 0) {
            ALERTSYSTEM.Toastr.ShowWarning(msg);
            history.replaceState({}, null, window.location.href.split('?')[0]);
        }
    }

    function bindNotImplemented() {
        $('body').on('click', '.notimplemented', function (e) {
            e.preventDefault();

            var msg = translatedMessages['msgNotImplementedYet'];

            ALERTSYSTEM.Toastr.ShowWarning(msg);

            return false;
        });
    }

    function bindYouNeedToLogIn() {
        $('body').on('click', '.needlogin', function (e) {
            e.preventDefault();
            var msgId = $(this).data('msgId');

            var msg = translatedMessages[msgId];

            ALERTSYSTEM.Toastr.ShowWarning(msg);

            return false;
        });
    }

    function setGlobalAjax() {
        $(document).ajaxStart(function () { Pace.restart(); });
    }

    function loadNotifications() {
        if (objs.notificationsMenu.length > 0) {
            MAINMODULE.Ajax.LoadHtml("/home/notifications", objs.notificationsMenu);
        }
    }

    function handlePointsEarned(response) {
        if (response.pointsEarned > 0) {
            var msg = translatedMessages['mgsPointsEarned'];
            msg = msg.replace('0', response.pointsEarned);

            if (response.message) {
                msg = response.message + ' ' + msg;
            }

            ALERTSYSTEM.Toastr.PointsEarned(msg);

            return true;
        }

        return false;
    }

    function setStickyElement(selector, offset, getWidthFrom) {
        var minimumOffset = 60;

        if (offset !== undefined) {
            minimumOffset = offset;
        }

        $(selector).sticky({
            topSpacing: minimumOffset,
            widthFromWrapper: getWidthFrom === undefined ? true : false,
            getWidthFrom: getWidthFrom === undefined ? '' : getWidthFrom
        });
    }

    function disableButton(btn) {
        btn.addClass('disabled');
        saveBtnOriginalText = btn.html();
        btn.html(MAINMODULE.Default.SpinnerBtn);
    }

    function enableButton(btn) {
        btn.removeClass('disabled');
    }

    function setButtonWithError(btn) {
        saveBtnOriginalText = btn.html();
        var errMsg = btn.data('errorMsg');
        btn.html(errMsg);
    }

    function removeErrorFromButton(btn) {
        btn.removeClass('disabled').html(saveBtnOriginalText);
    }

    function postSaveCallback(response, btn) {
        if (response.success === true) {
            btn.removeClass('disabled').html(MAINMODULE.Default.DoneBtn);
        }
        else {
            btn.html(saveBtnOriginalText);
            btn.removeClass('disabled');
        }
    }

    function renameInputs(objContainer, itemSelector, propPreffix) {
        var count = 0;

        var idPreffix = propPreffix + "_0__";
        var namePreffix = propPreffix + "[0].";

        objContainer.find(itemSelector).each(function () {
            var item = $(this);

            item.find(':input').each(function () {
                var inputId = $(this).attr('id');
                var inputName = $(this).attr('name');

                if (inputId !== undefined && inputName !== undefined) {
                    var idProp = inputId.split('__')[1];
                    var newId = idPreffix.replace('0', count) + idProp;
                    $(this).attr('id', newId);

                    var nameProp = inputName.split('].')[1];
                    var newName = namePreffix.replace('0', count) + nameProp;
                    $(this).attr('name', newName);

                    var describedBy = $(this).attr('aria-describedby');
                    if (describedBy !== undefined) {
                        var describedByProp = describedBy.split('__')[1];
                        var newdescribedBy = idPreffix.replace('0', count) + describedByProp;
                        $(this).attr('aria-describedby', newdescribedBy);
                    }
                }
            });

            item.find('span[data-valmsg-for]').each(function (index2, element2) {
                var msgFor = $(this).attr('data-valmsg-for');

                if (msgFor !== undefined) {
                    var msgForProp = msgFor.split('].')[1];
                    var newMsgFor = namePreffix.replace('0', count) + msgForProp;
                    $(this).attr('data-valmsg-for', newMsgFor);
                }

                var innerSpan = $(this).find('span');

                if (innerSpan.length > 0) {
                    var spanId = innerSpan.attr('id');
                    var idProp = spanId.split('__')[1];
                    var newId = idPreffix.replace('0', count) + idProp;
                    innerSpan.attr('id', newId);
                }
            });

            count++;
        });
    }

    function getDeleteMessages(btn) {
        var msg = btn.data('confirmationmessage');
        var confirmationTitle = btn.data('confirmationtitle');
        var confirmationButtonText = btn.data('confirmationbuttontext');
        var cancelButtonText = btn.data('cancelbuttontext');

        if (msg === undefined) {
            msg = 'Are you sure you want to delete this?';
        }

        if (confirmationTitle === undefined) {
            confirmationTitle = 'Are you sure?';
        }

        if (confirmationButtonText === undefined) {
            confirmationButtonText = 'Yes, delete it!';
        }

        if (cancelButtonText === undefined) {
            cancelButtonText = 'Cancel';
        }

        return {
            msg: msg,
            confirmationTitle: confirmationTitle,
            confirmationButtonText: confirmationButtonText,
            cancelButtonText: cancelButtonText
        };
    }

    function handleSuccessDefault(response, callback, successCallback) {
        if (callback) {
            callback(response);
        }

        if (response.message) {
            if (successCallback) {
                successCallback(response);
            }

            ALERTSYSTEM.Toastr.ShowSuccess(response.message, function () {
                MAINMODULE.Ajax.HandleUrlResponse(response);
            });
        }
        else {
            MAINMODULE.Ajax.HandleUrlResponse(response);
        }
    }

    function bindPopOvers(multiple) {
        if (multiple) {
            $("[data-toggle='popover']").each(function () {
                var data = $(this).data();
                if (data.target) {
                    var contentElementId = data.target;
                    var contentHtml = $(contentElementId).html().trim();
                    data.content = contentHtml;
                }

                $(this).popover({ html: true });
            });
        } else {
            $("[data-toggle='popover']").popover({ html: true });
        }
    }

    async function getHtml(url) {
        var useJquery = false;
        var promise;

        if (useJquery) {
            promise = $.get(url);
        }
        else {
            promise = await fetch(url, {
                method: 'GET',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            }).then(function (response) {
                return response.text();
            });
        }

        return promise;
    }

    async function loadHtml(url, listObj) {
        var idList = '';

        if (listObj instanceof jQuery) {
            idList = '#' + listObj.attr('id');
        }
        else {
            idList = listObj;

            if (idList.indexOf('#') !== 0) {
                idList = '#' + idList;
            }
        }

        if (!idList) {
            return Promise.resolve();
        }
        else {
            document.querySelector(idList).innerHTML = MAINMODULE.Default.SpinnerTop;

            return getHtml(url)
                .then(function (body) {
                    document.querySelector(idList).innerHTML = body;

                    return body;
                });
        }
    }

    function callBackendAction(url, callback) {
        $.post(url).done(function (response) {
            if (response.success === true) {
                if (callback) {
                    callback(response);
                }

                ALERTSYSTEM.ShowSuccessMessage(response.message, function () {
                    MAINMODULE.Ajax.HandleUrlResponse(response);
                });
            }
            else {
                ALERTSYSTEM.ShowWarningMessage(response.message);
            }
        });
    }

    function post(url, data, options, callback) {
        $.post(url, data).done(function (response) {
            if (response.success === true) {
                if (callback) {
                    callback(response);
                }

                if (options !== undefined && options.showSuccessMessage === true) {
                    ALERTSYSTEM.ShowSuccessMessage(response.message, function () {
                        MAINMODULE.Ajax.HandleUrlResponse(response);
                    });
                }
                else {
                    MAINMODULE.Ajax.HandleUrlResponse(response);
                }
            }
            else {
                ALERTSYSTEM.ShowWarningMessage(response.message);
            }
        });
    }

    function deleteEntity(btn, callback) {
        postOrDeleteWithConfirmation(btn, 'DELETE', callback);
    }

    function postWithConfirmation(btn, callback) {
        postOrDeleteWithConfirmation(btn, 'POST', callback);
    }

    function postWithoutConfirmation(btn, callback) {
        var url = btn.data('url');

        $.ajax({
            url: url,
            type: 'POST'
        }).done(function (response) {
            if (response.success) {
                MAINMODULE.Common.HandleSuccessDefault(response);

                if (callback) {
                    callback(response);
                }
            }
            else {
                ALERTSYSTEM.ShowWarningMessage(response.message);
            }
        });
    }


    function postOrDeleteWithConfirmation(btn, httpmethod, callback) {
        var url = btn.data('url');

        var msgs = MAINMODULE.Common.GetDeleteMessages(btn);

        ALERTSYSTEM.ShowConfirmMessage(msgs.confirmationTitle, msgs.msg, msgs.confirmationButtonText, msgs.cancelButtonText, function () {
            $.ajax({
                url: url,
                type: httpmethod
            }).done(function (response) {
                if (response.success) {
                    MAINMODULE.Common.HandleSuccessDefault(response, null, callback);
                }
                else {
                    ALERTSYSTEM.ShowWarningMessage(response.message);
                }
            });
        });
    }

    function getLocale() {
        if (!objs.locale) {
            objs.locale = $(selectors.locale);
        }

        return objs.locale.val();
    }

    function getSelectedFileUrl(files, done) {
        if (files && files.length > 0) {
            var reader;
            var file = files[0];

            if (URL) {
                done(URL.createObjectURL(file));
            } else if (FileReader) {
                reader = new FileReader();
                reader.onloadend = function (e2) {
                    done(reader.result);
                };
                reader.readAsDataURL(file);
            }
        }
    }

    function dataURItoBlob(dataURI) {
        var byteString = atob(dataURI.split(',')[1]);
        var ab = new ArrayBuffer(byteString.length);
        var ia = new Uint8Array(ab);
        for (var i = 0; i < byteString.length; i++) {
            ia[i] = byteString.charCodeAt(i);
        }
        return new Blob([ab], { type: 'image/jpeg' });
    }

    function handleUrlResponse(response) {
        if (response.url) {
            window.location = response.url;
        }
    }

    return {
        Init: init,
        GetLocale: getLocale,
        Layout: {
            SetStickyElement: setStickyElement
        },
        Ajax: {
            Post: post,
            GetHtml: getHtml,
            LoadHtml: loadHtml,
            CallBackendAction: callBackendAction,
            HandleUrlResponse: handleUrlResponse
        },
        Common: {
            HandlePointsEarned: handlePointsEarned,
            HandleSuccessDefault: handleSuccessDefault,
            TranslatedMessages: translatedMessages,
            DeleteEntity: deleteEntity,
            PostWithConfirmation: postWithConfirmation,
            PostWithoutConfirmation: postWithoutConfirmation,
            DisableButton: disableButton,
            EnableButton: enableButton,
            SetButtonWithError: setButtonWithError,
            RemoveErrorFromButton: removeErrorFromButton,
            PostSaveCallback: postSaveCallback,
            RenameInputs: renameInputs,
            GetDeleteMessages: getDeleteMessages,
            BindPopOvers: bindPopOvers
        },
        Default: {
            Spinner: spinnerCenter,
            SpinnerTop: spinnerTop,
            SpinnerBtn: spinnerBtn,
            DoneBtn: doneBtn
        },
        Utils: {
            DataURItoBlob: dataURItoBlob,
            GetSelectedFileUrl: getSelectedFileUrl
        }
    };
}());

MAINMODULE.Init();