﻿var GAMEJAMCOMMON = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

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

        bindAll();
    }

    function bindAll() {
        bindDelete();
    }

    function bindDelete() {
        objs.container.on('click', selectors.btnDelete, function (e) {
            e.preventDefault();

            var btn = $(this);

            if (canInteract) {
                MAINMODULE.Common.DeleteEntity(btn, GAMEJAMCOMMON.Callback.DeleteEntity);
            }

            return false;
        });
    }

    return {
        Init: init,
        Callback: {
            DeleteEntity: function () {
                console.log('empty GAMEJAMCOMMON.Callback.DeleteEntity');
            }
        }
    };
}());

$(function () {
    GAMEJAMCOMMON.Init();
});