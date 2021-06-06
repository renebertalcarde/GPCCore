using coreApp;
using Module.Core;

namespace System.Web.Mvc
{

    public class UserAccessAuthorizeAttribute : AuthorizeAttribute
    {
        string allowedRoles = "";
        string allowedAccess = "";
        string denyUrl = "";
        
        public UserAccessAuthorizeAttribute(string allowedRoles = "", string allowedAccess = "", string denyUrl = "/HL")
        {
            this.allowedRoles = allowedRoles;
            this.allowedAccess = allowedAccess;
            this.denyUrl = denyUrl;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!EvaluateRoles()) return false;
            if (!EvaluateAccess()) return false;

            return true;
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
            HttpRequestBase request = filterContext.HttpContext.Request;
            string msg = string.Format("{0}, \"{1}\"", ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACCESS_TO_RESOURCE, request.Url.AbsolutePath);

            Procs.accessDenied(filterContext, msg, denyUrl);
        }

        private bool EvaluateRoles()
        {
            bool ret = true;

            UserAccess access = Cache.Get().userAccess;

            if (allowedRoles != "")
            {
                ret = false;

                foreach (string role in allowedRoles.Split(','))
                {
                    if (role == "") continue;

                    if ((role == "admin" && access.IsAdmin) || (role == "staff" && access.IsStaff))
                    {
                        ret = true;
                        break;
                    }
                }
            }

            return ret;
        }

        private bool EvaluateAccess()
        {
            bool ret = true;

            UserAccess access = Cache.Get().userAccess;

            if (allowedAccess != "")
            {
                ret = false;

                foreach (string role in allowedAccess.Split(','))
                {
                    if (role == "") continue;

                    if (access.HasAccess(role))
                    {
                        ret = true;
                        break;
                    }
                }
            }

            return ret;
        }
    }
}