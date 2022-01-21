var ALERTSYSTEM = (function () {
    "use strict";

    function showInfoAlert(text, callback) {
        showAlert(text, 'info', callback);
    }

    function showSuccessAlert(text, callback) {
        showAlert(text, 'success', callback);
    }

    function showWarningAlert(text, callback) {
        showAlert(text, 'warning', callback);
    }

    function showPointsEarned(text, callback) {
        showAlert(text, 'info', callback);
    }

    function showAlert(text, type, callback) {
        if (text) {
            var message = generateAlert(text, type);

            Swal.fire(message).then(
                function (result) {
                    callbackResult(callback, result);
                }
            );
        }
        else {
            console.log('no text to show');
        }
    }

    function generateAlert(text, type) {
        return {
            toast: true,
            position: 'top-end',
            type: type,
            showConfirmButton: false,
            title: text,
            timer: 3000
        };
    }

    function showInfoMessage(msg, callback) {
        Swal.fire({
            text: msg,
            type: "info"
        }).then(
            function (result) {
                callbackResult(callback, result);
            }
        );
    }

    function showSuccessMessage(msg, callback) {
        Swal.fire({
            title: "Good job!",
            text: msg,
            type: "success"
        }).then(
            function (result) {
                callbackResult(callback, result);
            }
        );
    }

    function showWarningMessage(msg, callback) {
        Swal.fire({
            toast: true,
            position: 'top-end',
            title: msg,
            type: "warning"
        }).then(
            function (result) {
                callbackResult(callback, result);
            }
        );
    }

    function callbackResult(callback, result) {
        if (callback) {
            callback(result);
        }
    }

    function showConfirmMessage(title, msg, confirmButtonText, cancelButtonText, callbackYes, callbackCancel) {
        Swal.fire({
            title: title,
            text: msg,
            type: "question",
            showCancelButton: true,
            confirmButtonColor: '#d33',
            confirmButtonText: confirmButtonText,
            cancelButtonText: cancelButtonText
        }).then(
            function (result) {
                if (result.value) {
                    console.log('yes');
                    if (callbackYes) {
                        callbackYes();
                    }
                }
                else {
                    console.log('no');
                    if (callbackCancel) {
                        callbackCancel();
                    }
                }
            }
        );
    }

    return {
        ShowInfoMessage: showInfoMessage,
        ShowSuccessMessage: showSuccessMessage,
        ShowWarningMessage: showWarningMessage,
        ShowConfirmMessage: showConfirmMessage,
        Toastr: {
            ShowInfo: showInfoAlert,
            ShowSuccess: showSuccessAlert,
            ShowWarning: showWarningAlert,
            PointsEarned: showPointsEarned
        }
    };
}());