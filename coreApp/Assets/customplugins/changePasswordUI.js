(function ($) {
    var _options = {
        initCallback: null,
        postSubmitCallback: null
    };

    var methods = {
        init: function (options) {
            var opts = $.extend(true, {}, _options, options);

            return $(this).each(function () {
                var obj = $(this);

                var cpw_form = obj.find('form');
                var btn_cpw = cpw_form.find('.btn-change-pw');
                var cpw_form_inputs = cpw_form.find('input[type="password"]');

                var myPlugin = cpw_form.find('#NewPassword').password_strength({
                    xmlUrl: vdUrl('/xml/PasswordPolicy.xml')
                });

                cpw_form.find('#passwordPolicy').click(function (e) {
                    modalEmpty('Password Validity Requirements', function (modal, recallback) {
                        var iframe = $('<iframe />');
                        modal.find('.modal-body').append(iframe);

                        modal.find('.modal-footer .btn-ok').html('Close');
                        modal.find('.modal-footer .btn-cancel').hide();

                        iframe
                            .css({ width: '100%', height: '180px' })
                            .attr('src', vdUrl('/xml/PasswordPolicy.xml'));

                        if (recallback) recallback();
                    });

                    e.preventDefault();
                    return false;
                });

                btn_cpw.click(function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    submitForm();
                });

                cpw_form_inputs.keyup(function (e) {
                    if (e.which == 13) {
                        submitForm();
                    }
                });

                var submitForm = function () {

                    cpw_form.validate();
                    if (cpw_form.valid()) {

                        if (!myPlugin.metReq()) {
                            modalMessage('New password did not meet requirements for a strong password', 'Change Password', true);
                            return;
                        }

                        var proc = modalProcessing();

                        cpw_form.ajaxForm(function (res) {
                            proc.addClass('confirm-close-event').modal('hide');

                            if (opts.postSubmitCallback) {
                                opts.postSubmitCallback(res);
                            }

                        });

                        cpw_form.submit();
                    }
                };

                if (opts.initCallback) {
                    opts.initCallback(obj);
                }
            });
        }
    };

    $.fn.changePasswordUI = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.changePasswordUI');
        }

    };

})(jQuery);