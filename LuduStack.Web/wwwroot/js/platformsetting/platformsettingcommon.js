var PLATFORMSETTINGCOMMON = (function () {
    "use strict";

    var selectors = {};
    var objs = {};
    var canInteract = false;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.btnReset = '.btn-entity-reset';
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
        bindReset();
    }

    function bindReset() {
        objs.container.on('click', selectors.btnReset, function (e) {
            e.preventDefault();

            var btn = $(this);

            if (canInteract) {
                MAINMODULE.Common.DeleteEntity(btn, PLATFORMSETTINGCOMMON.Callback.ResetEntity);
            }

            return false;
        });
    }

    return {
        Init: init,
        CanInteract: canInteract,
        Callback: {
            ResetEntity: function () {
                console.log('empty PLATFORMSETTINGCOMMON.Callback.ResetEntity');
            }
        }
    };
}());

$(function () {
    PLATFORMSETTINGCOMMON.Init();
});