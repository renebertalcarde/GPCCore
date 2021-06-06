using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Module.DB;
using System.Data.Linq;
using System.Collections;
using System.Reflection;

namespace coreApp
{
    public class MimeMappingStealer
    {
        // The get mime mapping method info
        private readonly MethodInfo _getMimeMappingMethod = null;

        /// <summary>
        /// Static constructor sets up reflection.
        /// </summary>
        public MimeMappingStealer()
        {
            // Load hidden mime mapping class and method from System.Web
            var assembly = Assembly.GetAssembly(typeof(HttpApplication));
            Type mimeMappingType = assembly.GetType("System.Web.MimeMapping");
            _getMimeMappingMethod = mimeMappingType.GetMethod("GetMimeMapping",
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        }

        /// <summary>
        /// Exposes the hidden Mime mapping method.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The mime mapping.</returns>
        public string GetMimeMapping(string fileName)
        {
            return (string)_getMimeMappingMethod.Invoke(null /*static method*/, new[] { fileName });
        }
    }

    public class SelectListItemExt : SelectListItem
    {
        public Hashtable Data { get; set; }
        public bool CanSpecifyDetails { get; set; }
        public string Details { get; set; }
    }
    public class Raw_ActionResult : coreLibWeb.Raw_ActionResult
    {
        public Raw_ActionResult(string contents) : base(contents)
        { }
    }
    public class SListParam
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public int Total { get; set; }
        public int ItemsCount { get; set; }
    }
    public class objAccountabilityExt : objAccountability
    {
        string module;

        public objAccountabilityExt(string module)
        {
            this.module = module;
        }

        public void InsertLog(string Page, int RecordId, string RecordName, object Data = null)
        {
            string Details = string.Format("Inserted record [{0}] {1}", RecordId, RecordName);
            WriteLog(Page, "Insert", Details, Data);
        }

        public void UpdateLog(string Page, int RecordId, string RecordName, ModifiedMemberInfo[] MMI)
        {
            string Details = string.Format("Updated record [{0}] {1}", RecordId, RecordName);
            object Data = MMI.Select(x => new { FieldName = x.Member.Name, Orig = x.OriginalValue, Current = x.CurrentValue });
            WriteLog(Page, "Update", Details, Data);
        }

        public void DeleteLog(string Page, int RecordId, string RecordName, object Data = null)
        {
            string Details = string.Format("Deleted record [{0}] {1}", RecordId, RecordName);
            WriteLog(Page, "Delete", Details, Data);
        }

        public void WriteLog(string Page, string Action, string Details, object Data = null)
        {
            DateTime LogDate = DateTime.Now;
            string UserId = Cache.Get().userAccess.user.Id;
            string Url = HttpContext.Current.Request.Url.AbsolutePath;

            base.WriteLog(LogDate, UserId, module, Page, Url, Action, Details, Data);
        }
    }

    public class objGreeting
    {
        public string Greeting { get; set; }
        public string Me { get; set; }

        public objGreeting(string firstName)
        {
            int t = DateTime.Now.Hour;
            Greeting = t < 12 ? "morning" :
                                t < 18 ? "afternoon" : "evening";

            Me = firstName;
        }
    }


    public class ModuleUrlParameter
    {
        public string Action { get; set; }
        public string Controller { get; set; }
        public ModuleUrlParameter_RouteValue RouteValues { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class ModuleUrlParameter_RouteValue
    {
        public string parentUrl { get; set; }
        public string area { get; set; }
        public string id { get; set; }
    }

    public class Breadcrumb
    {
        public string Description { get; set; }
        public string Link { get; set; }
    }


    public class WebData
    {
        public void sendEmail(string sender, string receipient, string subject, string message, string host, int port, string username, string password, bool usedefaultcredentials, bool enablessl)
        {
            try
            {
                var loginInfo = new NetworkCredential(username, password);
                var msg = new MailMessage();
                var smtpClient = new SmtpClient(host, port);

                msg.From = new MailAddress(sender);
                msg.To.Add(new MailAddress(receipient));
                msg.Subject = subject.ToString().Replace("\n", "");
                msg.Body = message;
                msg.IsBodyHtml = true;

                smtpClient.EnableSsl = enablessl;
                smtpClient.UseDefaultCredentials = usedefaultcredentials;
                smtpClient.Credentials = loginInfo;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(msg);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }

    public class LayoutArea
    {
        public string Area { get; set; }
        public LayoutItem[] Controllers = new LayoutItem[] { };

        public bool Match(string area, string controller, string action)
        {
            bool ret = false;

            if (Area == area)
            {
                if (Controllers == null)
                {
                    ret = true;
                }
                else
                {
                    ret = Controllers.Any(x => x.MatchController(controller, action));
                }                
            }

            return ret;
        }

        public LayoutArea()
        { }

        public LayoutArea(HttpRequestBase Request, string toParse)
        {
            if (!string.IsNullOrEmpty(toParse))
            {
                string[] ss = toParse.Split('{');
                if (ss.Length > 1)
                {
                    Area = ss[0];

                    List<LayoutItem> _controllers = new List<LayoutItem>();
                    foreach (Match match in Regex.Matches(toParse, "{.*?}"))
                    {
                        string v = match.Value;
                        string str = v.Substring(1, v.Length - 2);

                        _controllers.Add(new LayoutItem(Request, 0, str));
                    }

                    Controllers = _controllers.ToArray();
                }
                else
                {
                    Area = toParse;
                }
            }
        }
    }
        
    public class LayoutItem
    {
        string delimeters = "|,?&";

        public string Name;
        public LayoutItem[] Items = new LayoutItem[] { };

        HttpRequestBase Request;
        
        KeyValuePair<string, string> Parameter
        {
            get
            {
                string[] tmp = Name.Split('=');
                return new KeyValuePair<string, string>(tmp[0], tmp[1]);
            }
        }

        public bool MatchKey()
        {
            bool ret = false;

            string urlParamValue = coreApp.coreProcs.getRouteParam(Request, Parameter.Key, "");
            string paramKey = string.Format("{0}={1}", Parameter.Key, urlParamValue).ToLower();

            ret = Name.ToLower() == paramKey;

            return ret;
        }

        public bool MatchAction(string action)
        {
            bool ret = false;
            
            if (Name == action)
            {
                if (Items.Any())
                {
                    ret = !Items.Any(x => !x.MatchKey());
                }
                else
                {
                    ret = true;
                }
            }

            return ret;
        }

        public bool MatchController(string controller, string action)
        {
            bool ret = false;

            if (Name == controller)
            {
                if (Items.Any())
                {
                    ret = Items.Any(x => x.MatchAction(action));
                }
                else
                {
                    ret = true;
                }
            }

            return ret;
        }

        public LayoutItem()
        { }

        public LayoutItem(HttpRequestBase Request, int level, string toParse)
        {
            this.Request = Request;

            if (!string.IsNullOrEmpty(toParse))
            {
                Name = toParse;

                if (delimeters.Length > level)
                {
                    string[] s = toParse.Split(delimeters[level++]);

                    if (s.Length > 1)
                    {
                        Name = s[0];

                        if (delimeters.Length > level)
                        {
                            string[] ss = s[1].Split(delimeters[level++]);

                            if (ss.Any())
                            {
                                Items = ss.Select(x => new LayoutItem(Request, level, x)).ToArray();
                            }
                        }
                    }
                }
            }
        }
    }

    public class LayoutUtility
    {
        public HttpRequestBase Request { get; set; }
        public string className { get; set; }

        public string MenuSelector(string _includedItems, string _excludedItems = null)
        {
            string ret = "";

            LayoutArea[] includedItems = new LayoutArea[] { };
            LayoutArea[] excludedItems = new LayoutArea[] { };

            if (!string.IsNullOrEmpty(_includedItems))
            {
                List<LayoutArea> _areas = new List<LayoutArea>();

                foreach (Match match in Regex.Matches(_includedItems, "\\[.*?]"))
                {
                    string v = match.Value;
                    string str = v.Substring(1, v.Length - 2);

                    _areas.Add(new LayoutArea(Request, str));
                }

                includedItems = _areas.ToArray();
            }

            if (!string.IsNullOrEmpty(_excludedItems))
            {
                List<LayoutArea> _areas = new List<LayoutArea>();

                foreach (Match match in Regex.Matches(_excludedItems, "\\[.*?]"))
                {
                    string v = match.Value;
                    string str = v.Substring(1, v.Length - 2);

                    _areas.Add(new LayoutArea(Request, str));
                }

                excludedItems = _areas.ToArray();
            }


            string area = "[NONE]";
            string controller = "";
            string action = "";

            if (HttpContext.Current.Request.RequestContext.RouteData.DataTokens["area"] != null)
            {
                area = HttpContext.Current.Request.RequestContext.RouteData.DataTokens["area"].ToString();
            }

            if (HttpContext.Current.Request.RequestContext.RouteData.Values["controller"] != null)
            {
                controller = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
            }

            if (HttpContext.Current.Request.RequestContext.RouteData.Values["action"] != null)
            {
                action = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
            }

            if (HttpContext.Current.Request.RequestContext.RouteData.Values["action"] != null)
            {
                action = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
            }

            bool include = !includedItems.Any() || includedItems.Any(x => x.Match(area, controller, action));

            bool exclude = excludedItems.Any() && excludedItems.Any(x => x.Match(area, controller, action)); ;
            
            if (include && !exclude)
            {
                ret = className;
            }

            return ret;
        }
    }

    public class objPaging
    {
        public int pageNo { get; set; }
        public int pageSize { get; set; }
        public int skip { get; set; }
        public int totalCount { get; set; }
        public int currentCount { get; set; }
        public int totalPages { get; set; }
        public string url { get; set; }

        public objPaging(int pageNo, int pageSize, int totalCount)
        {
            totalPages = Decimal.ToInt32(Math.Ceiling((decimal)totalCount / pageSize));

            if (pageNo < 1) this.pageNo = 1;
            else if (pageNo > totalPages && totalPages > 0) this.pageNo = totalPages;
            else this.pageNo = pageNo;

            this.pageSize = pageSize;
            this.totalCount = totalCount;

            skip = pageSize * (this.pageNo - 1);
            url = HttpContext.Current.Request.Url.AbsolutePath;
        }
    }

    public class List
    {
        public string url_details { get; set; }
        public objPaging paging { get; set; }
        public object list { get; set; }
    }

    public class queryResult : HRLogixMobileLib.queryResult
    { }

    public static class FixedSettings
    {
        public static string Owner = "iLogix Phils. Inc.";
        public static string ApplicationName {
            get
            {
                return typeof(MvcApplication).Assembly.GetName().Name;
            }
        }

        public static string AgencyName
        {
            get
            {
                return "Green Pastures Corporation";
            }
        }

        public static string ContactUs_Mobile = "+63 917 1088 051";
        public static string ContactUs_Email = "admin@ilogixphils.com";

        public static string DefaultPassword;        
        public static string AgencyAddress;
        public static long PhotoFileSize;
        public static string PhotoFileTypes;
        public static int AutoLogout;
        public static bool UseSSL;
        public static bool LogExceptions;

        public static int PasswordChangeInterval;
        public static string TemplateSet;

        public static void Init()
        {
            DefaultPassword = coreProcs.getBaseModuleSetting("DefaultPassword", "General");
            AgencyAddress = coreProcs.getBaseModuleSetting("AgencyAddress", "General");

            PhotoFileSize = Convert.ToInt32(coreProcs.getBaseModuleSetting("PhotoFileSize", "General"));
            PhotoFileTypes = coreProcs.getBaseModuleSetting("PhotoFileTypes", "General");

            AutoLogout = Convert.ToInt32(coreProcs.getBaseModuleSetting("AutoLogout", "General"));

            UseSSL = Convert.ToBoolean(coreProcs.getBaseModuleSetting("UseSSL", "General"));
            LogExceptions = Convert.ToBoolean(coreProcs.getBaseModuleSetting("LogExceptions", "General"));

            PasswordChangeInterval = Convert.ToInt32(coreProcs.getBaseModuleSetting("PasswordChangeInterval", "General"));

            TemplateSet = coreProcs.getBaseModuleSetting("TemplateSet", "Reports");

        }

        public static string Copyright
        {
            get
            {
                return string.Format("{0} ver. {1} for {2}",
                    FixedSettings.ApplicationName,
                    typeof(MvcApplication).Assembly.GetName().Version,
                    FixedSettings.AgencyName
                    );
            }
        }
    }
    
}
