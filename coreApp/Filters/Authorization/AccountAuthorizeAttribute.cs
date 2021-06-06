using coreApp;
using Microsoft.AspNet.Identity;

namespace System.Web.Mvc
{

    public class AccountAuthorizeAttribute : AuthorizeAttribute
    {
        bool IsDisabled;
        bool InActive;

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool ret = true;

            IsDisabled = false;
            InActive = false;

            if (httpContext.User.Identity.IsAuthenticated)
            {
                string userName = httpContext.User.Identity.Name;
                UserAccess access = new UserAccess(userName);

                if (Cache.Get().userAccess.stakeholder == null)
                {
                    Cache.Update();
                }

                if (Module.DB.Procs.Account.IsAccountDisabled(userName: userName))
                {
                    IsDisabled = true;
                    ret = false;
                }

                if (access.stakeholder != null)
                {
                    if (!access.stakeholder.IsActive())
                    {
                        InActive = true;
                        ret = false;
                    }
                }
            }

            return ret;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                this.HandleUnauthorizedRequest(filterContext);
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            string err = "";
            if (IsDisabled)
            {
                err += "Your account is disabled";
            }

            if (InActive)
            {
                err += (err == "" ? "" : ". ") + "Your career is inactive";
            }

            filterContext.HttpContext.Session["AccountErrorMessage"] = err;
            filterContext.RequestContext.HttpContext.Response.Redirect(Procs.vdUrl("/Error"));
        }

    }

}