var pauseOp = false;
var dialogTimer, dialogTimer_sec;
var autoLogoutInSec = autoLogoutInMins * 60;

var autoLogoutModal;

function showautologoutDialog() {
    pauseOp = true;
    clearInterval(dialogTimer);
    dialogTimer_sec = 30;

    modalEmpty('Auto Logout', function (modal, recallback) {
        autoLogoutModal = modal;

        var text1 = $('<span />').text('Page has been idle for ' + autoLogoutInMins + ' minutes. Session will be logged out in ');
        var sec = $('<span />').addClass('tmp-sec');
        var text2 = $('<span />').text(' seconds');

        var confirmLogout = modal.find('.modal-footer .btn-ok');
        var stayLoggedIn = modal.find('.modal-footer .btn-cancel');

        confirmLogout.html('Confirm Logout');
        stayLoggedIn.html('Stay Logged In');

        modal.find('.modal-body')
            .append(text1)
            .append(sec)
            .append(text2);

        dialogTimer = setInterval(function () {
            dialogTimer_sec -= 1;
            if (dialogTimer_sec <= 0) {
                clearInterval(dialogTimer);
                confirmLogout.click();
            } else {
                sec.text(dialogTimer_sec);
            }

        }, 1000);

        if (recallback) recallback();

    }, function (ret, modal) {
        if (ret) {
            $('#logoutForm').submit();
        } else {
            pauseOp = false;
            initautoLogoutTimer();
        }
    });
}

function autoLogoutExpired() {
    return parseInt(localStorage.autoLogout_tmpTimer) >= autoLogoutInSec;
}

function resetautoLogoutTimer() {
    if (pauseOp) {
        if (!autoLogoutExpired()) {
            clearInterval(dialogTimer);
            autoLogoutModal.addClass('confirm-close-event').modal('hide');
            initautoLogoutTimer(true);
        }
    } else {
        clearInterval(dialogTimer);
        localStorage.autoLogout_tmpTimer = 0;
    }
}

function initautoLogoutTimer(skipInitTimer) {
    if (!skipInitTimer) {
        localStorage.autoLogout_tmpTimer = 0;
    }
    
    if (autoLogoutInSec > 0) {
        var a = setInterval(function () {
            localStorage.autoLogout_tmpTimer++;

            if (autoLogoutExpired()) {
                clearInterval(a);
                showautologoutDialog();
            }
        }, 1000);
    }
}

$(document).ready(function () {
    initautoLogoutTimer();
});