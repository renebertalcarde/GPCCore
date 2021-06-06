using coreLib.Objects;
using HRLogixMobileLib;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Module.DB
{
    public class AddressData
    {
        public _country[] Countries { get; set; }
        public _region[] Regions { get; set; }
        public _province[] Provinces { get; set; }
        public _mun_city[] MunCities { get; set; }
        public vwCity[] vwCities { get; set; }
        public _brgy[] Brgies { get; set; }


        public void Refresh()
        {
            List<Task> tasks = new List<Task>();

            string AddressServerUrl = Properties.Settings.Default.AddressServerUrl;
            string url_refresh = AddressServerUrl + (AddressServerUrl.EndsWith("/") ? "" : "/") + "address/refresh";
            string url = AddressServerUrl + (AddressServerUrl.EndsWith("/") ? "" : "/") + "address/list/";
            WebData web = new WebData();

            web.getDataFromUrl(url_refresh);

            string s = web.getDataFromUrl(url + "0");
            queryResult res = Newtonsoft.Json.JsonConvert.DeserializeObject<queryResult>(s);
            Countries = Newtonsoft.Json.JsonConvert.DeserializeObject<_country[]>(res.Data.ToString());

            s = web.getDataFromUrl(url + "1");
            res = Newtonsoft.Json.JsonConvert.DeserializeObject<queryResult>(s);
            Regions = Newtonsoft.Json.JsonConvert.DeserializeObject<_region[]>(res.Data.ToString());

            s = web.getDataFromUrl(url + "2");
            res = Newtonsoft.Json.JsonConvert.DeserializeObject<queryResult>(s);
            Provinces = Newtonsoft.Json.JsonConvert.DeserializeObject<_province[]>(res.Data.ToString());

            s = web.getDataFromUrl(url + "3");
            res = Newtonsoft.Json.JsonConvert.DeserializeObject<queryResult>(s);
            MunCities = Newtonsoft.Json.JsonConvert.DeserializeObject<_mun_city[]>(res.Data.ToString());

            s = web.getDataFromUrl(url + "4");
            res = Newtonsoft.Json.JsonConvert.DeserializeObject<queryResult>(s);
            vwCities = Newtonsoft.Json.JsonConvert.DeserializeObject<vwCity[]>(res.Data.ToString());

            s = web.getDataFromUrl(url + "5");
            res = Newtonsoft.Json.JsonConvert.DeserializeObject<queryResult>(s);
            Brgies = Newtonsoft.Json.JsonConvert.DeserializeObject<_brgy[]>(res.Data.ToString());

        }
    }

    public class AddressModel
    {
        public string Address { get; set; }
        public int BrgyId { get; set; }
        public int? PostalCode { get; set; }
        public string Prefix { get; set; }

        public _brgy Brgy
        {
            get
            {
                return Module.DB.Procs.Common.Global_CachedTables.ADDR.Brgies.Where(x => x.Id == BrgyId).SingleOrDefault();
            }
        }

        public vwCity vwCity
        {
            get
            {
                _brgy brgy = Brgy;
                if (brgy == null)
                {
                    return null;
                }
                else
                {
                    return Module.DB.Procs.Common.Global_CachedTables.ADDR.vwCities.Where(x => x.Id == brgy.CityId).SingleOrDefault();
                }
            }
        }
    }

}
