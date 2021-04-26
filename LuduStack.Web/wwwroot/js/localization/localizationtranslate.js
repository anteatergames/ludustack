﻿var LOCALIZATIONTRANSLATE = (function () {
    "use strict";

    var entrySaving = false;

    var selectors = {};
    var objs = {};

    var changedEntries = [];

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';

        selectors.form = '#frmTranslationSave';
        selectors.btnSave = '#btnSaveTranslation';

        selectors.ddlLanguage = '#Language';
        selectors.btnFilter = '.btn-filter';
        selectors.entry = '.translation-entry';
        selectors.entryInput = ':input.entry-input';
        selectors.entryActions = '.translation-entry-actions';
        selectors.entrySave = selectors.entryActions + ' .entry-save';
        selectors.entryCancel = selectors.entryActions + ' .entry-cancel';
        selectors.entryAuthors = '.entry-authors';
        selectors.entryAuthorTemplate = '.entry-author.template';
        selectors.entryAuthorButton = '.entry-author-btn';
        selectors.divUploadTerms = 'div#divUploadTerms';
        selectors.btnUploadTerms = '#btnUploadTerms';
        selectors.btnSaveTerms = '#btnSaveTerms';
        selectors.ddlColumn = '.ddlcolumn';
        selectors.id = '#Id';
        selectors.currentUserHelped = '#CurrentUserHelped';
        selectors.currentUserIsOwner = '#CurrentUserIsOwner';
        selectors.termItem = '.translation-term';
        selectors.changedCounter = '#changedtranslationscounter';
        selectors.btnSaveTranslationChanges = '#btnSaveTranslationChanges';
        selectors.divNoItems = '#divNoItems';
    }

    function cacheObjsTranslate() {
        objs.currentUserHelped = $(selectors.currentUserHelped);
        objs.currentUserIsOwner = $(selectors.currentUserIsOwner);
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.container = $(selectors.container);

        objs.ddlLanguage = $(selectors.ddlLanguage);
        objs.entryAuthors = $(selectors.entryAuthors);
        objs.changedCounter = $(selectors.changedCounter);
    }

    function init() {
        setSelectors();
        cacheObjsTranslate();

        bindTranslate();

        setStickyElements();

        loadSelectedLanguage(objs.ddlLanguage);
    }

    function bindTranslate() {
        bindFilter();
        bindLanguageChange();
        bindEntrySave();
        bindAuthorChange();
        bindEntryInputBlur();
        bindSaveTranslationChanges();
    }

    function bindLanguageChange() {
        objs.container.on('change', selectors.ddlLanguage, function () {
            var ddl = $(this);

            objs.container.find(selectors.entryInput).val('');
            $(selectors.entryInput).data('originalval', '');

            resetChangeCounter();

            loadSelectedLanguage(ddl);
        });
    }

    function bindFilter() {
        objs.container.on('click', selectors.btnFilter, function () {
            var btn = $(this);
            var filter = btn.data('filter');

            if (filter !== 'Untranslated') {
                loadSelectedLanguage(objs.ddlLanguage);
            }

            var inputs = objs.container.find(selectors.entryInput);

            var showTranslated = filter === 'All' || filter === 'Translated';
            var showUntranslated = filter === 'All' || filter === 'Untranslated';

            inputs.each(function () {
                var input = $(this);
                var translated = input.data('translated');

                if ((translated && showTranslated) || (!translated && showUntranslated)) {
                    input.closest(selectors.entry).show();
                }
                else {
                    input.closest(selectors.entry).hide();
                }
            });
        });
    }

    function bindEntrySave() {
        objs.container.on('click', selectors.entrySave, function (e) {
            e.preventDefault();
            var btn = $(this);
            var input = btn.closest(selectors.entry).find(selectors.entryInput);
            var termId = input.data('termid');
            var url = objs.urls.data('urlEntrySave');
            var language = objs.ddlLanguage.val();
            var currentUserHelped = objs.currentUserHelped.val();
            var currentUserIsOwner = objs.currentUserIsOwner.val();

            if (entrySaving !== true && language && input.val().length > 0) {
                entrySaving = true;

                var data = {
                    currentUserHelped: currentUserHelped,
                    currentUserIsOwner: currentUserIsOwner,
                    termId: termId,
                    language: language,
                    value: input.val()
                };

                $.post(url, data).done(function (response) {
                    entrySaveCallback(response, termId);
                });
            }
        });
    }

    function entrySaveCallback(response, termId) {
        if (response.success === true) {
            objs.currentUserHelped.val('True');

            deleteEntryAuthorButton(response.value.termId, response.value.userId);

            loadSingleTranslation(response.value);

            decreaseChangeCounter();

            removeFromArray(changedEntries, termId);
        }
        else {
            MAINMODULE.Ajax.HandleErrorResponse(response);
        }

        if (response.message) {
            var scoreEarned = MAINMODULE.Common.HandlePointsEarned(response);
            if (!scoreEarned) {
                ALERTSYSTEM.Toastr.ShowWarning(response.message);
            }
        }

        entrySaving = false;
    }

    function bindSaveTranslationChanges() {
        objs.container.on('click', selectors.btnSaveTranslationChanges, function () {
            var url = objs.urls.data('urlEntriesSave');
            var language = objs.ddlLanguage.val();

            if (language) {
                var data = [];

                for (let entry of changedEntries) {
                    var input = $(selectors.entryInput + '[data-termid=' + entry + ']');

                    var item = {
                        termId: entry,
                        value: input.val()
                    };

                    data.push(item);
                }

                $.post(url, { language: language, entries: data }).done(function (response) {
                    saveTranslationChangesCallback(response);
                });
            }
        });
    }

    function saveTranslationChangesCallback(response) {
        if (response.success === true) {
            for (let entry of changedEntries) {
                var input2 = $(selectors.entryInput + '[data-termid=' + entry + ']');

                input2.data('changed', false);
                input2.data('originalval', input2.val());
            }

            changedEntries = [];

            resetChangeCounter();

            if (response.message) {
                ALERTSYSTEM.Toastr.ShowWarning(response.message);
            }
        }
        else {
            MAINMODULE.Ajax.HandleErrorResponse(response);
        }
    }

    function bindEntryInputBlur() {
        objs.container.on('blur', selectors.entryInput, function () {
            var input = $(this);
            var language = objs.ddlLanguage.val();
            var originalVal = input.data('originalval');
            var termId = input.data('termid');
            var currentVal = input.val();
            var different = originalVal !== currentVal;

            if (language) {
                if (different) {
                    if (input.data('changed') !== true) {
                        changedEntries.push(termId);
                        input.data('changed', true);
                        increaseChangeCounter();
                    }
                }
                else {
                    if (input.data('changed') === true) {
                        removeFromArray(changedEntries, termId);
                        input.data('changed', false);
                        decreaseChangeCounter();
                    }
                }
            }
        });
    }

    function bindAuthorChange() {
        objs.container.on('click', selectors.entryAuthorButton, function (e) {
            e.preventDefault();

            var btn = $(this);
            var input = btn.closest(selectors.entry).find(selectors.entryInput);
            var v = btn.data('value');
            input.val(v);

            return false;
        });
    }

    function addNewAuthor(container, translation) {
        var newAuthorObj = $(selectors.entryAuthorTemplate).first().clone();
        var btn = newAuthorObj.find('a');

        var urlAvatar = objs.urls.data('urlAvatar').replace(/xpto/g, translation.userId);
        btn.find('img').attr('data-src', urlAvatar);

        btn.attr('data-userid', translation.userId);
        btn.attr('title', translation.authorName);
        btn.attr('data-value', translation.value);

        newAuthorObj.removeClass('template');

        newAuthorObj.appendTo(container);
    }

    function deleteEntryAuthorButton(termId, userId) {
        var entryInput = objs.container.find(selectors.entryInput + '[data-termid=' + termId + ']');

        var entry = entryInput.closest(selectors.entry);

        var authorBtn = entry.find(selectors.entryAuthorButton + '[data-userId=' + userId + ']');

        authorBtn.parent().remove();
    }

    function loadSelectedLanguage(ddl) {
        var url = objs.urls.data('urlEntriesGet');
        var language = ddl.val();

        var data = {
            language: language
        };

        $.post(url, data).done(function (response) {
            if (response.success === true) {
                resetTranslationStatus();

                for (let translation of response.value) {
                    loadSingleTranslation(translation);
                }
            }
            else {
                MAINMODULE.Ajax.HandleErrorResponse(response);
            }
        });
    }

    function resetTranslationStatus() {
        var entryInputs = objs.container.find(selectors.entryInput);

        entryInputs.each(function () {
            $(this).data('translated', false);
        });

        $(selectors.entryAuthors).html('');
    }

    function loadSingleTranslation(translation) {
        var entryInput = objs.container.find(selectors.entryInput + '[data-termid=' + translation.termId + ']');

        entryInput.val(translation.value);
        entryInput.data('originalval', translation.value);

        entryInput.data('translated', true);

        addNewAuthor(entryInput.closest(selectors.entry).find(selectors.entryAuthors), translation);
    }

    function increaseChangeCounter() {
        var currentCount = objs.changedCounter.data('count');

        currentCount++;

        objs.changedCounter.data('count', currentCount);

        objs.changedCounter.html(currentCount);

        return currentCount;
    }

    function decreaseChangeCounter() {
        var currentCount = objs.changedCounter.data('count');

        currentCount--;

        if (currentCount < 0) {
            currentCount = 0;
        }

        objs.changedCounter.data('count', currentCount);

        objs.changedCounter.html(currentCount);

        return currentCount;
    }

    function resetChangeCounter() {
        objs.changedCounter.data('count', 0);
        objs.changedCounter.html(0);
    }

    function removeFromArray(array, element) {
        const index = array.indexOf(element);
        array.splice(index, 1);
    }

    function setStickyElements() {
        MAINMODULE.Layout.SetStickyElement('#divTranslationSelector', 50, '#divTranslationSelector');
    }

    return {
        Init: init
    };
}());

$(function () {
    LOCALIZATIONTRANSLATE.Init();
});