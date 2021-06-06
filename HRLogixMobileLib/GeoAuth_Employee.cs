using GeoAuth_Location;
using HRLogixMobileLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRLogixMobileLib
{
    public class GeoAuth_Employee
    {
        public GeoAuth Auth { get; set; }        
        public string GeoAuthDeviceRef { get; set; }
        public string LocalDeviceId { get; set; }

        bool AllowGeoAuthentication;

        GeoAuth_CircularArea[] CircularAreas;
        GeoAuth_Location.GeoAuth_Location Location;

        public bool CanLog
        {
            get
            {
                return AllowGeoAuthentication && Auth.InRange && RegisteredDevice;
            }
        }

        public bool RegisteredDevice
        {
            get
            {
                bool ret = false;

                if (!string.IsNullOrEmpty(GeoAuthDeviceRef) && !string.IsNullOrEmpty(LocalDeviceId))
                {
                    ret = GeoAuthDeviceRef == LocalDeviceId;
                }

                return ret;
            }
        }

        public void Update(double Latitude, double Longitude)
        {
            Location = new GeoAuth_Location.GeoAuth_Location(Latitude, Longitude);
            Auth = new GeoAuth(CircularAreas, Location);
        }

        public GeoAuth_Employee(bool AllowGeoAuthentication, string GeoAuthDeviceRef, string LocalDeviceId, double Latitude, double Longitude, List<tblGeoAuth_Area> Areas)
        {
            this.AllowGeoAuthentication = AllowGeoAuthentication;
            this.GeoAuthDeviceRef = GeoAuthDeviceRef;
            this.LocalDeviceId = LocalDeviceId;

            CircularAreas = Areas.Select(x => x.CircularArea).ToArray();
            Update(Latitude, Longitude);            
        }
    }
}