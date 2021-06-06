(function ($) {

    var getParameters = function (obj) {
        var excludeNoCareer = obj.attr('data-exclude-nocareer') == 'True';
        var excludeNoOffice = obj.attr('data-exclude-nooffice') == 'True';
        var includeLocations = obj.attr('data-include-locations') == 'True';
        var noPaging = obj.attr('data-nopaging') == 'True';
        var forScheduling = obj.attr('data-forscheduling') == 'True';
        
        var params = {
            lastName: obj.find('.search-lastname').val(),
            firstName: obj.find('.search-firstname').val(),
            mfoId: -1,
            departmentIds: obj.find('.office-structure').attr('data-selection'),
            positionId: obj.find('.search-positionid').val(),
            groupId: obj.find('.search-groupid').val(),
            employmentType: obj.find('.search-employmenttype').val(),
            active: obj.find('.search-active').val(),
            nodepartment: obj.find('.search-nodepartment').val(),            
            excludeNoCareer: excludeNoCareer,
            excludeNoOffice: excludeNoOffice,
            altSource: obj.find('.altSource').val(),
            noPaging: noPaging,
            forScheduling: forScheduling,
            includeLocations: includeLocations,
            country: -1,
            province: -1,
            city: -1,
            brgy: -1
        };

        if (includeLocations) {
            params.country = obj.find('.select-country').val();
            params.province = obj.find('.select-province').val();
            params.city = obj.find('.select-city').val();
            params.brgy = obj.find('.select-brgy').val();
        }

        var mfo = obj.find('.search-mfoid');
        if (mfo.length > 0) {
            params.mfoId = mfo.val();
        }

        return params;
    };

    var showSearchPanel = function (obj, show, disableShowPanelTrigger) {
        if (disableShowPanelTrigger) return;

        obj.find('.search-toggle-contents').collapse(show ? 'show' : 'hide');
    }

    var methods = {
        init: function () {

            return $(this).each(function () {
                var obj = $(this);

                var dataUrl = obj.attr('data-url');
                var excludeNoCareer = obj.attr('data-exclude-nocareer') == 'True';
                var excludeNoOffice = obj.attr('data-exclude-nooffice') == 'True';
                var includeLocations = obj.attr('data-include-nooffice') == 'True';
                var showSearchBtn = obj.attr('data-show-searchbtn') == 'True';
                var showResult = obj.attr('data-show-result') == 'True';
                var allowSingleSelection = obj.attr('data-allow-single-selection') == 'True';
                var searchCallback = obj.attr('event-search-callback');
                var sortCallback = obj.attr('event-sort-callback');
                var disableShowPanelTrigger = obj.attr('data-disable-showpaneltrigger') == 'True';

                var minimalView = obj.hasClass('minimal-view');
                var multiSelect = obj.hasClass('multi-select');
                var searchStakeholdersSelectedItems = [];

                var searchBtn = obj.find('.btn-search');
                var paramsBtn = obj.find('.params-trigger');

                obj.keyup(function (e) {
                    if (e.which == 13) {
                        searchBtn.click();
                    }
                });

                paramsBtn.click(function (e) {
                    e.preventDefault();
                    obj.toggleClass('more-params');

                    if (!obj.hasClass('more-params')) {
                        obj.find('select.xparams').each(function (i, v) {
                            v.selectedIndex = 0;
                            $(v).change();
                        });
                    }
                });

                searchBtn.click(function (e) {
                    e.preventDefault();

                    if (!showSearchBtn) return;

                    var params;

                    if (!showResult) {                        
                        params = getParameters(obj);
                        if (searchCallback) {
                            window[searchCallback](params);
                        }

                        showSearchPanel(obj, false, disableShowPanelTrigger);
                        return;
                    }
                    
                    var result = obj.find('.result-section');
                    var selectedItems = obj.find('.selectedItems');
                    
                    loadingUI(result, true);

                    params = getParameters(obj);

                    result.load(vdUrl('/HRModule/StakeholderSearch/GetList'), params, function () {
                        result.find('.table').attr('data-url', dataUrl);
                        result.find('.table').attr('data-multiselect', (multiSelect ? 'true' : 'false'));
                        result.find('.table').attr('data-selection', selectedItems.val());

                        if (!allowSingleSelection) {
                            result.find('.table tr').attr('locked', '');
                        }
                        
                        setTables(result, multiSelect,
                            function (selection) {
                                selectedItems.val(selection);
                            },
                            function (table, order) {
                                if (sortCallback) {
                                    window[sortCallback](params, table, order);
                                }
                            },
                            function (table) {
                                if (searchCallback) {
                                    window[searchCallback](params, table);
                                }
                            });


                        showSearchPanel(obj, false, disableShowPanelTrigger);
                        loadingUI(result, false);     

                        adjustFormActions();
                    })
                });
            });
        },
        showSearchPanel: function (show) {
            var obj = $(this);
            var disableShowPanelTrigger = obj.attr('data-disable-showpaneltrigger') == 'True';

            showSearchPanel(obj, show, disableShowPanelTrigger);
        },
        getParameters: function () {
            var obj = $(this);
            return getParameters(obj);
        },
        getSelectedItems: function () {
            var obj = $(this);
            return obj.find('.selectedItems').val();
        }
    };

    $.fn.stakeholderSearch = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.stakeholderSearch');
        }

    };

})(jQuery);
