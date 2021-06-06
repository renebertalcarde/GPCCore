(function ($) {
    var _options = {        
    };

    var opts;

    var methods = {
        init: function (options) {
            return $(this).each(function () {                
                opts = $.extend(true, {}, _options, options);

                var obj = $(this);
                var showInModal = obj.hasClass('showinmodal');

                if (showInModal) {
                    var trigger = obj.find('.modal-trigger');
                    var dataSourceSelector = trigger.attr('data-source');
                    var templateName = trigger.attr('data-templatename');

                    var setTriggerText = function () {
                        var s = 'No item selected';
                        var a = trigger.attr('data-selectedtexts');
                        if (a != '') {
                            var b = a.split(',');
                            var n = b.length;
                            if (n == 1) {
                                s = b[0];
                            } else if (n > 1) {
                                var r = n - 1;
                                var p = r > 1 ? 's' : '';

                                s = b[0] + ' and ' + r + ' other item' + p;
                            }
                        }

                        trigger.html(s);
                    };

                    var loadValues = function (cont) {
                        var DS = $(dataSourceSelector);
                        var s = DS.val();

                        cont.find('input[type="checkbox"]').prop('checked', false);
                        if (s == '') return;

                        var data = s.split(',');
                        data.forEach(function (v) {
                            cont.find('input[value="' + v + '"]').prop('checked', true);
                        });
                    };

                    trigger.click(function (e) {
                        e.preventDefault();

                        modalEmpty('Select Items', function (modal, recallback) {
                            modal.find('.modal-dialog').addClass('modal-xl');

                            var div = $('<div />').css('min-height', '100px');
                            modal.find('.modal-body').append(div);

                            loadingUI(div, true);

                            var t = $('#' + templateName).html();
                            div.empty().append($(t));

                            setSelectAll(div);
                            loadValues(div);

                            loadingUI(div, false);

                            var btnOk = modal.find('.modal-footer .btn-ok');
                            
                            btnOk.click(function (e) {
                                e.preventDefault();

                                var selectAll = modal.find('.select-all');
                                var v = selectAll.attr('data-op-selection');
                                var t = selectAll.attr('data-op-selectiontext');

                                $(dataSourceSelector).val(v);

                                trigger.attr('data-selectedtexts', t);                                
                                setTriggerText();

                                modal.addClass('confirm-close-event').modal('hide');
                            });

                            if (recallback) recallback();
                        })
                    });

                    setTriggerText();
                }
            });
        }
    };

    $.fn.multiselectUI = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.multiselectUI');
        }

    };

})(jQuery);