using System.Linq;
using System.Web.Mvc;
using System;
using HRLogixMobileLib;
using Module.DB;
using coreLib.Extensions;
using System.IO;
using coreApp.Filters;
using System.Collections.Generic;
using Newtonsoft.Json;
using Module.DB.Enums;

namespace coreApp.Areas.API.Controllers
{
    [Authorize]
    [APIFilter]
    public class MobileController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetCompanySettings()
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {

                    string logoPath = Server.MapPath("~/Assets/images/company-logo.png");

                    res.Data = new CompanySettings
                    {
                        companyName = FixedSettings.AgencyName,
                        showCompanyName = Properties.Settings.Default.ShowCompanyName,
                        imageArray = System.IO.File.ReadAllBytes(logoPath),
                        width = Properties.Settings.Default.CompanyLogoWidth,
                        height = Properties.Settings.Default.CompanyLogoHeight,
                        x = Properties.Settings.Default.CompanyLogoX,
                        y = Properties.Settings.Default.CompanyLogoY,
                        aspect = Properties.Settings.Default.CompanyLogoAspect,
                        bgColor = Properties.Settings.Default.CompanyLogoBgColor,
                        bgImageHeight = Properties.Settings.Default.BackgroundImageHeight                        
                    };
                }
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetServerTime()
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                res.Data = DateTime.Now.ToString();
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetRecentTimeLogs(DateTime startDate)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };
            try
            {
                UserAccess access = new UserAccess(User.Identity.Name);

                using (dalDataContext context = new dalDataContext())
                {
                    DateTime eDt = DateTime.Now;
                    DateTime sDt = eDt.AddDays(-1);

                    if (startDate < sDt)
                    {
                        sDt = startDate;
                    }

                    res.Data = new TimeLogsInfo
                    {
                        sDt = sDt,
                        eDt = eDt,
                        ServerTimeLogs = context.tblEmployee_TimeLogs
                                    .Where(x => x.EmployeeId == access.employee.EmployeeId && x.TimeLog >= sDt && x.TimeLog <= eDt)
                                    .Select(x => x.Mobile())
                                    .ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveProfilePhoto()
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };
            try
            {
                UserAccess access = new UserAccess(User.Identity.Name);

                using (dalDataContext context = new dalDataContext())
                {
                    AspNetUsers_Photo user = context.AspNetUsers_Photos.Where(x => x.EmployeeId == access.employee.EmployeeId).SingleOrDefault();
                    if (user == null)
                    {
                        throw new Exception("No Profile photo to remove");
                    }

                    context.AspNetUsers_Photos.DeleteOnSubmit(user);
                    context.SubmitChanges();

                    res.Remarks = "Profile photo was successfully removed";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);
        }

        public ActionResult UpdateProfilePhoto()
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };
            try
            {
                UserAccess access = new UserAccess(User.Identity.Name);

                if (Request.Files.Count == 0)
                {
                    throw new Exception("No file received");
                }

                var file = Request.Files[0];
                if (FixedSettings.PhotoFileSize < file.ContentLength)
                {
                    throw new Exception("File size exceeded file-size limit (" + FixedSettings.PhotoFileSize.ToBytes() + ")");                    
                }

                MemoryStream stream = new MemoryStream();
                file.InputStream.CopyTo(stream);
                byte[] arr = stream.ToArray();

                using (dalDataContext context = new dalDataContext())
                {
                    AspNetUsers_Photo user = context.AspNetUsers_Photos.Where(x => x.EmployeeId == access.employee.EmployeeId).SingleOrDefault();
                    if (user == null)
                    {
                        user = new AspNetUsers_Photo { EmployeeId = access.employee.EmployeeId };
                        context.AspNetUsers_Photos.InsertOnSubmit(user);
                    }

                    user.Photo = arr;
                    context.SubmitChanges();

                    res.Remarks = "Profile photo was successfully updated";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);
        }

        public ActionResult PostOfflineLogs(string data)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };
            try
            {
                UserAccess access = new UserAccess(User.Identity.Name);

                if (!string.IsNullOrEmpty(data))
                {
                    List<objTimeLog> _logs = JsonConvert.DeserializeObject<List<objTimeLog>>(data);

                    using (dalDataContext context = new dalDataContext())
                    {
                        List<tblEmployee_TimeLog> logs = new List<tblEmployee_TimeLog>();

                        foreach (objTimeLog _log in _logs)
                        {
                            logs.Add(new tblEmployee_TimeLog
                            {
                                EmployeeId = access.employee.EmployeeId,
                                TimeLog = _log.timeLog,
                                Mode = _log.action == "in" ? (int)DeviceLogMode.In : (int)DeviceLogMode.Out,
                                EntryType = (int)TimeLogEntryType.GeoLocation,
                                DeviceReference = String.Format(HRLogixMobileLib.Constants.GEO_REFERENCE_FORMAT, _log.areaId, _log.areaName)
                            });
                        }

                        context.tblEmployee_TimeLogs.InsertAllOnSubmit(logs);
                        context.SubmitChanges();

                        res.Remarks = logs.Count + " offline time logs were successfully posted";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);
        }

        public ActionResult DoLog(string action, int areaId, string areaName)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };
            try
            {
                UserAccess access = new UserAccess(User.Identity.Name);

                res.Data = DBProcs.doGeoLog(access.employee.EmployeeId, action, areaId, areaName);
                res.Remarks = "Time log was successfully submitted";
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);
        }

        public ActionResult RegisterDevice(string deviceId)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };
            try
            {
                UserAccess access = new UserAccess(User.Identity.Name);

                coreProcs.CreateNewDeviceRegistrationKey(access.employee.EmployeeId, deviceId);
                
                res.Remarks = "You device was successfully registered";
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);
        }

        [HttpGet]
        public ActionResult GetUserInfo()
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };
            try
            {
                UserAccess access = new UserAccess(User.Identity.Name);

                UserInfo uInfo = new UserInfo
                {
                    Employee = access.employee.Mobile(),
                    Info = new Module.DB.Procs.procs_tblEmployee(access.employee).Info().Mobile(),
                    Career = access.career.Mobile(),
                    AllowGeoLocation = FixedSettings.AllowGeoLocation,
                    Areas = DBProcs.get_geoAuthAreas(access.employee.GeoAuth_Areas),             
                };

                res.Data = uInfo;

            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetUserPhoto()
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };
            try
            {
                UserAccess access = new UserAccess(User.Identity.Name);

                HRLogixMobileLib.UserPhoto uPhoto = new HRLogixMobileLib.UserPhoto
                {
                    imageArray = coreApp.coreProcs.getPhoto("profile-photo", access.employee.EmployeeId)
                };

                res.Data = uPhoto;

            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetOnlineApplications()
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };
            try
            {
                UserAccess access = new UserAccess(User.Identity.Name);
                tblEmployee e = access.employee;

                OnlineApplicationsModel model = new coreApp.Controllers.WidgetsController().GetApplicationsModel(access, Request);
                res.Data = model;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
    }
}