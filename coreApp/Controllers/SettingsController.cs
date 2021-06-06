using System.Web.Mvc;
using System.Linq;
using System;
using Module.DB;
using Module.Core;

namespace coreApp.Controllers
{
    [UserAccessAuthorize("admin", denyUrl: "/HL")]
    public class SettingsController : HLBase_NoCoreAuthorizedController
    {

        public ActionResult Index()
        {
            var model = Cache.Get_Tables().BaseSettings;
            return View(model);
        }
        
        public ActionResult Edit(int id)
        {
            tblSetting s = Cache.Get_Tables().BaseSettings.Where(x => x.Id == id).SingleOrDefault();
            if (s == null)
            {
                throw new Exception(ModuleConstants.ID_NOT_FOUND);
            }

            return PartialView(s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(tblSetting model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            if (ModelState.IsValid)
            {
                using (dalDataContext context = new dalDataContext())
                {
                    tblSetting setting = context.tblSettings.Where(x => x.Id == model.Id).Single();
                    if(model.ValueType == "bool")
                    {
                        setting.SettingValue = model.SettingValue;
                    }
                    else
                    {
                        setting.SettingValue = model.SettingValue;
                    }

                    context.SubmitChanges();
                    
                    res.Remarks = "Record was successfully updated";

                    Cache.Update_Tables(_settings: true);
                }

                FixedSettings.Init();
            }
            else
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ModelState);
            }

            return Json(res);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RefreshCachedTables()
        {
            Cache.Clear_Tables();
            Cache.Update_Tables();

            TempData["GlobalMessage"] = "Successfully cached tables";

            return RedirectToAction("Index");
        }
    }
}