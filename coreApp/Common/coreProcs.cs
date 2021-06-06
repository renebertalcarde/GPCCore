using coreLibWeb.Encryption;
using Module.DB;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace coreApp
{
    public static class coreProcs
    {
        public static byte[] getPhoto(string type, int stakeholderId)
        {
            using (dalDataContext context = new dalDataContext())
            {
                byte[] arr = new byte[] { };

                if (type == "profile-photo")
                {
                    tblUserPhoto p = context.tblUserPhotos.Where(x => x.StakeholderId == stakeholderId).SingleOrDefault();
                    if (p != null)
                    {
                        if (p.Photo != null)
                        {
                            arr = p.Photo.ToArray();
                        }
                    }
                }
                return arr;
            }
        }

        public static string GetSessionKey(string sessionId, double latitude, double longitude)
        {
            string data = string.Format("{0}_{1},{2}", sessionId, latitude, longitude);
            return new Sym().Encrypt(data);
        }

        public static void SaveExceptionLog(string serverpath, Exception ex)
        {
            if (FixedSettings.LogExceptions)
            {
                DateTime n = DateTime.Now;
                string folder = Path.Combine(serverpath, "Exceptions");
                string filePath = Path.Combine(folder, n.ToString("MM-dd-yyyy") + ".txt");

                StreamWriter sw;

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                if (File.Exists(filePath))
                {
                    sw = File.AppendText(filePath);
                    sw.WriteLine("");
                }
                else
                {
                    sw = File.CreateText(filePath);
                }

                sw.WriteLine(string.Format("[{0}]", n.ToString("hh:mm:ss tt")));
                sw.WriteLine(ex.Message);
                sw.WriteLine("");
                sw.WriteLine(ex.StackTrace);

                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
        }

        public static bool hasPermission(string controller = "", string controllers = "")
        {
            if (controller == "")
            {
                controller = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
            }

            return (controllers ?? "").Split(',').Contains(controller) ||
                controller == "UserPhotos" ||
                controller == "MyPhotos";
        }

        public static string getRouteParam(HttpRequestBase Request, string paramName = "id", string defaultValue = "-1")
        {
            string id = defaultValue;
            if (Request.RequestContext.RouteData.Values[paramName] != null)
            {
                id = Request.RequestContext.RouteData.Values[paramName].ToString();
            }
            else if (Request.QueryString[paramName] != null)
            {
                id = Request.QueryString[paramName].ToString();
            }

            return id;
        }

        public static string ShowErrors(ModelStateDictionary ModelState, string delimeter = "\n")
        {
            string ret = "";

            if (ModelState.Values.ToArray().Any(x => x.Errors.Count > 0))
            {
                for (int i = ModelState.Values.Count - 1; i >= 0; i--)
                {
                    foreach (var err in ModelState.Values.ToArray()[i].Errors)
                    {
                        ret += (!String.IsNullOrEmpty(ret) ? delimeter : "") + err.ErrorMessage;
                    }
                }
            }

            return ret;
        }

        public static string ShowErrors(Exception e, string delimeter = "\n")
        {
            return coreLib.Procs.ShowErrors(e, delimeter);
        }


        public static string randomString(int length = 5, bool randomCase = true)
        {
            int intZero = '0';
            int intNine = '9';
            int intA = 'A';
            int intZ = 'Z';
            int intCount = 0;
            int intRandomNumber = 0;
            string strCaptchaString = "";

            Random random = new Random(System.DateTime.Now.Millisecond);
            Random rCase = new Random();

            while (intCount < length)
            {
                intRandomNumber = random.Next(intZero, intZ);
                if (((intRandomNumber >= intZero) && (intRandomNumber <= intNine) || (intRandomNumber >= intA) && (intRandomNumber <= intZ)))
                {
                    char c = (char)intRandomNumber;
                    string s = "";

                    if (randomCase)
                    {
                        if (rCase.Next(0, 2) == 0) s = c.ToString().ToLower(); else s = c.ToString().ToUpper();
                    }

                    strCaptchaString = strCaptchaString + s;
                    intCount = intCount + 1;
                }
            }
            return strCaptchaString;
        }

       
        public static string getBaseModuleSetting(string setting, string category)
        {
            return Cache.Get_Tables().BaseSettings.Where(x => x.Setting == setting && x.Category == category).Single().SettingValue;
        }

        public static void sendEmail(string receipientEmail, string senderEmail, string subject, string message)
        {
            string host = getBaseModuleSetting("Host", "Mail");
            int port = int.Parse(getBaseModuleSetting("Port", "Mail"));
            string username = getBaseModuleSetting("User", "Mail");
            string password = getBaseModuleSetting("Password", "Mail");
            bool usedefaultcredentials = bool.Parse(getBaseModuleSetting("UseDefaultCredentials", "Mail"));
            bool enablessl = bool.Parse(getBaseModuleSetting("EnableSSL", "Mail"));

            (new WebData()).sendEmail(senderEmail, receipientEmail, subject, message, host, port, username, password, usedefaultcredentials, enablessl);
        }
    }
}
