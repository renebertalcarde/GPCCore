using System.Linq;

namespace GeoAuth_Location
{
    public static class GMaps
    {
        public static string Load_API_Library(string GMaps_APIKey, string GMaps_APIUrl, string callback = "", bool sync = false)
        {
            return string.Format("<script src=\"{0}/js?libraries=visualization&key={1}{2}\" {3}defer ></script>",
                GMaps_APIUrl,
                GMaps_APIKey,
                string.IsNullOrEmpty(callback) ? "" : "&callback=" + callback,
                sync ? "" : "async "
                );
        }

        public static double[] parseLatLng(string str)
        {
            double[] ret = new double[] { };

            if (!string.IsNullOrEmpty(str))
            {
                ret = str.Replace(" ", "").Split(',').Select(x => double.Parse(x)).ToArray();
            }

            return ret;
        }
    }
}