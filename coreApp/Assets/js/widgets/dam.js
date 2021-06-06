
function setDAM() {
    var dt = $('#DAM_Date');

    dt.daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        minYear: 1901,
        maxYear: parseInt(moment().format('YYYY'), 10)
    })
        .on('apply.daterangepicker', function (ev, picker) {
            DAM_applyDate(dt.val());
        });
    
}

function DAM_applyDate(dt) {
    var cont = $('.dashboard-item .dam');
    var url = url_dam;

    if (dt != null) {
        url += (url.indexOf('?') >= 0 ? '&' : '?') + 'dt=' + dt.replace(/\//g, '-');
    }

    url = $('<span />').html(url).text();

    loadingUI(cont, true);
    cont.load(vdUrl(url), function (res) {
        setDAM();
        loadingUI(cont, false);
    });
}

$(document).ready(function () {
    setDAM();
});