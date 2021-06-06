using System.Web.Mvc;

namespace coreApp.Controllers
{
    [UserAccessAuthorize("stakeholder")]
    public class BaseStakeholderController : BaseAuthorizedController
    { }
}
