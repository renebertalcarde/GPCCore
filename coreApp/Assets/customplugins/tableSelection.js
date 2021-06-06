(function ($) {
    var _options = {
        table: null,
        selectionCallback: null,
        selectionEvent: null
    };


    var getTableSelection = function (cont, oTable) {
        loadingUI(cont, true);
        var ret = [];
        var nodes = oTable.rows().nodes();

        for (var i = 0; i < nodes.length; i++) {
            var tr = $(nodes[i]);
            var cb = tr.find('.table-select-item input[type="checkbox"]');

            if (cb.prop('checked')) {
                ret.push(cb.attr('value'));
            }
        }
        loadingUI(cont, false);

        return ret;
    };

    var setTableSelection = function (cont, oTable, v, values) {
        loadingUI(cont, true);

        var nodes = oTable.rows().nodes();
        var trs = [];
        for (var i = 0; i < nodes.length; i++) {
            var tr = $(nodes[i]);

            if (values) {
                var id = tr.attr('record-id');
                if (!values.includes(id)) continue;
            }

            var cb = tr.find('.table-select-item input[type="checkbox"]');

            cb.prop('checked', v);

            setTableSelection_TrUI(oTable, tr, cb);
        }

        loadingUI(cont, false);
    };

    var setTableSelection_TrUI = function (oTable, tr, cb) {
        var v = cb.prop('checked');

        cb.closest('td')
            .attr('data-order', v ? '1' : '0')
            .attr('data-sort', v ? '1' : '0');

        tr.removeClass('table-item-selected');
        if (v) {
            tr.addClass('table-item-selected');
        }

        oTable.rows(tr).invalidate();
    };

    var getTableSelectItems = function (cont, oTable, values) {
        loadingUI(cont, true);
        var ret = [];
        var nodes = oTable.rows().nodes();

        for (var i = 0; i < nodes.length; i++) {
            var tr = $(nodes[i]);

            if (values) {
                var id = tr.attr('record-id');
                if (!values.includes(id)) continue;
            }

            var cb = tr.find('.table-select-item input[type="checkbox"]');
            ret.push(cb);
        }

        loadingUI(cont, false);
        return ret;
    };

    var methods = {
        init: function (options) {
            var opts = $.extend(true, {}, _options, options);

            return this.each(function () {
                var obj = $(this);
                
                var selectAll = obj.find('.table-select-all input[type="checkbox"]');
                var tableSelectItems = getTableSelectItems(obj, opts.table);


                var getArrDataSelection = function () {
                    var _selection = [];

                    if (obj.attr('data-selection')) {
                        _selection = obj.attr('data-selection').split(",");
                    }

                    return _selection;
                };

                var getSelection = function () {
                    var arrSelection = getTableSelection(obj, opts.table);
                    selection = arrSelection.join(",");
                    obj.attr('data-selection', selection);

                    if (opts.selectionCallback) {
                        window[opts.selectionCallback](selection);
                    }

                    if (opts.selectionEvent) {
                        opts.selectionEvent(selection);
                    }
                };

                selectAll.parent().click(function (e) {
                    e.stopPropagation();
                });

                selectAll.click(function () {
                    var cb = $(this);
                    var v = cb.prop('checked');
                    var tbl = cb.closest('table');
                    setTableSelection(tbl, opts.table, v);
                    getSelection();
                });

                tableSelectItems.forEach(function (v) {
                    $(v).parent().click(function (e) {
                        e.stopPropagation();
                    });

                    $(v).click(function () {

                        var cb = $(this);
                        var tr = cb.closest('tr');

                        setTableSelection_TrUI(opts.table, tr, cb);

                        opts.table.rows(tr).invalidate();

                        getSelection();
                        setSelectAllUI();
                    });
                });

                var setSelectAllUI = function () {
                    if (tableSelectItems.length > 0 && tableSelectItems.length == getArrDataSelection().length) {
                        selectAll.prop('checked', true);
                    } else {
                        selectAll.prop('checked', false);
                    }
                };

                setTableSelection(obj, opts.table, true, getArrDataSelection());
                setSelectAllUI();

            });
        }
    };

    $.fn.tableSelection = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on tableSelection');
        }

    };

})(jQuery);
