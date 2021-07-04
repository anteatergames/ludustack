var GIVEAWAYCOMMON = (function () {
    "use strict";

    var selectors = {};
    var objs = {};
    var canInteract = false;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.btnDelete = '.btn-giveaway-delete';
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
        bindDeleteGiveaway();
    }

    function bindDeleteGiveaway() {
        objs.container.on('click', selectors.btnDelete, function (e) {
            e.preventDefault();

            var btn = $(this);

            if (canInteract) {
                MAINMODULE.Common.DeleteEntity(btn, GIVEAWAYCOMMON.Callback.DeleteEntity);
            }

            return false;
        });
    }

    return {
        Init: init,
        CanInteract: canInteract,
        Callback: {
            DeleteEntity: function () {
                console.log('empty GIVEAWAYCOMMON.Callback.Delete');
            }
        }
    };
}());

$(function () {
    GIVEAWAYCOMMON.Init();
});