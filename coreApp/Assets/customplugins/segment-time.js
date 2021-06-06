function setSegmentTime () {

    var segmentType = $('#SegmentType');

    var segment = $('div.segment-time');
    var segmentToggle = segment.find('.segment-toggle a');
    

    var setui = function () {
        if (segment.hasClass('advanced')) {
            segmentToggle.html('Basic&nbsp;&raquo;');
        } else {
            segmentToggle.html('Advanced&nbsp;&raquo;');
        }
    };

    segmentType.change(function () {
        var sel = $(this);
        sel.closest('form').attr('segment-type', sel.val());
    });

    segmentToggle.unbind('click').bind('click', function (e) {
        e.preventDefault();

        segment.toggleClass('advanced');

        setui();
    });

    setui();

    segmentObject.init(segment);

    $('#TimeIn, #TimeOut').blur(function (e) {
        segmentObject.set();
    });
    
}


var segmentObject = {
    ui: null,
    _tIn: null,
    _tInFrom: null,
    _tInFromPrev: null,
    _tInTo: null,
    _tInToNext: null,
    _tOut: null,
    _tOutFrom: null,
    _tOutFromNext: null,
    _tOutTo: null,
    _tOutToNext: null,
    _wdEq: null,
    _whpd: null,
    tIn: null,
    tInFrom: null,
    tInFromPrev: false,
    tInTo: null,
    tInToNext: false,
    tOut: null,
    tOutNext: false,
    tOutFrom: null,
    tOutFromNext: false,
    tOutTo: null,
    tOutToNext: false,
    wdEq: 0,
    workHoursPerDay: 0,
    dayStartTime: new moment('5:00 AM', 'h:mm a')._d,
    dayEndTime: new moment('11:59 PM', 'h:mm a')._d,
    init: function (ui) {
        this.ui = ui;
        this._tIn = ui.find('#TimeIn');
        this._tInFrom = ui.find('#TimeInFrom');
        this._tInFromPrev = ui.find('#TimeInFrom_IsPrev');
        this._tInTo = ui.find('#TimeInTo');
        this._tInToNext = ui.find('#TimeInTo_IsNext');
        this._tOut = ui.find('#TimeOut');
        this._tOutNext = ui.find('#TimeOut_IsNext');
        this._tOutFrom = ui.find('#TimeOutFrom');
        this._tOutFromNext = ui.find('#TimeOutFrom_IsNext');
        this._tOutTo = ui.find('#TimeOutTo');
        this._tOutToNext = ui.find('#TimeOutTo_IsNext');
        this._wdEq = ui.find('#WorkDayEq');
        this._whpd = ui.find('#WorkHoursPerDay');

        this.wdEq = parseFloat(this._wdEq.val());
        this.workHoursPerDay = parseFloat(this._whpd.val());
    },
    get: function () {
        var ui = this.ui;
        this.tIn = new moment(this._tIn.val(), 'h:mm a')._d;
        this.tInFrom = new moment(this._tInFrom.val(), 'h:mm a')._d;
        this.tInFromPrev = this._tInFromPrev.prop('checked');
        this.tInTo = new moment(this._tInTo.val(), 'h:mm a')._d;
        this.tInToNext = this._tInToNext.prop('checked');
        this.tOut = new moment(this._tOut.val(), 'h:mm a')._d;
        this.tOutNext = this._tOutNext.prop('checked');
        this.tOutFrom = new moment(this._tOutFrom.val(), 'h:mm a')._d;
        this.tOutFromNext = this._tOutFromNext.prop('checked');
        this.tOutTo = new moment(this._tOutTo.val(), 'h:mm a')._d;
        this.tOutToNext = this._tOutToNext.prop('checked');
    },
    set: function () {
        this.get();

        if (this.tInFromPrev) {
            this.tInFrom = moment(this.tInFrom).add(-1, 'days')._d;
        }
        
        this.tInTo = new Date(this.tOut.getFullYear(), this.tOut.getMonth(), this.tOut.getDate() + (this.tInToNext ? 1 : 0), this.tOut.getHours(), this.tOut.getMinutes() - 1, this.tOut.getSeconds());

        if (this.tOutNext) {
            this.tOut = moment(this.tOut).add(1, 'days')._d;
        }

        this.tOutFrom = new Date(this.tIn.getFullYear(), this.tIn.getMonth(), this.tIn.getDate() + (this.tOutFromNext ? 1 : 0), this.tIn.getHours(), this.tIn.getMinutes() + 1, this.tIn.getSeconds());

        if (this.tOutToNext) {
            this.tOutTo = moment(this.tOutTo).add(1, 'days')._d;
        }

        var hours = Math.abs(this.tOut - this.tIn) / 3600000;

        this.wdEq = hours / this.workHoursPerDay;

        this.show();
    },
    show: function () {
        var ui = this.ui;

        this._tIn.val(new moment(this.tIn).format('h:mm a').toUpperCase());
        this._tInFrom.val(new moment(this.tInFrom).format('h:mm a').toUpperCase());
        this._tInFromPrev.prop('checked', this.tInFromPrev);
        this._tInTo.val(new moment(this.tInTo).format('h:mm a').toUpperCase());
        this._tInToNext.prop('checked', this.tInToNext);
        this._tOut.val(new moment(this.tOut).format('h:mm a').toUpperCase());
        this._tOutNext.prop('checked', this.tOutNext);
        this._tOutFrom.val(new moment(this.tOutFrom).format('h:mm a').toUpperCase());
        this._tOutFromNext.prop('checked', this.tOutFromNext);
        this._tOutTo.val(new moment(this.tOutTo).format('h:mm a').toUpperCase());
        this._tOutToNext.prop('checked', this.tOutToNext);

        this._wdEq.val(this.wdEq);
    }
};