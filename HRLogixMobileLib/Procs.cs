using GeoAuth_Location;
using HRLogixMobileLib.Models;
using System;
using System.Collections.Generic;

namespace HRLogixMobileLib
{
    public static class Procs
    {
        public static bool IsJson(string value)
        {
            bool ret = false;

            if (!string.IsNullOrEmpty(value))
            {
                string s = value.Trim();
                ret = s.StartsWith("{") || s.StartsWith("[");
            }

            return ret;
        }

        public static string GetLocationString(double latitude, double longitude)
        {
            return string.Format("{0}, {1}", latitude, longitude);
        }
    }
}