using Aspose.Words;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace coreApp
{
    public static class Procs
    {
       

        public static string providerUrl(string url)
        {
            string providerUrl = Properties.Settings.Default.ProviderUrl;
            bool IsProvider = string.IsNullOrEmpty(providerUrl);

            return IsProvider ? Procs.vdUrl(url) : providerUrl + url;
        }

        public static string vdUrl(string url)
        {
            string ret = url;

            if (!string.IsNullOrEmpty(url))
            {
                string _url = url.ToLower();
                string _vd = MvcApplication.VirtualDirectory.ToLower();

                if (!_url.StartsWith(_vd) && !_url.StartsWith("http") && !_url.StartsWith("www"))
                {
                    ret = MvcApplication.VirtualDirectory + url;
                }
            }

            return ret;
        }

        public static int GetNoOfMonthsCovered(DateTime startDate, DateTime endDate)
        {
            int ret = 1;

            DateTime d = startDate;
            int m = d.Month;

            while (d <= endDate)
            {
                if (d.Month != m)
                {
                    m = d.Month;
                    ret++;
                }

                d = d.AddDays(1);
            }

            return ret;
        }

        public static void setTick(bool value, string bookmark, Aspose.Words.Document wordDoc)
        {
            object saveWithDocument = true;
            object oMissing = System.Reflection.Missing.Value;

            string checkedPath = Path.Combine(getServerPath(), "Assets", "images", "checked.png");
            string unCheckedPath = Path.Combine(getServerPath(), "Assets", "images", "unchecked.png");

            DocumentBuilder builder = new DocumentBuilder(wordDoc);
            builder.MoveToBookmark(bookmark);
            builder.InsertImage(value ? checkedPath : unCheckedPath, 12, 12);
        }

       

        public static string getServerPath()
        {
            return HttpContext.Current.Server.MapPath("~/");
        }

        public static string friendlyPeriod(DateTime d1, DateTime d2, bool fullMonth = false)
        {
            return coreLib.Procs.friendlyPeriod(d1, d2, fullMonth);
        }

        public static string friendlyDate(DateTime d)
        {
            return coreLib.Procs.friendlyDate(d);
        }

        public static void accessDenied(ControllerContext filterContext, string msg, string url = "/")
        {
            HttpRequestBase request = filterContext.HttpContext.Request;
            RouteData rd = new RouteData();

            filterContext.Controller.TempData["GlobalError"] = msg;

            if (request.UrlReferrer != null)
            {
                url = request.UrlReferrer.AbsoluteUri;
            }
            else
            {
                rd.DataTokens.Add("area", "");
                rd.Values.Add("controller", "Home");
                rd.Values.Add("action", "Index");
            }
            
            filterContext.Controller.TempData["GlobalDestination"] = rd;
            filterContext.HttpContext.Response.Redirect(Procs.vdUrl(url));
        }

        public static string getFullName(string LastName, string FirstName, string MiddleName, string NameExt)
        {
            string ret = "";

            if (!string.IsNullOrEmpty(LastName) || !string.IsNullOrEmpty(FirstName) || !string.IsNullOrEmpty(MiddleName) || !string.IsNullOrEmpty(NameExt))
            {
                string mn = "";
                if (!string.IsNullOrEmpty(MiddleName))
                {
                    List<string> _mn = MiddleName.Split(' ').ToList();
                    mn = " " + _mn.Last().ToString()[0].ToString() + ".";
                }

                ret = string.Format("{0}{1}, {2}{3}",
                    LastName,
                    string.IsNullOrEmpty(NameExt) ? "" : (" " + NameExt),
                    FirstName,
                    mn
                    );
            }
            return ret;
        }

        public static string getFullName_FN(string LastName, string FirstName, string MiddleName, string NameExt)
        {
            string ret = "";

            if (!string.IsNullOrEmpty(LastName) || !string.IsNullOrEmpty(FirstName) || !string.IsNullOrEmpty(MiddleName) || !string.IsNullOrEmpty(NameExt))
            {
                string mn = "";
                if (!string.IsNullOrEmpty(MiddleName))
                {
                    List<string> _mn = MiddleName.Split(' ').ToList();
                    mn = _mn.Last().ToString()[0].ToString() + ". ";
                }

                ret = string.Format("{0} {1}{2}{3}",
                    FirstName,
                    mn,
                    LastName,
                    string.IsNullOrEmpty(NameExt) ? "" : (" " + NameExt)
                    );
            }
            return ret;
        }
        
    }


    public static class Account
    {

        public static IdentityResult CreateUser(ApplicationUserManager userManager, Models.NewUserModel model)
        {
            var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };

            var result = userManager.Create(user, model.Password);

            if (result.Succeeded)
            {
                result = SaveUserRoles(userManager, user.Id, model.Roles);

                Module.DB.Procs.Account.DisableAccount(user.Id, model.Disabled);
            }

            return result;
        }

        public static IdentityResult EditUser(ApplicationUserManager userManager, Models.UserModel model)
        {
            IdentityResult result;

            var user = userManager.FindById(model.Id);
            if (user == null)
            {
                result = new IdentityResult(new string[] { "User Id not found" });
            }
            else
            {
                user.UserName = model.UserName;
                user.Email = model.Email;

                result = SaveUserRoles(userManager, user.Id, model.Roles);

                Module.DB.Procs.Account.DisableAccount(user.Id, model.Disabled);
            }

            return result;
        }

        public static IdentityResult DeleteUser(ApplicationUserManager userManager, string Id)
        {
            var result = RemoveUserRoles(userManager, Id);

            if (result.Succeeded)
            {
                var user = userManager.FindById(Id);

                result = userManager.Delete(user);

            }

            return result;
        }

        public static IdentityResult SaveUserRoles(ApplicationUserManager userManager, string Id, string[] newRoles)
        {
            var userRoles = userManager.GetRoles(Id);

            newRoles = newRoles ?? new string[] { };

            var result = userManager.AddToRoles(Id, newRoles.Except(userRoles).ToArray<string>());

            if (result.Succeeded)
            {
                result = userManager.RemoveFromRoles(Id, userRoles.Except(newRoles).ToArray<string>());
            }

            return result;
        }

        public static IdentityResult RemoveUserRoles(ApplicationUserManager userManager, string userId)
        {
            var userRoles = userManager.GetRoles(userId);
            var result = userManager.RemoveFromRoles(userId, userRoles.ToArray());

            return result;
        }
    }
}
