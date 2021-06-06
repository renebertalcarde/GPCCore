(function ($) {
    var _options = {
        multiSelect: false,
        forceItemSelection: false,
        modalViewWidth: 'modal-md',
        modalEditWidth: 'modal-md',
        modalCreateWidth: 'modal-md',
        modalCustomWidth: 'modal-md',
        modalCustomTitle: '',
        modalCustomButton: '',
        modalNoCancel: false,
        preLoadCallback: null,
        loadCallback: null,
        preSubmitCallback: null,        
        postSubmitCallback: null,
        loadEvent: null,
        preLoadEvent: null,
        postSubmitEvent: null,
        listLoadedCallback: null,
        listItemSelectedCallback: null,
        url_data: null,
        nodes: null,
        noListCommands: false,
        listCommands: {
            enable: true
        }
    };



    var loading = function (cont, show) {
        var content = cont.find('.modal-content');
        loadingUI(content, show);
    };
    
    var doInitModal = function (modal) {
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
                show: true,
                keyboard: false
            });
    };

    var activateView = function (modal, viewType, recordId, modalNoView, modalViewUrl, modalEditUrl, modalDeleteUrl, modalCreateUrl, modalCustomUrl, modalOptions, tr, _initModal, _internalLoadCallback) {
        modal.find('.modal-dialog').removeClass('modal-md modal-lg modal-xl');
        modal.removeClass('show-view show-edit show-create');
        var url = '';
        var view = modal.find('.modal-body');

        modal.removeClass('show-view show-edit show-create show-custom');


        switch (viewType) {
            case 'view':
                modal.find('.modal-dialog').addClass(modalOptions.modalViewWidth);

                modal.addClass('show-view');
                modal.find('.modal-title').html('Record Details');
                url = modalViewUrl.replace('{0}', recordId);
                break;
            case 'edit':
                modal.find('.modal-dialog').addClass(modalOptions.modalEditWidth);

                modal.addClass('show-edit');
                modal.find('.modal-title').html('Edit Record');
                url = modalEditUrl.replace('{0}', recordId);
                break;
            case 'create':
                modal.find('.modal-dialog').addClass(modalOptions.modalCreateWidth);

                modal.addClass('show-create');
                modal.find('.modal-title').html('New Record');
                url = modalCreateUrl;
                break;
            case 'custom':
                modal.find('.modal-dialog').addClass(modalOptions.modalCustomWidth);

                modal.addClass('show-custom');
                modal.find('.modal-title').html(modalOptions.modalCustomTitle);
                modal.find('.btn-custom').html(modalOptions.modalCustomButton);
                url = modalCustomUrl;
                break;
        }

        modal.find('.modal-footer a[data-link], .modal-footer .btn-group > button[data-link]').each(function () {
            var a = $(this);
            var link = a.attr('data-link').replace('{0}', recordId)

            if (a.attr('data-link-custom') == undefined) {
                a.attr('href', vdUrl(link));
            } else {
                a.attr('data-link-tmp', link);
            }
        });

        modal.find('.modal-footer > *').removeAttr('style');

        if (modalOptions.modalNoCancel) {
            modal.find('.btn-cancel').remove();
        }

        var e = {
            modal: modal,
            view: view,
            viewType: viewType,
            recordId: recordId,
            modalNoView: modalNoView,
            modalViewUrl: modalViewUrl,
            modalEditUrl: modalEditUrl,
            modalDeleteUrl: modalDeleteUrl,
            modalCreateUrl: modalCreateUrl,
            modalCustomUrl: modalCustomUrl,
            modalOptions: modalOptions,
            url: url,
            tr: tr
        };

        if (e.modalOptions.preLoadCallback) {
            e = $.extend({}, e, window[e.modalOptions.preLoadCallback](e));
        } else if (e.modalOptions.preLoadEvent) {
            e = $.extend({}, e, e.modalOptions.preLoadEvent(e));
        }

        loading(e.modal, true);

        var loaded = 0;

        e.view.load(vdUrl(e.url), e.modalOptions.url_data, function (res) {

            if (loaded > 0) return;
            loaded++;

            setModalHeights(e.modal);

            initPlugins(e.view);

            e.modal.find('.btn-edit').unbind('click').bind('click', function () {
                activateView(e.modal, 'edit', e.recordId, e.modalNoView, e.modalViewUrl, e.modalEditUrl, e.modalDeleteUrl, e.modalCreateUrl, e.modalCustomUrl, e.modalOptions, e.tr);
            });
            e.modal.find('.btn-delete').unbind('click').bind('click', function () {                
                $.fn.list('modalOp', null, 'delete', e.modalDeleteUrl, $.extend({}, e.modalOptions, { url_data: { id: e.recordId } }));
            });

            e.modal.find('.btn-cancel').unbind('click').bind('click', function () {
                activateView(e.modal, 'view', e.recordId, e.modalNoView, e.modalViewUrl, e.modalEditUrl, e.modalDeleteUrl, e.modalCreateUrl, e.modalCustomUrl, e.modalOptions, e.tr);
            });
            e.modal.find('.btn-update').unbind('click').bind('click', function () {
                var proceed = true;
                if (e.modalOptions.preSubmitCallback) {
                    proceed = window[e.modalOptions.preSubmitCallback]('update', e);
                }

                if (proceed) {
                    var proc = modalProcessing();
                    var form = e.modal.find('.modal-body form');

                    form.ajaxForm(function (res) {
                        if (e.modalOptions.postSubmitCallback) {
                            proc.addClass('confirm-close-event').modal('hide');
                            loadingUI(e.modal.find('.modal-content'), true);
                            proceed = window[e.modalOptions.postSubmitCallback](res, e, 'update', proc);
                            loadingUI(e.modal.find('.modal-content'), false);
                        } else if (e.modalOptions.postSubmitEvent) {
                            proc.addClass('confirm-close-event').modal('hide');
                            loadingUI(e.modal.find('.modal-content'), true);
                            proceed = e.modalOptions.postSubmitEvent(res, e, 'update', proc);
                            loadingUI(e.modal.find('.modal-content'), false);
                        }

                        if (proceed) {
                            proc.addClass('confirm-close-event').modal('hide');
                            proc = modalProcessing();

                            if (res.IsSuccessful) {
                                sessionStorage.clientMessage = res.Remarks;
                                window.location.reload(true);
                            } else {
                                proc.addClass('confirm-close-event').modal('hide');
                                modalMessage(res.Err.split('\n'), 'Update Record', true);
                            }
                        }
                    });

                    form.submit();
                }
            });

            e.modal.find('.btn-create').unbind('click').bind('click', function () {
                var proceed = true;
                if (e.modalOptions.preSubmitCallback) {
                    proceed = window[e.modalOptions.preSubmitCallback]('create', e);
                }

                if (proceed) {
                    var proc = modalProcessing();
                    var form = e.modal.find('.modal-body form');

                    form.ajaxForm(function (res) {
                        if (e.modalOptions.postSubmitCallback) {
                            proc.addClass('confirm-close-event').modal('hide');
                            loadingUI(e.modal.find('.modal-content'), true);
                            proceed = window[e.modalOptions.postSubmitCallback](res, e, 'create', proc);
                            loadingUI(e.modal.find('.modal-content'), false);
                        } else if (e.modalOptions.postSubmitEvent) {
                            proc.addClass('confirm-close-event').modal('hide');
                            loadingUI(e.modal.find('.modal-content'), true);
                            proceed = e.modalOptions.postSubmitEvent(res, e, 'create', proc);
                            loadingUI(e.modal.find('.modal-content'), false);
                        }

                        if (proceed) {
                            proc.addClass('confirm-close-event').modal('hide');
                            proc = modalProcessing();

                            if (res.IsSuccessful) {
                                sessionStorage.clientMessage = res.Remarks;
                                window.location.reload(true);
                            } else {
                                proc.addClass('confirm-close-event').modal('hide');
                                modalMessage(res.Err.split('\n'), 'New Record', true);
                            }
                        }
                    });

                    form.submit();
                }
            });

            e.modal.find('.btn-custom').unbind('click').bind('click', function () {
                var proceed = true;
                if (e.modalOptions.preSubmitCallback) {
                    proceed = window[e.modalOptions.preSubmitCallback]('custom', e);
                }

                if (proceed) {
                    var proc = modalProcessing();
                    var form = e.modal.find('.modal-body form');

                    form.ajaxForm(function (res) {
                        if (e.modalOptions.postSubmitCallback) {
                            proc.addClass('confirm-close-event').modal('hide');
                            loadingUI(e.modal.find('.modal-content'), true);
                            proceed = window[e.modalOptions.postSubmitCallback](res, e, 'custom', proc);
                            loadingUI(e.modal.find('.modal-content'), false);
                        } else if (e.modalOptions.postSubmitEvent) {
                            proc.addClass('confirm-close-event').modal('hide');
                            loadingUI(e.modal.find('.modal-content'), true);
                            proceed = e.modalOptions.postSubmitEvent(res, e, 'custom', proc);
                            loadingUI(e.modal.find('.modal-content'), false);
                        }

                        if (proceed) {
                            proc.addClass('confirm-close-event').modal('hide');
                            proc = modalProcessing();

                            if (res.IsSuccessful) {
                                sessionStorage.clientMessage = res.Remarks;
                                window.location.reload(true);
                            } else {
                                proc.addClass('confirm-close-event').modal('hide');
                                modalMessage(res.Err.split('\n'), modalOptions.modalCustomTitle, true);
                            }
                        }
                    });

                    form.submit();
                }
            });

            var form = e.modal.find('.modal-body form');
            var textarea = form.find('textarea');

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
                    if (e.modal.hasClass('show-edit')) {
                        var btn = e.modal.find('.modal-footer .btn-update');
                        if (btn.is(':visible')) {
                            btn.click();
                        }
                    } else if (e.modal.hasClass('show-create')) {
                        var btn = e.modal.find('.modal-footer .btn-create');
                        if (btn.is(':visible')) {
                            btn.click();
                        }
                    }
                }
            });
            
            if (e.modalOptions.loadCallback) {
                window[e.modalOptions.loadCallback](e);
            } else if (e.modalOptions.loadEvent) {
                e.modalOptions.loadEvent(e);
            }

            showModalFooter(e.modal);

            if (_internalLoadCallback) {
                _internalLoadCallback(e);
            }

            loading(e.modal, false);
        });

        if (_initModal) {
            doInitModal(e.modal);
        }
    };

    var getOptions = function (tbl, opts) {

        if (tbl.attr('modal-preload-callback')) {
            opts.preLoadCallback = tbl.attr('modal-preload-callback');
        }
        if (tbl.attr('modal-load-callback')) {
            opts.loadCallback = tbl.attr('modal-load-callback');
        }
        if (tbl.attr('modal-presubmit-callback')) {
            opts.preSubmitCallback = tbl.attr('modal-presubmit-callback');
        }
        if (tbl.attr('modal-postsubmit-callback')) {
            opts.postSubmitCallback = tbl.attr('modal-postsubmit-callback');
        }
        if (tbl.attr('modal-listloaded-callback')) {
            opts.listLoadedCallback = tbl.attr('modal-listloaded-callback');
        }
        if (tbl.attr('modal-no-cancel')) {
            opts.modalNoCancel = tbl.attr('modal-no-cancel') == 'true';
        }
        if (tbl.attr('modal-view-width')) {
            opts.modalViewWidth = tbl.attr('modal-view-width');
        }
        if (tbl.attr('modal-edit-width')) {
            opts.modalEditWidth = tbl.attr('modal-edit-width');
        }
        if (tbl.attr('modal-create-width')) {
            opts.modalCreateWidth = tbl.attr('modal-create-width');
        }
        if (tbl.attr('modal-custom-width')) {
            opts.modalCustomWidth = tbl.attr('modal-custom-width');
        }
        if (tbl.attr('modal-list-item-selected')) {
            opts.listItemSelectedCallback = tbl.attr('modal-list-item-selected');
        }

        if (tbl.attr('listcommands-enable')) {
            opts.listCommands.enable = JSON.parse(tbl.attr('listcommands-enable'));
        }

        return opts;
    };

    var getModal = function () {
        var modal = getModalUI();
        modal.addClass('modal-view');

        var modalFooter = getTemplateUIById('modalViewTemplate');
        modal.find('.modal-footer')
            .empty()
            .append(modalFooter.children());

        return modal;
    };

    var methods = {
        init: function (options) {
            var opts = $.extend(true, {}, _options, options);

            return this.each(function () {
                var tbl = $(this);

                var modal;

                var attr = tbl.attr('modal-target');
                var useModal = typeof attr !== typeof undefined && attr !== false;

                var modalNoView = tbl.attr('modal-noview') == 'true';
                
                var modalViewUrl = tbl.attr('modal-view-url');
                var modalEditUrl = tbl.attr('modal-edit-url');
                var modalDeleteUrl = tbl.attr('modal-delete-url');
                var modalCreateUrl = tbl.attr('modal-create-url');

                var modalStart = tbl.attr('modal-start');
                var modalStartIsEdit = modalStart == 'edit';

                if (tbl.attr('data-multiselect')) {
                    opts.multiSelect = tbl.attr('data-multiselect') == 'true';
                }

                opts.multiSelect = opts.multiSelect || tbl.hasClass('multi-select');
                opts.forceItemSelection = tbl.hasClass('force-itemselection');

                opts = getOptions(tbl, opts);

                var spl = tbl.find('tr .spl');
                spl.click(function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    window.location = vdUrl($(this).attr('href'));
                });

                var setListCommands = function (tr) {

                    var getTr = function (a) {
                        var btn = $(a);
                        return btn.closest('tr');
                    };

                    var init = function () {
                        var td = tr.find('.list-commands-col');
                        if (td.length == 0) {
                            td = tr.find('td:last-child');
                            td.addClass('list-commands-col');                            
                        }
                        td.append(getTemplateUIById('ListCommandsTemplate'));
                        return td.find('.list-commands-cont');
                    };
                    
                    var listCommandsCont = init();
                    var btnEdit = listCommandsCont.find('.lstcmd-edit').hide();
                    var btnDelete = listCommandsCont.find('.lstcmd-delete').hide();

                    listCommandsCont.unbind().click(function (e) {
                        e.stopPropagation();
                    });

                    if (useModal) {                       

                        if (modalEditUrl && !modalStartIsEdit) {
                            btnEdit.unbind().click(function (e) {
                                e.preventDefault();
                                e.stopPropagation();

                                var recordId = getTr(this).attr('record-id');

                                modal = getModal();
                                activateView(modal, 'edit', recordId, modalNoView, modalViewUrl, modalEditUrl, modalDeleteUrl, modalCreateUrl, '', opts, tr, true, function (e) {
                                    modal.find('.modal-footer .btn-cancel').hide();
                                    modal.find('.modal-footer .btn-close').html('Cancel');
                                });
                            });

                            btnEdit.show();
                        } else {
                            btnEdit.hide();
                        }

                        if (modalDeleteUrl) {
                            btnDelete.unbind().click(function (e) {
                                e.preventDefault();
                                e.stopPropagation();

                                var recordId = getTr(this).attr('record-id');
                                $.fn.list('modalOp', null, 'delete', modalDeleteUrl, $.extend({}, opts, { url_data: { id: recordId } }));
                            });

                            btnDelete.show();
                        } else {
                            btnDelete.hide();
                        }
                    }
                };

                var bindTr = function (trs) {
                    trs.unbind('click').bind('click', function (e) {
                        var tr = $(this);

                        if ((tr.attr('locked') != undefined || opts.multiSelect) && !opts.forceItemSelection) return;

                        e.preventDefault();

                        var recordId = tr.attr('record-id');

                        if (useModal) {
                            modal = getModal();

                            if (modalNoView) {
                                modal.addClass('modal-noview');
                            }

                            activateView(modal, modalStart, recordId, modalNoView, modalViewUrl, modalEditUrl, modalDeleteUrl, modalCreateUrl, '', opts, tr, true, function (e) {
                                modal.find('.modal-footer .btn-close').html('Close');
                            });

                        } else {
                            var detailsUrl = tbl.attr('data-url');
                            var url = detailsUrl.replace('{0}', recordId).replace('{1}', tr.attr('template-id'));

                            if (tr.attr('newtab') == undefined) {
                                var proc = modalProcessing('Please wait...', 'Redirecting');
                                window.location = vdUrl(url);
                            } else {
                                window.open(vdUrl(url), '_blank');
                            }
                        }
                    });

                    if (opts.listCommands.enable) {
                        $(trs).each(function (i, v) {
                            var tr = $(v);
                            setListCommands(tr);
                        });                        
                    }
                };                

                if (opts.nodes == null) {
                    bindTr(tbl.find('tbody tr').not('[nolistclickbind]'));
                } else {
                    var nodeCount = opts.nodes.length;
                    for (var i = 0; i < nodeCount; i++) {
                        var tr = $(opts.nodes[i]);
                        if (tr.attr('nolistclickbind') == undefined) {

                            if (tr.attr('data-tableid') == undefined) {
                                bindTr(tr);
                            } else {
                                if (tbl.attr('id') == tr.attr('data-tableid')) {
                                    bindTr(tr);
                                }
                            }
                        }
                    }
                }

                var row_commands = tbl.find('.row-commands');
                row_commands.find('.row-cmd').click(function (e) {
                    e.stopPropagation();
                });

                if (opts.listLoadedCallback) {
                    window[opts.listLoadedCallback](opts.nodes);
                }
            });
        },
        modalOp: function (btn, op, url, _opts) {
            var opts = $.extend(true, {}, _options, _opts);

            var getUrl = function () {
                var hasUrl = url != null;

                switch (op) {
                    case 'create':
                        modalCreateUrl = hasUrl ? url : tbl.attr('modal-create-url');
                        break;
                    case 'edit':
                        modalEditUrl = hasUrl ? url : tbl.attr('modal-edit-url');
                        break;
                    case 'delete':
                        modalDeleteUrl = hasUrl ? url : tbl.attr('modal-delete-url');
                        break;
                    case 'view':
                        modalViewUrl = hasUrl ? url : tbl.attr('modal-view-url');
                        break;
                }
            };

            var doActivate = function () {
                switch (op) {
                    case 'create':
                        activateView(modal, op, null, true, '', '', '', modalCreateUrl, '', opts, null, true);
                        break;
                    case 'edit':
                        activateView(modal, op, null, true, '', modalEditUrl, '', '', '', opts, null, true);
                        break;
                    case 'delete':
                        activateView(modal, op, null, true, '', '', modalDeleteUrl, '', '', opts, null, true);
                        break;
                    case 'view':
                        activateView(modal, op, null, true, modalViewUrl, '', '', '', '', opts, null, true);
                        break;
                }
            };

            var selector = $(btn).attr('table-selector');
            var tbl = $(selector);

            getUrl();

            if (_opts == null) {
                opts = getOptions(tbl, opts);
            }

            if (op == 'delete') {
                modalConfirm('Are you sure you want to delete this record?', function (ret) {
                    if (ret) {
                        var proceed = true;
                        if (opts.preSubmitCallback) {
                            proceed = window[opts.preSubmitCallback]('delete', { viewType: 'delete' });
                        }

                        if (proceed) {
                            var proc = modalProcessing();

                            $.post(vdUrl(modalDeleteUrl), opts.url_data, function (res) {
                                if (opts.postSubmitCallback) {
                                    proc.addClass('confirm-close-event').modal('hide');
                                    proceed = window[opts.postSubmitCallback](res, null, 'delete', proc);
                                } else if (opts.postSubmitEvent) {
                                    proc.addClass('confirm-close-event').modal('hide');
                                    proceed = opts.postSubmitEvent(res, null, 'delete', proc);
                                }

                                if (proceed) {
                                    proc.addClass('confirm-close-event').modal('hide');
                                    proc = modalProcessing();

                                    if (res.IsSuccessful) {
                                        sessionStorage.clientMessage = res.Remarks;
                                        window.location.reload(true);
                                    } else {
                                        proc.addClass('confirm-close-event').modal('hide');
                                        modalMessage(res.Err.split('\n'), 'Delete Record', true);
                                    }
                                }
                            }, 'json');
                        }
                    }
                });

            } else {
                modal = getModal();

                doActivate();
            }
        },
        modalCustom: function (btn, url, _opts) {
            var opts = $.extend(true, {}, _options, _opts);

            var selector = $(btn).attr('table-selector');
            var tbl = $(selector);

            opts = getOptions(tbl, opts);

            modal = getModal();

            activateView(modal, 'custom', null, true, '', '', '', '', url, opts, null, true);
        }
    };

    $.fn.list = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.list');
        }

    };

})(jQuery);
