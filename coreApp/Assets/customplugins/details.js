
(function ($) {
    var _options = {
        modalWidth: 'modal-md',
        preLoadCallback: null,
        loadCallback: null,
        preSubmitCallback: null,
        postSubmitCallback: null
    };

    var activate = function (a, title, modalOptions, viewType) {
        var modal = getModalUI();
        modal.addClass('modal-view-details');
        modal.find('.modal-footer .btn-ok')
            .removeClass('btn-ok')
            .addClass('btn-submit')
            .html('Save');

        modal.find('.modal-title').html(title);


        modal.removeClass('show-view show-edit show-create show-custom');
        
        modal.find('.modal-dialog').addClass(modalOptions.modalWidth);

        var loading = function (show) {
            var content = modal.find('.modal-content');
            loadingUI(content, show);
        };

        var e = {
            modal: modal,
            modalOptions: modalOptions,
            viewType: viewType
        };

        if (modalOptions.preLoadCallback) {
            e = $.extend({}, e, window[modalOptions.preLoadCallback](e));
        }

        loading(true);

        var view = modal.find('.modal-body');
        var redirectUrl = a.attr('data-redirect-url');
        var url = a.attr('data-url');

        view.load(vdUrl(url), function () {

            setModalHeights(modal);

            initPlugins(view);

            var form = view.find('form');
            var textarea = form.find('textarea');
            var submitBtn = modal.find('.btn-submit');

            submitBtn.unbind('click').bind('click', function () {
                var proceed = true;
                if (e.modalOptions.preSubmitCallback) {
                    proceed = window[e.modalOptions.preSubmitCallback]('update', e);
                }

                if (proceed) {
                    var proc = modalProcessing();
                    var form = e.modal.find('.modal-body form');

                    form.ajaxForm(function (res) {
                        if (e.modalOptions.postSubmitCallback) {
                            loading(true);
                            window[e.modalOptions.postSubmitCallback](res, e);
                            proc.addClass('confirm-close-event').modal('hide');
                        } else {
                            if (res.IsSuccessful) {
                                sessionStorage.clientMessage = res.Remarks;
                                if (redirectUrl == '{close-modal}') {
                                    loading(false);
                                    modal.addClass('confirm-close-event').modal('hide');
                                    proc.addClass('confirm-close-event').modal('hide');
                                } else if (redirectUrl == '{page-reload}') {
                                    window.location.reload(true);
                                } else if (redirectUrl && redirectUrl != '') {
                                    window.location = vdUrl(redirectUrl.replace('{0}', res.Data));
                                } else {
                                    window.location.reload(true);
                                }
                            } else {
                                proc.addClass('confirm-close-event').modal('hide');
                                modalMessage(res.Err.split('\n'), 'Update Record', true);
                            }
                        }
                    });

                    form.submit();
                }
            });

            textarea.unbind('keypress').bind('keypress', function (e) {
                if (e.keyCode == 13) {
                    e.stopPropagation();
                }
            });

            textarea.unbind('keyup').keyup(function (e) {
                if (e.which == 13) {
                    e.stopPropagation();
                }
            });

            form.unbind('keypress').bind('keypress', function (e) {
                if (e.keyCode == 13) {
                    return false;
                }
            });

            form.unbind('keyup').keyup(function (ee) {
                if (ee.which == 13) {
                    submitBtn.click();
                }
            });


            if (e.modalOptions.loadCallback) {
                window[e.modalOptions.loadCallback](e);
            }

            showModalFooter(modal);

            loading(false);
        });

        modal
            .unbind('hide.bs.modal')
            .unbind('shown.bs.modal')
            .on('shown.bs.modal', function () {
                
            })
            .on('hide.bs.modal', function (e) {
                return closeModal(modal, e);
            })
            .modal({
                backdrop: 'static',
                show: true
            });
    };

    var methods = {
        init: function (options) {

            return this.each(function () {
                var opts = $.extend({}, _options, options);

                var obj = $(this);
                
                var modalTitle = obj.attr('modal-title');

                var openBtn = obj.find('.btn-open:not("[table-selector]")');
                var createBtn = obj.find('.btn-create:not("[table-selector]")');
                var editBtn = obj.find('.btn-edit:not("[table-selector]")');
                var deleteBtn = obj.find('.btn-delete:not("[table-selector]")');

                if (obj.attr('modal-width')) {
                    opts.modalWidth = obj.attr('modal-width');
                }

                if (obj.attr('modal-preload-callback')) {
                    opts.preLoadCallback = obj.attr('modal-preload-callback');
                }

                if (obj.attr('modal-load-callback')) {
                    opts.loadCallback = obj.attr('modal-load-callback');
                }

                if (obj.attr('modal-presubmit-callback')) {
                    opts.preSubmitCallback = obj.attr('modal-presubmit-callback');
                }
                if (obj.attr('modal-postsubmit-callback')) {
                    opts.postSubmitCallback = obj.attr('modal-postsubmit-callback');
                }

                openBtn.unbind('click').bind('click', function (e) {                
                    e.preventDefault();
                    var a = $(this);
                    if (a.attr('modal-title')) {
                        modalTitle = a.attr('modal-title');
                    }
                    activate(a, modalTitle, opts, 'view');
                });

                createBtn.unbind('click').bind('click', function (e) {
                    e.preventDefault();
                    var a = $(this);
                    if (a.attr('modal-title')) {
                        modalTitle = a.attr('modal-title');
                    }

                    if (modalTitle == undefined || modalTitle == false || modalTitle == '') {
                        modalTitle = 'New Record';
                    }

                    activate(a, modalTitle, opts, 'create');
                });

                editBtn.unbind('click').bind('click', function (e) {
                    e.preventDefault();
                    var a = $(this);
                    if (a.attr('modal-title')) {
                        modalTitle = a.attr('modal-title');
                    }

                    if (modalTitle == undefined || modalTitle == false || modalTitle == '') {
                        modalTitle = 'Edit Record';
                    }

                    activate(a, modalTitle, opts, 'edit');
                });

                deleteBtn.unbind('click').bind('click', function (e) {
                    e.preventDefault();
                    var a = $(this);
                    var url = a.attr('data-url');
                    var redirectUrl = a.attr('data-redirect-url');
                    var recordId = a.attr('data-record-id');

                    modalConfirm('Are you sure you want to delete this record?', function (ret) {
                        if (ret) {
                            $.post(vdUrl(url), { id: recordId }, function (res) {
                                if (res.IsSuccessful) {
                                    sessionStorage.clientMessage = res.Remarks;
                                    if (redirectUrl && redirectUrl != '') {
                                        window.location = vdUrl(redirectUrl);
                                    } else {
                                        window.location.reload(true);
                                    }

                                } else {
                                    if (a.attr('modal-title')) {
                                        modalTitle = a.attr('modal-title');
                                    }

                                    if (modalTitle == undefined || modalTitle == false || modalTitle == '') {
                                        modalTitle = 'Delete Record';
                                    }

                                    modalMessage(res.Err.split('\n'), modalTitle, true);
                                }
                            }, 'json');
                        }
                    });
                });
            });
        },
        modalCustom: function (btn, title, _opts) {
            var opts = $.extend(true, {}, _options, _opts);

            activate(btn, title, opts);
        }
    };

    $.fn.details = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.details');
        }

    };

})(jQuery);
