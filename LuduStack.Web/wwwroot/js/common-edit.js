var COMMONEDIT = (function () {
    "use strict";

    function init() {
        console.log('COMMONEDIT.init');
    }

    function resetValidator(formObject) {
        formObject.removeData("validator").removeData("unobtrusiveValidation");

        $.validator.unobtrusive.parse(formObject);
    }

    function isNew() {
        return window.location.href.indexOf('/add') > -1 || window.location.href.indexOf('/new') > -1;
    }

    return {
        Init: init,
        ResetValidator: resetValidator,
        IsNew: isNew
    };
}());

COMMONEDIT.Init();