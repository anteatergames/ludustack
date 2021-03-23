﻿var MENTORDASHBOARD = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.urls = '#urls';
        selectors.studyProfile = '#studyProfile';
        selectors.container = '#featurecontainer';
        selectors.listCourses = '#divListCourses';
        selectors.listMentors = '#divListMentors';
        selectors.listStudents = '#divListStudents';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.studyProfile = $(selectors.studyProfile);
        objs.listCourses = $(selectors.listCourses);
        objs.listMentors = $(selectors.listMentors);
        objs.listStudents = $(selectors.listStudents);
    }

    function init() {
        setSelectors();
        cacheObjs();

        console.log(objs.studyProfile.val());

        var urlCourses = objs.urls.data('urlListmycourses');

        if (objs.studyProfile.val() === 'Mentor') {
            urlCourses = objs.urls.data('urlListcoursesbyme');

            var urlStudents = objs.urls.data('urlListmystudents');
            loadMyStudents(urlStudents);
        }
        else {
            var urlMentors = objs.urls.data('urlListmymentors');
            loadMyMentors(urlMentors);
        }

        loadMyCourses(urlCourses);
    }

    function loadMyCourses(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.listCourses);
    }

    function loadMyMentors(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.listMentors);
    }

    function loadMyStudents(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.listStudents);
    }

    return {
        Init: init
    };
}());

$(function () {
    MENTORDASHBOARD.Init();
});