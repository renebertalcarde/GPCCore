using coreLibWeb.Encryption;
using Module.Core;
using Module.DB;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace coreApp.Controllers
{
    public class HomeController : Base
    {
        public ActionResult Index()
        {
            return View();
        }


        [ValidateInput(false)]
        public ActionResult IdVerify(string codeData)
        {
            try
            {
                if (!string.IsNullOrEmpty(codeData))
                {
                    int id = int.Parse(codeData, System.Globalization.NumberStyles.HexNumber);
                    using (dalDataContext context = new dalDataContext())
                    {
                        tblStakeholder stakeholder = context.tblStakeholders.Where(x => x.StakeholderId == id).SingleOrDefault();
                        if (stakeholder == null)
                        {
                            return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                        }

                        ViewBag.CodeData = codeData;

                        return View(stakeholder);
                    }
                }
            }
            catch (Exception ex)
            {
                return Content(coreProcs.ShowErrors(ex));
            }

            return new Raw_ActionResult(ModuleConstants.BAD_REQUEST);
        }

        public ActionResult GetStakeholderPhotoByCodeData(string type, string codeData)
        {
            try
            {
                if (!string.IsNullOrEmpty(codeData))
                {
                    int id = int.Parse(codeData, System.Globalization.NumberStyles.HexNumber);
                    using (dalDataContext context = new dalDataContext())
                    {
                        tblStakeholder stakeholder = context.tblStakeholders.Where(x => x.StakeholderId == id).SingleOrDefault();
                        if (stakeholder == null)
                        {
                            return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                        }
                        return GetPhoto(type, id);
                    }
                }
            }
            catch { }

            return new Raw_ActionResult(ModuleConstants.BAD_REQUEST);
        }
    }
}