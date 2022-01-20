var GAMEIDEACOMMON = (function () {
    "use strict";

    var selectors = {};
    var objs = {};
    var canInteract = false;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.btnDelete = '.btn-entity-delete';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
    }

    function init() {
        setSelectors();
        cacheObjs();

        canInteract = MAINMODULE.CanInteract();

        bindAll();
    }

    function bindAll() {
        bindDelete();
    }

    function bindDelete() {
        objs.container.on('click', selectors.btnDelete, function (e) {
            e.preventDefault();

            var btn = $(this);

            if (canInteract && !btn.hasClass('disabled')) {
                MAINMODULE.Common.DeleteEntity(btn, GAMEIDEACOMMON.Callback.DeleteEntity);
            }

            return false;
        });
    }

    return {
        Init: init,
        CanInteract: canInteract,
        Callback: {
            DeleteEntity: function () {
                console.log('empty GAMEIDEACOMMON.Callback.DeleteEntity');
            }
        }
    };
}());

$(function () {
    GAMEIDEACOMMON.Init();
});