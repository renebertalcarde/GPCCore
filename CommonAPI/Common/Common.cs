using System.Linq;

namespace CommonAPI
{
    public class AddressData
    {
        public _country[] Countries { get; set; }
        public _region[] Regions { get; set; }
        public _province[] Provinces { get; set; }
        public _mun_city[] MunCities { get; set; }
        public vwCity[] vwCities { get; set; }
        public _brgy[] Brgies { get; set; }
    }

    public static class Procs
    {
        public static void Refresh()
        {
            using (dalDataContext context = new dalDataContext())
            {
                MvcApplication.ADDR.Countries = context._countries.OrderBy(x => x.Country).ToArray();
                MvcApplication.ADDR.Regions = context._regions.OrderBy(x => x.Region).ToArray();
                MvcApplication.ADDR.Provinces = context._provinces.OrderBy(x => x.Province).ToArray();
                MvcApplication.ADDR.MunCities = context._mun_cities.OrderBy(x => x.City).ToArray();
                MvcApplication.ADDR.vwCities = context.vwCities.OrderBy(x => x.City).ToArray();
                MvcApplication.ADDR.Brgies = context._brgies.OrderBy(x => x.Brgy).ToArray();
            }
        }
    }
}