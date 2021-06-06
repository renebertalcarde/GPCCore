var slist_src = {
    url: '',
    data: null
};

var slist_datainfo = {
    Take: 0,
    Skip: 0,
    Total: 0,
    ItemsCount: 0
};

var slist_src = {
    url: '',
    data: null
};

var slist_datainfo = {
    Take: 0,
    Skip: 0,
    Total: 0,
    ItemsCount: 0
};

var slist = {
    src: null,
    cont: null,
    initialRows: 20,
    triggerLimit: 500,
    loadRow: null,
    event_loading: null,
    table: null,
    itemsFieldName: 'Items',
    _slider: null,
    _tableCont: null,  
    _loadingCont: null,
    _moreDataCont: null,  
    _btnRequestLoad: null,    
    _loadingProgress: null,
    _sliderInstance: null,
    init: function () {
        var me = this;
        var body = me.cont;

        var tmp = $('#SListTemplate_Loading').html();
        var a = $(tmp);
        body.prepend(a);

        var tmp2 = $('#SListTemplate').html();
        var b = $(tmp2);
        body.prepend(b);

        me.table = body.find('table').eq(0);
        me._tableCont = me.table.closest('.table-responsive');
        me._loadingCont = body.find('.slist-loading');
        me._moreDataCont = body.find('.more-data-cont');
        me._slider = me._moreDataCont.find('.reqload-value');
        me._btnRequestLoad = me._moreDataCont.find('.btn-reqload');
        me._loadingProgress = me._moreDataCont.find('.progress-ui');

        me._setUI();
    },
    _setBtnLabel: function () {
        var me = this;

        var dataInfo = me._getDataInfo();
        var remaining = dataInfo.Total - dataInfo.ItemsCount;

        var n = me._slider.val();
        var txt = n >= remaining ?
            'Click here to load remaining ' + n + ' items' :
            'Click here to load additional ' + n + ' items';

        me._btnRequestLoad.html(txt);
    },
    _setUI: function () {
        var me = this;

        var slider = me._moreDataCont.find('.reqload-value');
        slider.ionRangeSlider({
            min: 0,
            max: me.initialRows,
            from: 0,
            onChange: function (data) {
                me._setBtnLabel();
            }
        });

        me._sliderInstance = slider.data('ionRangeSlider');

        me._btnRequestLoad.unbind().click(function (e) {
            e.preventDefault();
            me._loadMoreData(me._slider.val());
        })

        me._loadingProgress.progressUI([{
            text: ''
        }]);
        me._loadingProgress.hide();

        me._setBtnLabel();        
        me._moreDataCont.hide();
        me._tableCont.hide();
        me._loadingCont.show();

        me._loadMoreData(me.initialRows, function () {
            me._updateUI();
            me._loadingCont.hide();
            me._tableCont.show();
        });

    },
    _updateUI: function () {
        var me = this;
        var dataInfo = me._getDataInfo();

        var isEverything = dataInfo.Total == dataInfo.ItemsCount;
        if (!isEverything) {
            me._moreDataCont.find('p').html('Loaded ' + dataInfo.ItemsCount + ' of ' + dataInfo.Total + ' items');

            var remaining = dataInfo.Total - dataInfo.ItemsCount;
            var initial = dataInfo.Take < remaining ? dataInfo.Take : remaining;
            var slider = me._moreDataCont.find('.reqload-value');

            me._sliderInstance.update({
                max: remaining,
                from: initial
            });

            me._setBtnLabel();
            me._moreDataCont.show();
        } else {
            me._moreDataCont.hide();
        }
    },
    _loadMoreData: function (noOfItems, callback) {
        var me = this;
        var body = me.cont;

        if (me.event_loading) me.event_loading(true);

        loadingUI_Mask(me._moreDataCont, true);

        var dataInfo = me._getDataInfo();

        var _src = $.extend(true, {}, me.src, {
            data: {
                Take: noOfItems,
                Skip: dataInfo.ItemsCount
            }
        });

        $.get(vdUrl(_src.url), _src.data, function (res) {
            if (res.IsSuccessful) {
                var d = res.Data;

                var listCont = me._tableCont;                

                var getValue = function (v) {
                    if (v == null) {
                        return '';
                    } else {
                        return v;
                    }
                };
                
                setTables(listCont, null, null, null, function (table) {

                    var ptotal = res.Data[me.itemsFieldName].length;
                    var pindex = 0;

                    me._loadingProgress.show();

                    var updateprogress = function () {
                        var pval = (pindex / ptotal) * 100;
                        var perc = SITE.Utility.formatNumber(pval, '#,##0') + '%';
                        me._loadingProgress.progressUI('update', perc, 1, perc);
                    };

                    me._loadingCont.hide();
                    me._moreDataCont.show();

                    var exit = function () {
                        var tableParent = me.table.closest('.table-responsive');
                        tableParent.addClass('loading-mask-text');

                        setTimeout(function () {
                            table.draw();
                            initPlugins(listCont);

                            table.rows().nodes().each(function (node) {
                                var tr = $(node);
                                if (tr.hasClass('selected')) {
                                    tr.find('.table-select-item input[type="checkbox"]').prop('checked', true);
                                }
                            });

                            tableParent.removeClass('loading-mask-text');

                            var dataInfo = me._getDataInfo();
                            dataInfo.ItemsCount += ptotal;
                            dataInfo.Take = d.Take;
                            dataInfo.Skip = d.Skip;
                            dataInfo.Total = d.Total;

                            me._putDatInfo(dataInfo);
                            me._updateUI();

                            me._loadingProgress.hide();

                            loadingUI_Mask(me._moreDataCont, false);

                            var scrollpos = $(window).scrollTop();
                            $(document).scrollTop(scrollpos + 1);

                            if (me.event_loading) me.event_loading(false);
                            if (callback) callback();
                        }, 100);
                    };

                    (function next(i) {
                        if (i >= ptotal) {
                            return exit();
                        }

                        var d = res.Data;
                        var v = d[me.itemsFieldName][i];
                        var tr = me.loadRow(v);
                        table.row.add(tr);

                        pindex = ++i;
                        updateprogress();

                        if (pindex >= ptotal) {
                            return exit();
                        }

                        setTimeout(function () {
                            next(i);
                        }, 1);
                    })(0); // initial value of i
                                       
                    
                });
            } else {
                modalMessage(res.Err.split('\n'), 'Load more data', true);
            }
        });
    },
    _putDatInfo: function (data) {
        var me = this;
        me.table.attr('data-info', JSON.stringify(data));
    },
    _getDataInfo: function () {
        var me = this;
        var d = me.table.attr('data-info');

        if (d == undefined) {
            return $.extend({}, slist_datainfo);
        } else {
            var dataInfo = JSON.parse(d);
            return dataInfo;
        }
    }
};