(function ($) {
    var _options = {
        opt1CountryText: '[Select Country]',
        opt1ProvinceText: '[Select Province]',
        opt1CityText: '[Select City]',
        opt1BrgyText: '[Select Barangay]',
    };

    var methods = {
        init: function (options) {
            var opts = $.extend(true, {}, _options, options);

            var url_list = vdUrl('/api/address/list/{type}/{parentId}');

            return $(this).each(function () {
                var obj = $(this);

                if (obj.hasClass('search-mode')) {
                    $.extend(opts, {
                        opt1CountryText: '[All Countries]',
                        opt1ProvinceText: '[All Provinces]',
                        opt1CityText: '[All Cities/Municipalities]',
                        opt1BrgyText: '[All Barangays]',
                    });
                }

                var country = obj.find('.select-country');
                var province = obj.find('.select-province');
                var city = obj.find('.select-city');
                var brgy = obj.find('.select-brgy');
                var address = obj.find('.tb-address');
                var postalcode = obj.find('.tb-postalcode');

                country.select2();
                province.select2();
                city.select2();
                brgy.select2();

                country.change(function () {
                    fillProvince(country.val());
                });

                province.change(function () {
                    fillCity(province.val());
                });

                city.change(function () {
                    fillBrgy(city.val());
                });

                var fillSelect = function (select, data, textname, opt1) {
                    var value = parseInt(select.attr('data-value'));

                    select.empty();
                    var opt = $('<option />');
                    opt.val(-1);
                    opt.html(opt1);
                    select.append(opt);

                    data.forEach(function (d) {
                        var opt = $('<option />');
                        opt.val(d.Id);
                        opt.html(d[textname]);
                        select.append(opt);
                    });

                    preSelectItem(select);
                    select.trigger('change');
                };

                var fillProvince = function (countryId) {
                    var url = url_list
                        .replace('{type}', 2)
                        .replace('{parentId}', countryId);

                    $.get(url, function (res) {
                        if (res.IsSuccessful) {
                            fillSelect(province, res.Data, 'Province', opts.opt1ProvinceText);
                        }
                    })
                };

                var fillCity = function (provinceId) {
                    var url = url_list
                        .replace('{type}', 3)
                        .replace('{parentId}', provinceId);

                    $.get(url, function (res) {
                        if (res.IsSuccessful) {
                            fillSelect(city, res.Data, 'City', opts.opt1CityText);
                        }
                    })
                };

                var fillBrgy = function (cityId) {
                    var url = url_list
                        .replace('{type}', 5)
                        .replace('{parentId}', cityId);

                    $.get(url, function (res) {
                        if (res.IsSuccessful) {
                            fillSelect(brgy, res.Data, 'Brgy', opts.opt1BrgyText);
                        }
                    })
                };

                var preSelectItem = function (select) {
                    var v = parseInt(select.attr('data-value'));
                    if (select.find('option[value="' + v + '"]').length > 0) {
                        select.val(v);
                    } else {
                        select.val(-1);
                    }
                    select.change();
                };

                preSelectItem(country);
            });
        }
    };

    $.fn.statAddress = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.statAddress');
        }

    };

})(jQuery);