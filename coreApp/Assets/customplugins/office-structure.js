(function ($) {
    var _options = {
        edit_event_listdisplayed: null,
        edit_event_itemselected: null,
        edit_event_selectioncleared: null,
        isDepartmentSelection: true,
        singleselect_data: null,
        singleselect_opencallback: null,
        multiselect_data: null,        
        multiselect_opencallback: null
    };

    var opts;
    var selectedItem = null;
    var _type;

    var item = function (element) {
        selectedItem = $(element).closest('.office-structure-item');
        return selectedItem;
    }

    var edit_binditems = function (obj) {
        var selectItemInfo = obj.find('.selected-item-info');
        var commands = obj.find('.office-structure-commands');

        var itemHandle = obj.find('.item-handle');
        var itemName = obj.find('.item-name');
        
        var selectItem = function (_item) {
            var selectItemName = selectItemInfo.find('.selected-item-name');
            var selectItemType = selectItemInfo.find('.selected-item-type');
            var itemName = _item.attr('data-itemname');
            var itemId = _item.attr('data-itemid');
            var itemType = _item.attr('data-itemtype');

            //clear
            selectItemName.html('[none]');
            selectItemType.empty();
            commands.removeClass('hasselection branch office department');

            if (_item.hasClass('active')) {
                selectItemName.html(itemName);
                selectItemType.html(itemType);
                commands.addClass('hasselection');
                commands.addClass(itemType);

                if (opts.edit_event_itemselected) opts.edit_event_itemselected(itemType, itemId);
            } else {
                if (opts.edit_event_selectioncleared) opts.edit_event_selectioncleared();
            }
        };
        
        itemHandle.unbind('click').bind('click', function () {
            var me = item(this);
            me.toggleClass('expanded');

            if (!me.hasClass('expanded')) {
                me.find('*').removeClass('active');
            }

            if (opts.edit_event_listdisplayed) opts.edit_event_listdisplayed();
        });

        itemName.unbind('click').bind('click', function () {
            var me = item(this);
            obj.find('.office-structure-item').not(me).removeClass('active');
            me.toggleClass('active');
            selectItem(me);
        });

        itemName.unbind('dblclick').bind('dblclick', function () {
            var me = item(this);
            if (me.hasClass('expandable')) {
                var children = me.children('ul').children('li').children('.office-structure-item.expandable');

                me.toggleClass('expand-all');

                if (me.hasClass('expand-all')) {
                    children.addClass('expanded');
                } else {
                    children.removeClass('expanded');
                }
            }
        });
    };

    var edit_bindcommands = function (obj) {
        var commands = obj.find('.office-structure-commands');
        var newBranch = commands.find('.cmd-newbr');
        var newOffice = commands.find('.cmd-newoffice');
        var newDepartment = commands.find('.cmd-newdepartment');
        var editItem = commands.find('.cmd-edititem');
        var deleteItem = commands.find('.cmd-deleteitem');


        var addItem = function (data) {
            var template;
            var ul;

            if (_type == 'branch') {
                template = $('<div class="office-structure-item expanded expandable" data-itemid="' + data.Id + '" data-itemname="' + data.Description + '" data-itemtype="branch" data-parentid="-1" data-stakeholdercountdesc="">' +
                    '<span class= "item-handle dummy"><i class="fa fa-plus"></i><i class="fa fa-minus"></i></span><span class="item-name">' + data.Description + '</span></div>');

                ul = obj.children('ul');
                if (ul.length == 0) {
                    ul = $('<ul />');
                    obj.append(ul);
                }

            } else if (_type == 'office') {
                template = $('<div class="office-structure-item expanded expandable" data-itemid="' + data.OfficeId + '" data-itemname="' + data.Office + '" data-itemtype="office" data-parentid="' + data.BranchId + '" data-stakeholdercountdesc="">' +
                    '<span class= "item-handle dummy"><i class="fa fa-plus"></i><i class="fa fa-minus"></i></span><span class="item-name">' + data.Office + '</span></div>');

                ul = selectedItem.children('ul');
                if (ul.length == 0) {
                    ul = $('<ul />');
                    selectedItem.append(ul);
                }

                var os = selectedItem.closest('.office-structure-item[data-itemtype="branch"]');
                os.addClass('expanded');
                os.children('.item-handle').removeClass('dummy');

            } else if (_type == 'department') {
                template = $('<div class="office-structure-item" data-itemid="' + data.DepartmentId + '" data-itemname="' + data.Department + '" data-itemtype="department" data-parentid="' + data.OfficeId + '" data-stakeholdercountdesc="">' +
                    '<span class= "item-handle dummy"><i class="fa fa-plus"></i><i class="fa fa-minus"></i></span><span class="item-name">' + data.Department + '</span></div>');

                ul = selectedItem.children('ul');
                if (ul.length == 0) {
                    ul = $('<ul />');
                    selectedItem.append(ul);
                }

                var os = selectedItem.closest('.office-structure-item[data-itemtype="office"]');
                os.addClass('expanded');
                os.children('.item-handle').removeClass('dummy');
            }

            ul.prepend(template);
            edit_binditems(obj);
        };

        var updateItem = function (data) {
            var name;
            if (_type == 'branch') {
                name = data.Description;
            } else if (_type == 'office') {
                name = data.Office;
            } else if (_type == 'department') {
                name = data.Department;
            }

            selectedItem.attr('data-itemname', name);
            selectedItem.children('.item-name').html(name);
        };

        var createItem_PostSubmitEvent = function (res, e, op, proc) {
            if (res.IsSuccessful) {
                showClientMessage(res.Remarks);
                e.modal.addClass('confirm-close-event').modal('hide');
                addItem(res.Data);
            } else {
                modalMessage(res.Err.split('\n'), 'New Record', true);
            }
            return false;
        };

        var createItem_PostSubmitEvent = function (res, e, op, proc) {
            e.modal.addClass('confirm-close-event').modal('hide');
            if (res.IsSuccessful) {
                showClientMessage(res.Remarks);
                addItem(res.Data);
            } else {
                modalMessage(res.Err.split('\n'), 'New Record', true);
            }
            return false;
        };

        var edititem_PostSubmitEvent = function (res, e, op, proc) {
            e.modal.addClass('confirm-close-event').modal('hide');
            if (res.IsSuccessful) {
                showClientMessage(res.Remarks);
                updateItem(res.Data);
            } else {
                modalMessage(res.Err.split('\n'), 'Edit Record', true);
            }
            return false;
        };

        var deleteitem_PostSubmitEvent = function (res, proc) {
            proc.addClass('confirm-close-event').modal('hide');
            if (res.IsSuccessful) {
                showClientMessage(res.Remarks);     
                selectedItem.closest('li').remove();
            } else {
                modalMessage(res.Err.split('\n'), 'Delete Record', true);
            }
        };
        
        var _newItem = function (type) {
            _type = type;
            if (type == 'branch') {
                office_structure_createitem(type, -1, createItem_PostSubmitEvent);
            } else {
                var parentId = selectedItem.attr('data-itemid');
                office_structure_createitem(type, parentId, createItem_PostSubmitEvent);
            }
        };

        var _editItem = function () {
            var id = selectedItem.attr('data-itemid');
            var type = selectedItem.attr('data-itemtype');
            _type = type;
            office_structure_edititem(type, id, edititem_PostSubmitEvent);
        };

        var _deleteItem = function () {
            var id = selectedItem.attr('data-itemid');
            var type = selectedItem.attr('data-itemtype');
            _type = type;
            var stakeholders = selectedItem.attr('data-stakeholdercountdesc');

            var warning = '';            
            if (stakeholders != '') {                
                warning = '<div style="color:#f00">WARNING!!! This ' + type + ' has ' + stakeholders + '</div>';
            }
            modalConfirm('Are you sure you want to delete this item?' + warning, function (ret) {
                if (ret) {                   
                    office_structure_deleteitem(type, id, deleteitem_PostSubmitEvent);
                }
            });
        };

        newBranch.click(function () {
            _newItem('branch');
        });

        newOffice.click(function () {
            _newItem('office');
        });

        newDepartment.click(function () {
            _newItem('department');
        });

        editItem.click(function () {
            _editItem();
        });

        deleteItem.click(function () {
            _deleteItem();
        });
    };
    
    var inlinetrigger_setlabel = function (obj) {
        var cont = obj.find('.internal-office-structure-cont');
        var isDepartmentSelection = obj.attr('data-isdepartmentselection') == 'True';
        var id = obj.attr('data-id');
        var ids = obj.attr('data-selection');
        var url_os = cont.attr('data-url');

        loadingUI(cont, true);

        var data = {
            Id: id + '-Internal-OS-PathsDisplay',
            Mode: 6,
            IsDepartmentSelection: isDepartmentSelection,
            SelectedIds: []
        };

        if (ids != '') {
            data.SelectedIds = ids.split(',');
        }

        cont.load(url_os, { data: JSON.stringify(data) }, function () {
            cont.find('.office-structure').officeStructure();

            setPopover(cont);
            loadingUI(cont, false);
        });
    };

    var inlinetrigger_bind = function (obj) {
        obj.click(function () {
            var ids = obj.attr('data-selection').split(',');
            var isDepartmentSelection = obj.attr('data-isdepartmentselection') == 'True';
            var multiSelection = obj.attr('data-multiselection') == 'True';
            var selectionEvent = obj.attr('data-selection-event');

            if (multiSelection) {

                multiselect_open(isDepartmentSelection, ids, function (ids, paths) {
                    obj.attr('data-selection', ids.join(','));
                    obj.attr('data-paths', JSON.stringify(paths));

                    inlinetrigger_setlabel(obj);

                    if (selectionEvent != '') {
                        if (window[selectionEvent]) {
                            window[selectionEvent](ids, paths);
                        }
                    }
                });

            } else {

                var id = ids.length == 1 ? ids[0] : -1;
                singleselect_open(isDepartmentSelection, id, function (id, path) {
                    obj.attr('data-selection', id);
                    obj.attr('data-paths', JSON.stringify(path));

                    inlinetrigger_setlabel(obj);

                    if (selectionEvent != '') {
                        if (window[selectionEvent]) {
                            window[selectionEvent](id, path);
                        }
                    }
                });

            }
            
        });
    };

    var singleselect_open = function (isDepartmentSelection, id, selectionCallback) {
        var a = isDepartmentSelection ? 'Department' : 'Office';
        var b = isDepartmentSelection ? 'department' : 'office';

        modalEmpty('Select ' + a, function (modal, recallback) {
            var div = $('<div />');

            modal.find('.modal-dialog').addClass('modal-lg');
            var body = modal.find('.modal-body');
            
            body.append(div);

            loadingUI(body, true);

            setTimeout(function () {

                var t = $('#OSSingleSelectTemplate').html();
                div.html($(t));

                setOfficeStructure(div);

                var cb = div.find('#' + b + '-' + id);
                if (cb.length > 0) {
                    cb.click();
                }

                modal.find('.modal-footer .btn-ok').click(function () {
                    var id, path;
                    var cbs = div.find('[id^="' + b + '-"]:checked');

                    var cb = $(cbs[0]);
                    id = cb.val();

                    path = cb.closest('li').attr('data-path');

                    if (selectionCallback) {
                        selectionCallback(id, path);
                    }
                });

                singleselect_bindcommands(modal);

                loadingUI(body, false);

                if (recallback) recallback();

            }, 500);
        });
    };

    var multiselect_open = function (isDepartmentSelection, ids, selectionCallback) {
        var a = isDepartmentSelection ? 'Departments' : 'Offices';
        var b = isDepartmentSelection ? 'department' : 'office';

        modalEmpty('Select ' + a, function (modal, recallback) {
            var div = $('<div />');

            modal.find('.modal-dialog').addClass('modal-lg');
            var body = modal.find('.modal-body');
            
            body.append(div);

            loadingUI(body, true);

            setTimeout(function () {
                var t = $('#OSMultiSelectTemplate').html();
                div.html($(t));

                setSelectAll(div);
                setOfficeStructure(div);

                ids.forEach(function (v) {
                    var cb = div.find('#' + b + '-' + v);
                    if (cb.length > 0) {
                        cb.click();
                    }
                });

                modal.find('.modal-footer .btn-ok').click(function () {
                    var arrIds = [];
                    var arrPaths = [];
                    var cbs = div.find('[id^="' + b + '-"]:checked');
                    cbs.each(function (i, v) {
                        var cb = $(v);
                        arrIds.push(cb.val());

                        var path = cb.closest('li').attr('data-path');
                        arrPaths.push(path);
                    });

                    if (selectionCallback) {
                        selectionCallback(arrIds, arrPaths);
                    }
                });

                loadingUI(body, false);

                if (recallback) recallback();
            }, 500);            
        });
    };
    
    var singleselect_bindcommands = function (obj) {
        var commands = obj.find('.office-structure-commands');
        var unSelectAll = commands.find('.cmd-unselectall');

        var select = function (v) {
            obj.find('[type="radio"]').prop('checked', v);
        }

        unSelectAll.click(function () {
            select(false);
        });
    };

    var multiselect_bindcommands = function (obj) {
        var commands = obj.find('.office-structure-commands');
        var selectAll = commands.find('.cmd-selectall');
        var unSelectAll = commands.find('.cmd-unselectall');

        var select = function (v) {
            obj.find('[type="checkbox"]').prop('checked', v);
        }

        selectAll.click(function () {
            select(true);
        });

        unSelectAll.click(function () {
            select(false);
        });
    };

    var methods = {
        init: function (options) {
            opts = $.extend(true, {}, _options, options);

            return $(this).each(function () {
                var obj = $(this);
                var mode = obj.attr('data-mode');

                if (mode == 'edit') {
                    edit_bindcommands(obj);
                    edit_binditems(obj);
                } else if (mode == 'inlinetrigger') {
                    inlinetrigger_setlabel(obj);
                    inlinetrigger_bind(obj);
                } else if (mode == 'singleselect') {
                    singleselect_bindcommands(obj);
                } else if (mode == 'multiselect') {
                    multiselect_bindcommands(obj);
                }
            });
        },
        openSingleselect: function (options) {
            opts = $.extend(true, {}, _options, options);
            singleselect_open(opts.isDepartmentSelection, opts.singleselect_data, opts.singleselect_opencallback);
        },
        openMultiselect: function (options) {
            opts = $.extend(true, {}, _options, options);
            multiselect_open(opts.isDepartmentSelection, opts.multiselect_data, opts.multiselect_opencallback);
        }
    };

    $.fn.officeStructure = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.officeStructure');
        }

    };

})(jQuery);