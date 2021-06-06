function PA_applyPeriod(start, end) {
    var pa = $('.period-attendance');
    var contSelector = pa.attr('data-contselector');
    var cont = contSelector == '' ? pa.parent() : pa.closest(contSelector);

    var url = url_pa;

    if (start != null) {
        url += (url.indexOf('?') >= 0 ? '&' : '?') + 'startDate=' + start + '&endDate=' + end;
    }

    url += (url.indexOf('?') >= 0 ? '&' : '?') + 'contSelector=' + contSelector;

    url = $('<span />').html(url).text();

    loadingUI(cont, true);
    cont.load(vdUrl(url), function (res) {
        setPeriod();
        setPA();
        loadingUI(cont, false);
    });
}

function setPA() {
    var green = '#f3b04e';
    var blue = '#4299CF';

    var config = {
        type: 'line',
        data: {
            labels: JSON.parse($('#pa-labels').val()),
            datasets: [{
                label: "Hours Worked",
                fill: false,
                backgroundColor: blue,
                borderColor: blue,
                data: JSON.parse($('#pa-dshw').val())
            },
            {
                label: "Work Hours",
                backgroundColor: green,
                borderColor: green,
                data: JSON.parse($('#pa-dswh').val()),
                fill: false
            }]
        },
        options: {
            responsive: true,
            tooltips: {
                mode: 'index',
                intersect: false,
                backgroundColor: 'rgba(0, 0, 0, 0.4)'
            },
            hover: {
                mode: 'nearest',
                intersect: true
            },
            scales: {
                xAxes: [{
                    display: true
                }],
                yAxes: [{
                    display: true
                }]
            }
        }
    };

    var ctx = document.getElementById("chart-0").getContext("2d");
    new Chart(ctx, config);
}