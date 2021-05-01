﻿var STUDYPROFILESELECTOR = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;

    function setSelectors() {
        selectors.container = '.content-wrapper';
        selectors.canInteract = '#caninteract';
        selectors.btnSetStudyProfile = '.btnsetstudyprofile';
    }

    function cacheObjects() {
        objs.canInteract = $(selectors.canInteract);
        objs.container = $(selectors.container);
    }

    function init() {
        setSelectors();
        cacheObjects();

        canInteract = objs.canInteract.val() === 'true';

        bindAll();
    }

    function bindAll() {
        bindBtnSetStudyProfile();
    }

    function bindBtnSetStudyProfile() {
        objs.container.on('click', selectors.btnSetStudyProfile, function (e) {
            e.preventDefault();

            var btn = $(this);
            var url = btn.prop('href');

            if (canInteract) {
                MAINMODULE.Ajax.Post(url);
            }

            return false;
        });
    }

    return {
        Init: init
    };
}());

$(function () {
    STUDYPROFILESELECTOR.Init();
});