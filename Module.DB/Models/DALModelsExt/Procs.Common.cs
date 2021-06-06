using coreLib.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Module.DB.Procs
{
    
    public static class Common
    {
        private static cachedTables _Global_CachedTables;

        public static cachedTables Global_CachedTables
        {
            get
            {
                if (_Global_CachedTables == null)
                {
                    return new cachedTables();
                }
                else
                {
                    return _Global_CachedTables;
                }
            }
            set
            {
                _Global_CachedTables = value;
            }
        }


        public static List<PeriodModel> GetPeriods(string v, string e = null)
        {
            List<PeriodModel> ret = new List<PeriodModel>();

            if (!string.IsNullOrEmpty(v))
            {
                foreach (string s in v.Split(','))
                {
                    var dd = s.Split('-');
                    PeriodModel pm = new PeriodModel(dd.First(), dd.Last());
                    ret.Add(pm);
                }
            }

            if (!string.IsNullOrEmpty(e))
            {
                List<PeriodModel> exs = new List<PeriodModel>();
                foreach (string s in e.Split(','))
                {
                    var dd = s.Split('-');
                    PeriodModel pm = new PeriodModel(dd.First(), dd.Last());
                    exs.Add(pm);
                }

                foreach (PeriodModel ex in exs)
                {
                    ret = _splice(ret, ex);
                }
            }

            return ret;
        }

        private static List<PeriodModel> _splice(List<PeriodModel> src, PeriodModel ex)
        {
            List<PeriodModel> ret = new List<PeriodModel>();
            foreach (PeriodModel pm in src)
            {
                ret.AddRange(pm.Splice(ex));
            }
            return ret;
        }

        public static string getStatAddress(string address, int? brgyId, int? postalCode)
        {
            string ret = "";
            if (!string.IsNullOrEmpty(address)) ret += address;

            _brgy brgy = Global_CachedTables.ADDR.Brgies.Where(x => x.Id == brgyId).SingleOrDefault();
            if (brgy != null)
            {
                ret += (string.IsNullOrEmpty(ret) ? "" : ", ") + brgy.Brgy;

                vwCity vwCity = Global_CachedTables.ADDR.vwCities.Where(x => x.Id == brgy.CityId).SingleOrDefault();
                if (vwCity != null)
                {
                    ret += ", " + vwCity.City + (!vwCity.IsCity || vwCity.City.Trim().ToLower().EndsWith("city") ? "" : " City");

                    if (!vwCity.IsCity)
                    {
                        ret += ", " + vwCity.Province;
                    }

                    ret += ", " + vwCity.Country;
                }
            }

            if (postalCode != null)
            {
                ret += (string.IsNullOrEmpty(ret) ? "" : ", ") + postalCode.Value.ToString();
            }

            return ret;
        }

        public static string getAddress(string address, int? countryId, int? postalCode)
        {
            _country country = getCountry(countryId ?? -1);
            string strCountry = country == null ? "" : country.Country;

            return coreLib.Procs.getAddress(address, strCountry, postalCode);
        }

        public static _country getCountry(int id)
        {
            return Global_CachedTables.ADDR.Countries.Where(x => x.Id == id).SingleOrDefault();
        }

        public static string getUser(string userId)
        {
            string ret = "";

            using (dalDataContext context = new dalDataContext())
            {
                var tmp = context.tblStakeholders.Where(x => x.UserId == userId).SingleOrDefault();
                if (tmp == null)
                {
                    var tmp2 = Account.GetUser(userId);
                    if (tmp2 != null)
                    {
                        ret = "[" + tmp2.UserName + "]";
                    }
                }
                else
                {
                    ret = tmp.FullName_FN;
                }
            }

            return ret;
        }


        public static string LeavePeriod(DateTime StartDate, DateTime EndDate, bool StartDate_IsHalfDay, bool EndDate_IsHalfDay, bool IncludeRestDays)
        {
            string ret = "";

            if (StartDate.Date == EndDate.Date)
            {
                ret = StartDate.ToString("MM/dd/yyyy");

                if (StartDate_IsHalfDay)
                {
                    ret += " [Half Day]";
                }
            }
            else
            {
                ret = string.Format("{0}{1} - {2}{3}",
                    StartDate.ToString("MM/dd/yyyy"),
                    StartDate_IsHalfDay ? " [Half Day]" : "",
                    EndDate.ToString("MM/dd/yyyy"),
                    EndDate_IsHalfDay ? " [Half Day]" : ""
                    );
            }

            ret += IncludeRestDays ? " [Including Rest Days]" : "";

            return ret;
        }

        public static string PeriodText(DateTime StartDate, DateTime EndDate)
        {
            string ret = "";

            if (StartDate.Date == EndDate.Date)
            {
                ret = StartDate.ToString("MM/dd/yyyy");
            }
            else
            {
                ret = string.Format("{0} - {1}",
                    StartDate.ToString("MM/dd/yyyy"),
                    EndDate.ToString("MM/dd/yyyy")
                    );
            }

            return ret;
        }
    }

    public static class Account
    {
        public static AspNetUser GetUser(string id = "", string email = "", string userName = "")
        {
            using (dalDataContext context = new dalDataContext())
            {
                return context.AspNetUsers.Where(x => x.Id == id || x.Email == email || x.UserName == userName).SingleOrDefault();
            }
        }

        public static void DisableAccount(string id, bool disable)
        {
            using (dalDataContext context = new dalDataContext())
            {
                AspNetUser u = context.AspNetUsers.Where(x => x.Id == id).SingleOrDefault();
                if (u != null)
                {
                    u.Disabled = disable;
                    context.SubmitChanges();
                }
            }
        }

        public static bool IsAccountDisabled(string id = "", string email = "", string userName = "")
        {
            bool ret = true;

            AspNetUser u = GetUser(id, email, userName);
            if (u != null)
            {
                ret = u.Disabled == true;
            }

            return ret;
        }
    }
}