var BILLRATECOMMON = (function () {
    "use strict";

    var selectors = {};
    var objs = {};
    var canInteract = false;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.btnDelete = '.btn-billrate-delete';
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
        bindDeletebillrate();
    }

    function bindDeletebillrate() {
        objs.container.on('click', selectors.btnDelete, function (e) {
            e.preventDefault();

            var btn = $(this);

            if (canInteract && !btn.hasClass('disabled')) {
                MAINMODULE.Common.DeleteEntity(btn, BILLRATECOMMON.Callback.DeleteEntity);
            }

            return false;
        });
    }

    return {
        Init: init,
        Callback: {
            DeleteEntity: function () {
                console.log('empty BILLRATECOMMON.Callback.Delete');
            }
        }
    };
}());

$(function () {
    BILLRATECOMMON.Init();
});