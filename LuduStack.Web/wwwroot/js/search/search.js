﻿var SEARCH = (function () {
    "use strict";

    var selectors = {};

    function init() {
        cacheSelectors();

        searchPosts();
    }

    function cacheSelectors() {
        selectors.term = $('.searchTerm');
        selectors.divSearchPosts = $('#divSearchPosts');
    }

    function searchPosts() {
        MAINMODULE.Ajax.LoadHtml("/search/posts?q=" + encodeURIComponent(selectors.term.val()), selectors.divSearchPosts).then(() => {
            MAINMODULE.Common.BindPopOvers();
        });
    }

    return {
        Init: init
    };
}());

$(function () {
    SEARCH.Init();
});