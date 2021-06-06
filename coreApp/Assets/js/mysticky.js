var stickyTableHeader = {
    table: null,
    newParent: null,
    headerContTemplateId: '',
    headerCont_OffsetTop: 0,
    headerCont_OffsetTop_XS: 0,
    stickyList_OffsetBottom: 0,
    stickyList_PaddingBottom: 100,
    cloneCallback: null,
    stickyCallback: null,
    _thead: null,
    _headerCont: null,
    _headerContTable: null,
    _headerContScroll: null,
    _divTableResponsive: null,
    init: function () {
        var me = this;
        var body = $('body');

        me._thead = me.table.children('thead');
        me._divTableResponsive = me._thead.closest('.table-responsive');

        me._headerCont = $($('#' + me.headerContTemplateId).html());
        me._headerContScroll = me._headerCont.find('.scroll-cont');
        me._headerContTable = me._headerCont.find('table');

        me.table.addClass('mysticky-table');
        me._thead.addClass('mysticky-thead');
        me._headerCont.addClass('mysticky-header-cont');        
        me._headerContTable.addClass('mysticky-header-cont-table');
        me.newParent.addClass('mysticky-parent');
        me._divTableResponsive.addClass('mysticky-table-responsive');
        
        $(window).scroll(function () {
            me.setUI();
        });

        $(window).resize(function () {
            me.setUI();
        });

        me._headerContScroll[0].onscroll = function (e) {
            var x = e.currentTarget.scrollLeft;
            me.scrollLeft_Main(x);
        };

        me._divTableResponsive[0].onscroll = function (e) {
            var x = e.currentTarget.scrollLeft;
            me.scrollLeft_Hdr(x);
        };

        me.setUI();
    },
    scrollLeft_Hdr: function (x) {
        this._headerContScroll[0].scrollLeft = x;
    },
    scrollLeft_Main: function (x) {
        this._divTableResponsive[0].scrollLeft = x;
    },
    setUI: function () {
        var me = this;
        var mobile= onMobileView();
        var table_Top = me.table[0].getBoundingClientRect().top;

        var xsCond = table_Top < me.headerCont_OffsetTop_XS;
        var normalCond = table_Top < me.headerCont_OffsetTop;

        if ((mobile && xsCond) || (!mobile && normalCond)) {
            me.makeSticky(true);
        } else {
            me.makeSticky(false);
        }

        me.makeStickyListBottom();
    },
    makeStickyListBottom: function () {
        var me = this;
        me._divTableResponsive[0].style.height = null;

        //if horizontal scrollbar is visible
        var hasHScrollBar = (me._divTableResponsive[0].scrollWidth - 2) >= me._divTableResponsive[0].clientWidth;

        if (!hasHScrollBar) {
            return;
        }

        var rect = me._divTableResponsive[0].getBoundingClientRect();

        var tblResponsive_Bottom = window.innerHeight - me.stickyList_OffsetBottom;
        var tblResponsive_Height = tblResponsive_Bottom - rect.top;
        
        if (tblResponsive_Height <= (me.table.height() + me.stickyList_PaddingBottom)) {
            me._divTableResponsive[0].style.height = tblResponsive_Height + 'px';
        }
    },
    syncWidths: function () {
        var me = this;
        var tr = me.table.find('tbody tr').first();
        if (tr) {
            tr.find('td').each(function (i, v) {
                var w = v.getBoundingClientRect().width;
                var ths = me._headerContTable.find('tr th');
                if (ths.length > 0) {
                    me._headerContTable.find('tr th').eq(i)[0].style.width = w + 'px';
                }
                
            });
        }
    },
    makeSticky: function (flag) {
        var me = this;

        var obj = me._thead;
        var w;

        if (flag) {
            me._headerCont.addClass('mysticky');
            if (me.newParent.children('.mysticky-header-cont').length == 0) {

                var _obj = obj.clone(true);
                
                me._headerContTable.empty().append(_obj);
                me.newParent.prepend(me._headerCont);
            }
            
            w = me.table.width();
            me._headerContTable[0].style.width = w + 'px';
            me.syncWidths();

            var x = me._divTableResponsive[0].scrollLeft;
            me.scrollLeft_Hdr(x);

        } else {

            me.newParent.children('.mysticky-header-cont').remove();
        }

        if (me.stickyCallback) {
            me.stickyCallback(flag);
        }
    }
};