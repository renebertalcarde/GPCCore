using System.Web.Mvc;

namespace coreApp.Controllers
{
    [UserAccessAuthorize("stakeholder")]
    public class Base_NoCoreStakeholderController : HLBase_NoCoreAuthorizedController
    { }
}
