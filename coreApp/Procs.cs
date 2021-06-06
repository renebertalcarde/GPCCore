using System;

namespace coreLibBase
{
    public static class Procs
    {
        public static string ShowErrors(Exception e, string delimeter = "\n")
        {
            string ret = "";

            if (e != null)
            {
                ret += e.Message;
                ret += (!String.IsNullOrEmpty(ret) ? delimeter : "") + ShowErrors(e.InnerException, delimeter);
            }

            return ret;
        }

        public static string ShowErrors(string[] array, string delimeter = "\n")
        {
            string ret = "";

            foreach (string err in array)
            {
                ret += (!String.IsNullOrEmpty(ret) ? delimeter : "") + err;
            }

            return ret;
        }
    }
}
