using System;
using System.Web.Mvc;
using coreApp.Filters;
using coreApp.Controllers;
using coreApp.Interfaces;
using Module.DB;
using System.Web;
using HRLogixMobileLib;
using coreLibWeb.Encryption;

namespace coreApp.Areas.EmployeeModule.Controllers
{
    [MyFilter]
    public class MyGeoAuthController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }

        [SSL]
        public ActionResult Index()
        {
            if (FixedSettings.AllowGeoLocation)
            {
                if (string.IsNullOrEmpty(employee.GeoAuth_DeviceRef))
                {
                    return RedirectToAction("RegisterIndex");
                }
                else
                {
                    return View();
                }                
            }
            else
            {
                return Content("Geo-Location is disabled");
            }
        }

        public ActionResult Logs(string Data, string DeviceId)
        {
            if (string.IsNullOrEmpty(Data))
            {
                throw new Exception("Invalid data");
            }

            string[] tmp = HttpUtility.UrlDecode(Data).Split(',');
            double latitude = double.Parse(tmp[0]);
            double longitude = double.Parse(tmp[1]);

            ViewBag.SessionKey = coreProcs.GetSessionKey(Session.SessionID, latitude, longitude);

            GeoAuth_Employee model = new GeoAuth_Employee(FixedSettings.AllowGeoLocation, employee.GeoAuth_DeviceRef, DeviceId, latitude, longitude, DBProcs.get_geoAuthAreas(employee.GeoAuth_Areas));
            return PartialView("Logs", model);
        }

        [HttpPost]
        public ActionResult DoLog(string Op, string DeviceId, string SessionKey)
        {
            string tmp1 = new Sym().Decrypt(SessionKey);
            string[] tmp2 = tmp1.Split('_');
            string[] tmp3 = tmp2[1].Split(',');

            string sessionId = tmp2[0];
            double latitude = double.Parse(tmp3[0]);
            double longitude = double.Parse(tmp3[1]);

            if (sessionId != Session.SessionID)
            {
                throw new Exception("Incorrect session");
            }

            GeoAuth_Employee model = new GeoAuth_Employee(FixedSettings.AllowGeoLocation, employee.GeoAuth_DeviceRef, DeviceId, latitude, longitude, DBProcs.get_geoAuthAreas(employee.GeoAuth_Areas));

            DBProcs.doGeoLog(employee.EmployeeId, Op, model.Auth.LogArea.Area.AreaId, model.Auth.LogArea.Area.AreaName);

            TempData["GlobalMessage"] = "Time log was successfully submitted";

            string dt = DateTime.Today.ToString("MM-dd-yyyy");
            return RedirectToAction("Index", "MyTimeLogs", new { area = "EmployeeModule", startDate = dt, endDate = dt });
        }

        [SSL]
        public ActionResult RegisterIndex()
        {
            if (FixedSettings.AllowGeoLocation)
            {
                return View();
            }
            else
            {
                return Content("Geo-Location is disabled");
            }
        }

        public ActionResult Register(string DeviceId)
        {
            GeoAuth_Employee model = new GeoAuth_Employee(FixedSettings.AllowGeoLocation, employee.GeoAuth_DeviceRef, DeviceId, 0, 0, DBProcs.get_geoAuthAreas(employee.GeoAuth_Areas));
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult PostRegisterIndex()
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                string key = coreProcs.CreateNewDeviceRegistrationKey(employee.EmployeeId);

                res.Data = key;
                res.Remarks = "New device registration key was successfully registered";
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);            
        }
        
        [HttpGet]
        public ActionResult GetServerTime()
        {
            return new API.Controllers.MobileController().GetServerTime();
        }
    }
}
