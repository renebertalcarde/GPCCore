(function ($) {
    var _options = {};

    var methods = {
        init: function (options) {
            
            return this.each(function () {
                var obj = $(this);

                var handle = obj.find('.form-actions-handle');
                var formActions = obj.find('.form-actions');

                handle.click(function (e) {
                    e.stopPropagation();
                    obj.toggleClass('minimized');
                });

                if (!formActions.is(':empty') && formActions[0].innerHTML.trim() != '') {
                    obj.show();
                }
            });
        }
    };

    $.fn.formActions = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.formActions');
        }

    };

})(jQuery);