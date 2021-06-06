using System.Linq;
using System.Net;
using System.Web.Mvc;
using System;
using Module.DB;
using System.IO;
using Module.DB.Enums;
using Module.Core;
using coreLibWeb.Encryption;

namespace coreApp.Areas.API.Controllers
{
    [Authorize]
    public class KioskController : Controller
    {
        public ActionResult GetEmployeePhoto(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new Raw_ActionResult(ModuleConstants.BAD_REQUEST);
            }

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee employee = context.tblEmployees.Where(x => x.UserId == userId).SingleOrDefault();
                if (employee != null)
                {
                    string dir = Server.MapPath("~/Assets/images");
                    string path = Path.Combine(dir, "anonymous.jpg");

                    byte[] arr = new byte[] { };

                    AspNetUsers_Photo p = context.AspNetUsers_Photos.Where(x => x.EmployeeId == employee.EmployeeId).SingleOrDefault();
                    if (p != null)
                    {
                        if (p.Photo != null)
                        {
                            arr = p.Photo.ToArray();
                        }
                    }

                    if (arr.Length > 0)
                    {
                        path = Path.Combine(dir, "unauthorized.jpg");

                        Response.Clear();
                        Response.ContentType = "image/jpeg";
                        //Response.AddHeader("Content-Disposition", "attachment; filename=myfile.docx");
                        Response.BinaryWrite(arr);
                        Response.Flush();
                        Response.Close();
                        Response.End();
                    }

                    return new Raw_ActionResult("No Photo");
                }
                else
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
            }
        }

        public ActionResult GetTime()
        {
            using (dalDataContext context = new dalDataContext())
            {
                string t = context.getDateTime().Value.ToString("MM/dd/yyyy h:mm:ss tt");
                return Content(t);
            }
        }

        public ActionResult GetData()
        {
            string s = Procs.GetOfflineData();
            return Content(s);
        }

        public ActionResult GetDataFileName()
        {
            string fn = string.Format("kiosk_{0}.txt", DateTime.Now.ToString("yyyyMMddhh"));
            string path = System.IO.Path.Combine(Server.MapPath("~/Temp"), fn);

            if (!System.IO.File.Exists(path))
            {
                string s = Procs.GetOfflineData();
                System.IO.File.WriteAllText(path, s);
            }

            return Content(fn);
        }

        [AllowAnonymous]
        public FileResult GetDataFile(string fn)
        {
            string path = System.IO.Path.Combine(Server.MapPath("~/Temp"), fn);
            return File(path, "text/plain", fn);
        }

        public ActionResult PostData()
        {
            try
            {
                string strReq = "";
                strReq = Request.RawUrl;
                strReq = strReq.Substring(strReq.IndexOf('?') + 1);

                if (strReq.Equals(""))
                {
                    throw new Exception("no parameter");
                }
                else
                {
                    strReq = new EncryptDecryptQueryString().Decrypt(strReq, "r0b1nr0y");
                }

                string[] d = strReq.Split('&');
                int terminalId = int.Parse(d[0].Split('=')[1]);
                int employeeId = int.Parse(d[1].Split('=')[1]);
                DateTime timeLog = DateTime.Parse(d[2].Split('=')[1]);
                int mode = d[3].Split('=')[1] == "In" ? (int)DeviceLogMode.In : (int)DeviceLogMode.Out;
                int deviceType = d[4].Split('=')[1] == "USB" ? (int)TerminalDeviceType.USB : (int)TerminalDeviceType.LAN;

                using (dalDataContext context = new dalDataContext())
                {

                    tblTerminalLog log = context.tblTerminalLogs
                                            .Where(x =>
                                                x.TerminalId == terminalId &&
                                                x.EmployeeId == employeeId &&
                                                x.LogDate == timeLog &&
                                                x.Mode == mode &&
                                                x.DeviceType == deviceType)
                                            .SingleOrDefault();

                    if (log == null)
                    {
                        log = new tblTerminalLog
                        {
                            TerminalId = terminalId,
                            EmployeeId = employeeId,
                            LogDate = timeLog,
                            Mode = mode,
                            DeviceType = deviceType
                        };

                        context.tblTerminalLogs.InsertOnSubmit(log);
                        context.SubmitChanges();
                    }

                    tblEmployee_TimeLog tl = context.tblEmployee_TimeLogs
                                                .Where(x =>
                                                    x.DeviceLogId == log.Id &&
                                                    x.EmployeeId == employeeId &&
                                                    x.EntryType == (int)TimeLogEntryType.Auto &&
                                                    x.TimeLog == timeLog &&
                                                    x.DeviceReference == log.GenerateReference())
                                                .SingleOrDefault();

                    if (tl == null)
                    {
                        tl = new tblEmployee_TimeLog
                        {
                            DeviceLogId = log.Id,
                            EmployeeId = employeeId,
                            EntryType = (int)TimeLogEntryType.Auto,
                            TimeLog = timeLog,
                            Mode = mode,
                            DeviceReference = log.GenerateReference()
                        };

                        context.tblEmployee_TimeLogs.InsertOnSubmit(tl);
                        context.SubmitChanges();
                    }
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