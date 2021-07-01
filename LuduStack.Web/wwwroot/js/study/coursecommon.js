var COURSECOMMON = (function () {
    "use strict";

    var selectors = {};
    var objs = {};
    var canInteract = false;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.btnDelete = '.btn-course-delete';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        canInteract = MAINMODULE.CanInteract();
    }

    function bindAll() {
        bindDeleteCourse();
    }

    function bindDeleteCourse() {
        objs.container.on('click', selectors.btnDelete, function (e) {
            e.preventDefault();

            var btn = $(this);

            if (canInteract) {
                MAINMODULE.Common.DeleteEntity(btn, COURSECOMMON.Callback.DeleteEntity);
            }

            return false;
        });
    }

    return {
        Init: init,
        Callback: {
            DeleteEntity: function () {
                console.log('empty COURSECOMMON.Callback.Delete');
            }
        }
    };
}());

$(function () {
    COURSECOMMON.Init();
});