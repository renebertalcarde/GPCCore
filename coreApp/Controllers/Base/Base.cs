using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Mvc;
using System.Web;
using coreApp.Filters;
using System.IO;
using System.Linq;
using Module.DB;

namespace coreApp.Controllers
{
    [ErrorFilter]
    public class Base : Controller
    {
        protected void AddErrors(IdentityResult result)
        {
            if (result != null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
        }

        protected void AddErrors(string[] errors)
        {
            foreach(string error in errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected void AddError(string error)
        {
            ModelState.AddModelError("", error);
        }

        public Base()
        { }

        public Base(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        protected ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        protected ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public ActionResult GetPhoto(string type, int stakeholderId)
        {
            string dir = Server.MapPath("~/Assets/images");
            string path = Path.Combine(dir, "anonymous.jpg");

            byte[] arr = coreApp.coreProcs.getPhoto(type, stakeholderId);

            if (arr.Length > 0)
            {
                path = Path.Combine(dir, "unauthorized.jpg");
                bool allowedToView = true; // Cache.Get().userAccess.IsAdmin || Cache.Get().userAccess.AllowedStakeholder(stakeholderId);

                if (allowedToView)
                {
                    Response.Clear();
                    Response.ContentType = "image/jpeg";
                    //Response.AddHeader("Content-Disposition", "attachment; filename=myfile.docx");
                    Response.BinaryWrite(arr);
                    Response.Flush();
                    Response.Close();
                    Response.End();
                }
            }

            if (!System.IO.File.Exists(path))
            {
                path = Path.Combine(dir, "noimage.png");
            }

            return base.File(path, "image/jpeg");
        }

    }
}
