using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Collections.Generic;
using GeoAuth_Location;

namespace HRLogixMobileLib.Models
{
    public class tblGeoAuth_Area
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string AreaName { get; set; }

        public string Description { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        [Display(Name = "Radius (km)")]
        [DisplayFormat(DataFormatString = "{0: #,##0.##}")]
        public double Radius { get; set; }

        public GeoAuth_CircularArea CircularArea
        {
            get
            {
                return new GeoAuth_CircularArea(Id, AreaName, Description, Latitude, Longitude, Radius * 1000);
            }
        }

        [Display(Name = "Map Location")]
        public string GMaps_LatLng
        {
            get
            {
                return HRLogixMobileLib.Procs.GetLocationString(Latitude, Longitude);
            }

            set
            {
                double[] tmp = GMaps.parseLatLng(value);
                Latitude = tmp[0];
                Longitude = tmp[1];
            }
        }
    }
}