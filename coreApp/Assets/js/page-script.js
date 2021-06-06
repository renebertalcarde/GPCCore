//var loadingIcon = '<div class="loader" title="loading..."><i class="icon-spinner"></i></div>';

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

function initPlugins(cont) {
    setDatePickers(cont);
    setTimePickers(cont);
    setDateTimePickers(cont);
    //setDateRangePickers(cont);
    setDateCollection(cont);
    setPeriod(cont);
    //setSwitches(cont);
    setSelectAll(cont);
    setTables(cont);
    setDetails(cont);
    //setOfficeDepartments(cont);
    //setEventSelection(cont);
    //setScroll(cont);
    setSegmentTime(cont);
    setPhotoCont(cont);
    setStakeholderSearch(cont);
    setTinyMCE(cont);
    setMaskedInput(cont);
    setLeaveEntry(cont);
    setStateKeys(cont);
    setTextInput(cont);
    setSelect2(cont);
    //setChosen(cont);
    //setIntSpinner(cont);
    //setBootstrapMultiselect(cont);
    setFormActions(cont);
    setSubmitFor(cont);
    setSelectTag(cont);
    setSelectPicker(cont);
    setEffectivityCont(cont);
    setFileInput(cont);
    setPopover(cont);
    setOfficeStructure(cont);
    scheduleUI(cont);
    setMultiselectUI(cont);
    setStatAddress(cont);
}

$(document).ready(function () {
    setMobileViewClass();
    initPlugins();
    setRightNavigation();
    initChangePW_TopLink();
});

$(window).resize(function () {
    setMobileViewClass();
    setModalHeights();

    var formActions = document.getElementsByClassName('form-actions-nav');
    if (formActions.length > 0) {
        formActions[0].style.transition = '';
        formActions[0].style.opacity = 0;

        setTimeout(function () {
            formActions[0].style.transition = 'opacity 1s';
            adjustFormActions();
            formActions[0].style.opacity = 1;
        }, 1000);   
    }    
});

//var isScrolling;
//window.onscroll = function () {
//    var formActions = document.getElementsByClassName('form-actions-nav');
//    if (formActions.length > 0) {
//        formActions[0].style.transition = '';
//        formActions[0].style.opacity = 0;

//        adjustFormActions();
//        window.clearTimeout(isScrolling);

//        isScrolling = setTimeout(function () {
//            formActions[0].style.transition = 'opacity .5s';
//            adjustFormActions();
//            formActions[0].style.opacity = 1;
//        }, 300);
//    }
//}

function adjustFormActions(addTransition) {
    var footer = document.getElementById('footer');
    if (footer) {
        var footerTop = footer.getBoundingClientRect().top;
        var windowHeight = window.innerHeight;
        var y = windowHeight - footerTop;
        if (y < 0) y = 0;

        var formActions = document.getElementsByClassName('form-actions-nav');
        if (formActions.length > 0) {
            formActions[0].style.bottom = y + 'px';
        }
    }
}

function onMobileView() {
    return $('#primary-menu-trigger').is(':visible');
}

function setMobileViewClass() {
    if (onMobileView()) {
        $('body').addClass('mobile-view');
    } else {
        $('body').removeClass('mobile-view');
    }
}

function setModalHeights(modal) {
    var obj;

    if (modal) {
        obj = modal;
    } else {
        obj = $('.modal');
    }

    var body = obj.find('.modal-body');
    var winHeight = $(window).height();
    var allowance = 200;

    body.removeAttr('style');
    var bodyHeight = body.height();
    var datepickerHeightAllowance = 350;
    var hasDatePickers = body.find('.datepicker, .timepicker, .datetimepicker').length > 0;

    if (hasDatePickers && bodyHeight < datepickerHeightAllowance) {
        body.css({ height: datepickerHeightAllowance });
        bodyHeight = body.height();
    }

    if (bodyHeight > winHeight - allowance) {
        body.css({ height: winHeight - allowance });
    }
}

function setPopoverField(cont) {
    var obj;

    if (cont) {
        obj = cont.find('[data-popover-content-id]');
    } else {
        obj = $('[data-popover-content-id]');
    }

    obj.each(function (i, v) {
        var o = $(v);

        var id = o.attr('data-popover-content-id');
        o.attr('data-container', 'body')
            .attr('data-toggle', 'popover')
            .attr('data-placement', 'right')
            .attr('data-trigger', 'hover')
            .attr('data-popover-content', id)
            .attr('data-html', 'true')
            .attr('data-original-title', '')
            .attr('title', '');

        o.removeAttr('data-popover-content-id');
    });
}

function setPopover(cont) {
    var obj;

    setPopoverField(cont);

    if (cont) {
        obj = cont.find('[data-toggle=popover]');
    } else {
        obj = $('[data-toggle=popover]');
    }

    obj.each(function (i, v) {
        var o = $(v)
        o.popover('dispose');

        if (o.attr('data-popover-content')) {
            o.popover({
                html: true,
                content: function () {
                    var content = $(this).attr("data-popover-content");
                    return $($(content).html());
                }
            });
        } else {
            o.popover();
        }
    });
}

function setFileInput(cont) {
    var obj;

    if (cont) {
        obj = cont.find('input[type="file"].file');
    } else {
        obj = $('input[type="file"].file');
    }

    obj.fileinput({
        showUpload: false
    });
}

function setOfficeStructure(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.office-structure');
    } else {
        obj = $('.office-structure');
    }

    obj.officeStructure();
}

function scheduleUI(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.schedule-ui');
    } else {
        obj = $('.schedule-ui');
    }

    obj.each(function () {
        $(this).scheduleUI();
    });
}

function setMultiselectUI(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.multiselect-ui');
    } else {
        obj = $('.multiselect-ui');
    }

    obj.multiselectUI();
}


function setSelectPicker(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.selectpicker');
    } else {
        obj = $('.selectpicker');
    }

    obj.selectpicker();
}

function setSelectTag(cont) {
    var obj;

    if (cont) {
        obj = cont.find('select');
    } else {
        obj = $('select');
    }

    obj.change(function () {
        _setSelectObject($(this));
    })

    obj.each(function (i, v) {
        _setSelectObject($(v));
    })
}

function _setSelectObject(obj) {
    var firstoption = obj.children('option').first();
    var isplaceholder = true;
    if (firstoption.length > 0) {
        var optVal = firstoption.val();

        if (obj.val() != optVal || (optVal != '-1' && optVal != '')) {
            isplaceholder = false;
        }
    }

    if (isplaceholder) {
        obj.attr('has-placeholder', '');
    } else {
        obj.removeAttr('has-placeholder');
    }
}

function setSubmitFor(cont) {
    var obj;

    if (cont) {
        obj = cont.find('[submit-for-selector]');
    } else {
        obj = $('[submit-for-selector]');
    }

    if (obj.length > 0) {

        var selector = obj.attr('submit-for-selector');
        var form = $(selector);

        obj.unbind('click').bind('click', function (e) {
            e.preventDefault();
            form.submit();
        });
    }
}

function setFormActions(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.form-actions-nav');
    } else {
        obj = $('.form-actions-nav');
    }

    if (obj.length > 0) {
        obj.formActions();
    }
}

function setBootstrapMultiselect(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.bootstrap-multiselect');
    } else {
        obj = $('.bootstrap-multiselect');
    }

    if (obj.length > 0) {
        obj.multiselect({
            includeSelectAllOption: true
        });
    }
}

function setIntSpinner(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.int-spinner');
    } else {
        obj = $('.int-spinner');
    }

    if (obj.length > 0) {
        obj.intSpinner();
    }
}

function setStateKeys(cont) {
    var obj;

    if (cont) {
        obj = cont.find('[state-key]');
    } else {
        obj = $('[state-key]');
    }

    if (obj.length > 0) {
        obj.stateManager();
    }
}

function setLeaveEntry(cont, options) {
    var obj;

    if (cont) {
        obj = cont.find('.leave-entry');
    } else {
        obj = $('.leave-entry');
    }

    if (obj.length > 0) {
        obj.leaveEntry(options);
    }
}

function setMaskedInput(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.masked-input');
    } else {
        obj = $('.masked-input');
    }

    if (obj.length > 0) {
        var maskTemplate = obj.attr('data-mask');
        obj.mask(maskTemplate);
    }
}

function setSelect2(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.select2, select:not(.selectpicker)').not('.noselect2');
    } else {
        obj = $('.select2, select:not(.selectpicker)').not('.noselect2');
    }

    obj.each(function (i, v) {
        var sel = $(v);
        if (sel.hasClass('select2')) {
            sel.select2();
        } else {
            sel.select2({
                minimumResultsForSearch: -1
            });
        }
    });
}

function setStatAddress(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.stat-address-cont');
    } else {
        obj = $('.stat-address-cont');
    }

    obj.statAddress();
}

function setChosen(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.chosen-select');
    } else {
        obj = $('.chosen-select');
    }

    if (obj.length > 0) {
        obj.chosen();
    }
}


function setTinyMCE(cont) {
    var obj;

    if (cont) {
        obj = cont.find('textarea.tmce');
    } else {
        obj = $('textarea.tmce');
    }

    var opts = {
        height: 200
    };

    if (obj.attr('data-height')) {
        opts.height = parseFloat(obj.attr('data-height'));
    }

    if (obj.length > 0) {
        tinymce.remove("#" + obj.attr('id'));
        tinymce.init({
            selector: "#" + obj.attr('id'),
            toolbar: "bold italic | alignleft aligncenter alignright alignjustify | undo redo",
            menu: [],
            height: opts.height
        });
    }
}

function setStakeholderSearch(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.search');
    } else {
        obj = $('.search');
    }

    obj.stakeholderSearch();
}

function setPhotoCont(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.photo-cont');
    } else {
        obj = $('.photo-cont');
    }

    obj.photoCont();
}

function setDatePickers(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.datepicker');
    } else {
        obj = $('.datepicker');
    }

    obj.attr('placeholder', '[Select date here]');

    if (obj.length > 0) {

        setPickers(obj, {
            format: 'MM/DD/YYYY'
        });
    }
}

function setTimePickers(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.timepicker');
    } else {
        obj = $('.timepicker');
    }

    obj.attr('placeholder', '[Select time here]');

    if (obj.length > 0) {

        setPickers(obj, {
            format: 'h:mm a',
            pickDate: false
        });
    }
}

function setDateTimePickers(cont, opts) {
    var obj;

    if (cont) {
        obj = cont.find('.datetimepicker');
    } else {
        obj = $('.datetimepicker');
    }

    obj.attr('placeholder', '[Select date/time here]');

    setPickers(obj);
}

function setPickers(_obj, opts) {

    _obj.each(function (i, v) {
        var obj = $(v);
        obj
            .addClass('datetimepicker-input')
            .attr('data-toggle', 'datetimepicker')
            .attr('data-target', '#' + obj.attr('id'));

        var dt = new Date();
        var origValue = obj.val();
        if (origValue != '') {

            if (obj.hasClass('datepicker')) {
                var tmp = moment(origValue).toDate();
                dt.setFullYear(tmp.getFullYear());
                dt.setMonth(tmp.getMonth());
                dt.setDate(tmp.getDate());
            } else {
                dt = moment(obj.val());
            }
        }

        var align = obj.css('text-align');
        var valign = obj.attr('data-vertical-orientation');

        if (opts) {
            opts.date = dt;
            opts.widgetPositioning = {
                horizontal: align == 'right' ? 'right' : 'left',
                vertical: valign ? valign : 'bottom'
            };

            obj.datetimepicker(opts);
        } else {
            obj.datetimepicker({
                date: dt,
                widgetPositioning: {
                    horizontal: align == 'right' ? 'right' : 'left',
                    vertical: valign ? valign : 'bottom'
                }
            });
        }

        obj.val(origValue);
    });
}

function setDateCollection(cont) {
    var obj;
    var objDisp;

    if (cont) {
        obj = cont.find('.date-collection');
        objDisp = cont.find('.date-collection-display');
    } else {
        obj = $('.date-collection');
        objDisp = $('.date-collection-display');
    }

    obj.dateCollection();
    objDisp.dateCollection({
        readonly: true
    });
}

var ranges = {
    'Today': [moment(), moment()],
    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
    //'This Week': [moment().subtract(moment().day(), 'days'), moment().add(6 - moment().day(), 'days')],
    //'Last 3 Months': [moment().subtract(2, 'months'), moment()],
    //'Last 6 Months': [moment().subtract(5, 'months'), moment()],
    'This Month': [moment().startOf('month'), moment().endOf('month')],
    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')],
    'This Year': [moment('1/1/' + moment().year()), moment('12/31/' + moment().year())],
    'Last Year': [moment('1/1/' + (moment().year() - 1)), moment('12/31/' + (moment().year() - 1))],
};

//function setDateRangePickers(cont) {
//    var obj;

//    if (cont) {
//        obj = cont.find('.daterangepicker-input');
//    } else {
//        obj = $('.daterangepicker-input');
//    }

//    obj.daterangepicker({
//        ranges: ranges
//    });
//}

function setPeriod(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.period-cont');
    } else {
        obj = $('.period-cont');
    }

    var callback = obj.attr('event-change');

    var start = moment(obj.attr('data-start-date'));
    var end = moment(obj.attr('data-end-date'));

    var minDate = obj.attr('data-min-date');
    var maxDate = obj.attr('data-max-date');

    var cb = function (start, end, isInit) {
        obj.attr('data-start-date', start.format('MM/DD/YYYY'));
        obj.attr('data-end-date', end.format('MM/DD/YYYY'));

        obj.find('span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));

        if (isInit != true) {
            window[callback](start.format('MM-DD-YYYY'), end.format('MM-DD-YYYY'), this);
        }
    };

    var opts = {
        "buttonClasses": "button button-rounded button-mini nomargin",
        "applyClass": "button-color",
        "cancelClass": "button-light",
        //showDropdowns: true,
        minDate: '1/1/2000',
        maxDate: '12/31/2030',
        alwaysShowCalendars: true,
        startDate: start,
        endDate: end,
        ranges: ranges
    };

    if (minDate) opts.minDate = minDate;
    if (maxDate) opts.maxDate = maxDate;

    obj.children('div').first().daterangepicker(opts, cb);

    cb(start, end, true);
}

function setSwitches(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.bt-switch');
    } else {
        obj = $('.bt-switch');
    }

    obj.bootstrapSwitch();
}

function setSelectAll(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.select-all');
    } else {
        obj = $('.select-all');
    }

    obj.selectAll();
}

function setTables(cont, multiSelect, selectionEvent, sortEvent, doneEvent, rowBindEvent, dataAndColumnsModel) {
    var _obj;

    if (cont) {
        _obj = cont.find('.table').not('.static-table').not('.skiptable');
    } else {
        _obj = $('.table').not('.static-table').not('.skiptable');
    }

    if (_obj.length == 0) return;

    _obj.each(function () {
        var obj = $(this);

        multiSelect = multiSelect || obj.hasClass('multi-select');

        var useEachInsteadOfEvery = obj.hasClass('use-each');
        var tableBindCallback = obj.attr('event-tablebind');
        var doneCallback = obj.attr('event-settables-done');
        var selectionCallback = obj.attr('event-selection');
        var paging = true;
        var autoWidth = true;

        if (obj.attr('data-paging')) {
            paging = JSON.parse(obj.attr('data-paging'));
        }

        if (obj.attr('data-autowidth')) {
            autoWidth = JSON.parse(obj.attr('data-autowidth'));
        }

        var ordering = true;

        if (obj.attr('data-ordering')) {
            ordering = JSON.parse(obj.attr('data-ordering'));
        }

        var stateSave = true;
        if (obj.attr('data-statesave')) {
            stateSave = JSON.parse(obj.attr('data-statesave'));
        }

        var opts = {
            "paging": paging,
            "searching": true,
            "ordering": ordering,
            "orderCellsTop": true,
            language: {
                paginate: {
                    next: '<i class="fa fa-angle-double-right">',
                    previous: '<i class="fa fa-angle-double-left">'
                }
            },
            "bAutoWidth": autoWidth,
            "bStateSave": stateSave,
            "bFilter": false,
            "fnStateSave": function (oSettings, oData) {
                sessionStorage.setItem('DataTables', JSON.stringify(oData));                
            },
            "fnStateLoad": function (oSettings) {
                return JSON.parse(sessionStorage.getItem('DataTables'));
            },
            "lengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
            "dom": (paging ? '<"row"<"col-sm-6"i><"col-sm-6 text-right"l>>' : '<"row"<"col-sm-6"i>>') + '<"info-text bg-warning hidden">rt<"bottom"p><"clear">'
        };

        if (rowBindEvent != null) {
            opts.fnCreatedRow = rowBindEvent;
        }

        if (dataAndColumnsModel != null) {
            opts.data = dataAndColumnsModel.data;
            opts.columns = dataAndColumnsModel.columns;
        }

        var dom = obj.attr('dom');
        if (dom) {
            opts.dom = dom;
        }

        obj.find('.tr-filters').find('th').not('.nofilter').each(function () {
            var th = $(this);
            if (th.hasClass('dropdown')) {
                th.html('<select class="form-control noselect2"><option value="">[All]</option></select>');
            } else {
                th.html('<input type="text" class="col-filter" placeholder="[search]" /><i class="fa fa-search"></i>');
            }            
        });

        obj.DataTable().destroy();

        moment('MM/DD/YYYY');
        moment('MM/DD/YYYY h:mm a');

        var table = obj.DataTable(opts);
        var infoText = obj.siblings('.info-text');

        obj.on('order.dt', function () {
            var order = table.order();

            infoText.addClass('hidden');

            if (multiSelect && order.length > 0) {
                if (order[0][0] == '0') {
                    var dir = order[0][1] == 'asc' ? 'unselected -> selected' : 'selected -> unselected';
                    infoText.html('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i>&nbsp;Table is sorted by selection (column 0, ' + dir + ')');
                    infoText.removeClass('hidden');
                }
            }

            if (sortEvent) {
                sortEvent(table, order);
            }
        });

        var bind = function () {
            obj.list({ nodes: table.rows().nodes() });

            if (tableBindCallback) {
                window[tableBindCallback](obj, table);
            }
        };

        if (useEachInsteadOfEvery) {
            table.columns().each(function (v, i) {
                var column = this;

                var input = $(column.header()).closest('table').find('.tr-filters th').eq(i).find('input');
                input.on('keyup change', function () {
                    column
                        .search(this.value)
                        .draw();
                });

                var select = $(column.header()).closest('table').find('.tr-filters th').eq(i).find('select');
                select.on('change', function () {
                    var val = $.fn.dataTable.util.escapeRegex(
                        $(this).val()
                    );

                    column
                        .search(val ? '^' + val + '$' : '', true, false )
                        .draw();
                });

                if (select.length > 0) {
                    column.data().unique().sort().each(function (d, j) {
                        var val = $.fn.dataTable.util.escapeRegex(d);
                        if (column.search() === "^" + val + "$") {
                            select.append(
                                '<option value="' + d + '" selected="selected">' + d + "</option>"
                            );
                        } else {
                            select.append('<option value="' + d + '">' + d + "</option>");
                        }
                    });
                }
            });
        } else {
            table.columns().every(function () {
                var column = this;

                var input = $(column.header()).closest('table').find('.tr-filters th').eq(column.index()).find('input');
                input.on('keyup change', function () {
                    column
                        .search(this.value)
                        .draw();
                });

                var select = $(column.header()).closest('table').find('.tr-filters th').eq(column.index()).find('select');
                select.on('change', function () {
                    var val = $.fn.dataTable.util.escapeRegex(
                        $(this).val()
                    );

                    column
                        .search(val ? '^' + val + '$' : '', true, false)
                        .draw();
                });

                if (select.length > 0) {
                    column.data().unique().sort().each(function (d, j) {
                        var val = $.fn.dataTable.util.escapeRegex(d);
                        if (column.search() === "^" + val + "$") {
                            select.append(
                                '<option value="' + d + '" selected="selected">' + d + "</option>"
                            );
                        } else {
                            select.append('<option value="' + d + '">' + d + "</option>");
                        }
                    });
                }
            });
        }

        if (multiSelect) {
            obj.tableSelection({
                table: table,
                selectionCallback: selectionCallback,
                selectionEvent: selectionEvent
            });
        }

        bind();

        obj.find('.tr-filters th').removeClass('sorting_asc sorting_desc');

        setSelect2(cont);

        if (doneCallback) {
            window[doneCallback](table);
        }

        if (doneEvent) {
            doneEvent(table);
        }
    });
}

function setDetails(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.details');
    } else {
        obj = $('.details');
    }

    obj.details();
}

//function setOfficeDepartments(cont) {
//    var obj;

//    if (cont) {
//        obj = cont.find('select[data-department]');
//    } else {
//        obj = $('select[data-department]');
//    }

//    var dept = $(obj.attr('data-department'));
//    var deptValue = obj.attr('data-department-value');

//    obj.change(function () {
//        var dd = $(this);
//        dept.find('option').removeAttr('selected');
//        dept.find('option').hide();
//        dept.find('option[data-officeid="' + dd.val() + '"], option[data-officeid="-1"]').show();
//        dept.val(-1);
//    });

//    obj.change();
//    dept.val(deptValue);

//}

//function setEventSelection(cont) {
//    var obj;

//    if (cont) {
//        obj = cont.find('.events-selection');
//    } else {
//        obj = $('.events-selection');
//    }

//    var events = $('#Events')

//    var getValue = function () {
//        var res = '';

//        obj.find('input[type="checkbox"]').each(function (i, v) {
//            var cb = $(v);
//            if (cb.prop('checked')) {
//                res += (res == '' ? '' : ',') + i.toString();
//            }
//        });

//        events.val(res);
//    };

//    obj.find('input[type="checkbox"]').each(function (i, v) {
//        var cb = $(v);
//        if ($.inArray(i.toString(), events.val().split(',')) >= 0) {
//            cb.prop('checked', true);
//        } else {
//            cb.prop('checked', false);
//        }

//        cb.click(function () {
//            getValue();
//        });
//    });
//}

//function setScroll(cont) {
//    var obj;

//    if (cont) {
//        obj = cont.find('.scroll');
//    } else {
//        obj = $('.scroll');
//    }

//    if (obj.length > 0) {
//        if (obj.attr('data-scroll-reltowinheight') != undefined) {
//            obj.height($(window).height() * 0.7);
//        }

//        obj.mCustomScrollbar({
//            advanced: { autoScrollOnFocus: false },
//            scrollInertia: 0
//        });
//    }
//}

function setEffectivityCont(cont) {
    var obj;

    if (cont) {
        obj = cont.find('.effectivity-field');
    } else {
        obj = $('.effectivity-field');
    }
    
    var hf = obj.find('.effectivity-field-hf');
        
    obj.click(function () {
        var v = obj.attr('data-value');
        modalSetDate(null, v, function (dt) {
            obj.attr('data-value', dt);
            obj.find('span').html(dt);
            hf.val(dt);
        });
    });
    
}

function setTextInput(cont) {
    var obj;

    if (cont) {
        obj = cont.find('input[type="text"], input[type="number"]');
    } else {
        obj = $('input[type="text"], input[type="number"]');
    }

    obj.each(function (i, v) {
        var tb = $(v);
        var a = tb.attr('maxlength');
        var b = tb.attr('autocomplete');

        var hasMaxLength = a != undefined;
        var hasAutoComplete = b != undefined;

        if (!hasMaxLength) {
            tb.attr('maxlength', 50);
        }

        if (!hasAutoComplete) {
            tb.attr('autocomplete', 'off');
        }
    });
}

function closeModal(modal, e, beforeModalRemove, beforeCancel) {
    var ae = $(document.activeElement);
    if (ae.attr('data-dismiss') || modal.hasClass('confirm-close-event')) {

        if (beforeModalRemove) {
            beforeModalRemove();
        }

        modal.remove();
        return true;
    } else {
        if (beforeCancel) {
            beforeCancel();
        }
        return false;
    }
}

function vdUrl(url) {
    var ret = url;

    if (_virtualDirectory != '') {

        var _url = url.toLowerCase();
        var _vd = _virtualDirectory.toLowerCase();

        if (_url.indexOf(_vd) != 0 && _url.indexOf('http') != 0 && _url.indexOf('www') != 0) {
            ret = _virtualDirectory + url;
        }
    }

    return ret;
}

function setRightNavigation() {
    var cont = $('#right-navigation');
    var pageMenu = $('#page-menu');

    var headerObj = pageMenu.find('nav > ul > li.current');
    
    if (headerObj.length > 0) {
        var title = headerObj.find('> a > div').text();
        cont.find('.widget_group_links_title').html(title);

        var listItems = headerObj.find('> ul > li');
        if (listItems.length > 0) {
            var itemCont = cont.find('.widget_group_links_list');

            listItems.each(function (ii, vv) {
                var li = $(vv);

                if (li[0].firstElementChild.tagName != "HR") {

                    var item = li.clone();
                    if (li.hasClass('current')) {
                        item.addClass('selected-item');
                    }

                    item.addClass('list-group-item list-group-item-action');
                    itemCont.append(item);
                }
            });
            
        }
    }

    var show = cont.find('.widget_group_links .list-group-item-action').length > 0;

    if (!show) {
        cont.remove();
    }
}

function setListCommandsNodes(nodes, callback) {
    for (var i = 0; i < nodes.length; i++) {
        var tr = $(nodes[i]);
        var cont = tr.find('.list-commands-cont');
        callback(tr, cont);
    }
};

function addLoadingTextToCont(cont, text, dontEmpty) {
    var div = $('<div />').addClass('loadingtext');
    div.append('<img src="' + vdUrl('/Assets/images/loading.gif') + '" alt=""/>');
    div.append('<span>&nbsp;' + text + '</span>');
    if (dontEmpty) {
        cont.empty().append(div);
    } else {
        cont.prepend(div);
    }    
}