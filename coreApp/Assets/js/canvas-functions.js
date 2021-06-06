$.fn.clearValidation = function () { var v = $(this).validate(); $('[name]', this).each(function () { v.successList.push(this); v.showErrors(); }); v.resetForm(); v.reset(); };

function getMessageElement(msg) {
    if (Array.isArray(msg)) {
        if (msg.length > 1) {
            var m = $('<ul />');
            msg.forEach(function (v) {
                v = v.trim();
                if (v != '') {
                    m.append($('<li />').append(v));
                }
            });

            return m;
        } else {
            return msg[0];
        }        
    } else {
        return msg;
    }
}

function loadingUI (cont, show) {
    var ui = getTemplateUIById('LoadingTemplate');

    if (show) {
        cont.prepend(ui);
    } else {
        cont.find('.css3-spinner').first().remove();
    }
};

function loadingUI_Mask(cont, show) {
    if (show) {
        cont.addClass('loading-mask');
    } else {
        cont.removeClass('loading-mask');
    }
};

function cookiesEnabled() {
    if (navigator.cookieEnabled) return true;

    // set and read cookie
    document.cookie = "cookietest=1";
    var ret = document.cookie.indexOf("cookietest=") != -1;

    // delete cookie
    document.cookie = "cookietest=1; expires=Thu, 01-Jan-1970 00:00:01 GMT";

    return ret;
}

//Change Password
function initChangePW_TopLink() {
    var cpw_cont = $('.top-link-section-pw');
    var cpw_form = cpw_cont.find('form');
    var cpw_trigger = $('.top-link-section-pw-trigger');
    var btn_cancel_cpw = cpw_cont.find('.btn-cancel-change-pw');
    var cpw_form_inputs = cpw_cont.find('input[type="password"]');

    var openUI = function () {
        resetUI();
        cpw_trigger.addClass('top-link-section-pw-open');

        cpw_form.find('#NewPassword').password_strength_clear();
    };

    var closeUI = function () {
        cpw_trigger.removeClass('top-link-section-pw-open');
    };

    var resetUI = function () {
        cpw_form_inputs.val('');
        cpw_form.clearValidation();
    };

    cpw_cont.changePasswordUI({
        postSubmitCallback: function (res) {
            if (res.IsSuccessful) {
                showClientMessage(res.Remarks);
                closeUI();
            } else {
                modalMessage(res.Err.split('\n'), 'Change Password', true);
            }
        }
    });

    $(document).click(function () {
        closeUI();
    });

    cpw_cont.click(function (e) {
        e.stopPropagation();
    });

    cpw_trigger.click(function (e) {
        e.preventDefault();
        e.stopPropagation();

        openUI();
    });

    btn_cancel_cpw.click(function (e) {
        e.preventDefault();
        e.stopPropagation();

        closeUI();
    });
}

//(function () {
//    var cpw_cont = $('.top-link-section-pw');
//    var cpw_trigger = $('.top-link-section-pw-trigger');
//    var cpw_form = cpw_cont.find('form');
//    var btn_cancel_cpw = cpw_cont.find('.btn-cancel-change-pw');
//    var btn_cpw = cpw_cont.find('.btn-change-pw');

//    var cpw_form_inputs = cpw_cont.find('input[type="password"]');
//    var myPlugin;

//    $(document).ready(function () {
//        myPlugin = cpw_form.find('#NewPassword').password_strength({
//            xmlUrl: vdUrl('/xml/PasswordPolicy.xml')
//        });

//        cpw_form.find('#passwordPolicy').click(function (e) {
//            modalEmpty('Password Requirements', function (modal, recallback) {
//                var iframe = $('<iframe />');
//                modal.find('.modal-body').append(iframe);

//                modal.find('.modal-footer .btn-ok').html('Close');
//                modal.find('.modal-footer .btn-cancel').hide();

//                iframe
//                    .css({ width: '100%', height: '180px' })
//                    .attr('src', vdUrl('/xml/PasswordPolicy.xml'));

//                if (recallback) recallback();
//            });            

//            e.preventDefault();
//            return false;
//        });
//    });

//    $(document).click(function () {
//        closeUI();
//    });

//    cpw_cont.click(function (e) {
//        e.stopPropagation();
//    });

//    cpw_form_inputs.keyup(function (e) {
//        if (e.which == 13) {
//            submitForm();
//        }
//    });
    
//    cpw_trigger.click(function (e) {
//        e.preventDefault();
//        e.stopPropagation();
        
//        openUI();
//    });

//    btn_cancel_cpw.click(function (e) {
//        e.preventDefault();
//        e.stopPropagation();

//        closeUI();
//    });

//    btn_cpw.click(function (e) {
//        e.preventDefault();
//        e.stopPropagation();

//        submitForm();
//    });

//    var openUI = function () {
//        resetUI();
//        cpw_trigger.addClass('top-link-section-pw-open');

//        cpw_form.find('#NewPassword').password_strength_clear();
//    };

//    var closeUI = function () {
//        cpw_trigger.removeClass('top-link-section-pw-open');
//    };

//    var submitForm = function () {

//        cpw_form.validate();
//        if (cpw_form.valid()) {

//            if (!myPlugin.metReq()) {
//                modalMessage('New password did not meet requirements for a strong password', 'Change Password', true);
//                return;
//            }

//            var proc = modalProcessing();

//            cpw_form.ajaxForm(function (res) {
//                proc.addClass('confirm-close-event').modal('hide');

//                if (res.IsSuccessful) {
//                    showClientMessage(res.Remarks);
//                    closeUI();
//                } else {
//                    modalMessage(res.Err.split('\n'), 'Change Password', true);
//                }
//            });

//            cpw_form.submit();
//        }
//    };

//    var resetUI = function () {
//        cpw_form_inputs.val('');
//        cpw_form.clearValidation();
//    };

//}());

