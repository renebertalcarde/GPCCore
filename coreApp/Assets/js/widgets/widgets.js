$(document).ready(function () {

    $('.dashboard-item.onrequest').click(function () {
        var dItem = $(this);
        var url = dItem.attr('data-url');
        var widgetName = dItem.attr('data-widgetname');

        loadingUI(dItem, true);

        var div = $('<div />').addClass('dashboard-item-cont');

        div.hide();
        div.click(function (e) {
            e.stopPropagation();
        });        

        div.load(vdUrl(url), function (res) {

            dItem.empty().append(div);
            initPlugins(div);

            switch (widgetName) {
                case 'pa':
                    setPA();
                    break;
                case 'dam':
                    setDAM();
                    break;
            };

            dItem.find('img').remove();
            div.show();
            
            loadingUI(dItem, false);
        })
    });

});