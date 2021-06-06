(function ($) {

    var methods = {
        init: function () {
            return $(this).each(function () {
                var obj = $(this);
                var opts = {
                    parentInverse: obj.hasClass('select-all-parent-inverse'),
                    dataSourceSelector: obj.attr('data-source'),
                    selectionSelector: obj.attr('data-selectionselector')
                };

                var dataSource = $(opts.dataSourceSelector);
                var dataSourceText = $(opts.dataSourceSelector + '-Text');
                var cbs = obj.find('[type="checkbox"]');

                var updateChildren = function (cb) {
                    var value = cb.prop('checked');
                    var children = cb.closest('li').find('li [type="checkbox"]');

                    children.prop('checked', value);                    
                };

                var getSelection = function (getText) {
                    var selector = opts.selectionSelector == undefined ? '[type="checkbox"]:checked' : opts.selectionSelector;
                    var selectedCBs = obj.find(selector);
                    var v = '';

                    selectedCBs.each(function () {
                        var cb = $(this);
                        v += (v == '' ? '' : ',') + (getText ? cb.attr('data-text') : cb.val());
                    });

                    return v;
                };

                var saveData = function () {
                    var v = getSelection(false);
                    var t = getSelection(true);

                    obj.attr('data-op-selection', v);
                    obj.attr('data-op-selectiontext', t);

                    if (dataSource.length > 0) {                        
                        dataSource.val(v);
                    }

                    if (dataSourceText.length > 0) {
                        dataSourceText.val(t);
                    }
                };

                var updateParents = function (cb) {
                    var siblings = cb.closest('ul').children('li').children('div');
                    var hasSelection = siblings.children('[type="checkbox"]:checked').length > 0;
                    var hasUnselected = siblings.children('[type="checkbox"]:checked').length < siblings.children('[type="checkbox"]').length;

                    cb.closest('li').parents('li').each(function (i, li) {
                        var cb = $(li).children('div').children('[type="checkbox"]');

                        if (cb.length > 0) {
                            if (opts.parentInverse) {
                                cb.prop('checked', !hasUnselected);
                            } else {
                                cb.prop('checked', hasSelection);
                            }
                            
                            updateParents(cb);
                        }                        
                    });
                };

                cbs.click(function () {
                    var cb = $(this);       
                    
                    updateChildren(cb);
                    updateParents(cb);
                    saveData();

                    if (cb.closest('li').attr('data-updatedcallback')) {
                        var updatedCallback = cb.closest('li').attr('data-updatedcallback');
                        if (window[updatedCallback]) {
                            window[updatedCallback]();
                        }
                    }
                })

                var lastCB = cbs.last();
                updateChildren(lastCB);
                updateParents(lastCB);
            });
        }
    };

    $.fn.selectAll = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.selectAll');
        }

    };

})(jQuery);