using coreApp;
using Module.Core;
using Module.DB;
using System.Linq;

namespace System.Web.Mvc
{

    public class StakeholderAuthorizeAttribute : AuthorizeAttribute
    {
        string idName = "stakeholderId";
        int id = -1;
        bool forScheduling = false;

        public StakeholderAuthorizeAttribute(string idName, bool forScheduling = false)
        {
            this.idName = idName;
            this.forScheduling = forScheduling;
        }

        public StakeholderAuthorizeAttribute(bool forScheduling = false)
        {
            this.forScheduling = forScheduling;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            id = int.Parse(coreProcs.getRouteParam(httpContext.Request, idName));

            if (id == -1)
            {
                return true;
            }
            else
            {
                return Cache.Get().userAccess.AllowedStakeholder(id, forScheduling);
            }
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

            string info = request.Url.AbsolutePath;

            using (dalDataContext context = new dalDataContext())
            {
                tblStakeholder stakeholder = context.tblStakeholders.Where(x => x.StakeholderId == id).SingleOrDefault();
                if (stakeholder != null)
                {
                    info = stakeholder.FullName_FN;
                }
            }

            string msg = string.Format("{0}, \"{1}\"", ModuleConstants_Authorization.EMPLOYEEACCESS_UNAUTHORIZED_ACCESS_TO_RESOURCE, info);

            Procs.accessDenied(filterContext, msg, "/HL");
        }

    }

}