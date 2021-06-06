using Module.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace coreApp
{
    public static class SelectItems
    {

        public static List<SelectListItem> getCountries()
        {
            List<SelectListItem> _countries = new List<SelectListItem>() { new SelectListItem() { Text = "[Select country]", Value = "-1", Selected = true } };
            _countries.AddRange(Cache.Get_Tables().ADDR.Countries.Select(x => new SelectListItem() { Text = x.Country, Value = x.Id.ToString() }));

            return _countries;
        }


        public static List<SelectListItem> getMonthDays()
        {
            List<SelectListItem> _days = new List<SelectListItem>() { new SelectListItem() { Text = "[Select day]", Value = "-1", Selected = true } };

            for (int i = 1; i <= 31; i++)
            {
                _days.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }

            return _days;
        }

        public static List<SelectListItem> getMonths()
        {
            List<SelectListItem> _months = new List<SelectListItem>() { new SelectListItem() { Text = "[Select month]", Value = "-1", Selected = true } };

            for (int i = 1; i <= 12; i++)
            {
                _months.Add(new SelectListItem() { Text = new DateTime(1900, i, 1).ToString("MMMM"), Value = i.ToString() });
            }

            return _months;
        }

        public static List<SelectListItem> getYears(int? maxYear = null, int? minYear = null, bool showEmptyItem = true, bool showPresent = false)
        {
            List<SelectListItem> _years = new List<SelectListItem>();

            if (showEmptyItem)
            {
                _years.Add(new SelectListItem() { Text = "[Select year]", Value = "-1", Selected = true });
            }

            if (showPresent)
            {
                _years.Add(new SelectListItem() { Text = "Present", Value = "-2" });
            }

            int max = maxYear == null ? DateTime.Today.Year : maxYear.Value;
            int min = minYear == null ? 1940 : minYear.Value;

            for (int i = max; i >= min; i--)
            {
                _years.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }

            return _years;
        }

        public static List<SelectListItem> getGender()
        {
            List<SelectListItem> _gender = new List<SelectListItem>() { new SelectListItem() { Text = "[Select gender]", Value = "-1", Selected = true } };

            int i = 0;
            foreach (string s in System.Enum.GetNames(typeof(Gender)))
            {
                _gender.Add(new SelectListItem() { Text = s, Value = i.ToString() });
                i++;
            }

            return _gender;
        }

        public static List<SelectListItem> getCivilStatus()
        {
            List<SelectListItem> _status = new List<SelectListItem>() { new SelectListItem() { Text = "[Select civil status]", Value = "-1", Selected = true } };

            int i = 0;
            foreach (string s in System.Enum.GetNames(typeof(CivilStatus)))
            {
                _status.Add(new SelectListItem() { Text = s, Value = i.ToString() });
                i++;
            }

            return _status;
        }

        public static List<SelectListItem> getActive(string defaultText = "[Select active status]", bool showAll = false, string defaultValue = "", bool showEmptyItem = true)
        {
            List<SelectListItem> _items = new List<SelectListItem>();

            if (showEmptyItem)
            {
                _items.Add(new SelectListItem() { Text = defaultText == "" ? "" : defaultText, Value = "", Selected = defaultValue == "" });
            }

            _items.Add(new SelectListItem() { Text = "Active", Value = "active" });
            _items.Add(new SelectListItem() { Text = "In-Active", Value = "inactive" });

            return _items;
        }

    }

}
