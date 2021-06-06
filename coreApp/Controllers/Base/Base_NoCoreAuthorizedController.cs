using coreApp.Filters;
using System.Web.Mvc;

namespace coreApp.Controllers
{
    [Authorize]
    [AccountAuthorize]
    [PeriodicChangePasswordFilter]
    public class Base_NoCoreAuthorizedController : Base
    { }
}
