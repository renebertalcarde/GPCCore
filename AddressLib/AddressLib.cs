using System.Linq;

namespace AddressLib
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
}
