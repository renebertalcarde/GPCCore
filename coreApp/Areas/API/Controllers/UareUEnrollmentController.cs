using System.Linq;
using System.Web.Mvc;
using Module.DB;
using System.Web;

namespace coreApp.Areas.API.Controllers
{
    [UserAccessAuthorize("admin")]
    public class UareUEnrollmentController : Controller
    {
        public ActionResult GetData()
        {
            string s = Procs.GetOfflineData();
            return Content(s);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PostEnrollmentData(FormCollection form)
        {
            try
            {

                int employeeId = int.Parse(form["employeeId"]);
                int finger = int.Parse(form["finger"]);
                string value = HttpUtility.UrlDecode(form["value"]);

                using (dalDataContext context = new dalDataContext())
                {
                    tblEmployee_FP fp = context.tblEmployee_FPs.Where(x => x.EmployeeId == employeeId).SingleOrDefault();
                    if (fp == null)
                    {
                        fp = new tblEmployee_FP
                        {
                            EmployeeId = employeeId
                        };

                        context.tblEmployee_FPs.InsertOnSubmit(fp);
                    }

                    value = value == "" ? null : value;

                    if (finger == 0) fp.FP1 = value;
                    if (finger == 1) fp.FP2 = value;
                    if (finger == 2) fp.FP3 = value;
                    if (finger == 3) fp.FP4 = value;
                    if (finger == 4) fp.FP5 = value;
                    if (finger == 5) fp.FP6 = value;
                    if (finger == 6) fp.FP7 = value;
                    if (finger == 7) fp.FP8 = value;
                    if (finger == 8) fp.FP9 = value;
                    if (finger == 9) fp.FP10 = value;

                    context.SubmitChanges();
                }

                return Content("success");
            }
            catch
            {
                return Content("failed");
            }
        }

    }
}