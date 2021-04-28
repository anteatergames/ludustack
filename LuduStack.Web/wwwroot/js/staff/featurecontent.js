var FEATURECONTENT = (function () {
    "use strict";

    var selectors = {};

    function init() {
        cacheSelectors();

        bindAll();

        loadList();
    }

    function cacheSelectors() {
        selectors.divListFeatured = $("#divListFeatured");
    }

    function bindAll() {
        bindTooltip();
        bindBtnFeatureOk();
        bindBtnFeatureNok();
    }

    function bindTooltip() {
        $('[data-toggle="tooltip"]').tooltip();
    }

    function bindBtnFeatureOk() {
        $('body').on('click', '.btn-feature-ok', function (e) {
            var btn = $(this);
            var id = btn.closest('tr').data('id');

            $.post('/staff/featuredcontent/add', { id: id }).done(featureActionCallback);
        });
    }

    function bindBtnFeatureNok() {
        $('body').on('click', '.btn-feature-nok', function (e) {
            var btn = $(this);
            var id = btn.closest('tr').data('featureid');

            $.post('/staff/featuredcontent/remove', { id: id }).done(featureActionCallback);
        });
    }

    function featureActionCallback(response) {
        if (response.success === true) {
            loadList();
        }
        else {
            MAINMODULE.Ajax.HandleErrorResponse(response);
        }
    }

    function loadList() {
        MAINMODULE.Ajax.LoadHtml("/staff/featuredcontent/list", selectors.divListFeatured);
    }

    return {
        Init: init
    };
}());

$(function () {
    FEATURECONTENT.Init();
});