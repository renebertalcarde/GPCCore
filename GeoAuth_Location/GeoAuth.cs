using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoAuth_Location
{
    public class GeoAuth
    {
        public List<GeoAuth_CircularAreaStatus> Areas { get; set; }
        public GeoAuth_Location Location { get; set; }

        public List<GeoAuth_CircularAreaStatus> InRangeAreas
        {
            get
            {
                return Areas.Where(x => x.InRange).ToList();
            }
        }

        public bool InRange
        {
            get
            {
                return InRangeAreas.Any();
            }
        }

        public GeoAuth_CircularAreaStatus LogArea
        {
            get
            {
                return InRangeAreas.OrderBy(x => x.DistToCenter).ThenBy(x => x.Area.AreaName).FirstOrDefault();
            }
        }

        public GeoAuth(GeoAuth_CircularArea[] CircularAreas, GeoAuth_Location Location)
        {
            this.Location = Location;
            this.Areas = CircularAreas.Select(x => new GeoAuth_CircularAreaStatus(x, Location)).ToList();
        }
    }

    public class GeoAuth_CircularAreaStatus
    {
        public GeoAuth_CircularArea Area { get; set; }
        public GeoAuth_Location Location { get; set; }
        public double DistToCenter { get; set; }
        public double DistToRadius { get; set; }
        public bool InRange { get; set; }

        public GeoAuth_CircularAreaStatus(GeoAuth_CircularArea Area, GeoAuth_Location Location)
        {
            this.Area = Area;
            this.Location = Location;

            double dist;
            InRange = Area.IsWithin(Location, out dist);
            DistToCenter = dist;
            DistToRadius = dist - Area.Radius;
        }
    }

    public class Util
    {
        public double ToRadians(double value)
        {
            return (Math.PI / 180) * value;
        }

        public double GetDistance(GeoAuth_Location loc1, GeoAuth_Location loc2)
        {
            double R = 6371e3;

            double _lat = ToRadians(loc2.Latitude - loc1.Latitude);
            double _lon = ToRadians(loc2.Longitude - loc1.Longitude);

            double a = Math.Sin(_lat / 2) * Math.Sin(_lat / 2) +
                        Math.Cos(loc1.Latitude_Rad) * Math.Cos(loc2.Latitude_Rad) *
                        Math.Sin(_lon / 2) * Math.Sin(_lon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double d = R * c;

            return d;
        }
    }

    public class GeoAuth_Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Latitude_Rad { get; set; }
        public double Longitude_Rad { get; set; }

        public GeoAuth_Location(double Latitude, double Longitude)
        {
            Util util = new Util();

            this.Latitude = Latitude;
            this.Longitude = Longitude;

            this.Latitude_Rad = util.ToRadians(Latitude);
            this.Longitude_Rad = util.ToRadians(Longitude);
        }
    }

    public class GeoAuth_CircularArea
    {
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public string Description { get; set; }
        public GeoAuth_Location Center { get; set; }
        public double Radius { get; set; }

        public GeoAuth_CircularArea(int AreaId, string AreaName, string Description, double Latitude, double Longitude, double Radius)
        {
            this.AreaId = AreaId;
            this.AreaName = AreaName;
            this.Description = Description;
            this.Center = new GeoAuth_Location(Latitude, Longitude);
            this.Radius = Radius;
        }

        public bool IsWithin(GeoAuth_Location loc, out double dist)
        {
            dist = new Util().GetDistance(Center, loc);
            return dist <= Radius;
        }
    }

}