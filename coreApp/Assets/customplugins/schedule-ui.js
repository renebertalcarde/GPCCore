(function ($) {
    var _options = {
        param_allowedsegments_load_validity: null,
        param_allowedsegments_load_exception: null,
        param_listCommandsNodes: null,
        param_scheduleUrl_Customize: '',
        param_scheduleUrl_Copy: '',
        param_scheduleUrl_Paste: '',
        param_scheduleUrl_OpenImport: '',
        param_scheduleUrl_Import: '',
        param_scheduleIsOverride: false,
        param_scheduleClipboardActive: false,
        param_scheduleKeyCode: '',
        param_scheduleStakeholderId: -1
    };

    var opts;
    var data = {
        readOnly: false,
        imported: false,
        readOnlySegments: function () {
            return this.readOnly || this.imported;
        }
    };
    var _url = {};

    var modal, allowedSegmentsCont, segmentListCont, segmentListInfo, segmentListMessages;
    var dataTable;

    var schedule_bind = function () {
        var tbDescription = modal.find('.modal-body [name="Description"]');

        if (data.isTemplate) {
            tbDescription.attr('readonly', 'true');
        }


        var btnCopy = modal.find('.modal-footer .btn-copy');

        btnCopy.unbind().click(function (e) {
            e.preventDefault();
            
            scheduleOps.copy(data.scheduleId, data.keyCode, function () {
                modal.addClass('confirm-close-event').modal('hide');
            });
        });
    };

    var allowedsegments_load = function () {
        loadingUI(allowedSegmentsCont, true);

        var d = {
            allowedSegmentKeys: data.scheduleSegmentKeys,
            validity: opts.param_allowedsegments_load_validity,
            exception: opts.param_allowedsegments_load_exception,
            excludeScheduleOverrideId: data.excludeScheduleOverrideId
        };

        allowedSegmentsCont.load(vdUrl(_url.allowedsegments), d, function (response, status, xhr) {
            setTables(allowedSegmentsCont);
            loadingUI(allowedSegmentsCont, false);
        });
    };

    var segmentList_load = function (reset) {
        loadingUI(segmentListCont, true);

        var d = {
            data: reset ? data.segmentsInit : data.segments,
            columns: [
                { data: 'Description' },
                { data: 'TimeIn_String' },
                { data: 'TimeOut_String' },
                { data: 'Days' },
                { data: 'Enabled' },
                { data: '_editStatus' }
            ]
        };

        setTables(segmentListCont, null, null, null, null, function (row, item, index) {
            var tr = $(row);
            tr.attr('data-index', index);
            tr.attr('record-id', item.Id);

            var status = getItemStatus(item, index);
            tr.removeClass('modified added').addClass(status);                        

            tr.find('td').eq(3).addClass('dayscol');
            tr.find('td').last().addClass('status').html(status);

            tr.removeClass('deleted');
            if (item.KeyCode == 'DELETED') {
                tr.addClass('deleted');
            }

        }, d);

        segmentList_bind();
        
        var status = getListStatus();
        segmentListCont.removeClass('modified includesegmentdays');

        if (data.includeSegmentDays) {
            segmentListCont.addClass('includesegmentdays');
        }

        if (status == 'modified') {
            segmentListCont.addClass('modified');
        }

        segmentListInfo.find('.list-status').html(status);

        loadingUI(segmentListCont, false);
    };
    
    var segmentList_bind = function () {
        var segmentListCommands = segmentListCont.find('.sl-commands');
        var segmentListItemCommands = segmentListCont.find('.list-commands-cont');
        var table = segmentListCont.find('.table');
        var rows = table.find('tbody tr');

        if (data.readOnlySegments()) {
            segmentListCommands.remove();
            segmentListItemCommands.remove();
        } else {
            segmentListCont.unbind().click(function () {
                showSLMessage(null);
            });

            var cmdAdd = segmentListCommands.find('.cmd-add');
            var cmdClear = segmentListCommands.find('.cmd-clear');
            var cmdReset = segmentListCommands.find('.cmd-reset');

            cmdAdd.unbind().click(function (e) {
                e.preventDefault();
                e.stopPropagation();

                segmentOps.add();
            });

            cmdClear.unbind().click(function (e) {
                e.preventDefault();
                e.stopPropagation();

                segmentOps.clear();
            });

            cmdReset.unbind().click(function (e) {
                e.preventDefault();
                e.stopPropagation();

                segmentOps.reset();
            });            
        }

        rows.unbind().click(function () {
            var tr = $(this);
            var index = tr.attr('data-index');

            if (index != undefined) {
                if (data.readOnlySegments()) {
                    segmentOps.view(index);
                } else {
                    segmentOps.edit(index, true);
                }
            }            
        });
    };

    var getItemStatus = function (item, index) {
        var status = '';

        var isnew = item.Id == 0;
        if (isnew) {
            status = 'added';
        } else {
            var a1 = [item];
            var a2 = [data.segmentsInit[index]];
            var match = SITE.Utility.arraysMatch(a1, a2);
            status = match ? 'prestine' : 'modified';
        } 

        return status;
    }

    var getListStatus = function () {
        var dataSegmentsClone = [];
        data.segments.forEach(function (d) {
            if (d.KeyCode != 'DELETED') {
                var item = $.extend({}, true, d);
                dataSegmentsClone.push(item);
            }
        });

        var match = SITE.Utility.arraysMatch(data.segmentsInit, dataSegmentsClone);
        return match ? 'prestine' : 'modified';
    }

    var scheduleOps = {
        customize: function (id) {
            var me = this;

            modalConfirm('Are you sure you want to customize this schedule?', function (ret) {
                if (ret) {
                    var proc = modalProcessing();

                    $.post(vdUrl(_url.customize_schedule), { id: id }, function (res) {
                        if (res.IsSuccessful) {
                            sessionStorage.clientMessage = res.Remarks;
                            window.location.reload(true);
                        } else {
                            proc.addClass('confirm-close-event').modal('hide');
                            modalMessage(res.Err.split('\n'), 'Customize Schedule', true);
                        }
                    }, 'json');
                }
            });
        },
        copy: function (id, keyCode, callback) {
            var me = this;

            var proc = modalProcessing();

            $.post(vdUrl(_url.copy_schedule), { id: id, keyCode: keyCode }, function (res) {
                proc.addClass('confirm-close-event').modal('hide');
                if (res.IsSuccessful) {
                    schedulePasteButton.show(true);
                    modalMessage(res.Remarks, 'Copy Schedule');
                } else {
                    schedulePasteButton.show(false);
                    modalMessage(res.Err.split('\n'), 'Copy Schedule', true);
                }

                if (callback) callback();
            }, 'json');
        },
        paste: function () {
            var me = this;

            var clear = false;
            modalConfirm('Would you like to remove the copied schedule from the session clipboard after pasting?', function (res) {
                if (res) {
                    clear = true;
                }

                var proc = modalProcessing();
                $.post(vdUrl(_url.paste_schedule), { keyCode: data.keyCode, clear: clear }, function (res) {
                    if (res.IsSuccessful) {
                        sessionStorage.clientMessage = res.Remarks;
                        window.location.reload(true);
                    } else {
                        proc.addClass('confirm-close-event').modal('hide');
                        modalMessage(res.Err.split('\n'), 'Paste Schedule', true);
                    }
                }, 'json');
            });
        },
        import: function () {
            var me = this;

            modalEmpty('Import Template', function (modal, recallback) {
                modal.find('.modal-dialog').addClass('modal-xl');

                var body = modal.find('.modal-body');
                var div = $('<div />');
                body.empty().append(div);

                loadingUI(div, true);

                if (data.isOverride) {
                    modal.find('.modal-footer').hide();

                    div.load(_url.openimport_schedule, function () {
                        setTables(body, null, null, null, function (oTable) {
                            var nodes = oTable.rows().nodes();

                            for (var i = 0; i < nodes.length; i++) {
                                var tr = $(nodes[i]);

                                tr.unbind('click').bind('click', function (e) {
                                    var id = $(this).attr('record-id');
                                    modal.addClass('confirm-close-event').modal('hide');

                                    var opts = {
                                        modalCreateWidth: 'modal-xl',
                                        preSubmitCallback: 'presubmit'
                                    };

                                    $.fn.list('modalOp', null, 'create', _url.import_schedule + '/' + id, opts);
                                });
                            }
                        });

                        body.find('.list-commands-cont').remove();

                        loadingUI(div, false);
                    });

                } else {

                    var btnOk = modal.find('.modal-footer .btn-ok');
                    btnOk.attr('disabled', 'disabled');

                    btnOk.unbind().bind('click', function (e) {                        
                        var ids = [];
                        var _ids = div.find('.import-schedule-table').attr('data-selection');
                        if (_ids != '') {
                            ids = _ids.split(',');
                        }

                        if (ids.length == 0) {
                            modalMessage('No template selected', 'Import Template', true);
                            return;
                        }

                        var pcont = div.find('.period-cont');

                        var data = {
                            ids: ids,
                            startDate: pcont.attr('data-start-date').replace(/\//g, '-'),
                            endDate: pcont.attr('data-end-date').replace(/\//g, '-'),
                            importOption: div.find('[name="schedule-import-options"]:checked').val()
                        };

                        var proc = modalProcessing();
                        $.post(vdUrl(_url.import_schedule), data, function (res) {
                            if (res.IsSuccessful) {
                                sessionStorage.clientMessage = res.Remarks;
                                window.location.reload(true);
                            } else {
                                proc.addClass('confirm-close-event').modal('hide');
                                modalMessage(res.Err.split('\n'), 'Import Template', true);
                            }
                        }, 'json');
                    });


                    div.load(_url.openimport_schedule, function () {
                        initPlugins(body);
                        btnOk.removeAttr('disabled');

                        body.find('.list-commands-cont').remove();
                        loadingUI(div, false);
                    });
                }

                if (recallback) recallback();
            });
        }
    };

    var segmentOps = {
        view: function (index, msg) {
            var me = this;

            $.fn.list('modalOp', null, 'view', _url.view_segment, {
                modalViewWidth: 'modal-xl',
                url_data: {
                    data: JSON.stringify(data.segments[index])
                },
                loadEvent: function (e) {
                    showModalMessage(e.modal, msg);

                    e.modal.find('.modal-footer .btn').not('.btn-close').hide();

                    if (!data.readOnlySegments()) {
                        var btnEdit = e.modal.find('.modal-footer .btn-edit');
                        var btnDelete = e.modal.find('.modal-footer .btn-delete');

                        var btnCopy = $('<button type="button" class="btn btn-sm btn-default btn-copysegment for-view">Copy</button>');
                        e.modal.find('.modal-footer').append(btnCopy);

                        btnCopy.unbind().click(function () {
                            e.modal.addClass('confirm-close-event').modal('hide');
                            segmentOps.copy(index);
                        });

                        btnEdit.unbind().click(function () {
                            e.modal.addClass('confirm-close-event').modal('hide');
                            me.edit(index);
                        });

                        btnDelete.unbind().click(function () {
                            me.remove(index);
                            e.modal.addClass('confirm-close-event').modal('hide');
                        });

                        btnDelete.html('Remove');

                        btnEdit.show();
                        btnDelete.show();
                        btnCopy.show();

                        var status = getItemStatus(data.segments[index], index);
                        if (status == 'modified') {
                            var btnReset = $('<button type="button">Reset</button>');
                            btnReset.addClass('btn btn-sm btn-default btn-reset for-view');

                            btnReset.unbind().click(function () {
                                me.itemReset(index);
                                e.modal.addClass('confirm-close-event').modal('hide');
                            });

                            btnReset.insertBefore(btnDelete);
                            btnReset.show();
                        }                        
                    }
                }
            });
        },
        add: function () {
            var me = this;

            $.fn.list('modalOp', null, 'create', _url.create_segment, {
                modalCreateWidth: 'modal-xl',
                loadEvent: function (e) {
                    e.modal.find('.btn-create').html('Add');                    
                },
                postSubmitEvent: function (res, e) {
                    if (res.IsSuccessful) {
                        var proc = modalProcessing();

                        var d = JSON.parse(res.Data);
                        data.segments.push(d);
                        me.commit();

                        proc.addClass('confirm-close-event').modal('hide');
                        e.modal.addClass('confirm-close-event').modal('hide');

                        showSLMessage('Item was successfully added');
                        return false;
                    } else {
                        return true;
                    }
                }
            });
        },
        edit: function (index, noCancel) {
            var me = this;

            $.fn.list('modalOp', null, 'edit', _url.edit_segment, {
                modalEditWidth: 'modal-xl',
                url_data: {
                    data: JSON.stringify(data.segments[index])
                },
                loadEvent: function (e) {
                    if (noCancel) {
                        e.modal.find('.modal-footer .btn-cancel').hide();
                        e.modal.find('.modal-footer .btn-close').html('Cancel');
                    } else {
                        var btnCancel = e.modal.find('.modal-footer .btn-cancel')
                        btnCancel.unbind().click(function () {
                            e.modal.addClass('confirm-close-event').modal('hide');
                            me.view(index);
                        });
                    }
                },
                postSubmitEvent: function (res, e) {
                    if (res.IsSuccessful) {
                        var proc = modalProcessing();

                        var d = JSON.parse(res.Data);
                        data.segments.splice(index, 1, d);
                        me.commit();

                        proc.addClass('confirm-close-event').modal('hide');
                        e.modal.addClass('confirm-close-event').modal('hide');

                        showSLMessage('Item was successfully updated');

                        return false;
                    } else {
                        return true;
                    }
                }
            });
        },
        _copy(id, keyCode) {
            var proc = modalProcessing();
            
            var d = {
                id: id,
                keyCode: keyCode
            };

            $.post(vdUrl(_url.copy_segment), d, function (res) {
                proc.addClass('confirm-close-event').modal('hide');
                if (res.IsSuccessful) {
                    segmentPasteButton.show(true);
                    showSLMessage(res.Remarks);
                } else {
                    segmentPasteButton.show(false);
                    showSLMessage(res.Err.split('\n'), true);
                }
            }, 'json');
        },
        _copyData(segment) {
            var proc = modalProcessing();

            var d = {
                data: JSON.stringify(segment),
                keyCode: segment.KeyCode
            };

            $.post(vdUrl(_url.copy_segmentdata), d, function (res) {
                proc.addClass('confirm-close-event').modal('hide');
                if (res.IsSuccessful) {
                    segmentPasteButton.show(true);
                    showSLMessage(res.Remarks);
                } else {
                    segmentPasteButton.show(false);
                    showSLMessage(res.Err.split('\n'), true);
                }
            }, 'json');
        },
        copy: function (index) {
            var me = this;
            var d = data.segments[index];

            if (d.Id > 0) {
                me._copy(d.Id, d.KeyCode);
            } else {
                me._copyData(d);
            }
        },
        paste: function () {
            var me = this;
            
            var clear = false;
            modalConfirm('Would you like to remove the copied segment from the session clipboard after pasting?', function (res) {
                if (res) {
                    clear = true;
                }

                var proc = modalProcessing();
                $.post(vdUrl(_url.paste_segment), { clear: clear }, function (res) {
                    if (res.IsSuccessful) {

                        var d = JSON.parse(res.Data);
                        d.Id = 0;
                        d.ScheduleId = data.scheduleId;

                        data.segments.push(d);
                        me.commit();

                        proc.addClass('confirm-close-event').modal('hide');

                        segmentPasteButton.show(!clear);
                        showSLMessage(res.Remarks);
                    } else {
                        showSLMessage(res.Err.split('\n'), true);
                    }
                }, 'json');
            });
        },
        remove: function (index) {
            data.segments[index].KeyCode = 'DELETED';
            this.commit();

            showSLMessage('Item was successfully removed');
        },
        clear: function () {
            data.segments = [];
            this.commit();

            showSLMessage('List was successfully cleared');
        },
        reset: function () {
            data.segments = JSON.parse(JSON.stringify(data.segmentsInit));
            this.commit();

            showSLMessage('List was successfully reset');
        },
        itemReset: function (index) {
            var d = JSON.parse(JSON.stringify(data.segmentsInit[index]));
            data.segments.splice(index, 1, d);
            this.commit();

            showSLMessage('Item was successfully reset');
        },
        commit: function () {
            data.segmentsCommit_Input.val(JSON.stringify(data.segments));
            data.segments_Input.val(JSON.stringify(data.segments));
            segmentList_load();
        }
    };
        
    var _init = function (obj) {
        data.isTemplate = JSON.parse(obj.attr('data-isTemplate'));
        data.readOnly = JSON.parse(obj.attr('data-readonly'));
        data.isOverride = JSON.parse(obj.attr('data-isOverride'));
        data.scheduleId = obj.attr('data-scheduleid');
        data.stakeholderId = obj.attr('data-stakeholderid');
        data.keyCode = obj.attr('data-keycode');

        data.segmentsInit_Input = obj.find('[name="SegmentsDataInit"]');
        data.segments_Input = obj.find('[name="SegmentsData"]');
        data.segmentsCommit_Input = obj.find('[name="SegmentsDataFinal"]');

        data.segmentsInit = JSON.parse(data.segmentsInit_Input.val());
        data.segments = JSON.parse(data.segments_Input.val());
        data.segmentClipboardActive = JSON.parse(obj.attr('data-segmentclipboardActive'));
        data.scheduleSegmentKeys = obj.attr('data-schedule-segmentkeys');
        data.excludeScheduleOverrideId = obj.attr('data-excludeScheduleOverrideId');
        data.includeSegmentDays = JSON.parse(obj.attr('data-includesegmentdays'));

        _url.allowedsegments = obj.attr('url-allowedsegments');
        _url.create_segment = obj.attr('url-create-segment');
        _url.get_segment = obj.attr('url-get-segment');
        _url.view_segment = obj.attr('url-view-segment');
        _url.edit_segment = obj.attr('url-edit-segment');
        _url.copy_segment = obj.attr('url-copy-segment');
        _url.copy_segmentdata = obj.attr('url-copy-segmentdata');
        _url.paste_segment = obj.attr('url-paste-segment');
        _url.customize_schedule = obj.attr('url-customize-schedule');
        _url.copy_schedule = obj.attr('url-copy-schedule');
        _url.paste_schedule = obj.attr('url-paste-schedule');
        _url.openimport_schedule = obj.attr('url-openimport-schedule');
        _url.import_schedule = obj.attr('url-import-schedule');

        modal = obj.closest('.modal');
        allowedSegmentsCont = obj.find('.allowedsegments-cont');
        segmentListCont = obj.find('.segment-list');
        segmentListInfo = segmentListCont.find('.sl-info');
        segmentListMessages = segmentListCont.find('.sl-messages');
    };

    var showSLMessage = function (msg, isError) {    
        segmentListMessages.find('.slMessage, .slErrorMessage').empty().hide();

        if (msg) {
            segmentListMessages.find(isError ? '.slErrorMessage' : '.slMessage').html(msg).show();
        }
    };

    var schedulePasteButton = {        
        append: function () {
            var btn = $('<button class="btn btn-sm btn-default btn-pasteschedule">Paste Schedule</button>');
            $('.form-actions').append(btn);

            btn.click(function () {
                scheduleOps.paste();
            });

            this.show(opts.param_scheduleClipboardActive);
        },
        show: function (show) {
            var btnPaste = $('.form-actions .btn-pasteschedule');
            if (show) {
                btnPaste.show();
            } else {
                btnPaste.hide();
            }
        }
    };

    var segmentPasteButton = {
        append: function () {
            var btn = $('<button class="btn btn-sm btn-default btn-pastesegment for-edit for-create">Paste Segment</button>');
            $('.modal-footer').append(btn);

            btn.click(function () {
                segmentOps.paste();
            });

            this.show(data.segmentClipboardActive);
        },
        show: function (show) {
            var btnPaste = modal.find('.modal-footer .btn-pastesegment');
            if (show) {
                btnPaste.show();
            } else {
                btnPaste.hide();
            }
        }
    };

    var methods = {
        init: function (options) {
            opts = $.extend(true, {}, _options, options);
            var obj = $(this);

            _init(obj);

            if (!data.readOnlySegments()) {
                segmentPasteButton.append();
            }
            
            schedule_bind();
            segmentList_load();
        },
        importSchedule: function (options) {
            opts = $.extend(true, {}, _options, options);
            var obj = $(this);
            
            _url.openimport_schedule = opts.param_scheduleUrl_OpenImport;
            _url.import_schedule = opts.param_scheduleUrl_Import;
            data.isOverride = opts.param_scheduleIsOverride;

            scheduleOps.import();
        },
        refreshAllowedSegments: function (options) {
            opts = $.extend(true, {}, _options, options);
            var obj = $(this);

            _init(obj);

            allowedsegments_load();
        },
        initScheduleListCommands: function (options) {
            opts = $.extend(true, {}, _options, options);

            data.keyCode = opts.param_scheduleKeyCode;
            _url.customize_schedule = opts.param_scheduleUrl_Customize;
            _url.copy_schedule = opts.param_scheduleUrl_Copy;
            _url.paste_schedule = opts.param_scheduleUrl_Paste;
            
            schedulePasteButton.append();
            
            setListCommandsNodes(opts.param_listCommandsNodes, function (tr, cont) {    

                data.isTemplate = JSON.parse(tr.attr('data-isTemplate'));

                var btnCopy = $('<a href="#" class="lstcmd lstcmd-copyschedule" title="copy"><i class="fa fa-copy"></i></a>');
                cont.append(btnCopy);
                
                btnCopy.unbind().click(function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    var btn = $(this);
                    var id = btn.closest('tr').attr('record-id');
                    var keyCode = btn.closest('tr').attr('data-keycode');

                    scheduleOps.copy(id, keyCode);
                });

                if (data.isTemplate) {
                    var btnCustomize = $('<a href="#" class="lstcmd lstcmd-customizeschedule" title="customize"><i class="fa fa-unlock"></i></a>');
                    cont.append(btnCustomize);

                    btnCustomize.unbind().click(function (e) {
                        e.preventDefault();
                        e.stopPropagation();

                        var btn = $(this);
                        var id = btn.closest('tr').attr('record-id');

                        scheduleOps.customize(id);
                    });
                }
            });
        },
        initAllowedSegmentsListCommands: function (options) {
            opts = $.extend(true, {}, _options, options);
            var obj = $(this);

            _init(obj);

            setListCommandsNodes(opts.param_listCommandsNodes, function (tr, cont) {
                cont.find('.lstcmd-edit, .lstcmd-delete, .lstcmd-copysegment').remove();

                var btnCopy = $('<a href="#" class="lstcmd lstcmd-copysegment" title="copy"><i class="fa fa-copy"></i></a>');
                cont.append(btnCopy);

                btnCopy.unbind().click(function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    var id = $(this).closest('tr').attr('record-id');
                    var keyCode = $(this).closest('tr').attr('data-keycode');
                    segmentOps._copy(id, keyCode);
                }); 
            });
        },
        initSegmentListCommands: function (options) {
            opts = $.extend(true, {}, _options, options);
            var obj = $(this);

            _init(obj);

            setListCommandsNodes(opts.param_listCommandsNodes, function (tr, cont) {
                var btnCopy = $('<a href="#" class="lstcmd lstcmd-copysegment" title="copy"><i class="fa fa-copy"></i></a>');
                cont.append(btnCopy);

                var btnReset = $('<a href="#" class="lstcmd lstcmd-reset" title="reset"><i class="fa fa-recycle"></i></a>');
                cont.append(btnReset);
                
                var btnDelete = cont.find('.lstcmd-delete').show();

                btnCopy.unbind().click(function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    var index = $(this).closest('tr').attr('data-index');
                    segmentOps.copy(index);
                });

                btnDelete.unbind().click(function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    var index = $(this).closest('tr').attr('data-index');
                    segmentOps.remove(index);
                });

                btnReset.unbind().click(function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    var index = $(this).closest('tr').attr('data-index');
                    segmentOps.itemReset(index);
                }); 
            });
        }
    };

    $.fn.scheduleUI = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.scheduleUI');
        }

    };

})(jQuery);