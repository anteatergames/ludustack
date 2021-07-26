﻿var LOCALIZATIONEDIT = (function () {
    "use strict";

    var selectors = {};
    var objs = {};
    var canInteract = false;
    var isNew = false;

    var propPrefix = 'Terms';

    var termsUploadDropZone = null;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';

        selectors.form = '#frmTranslationSave';
        selectors.btnSave = '#btnSaveTranslation';
        selectors.btnEdit = '.btnEditTranslationProject';
        selectors.btnDelete = '.btnDeleteTranslationProject';
        selectors.divTerms = '#divTerms';
        selectors.template = '.translation-term.template';
        selectors.btnAddTerm = '#btn-translation-term-add';
        selectors.btnDeleteTerm = '.btn-term-delete';
        selectors.divUploadTerms = 'div#divUploadTerms';
        selectors.btnUploadTerms = '#btnUploadTerms';
        selectors.btnSaveTerms = '#btnSaveTerms';
        selectors.ddlColumn = '.ddlcolumn';
        selectors.id = '#Id';
        selectors.termItem = '.translation-term';
        selectors.divNoItems = '#divNoItems';
        selectors.termCounter = '#termCounter';
    }

    function cacheObjsCreateEdit() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);

        objs.form = $(selectors.form);
        objs.divUploadTerms = $(selectors.divUploadTerms);
        objs.divTerms = $(selectors.divTerms);
        objs.btnUploadTerms = $(selectors.btnUploadTerms);
        objs.id = $(selectors.id);
        objs.termCounter = $(selectors.termCounter);
    }

    function setCreateEdit() {
        cacheObjsCreateEdit();
        bindDeleteProject();
        bindBtnSaveForm();
        bindBtnAddTerm();
        bindBtnDeleteTerm();
        bindBtnUploadTerms();
        bindBtnSaveTerms();

        MAINMODULE.Common.BindPopOvers();

        if (!isNew) {
            instantiateDropZone();
        }

        loadTerms();

        setStickyElementsEdit();
    }

    function init() {
        canInteract = MAINMODULE.CanInteract();
        isNew = COMMONEDIT.IsNew();

        setSelectors();

        setCreateEdit();
    }

    function bindBtnSaveForm() {
        objs.container.on('click', selectors.btnSave, function (e) {
            e.preventDefault();
            var btn = $(this);

            MAINMODULE.Common.RemoveErrorFromButton(btn);
            MAINMODULE.Common.DisableButton(btn);

            var valid = objs.form.valid();

            if (valid && canInteract) {
                var allRequiredFilled = true;
                var allIlRequired = objs.form.find(':input[data-val-required]');
                allIlRequired.each(function () {
                    if (allRequiredFilled === true && $(this).val().length === 0) {
                        allRequiredFilled = false;
                    }
                });

                if (!allRequiredFilled) {
                    MAINMODULE.Common.EnableButton(btn);
                    ALERTSYSTEM.ShowWarningMessage("All terms must have key and value!");
                }
                else {
                    submitForm(btn);
                }
            }
            else {
                MAINMODULE.Common.SetButtonWithError(btn);
            }

            return false;
        });
    }

    function bindBtnUploadTerms() {
        objs.container.on('click', selectors.btnUploadTerms, function (e) {
            e.preventDefault();
            var btn = $(this);

            if (window.Dropzone) {
                if (termsUploadDropZone.getQueuedFiles().length === 0) {
                    ALERTSYSTEM.Toastr.ShowWarning('You must select a XLSX file to upload!');
                }
                else {
                    MAINMODULE.Common.RemoveErrorFromButton(btn);
                    MAINMODULE.Common.DisableButton(btn).ready(() => {
                        uploadTerms(btn, function (response) {
                            MAINMODULE.Ajax.HandleUrlResponse(response);
                        });
                    });
                }
            }

            return false;
        });
    }

    function bindBtnSaveTerms() {
        objs.container.on('click', selectors.btnSaveTerms, function (e) {
            e.preventDefault();
            var btn = $(this);

            MAINMODULE.Common.DisableButton(btn);

            var valid = objs.form.valid();

            if (valid && canInteract) {
                saveTerms(btn, function (response) {
                    if (response.message) {
                        ALERTSYSTEM.Toastr.ShowWarning(response.message);
                    }
                });
            }
            else {
                MAINMODULE.Common.RemoveErrorFromButton(btn);
            }

            return false;
        });
    }

    function bindDeleteProject() {
        objs.container.on('click', selectors.btnDelete, function (e) {
            e.preventDefault();

            var btn = $(this);

            if (canInteract) {
                deleteProject(btn);
            }

            return false;
        });
    }

    function bindBtnAddTerm() {
        objs.container.on('click', selectors.btnAddTerm, function (e) {
            e.preventDefault();

            addNewTerm();

            return false;
        });
    }

    function bindBtnDeleteTerm() {
        objs.container.on('click', selectors.btnDeleteTerm, function (e) {
            e.preventDefault();

            var btn = $(this);

            deleteTerm(btn);

            return false;
        });
    }

    function loadTerms(callback) {
        objs.termCounter.html('...');

        var urlTerms = objs.urls.data('urlTerms');

        var id = objs.id.val();

        if (id !== '000000-0000-0000-0000-000000000000') {
            urlTerms = urlTerms + id;

            MAINMODULE.Ajax.LoadHtml(urlTerms, objs.divTerms).then(() => {
                objs.btnSaveTerms = $(selectors.btnSaveTerms);
                objs.divNoItems = $(selectors.divNoItems);

                checkNoItems();

                COMMONEDIT.ResetValidator(objs.form);

                if (callback) {
                    callback(response);
                }

                MAINMODULE.Common.BindPopOvers();
            });
        }
    }

    function deleteProject(btn, callback) {
        var url = btn.data('url');

        var msgs = MAINMODULE.Common.GetPostConfirmationMessages(btn);

        ALERTSYSTEM.ShowConfirmMessage(msgs.confirmationTitle, msgs.msg, msgs.confirmationButtonText, msgs.cancelButtonText, function () {
            $.ajax({
                url: url,
                type: 'DELETE'
            }).done(function (response) {
                if (response.success) {
                    if (callback) {
                        callback(response);
                    }

                    MAINMODULE.Common.HandleSuccessDefault(response);
                }
                else {
                    ALERTSYSTEM.ShowWarningMessage(response.message);
                }
            });
        });
    }

    function addNewTerm() {
        var newTermObj = $(selectors.template).first().clone();

        newTermObj.find(':input').val('');

        newTermObj.removeClass('template');

        newTermObj.prependTo(selectors.divTerms);

        newTermObj.find('input.form-control').first().focus();

        MAINMODULE.Common.RenameInputs(objs.divTerms, selectors.termItem, propPrefix);

        checkNoItems();

        MAINMODULE.Common.BindPopOvers();

        COMMONEDIT.ResetValidator(objs.form);
    }

    function deleteTerm(btn) {
        var term = btn.closest(selectors.termItem);

        term.remove();

        MAINMODULE.Common.RenameInputs(objs.divTerms, selectors.termItem, propPrefix);

        checkNoItems();

        COMMONEDIT.ResetValidator(objs.form);
    }

    function submitForm(btn, callback) {
        var url = objs.form.attr('action');

        var data = objs.form.serialize();

        $.post(url, data).done(function (response) {
            if (response.success === true) {
                MAINMODULE.Common.PostSaveCallback(response, btn);

                if (callback) {
                    callback();
                }

                if (!isNew && window.Dropzone) {
                    if (termsUploadDropZone.getQueuedFiles().length > 0) {
                        console.log(termsUploadDropZone.getQueuedFiles());
                    }

                    termsUploadDropZone.on("sending", function (file, xhr, formData) {
                        formData.append("projectId", response.value);
                    });

                    termsUploadDropZone.processQueue();

                    termsUploadDropZone.on("success", function (file) {
                        var parsedResponse = JSON.parse(file.xhr.response);
                        console.log(parsedResponse);
                    });

                    termsUploadDropZone.on("queuecomplete", function (file) {
                        console.log(file);
                    });
                }

                MAINMODULE.Ajax.HandleUrlResponse(response);
            }
            else {
                MAINMODULE.Ajax.HandleErrorResponse(response);
            }
        });
    }

    function uploadTerms(btn, callback) {
        if (window.Dropzone) {
            var id = btn.data('projectId');

            var columns = $(selectors.ddlColumn);

            termsUploadDropZone.on("sending", function (file, xhr, formData) {
                formData.append("projectId", id);

                var i = 0;
                columns.each(function () {
                    var ddl = $(this);
                    var column = ddl.data('column');
                    var language = ddl.val();
                    if (column !== undefined && language !== undefined && language.length > 0) {
                        formData.append('columns[' + i + '].Column', column);
                        formData.append('columns[' + i + '].Language', language);
                        i++;
                    }
                });
            });

            termsUploadDropZone.processQueue();

            termsUploadDropZone.on("success", function (file, response) {
                console.log(file);
                if (callback) {
                    callback(response);
                }
            });

            termsUploadDropZone.on("queuecomplete", function (file) {
                console.log(file);
            });
        }
    }

    function saveTerms(btn, callback) {
        var url = objs.urls.data('urlTermsSave');

        var id = objs.id.val();

        url = url + id;

        var terms = $(selectors.termItem);
        var data = terms.find(':input, :hidden').serializeObject();

        $.post(url, data).done(function (response) {
            if (response.success === true) {
                MAINMODULE.Common.PostSaveCallback(response, btn);

                MAINMODULE.Common.HandleSuccessDefault(response, callback, function () {
                    MAINMODULE.Common.RemoveErrorFromButton(btn);

                    loadTerms();
                });
            }
            else {
                MAINMODULE.Ajax.HandleErrorResponse(response);
            }
        });
    }

    function instantiateDropZone() {
        if (window.Dropzone) {
            var url = objs.divUploadTerms.data('url');

            if (termsUploadDropZone) {
                termsUploadDropZone.destroy();
                termsUploadDropZone = null;
            }

            termsUploadDropZone = new Dropzone(selectors.divUploadTerms, {
                url: url,
                paramName: 'termsFile',
                addRemoveLinks: true,
                autoProcessQueue: false,
                maxFiles: 1
                //resizeWidth
            });
        }
    }

    function setStickyElementsEdit() {
        MAINMODULE.Layout.SetStickyElement('#divManualTerms', 50, '#divManualTerms');
    }

    function checkNoItems() {
        var termCount = $(selectors.termItem + ':not(.template)').length;

        objs.termCounter.html(termCount);

        if (termCount === 0) {
            objs.divNoItems.show();
        }
        else {
            objs.divNoItems.hide();
        }
    }
    return {
        Init: init
    };
}());

$(function () {
    LOCALIZATIONEDIT.Init();
});

if (window.Dropzone !== undefined) {
    Dropzone.autoDiscover = false;
}