using System.Web;
using System.Web.Mvc;

namespace coreApp.Controllers
{
    [AllowAnonymous]
    public class LicenseController : Controller
    {
        public ActionResult Index()
        {
            machine.machine mac = new machine.machine();
            string sn = MvcApplication.ApplicationLicense.all_space_SN(mac.getMotherBoardSerial(), mac.getMachineUniqueID(false));
            ViewBag.SN = sn;

            return View();
        }

        [HttpPost]
        public ActionResult UploadLicenseFile(HttpPostedFileBase LicenseFile)
        {
            if (LicenseFile == null)
            {
                TempData["GlobalError"] = "No file to upload";
            }
            else if (LicenseFile.ContentType != "application/octet-stream")
            {
                TempData["GlobalError"] = "Invalid file type";
            }
            else
            {
                string path = Server.MapPath("~/application.lic");

                LicenseFile.SaveAs(path);
                MvcApplication.ApplicationLicense.Validate();
                

                TempData["GlobalMessage"] = "File was successfully uploaded";
            }            

            return RedirectToAction("Index");
        }
    }
}