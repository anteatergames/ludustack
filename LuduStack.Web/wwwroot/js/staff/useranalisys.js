var USERANALISYS = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    function init() {
        setSelectors();
        cacheObjects();

        bindAll();
    }

    function setSelectors() {
        selectors.container = '#contentwrapper';
        selectors.btnDelete = '#btnDeleteUser';
    }

    function cacheObjects() {
        objs.container = $(selectors.container);
    }

    function bindAll() {
        bindDelete();
    }

    function bindDelete() {
        objs.container.on('click', selectors.btnDelete, function (e) {
            e.preventDefault();

            var btn = $(this);

            MAINMODULE.Common.DeleteEntity(btn, deleteCallback);

            return false;
        });
    }

    function deleteCallback(response) {
        if (response.success) {
            location.href = '/';
        }
        else {
            console.log(response);
        }
    }

    return {
        Init: init
    };
}());

$(function () {
    USERANALISYS.Init();
});