using System.Linq;
using System.Web.Mvc;
using System;
using Module.DB;
using Newtonsoft.Json;
using System.Collections.Generic;
using Module.DB.Enums;
using Devices;
using coreApp.Filters;

namespace coreApp.Areas.API.Controllers
{
    [Authorize]
    [APIFilter]
    public class DeviceSyncController : Controller
    {

        [HttpGet]
        public ActionResult GetDevices()
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    res.Data = context.tblDevices.Where(x => !x.ManagedByKiosk)
                        .Select(x => x.Mobile())
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveLogs(int deviceId, string logItems)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                List<clLogItem> LogItems = JsonConvert.DeserializeObject<List<clLogItem>>(logItems);

                using (dalDataContext context = new dalDataContext())
                {
                    List<tblDeviceLog> logs = new List<tblDeviceLog>();
                    List<tblDeviceLog> dbLogs = context.tblDeviceLogs.Where(x => x.DeviceId == deviceId).ToList();

                    var tmp = LogItems.Where(x => x.Exception == null).ToList();
                    foreach (clLogItem logItem in tmp)
                    {
                        int mode = Devices.Common.getMode(logItem.Mode);

                        logs.Add(new tblDeviceLog
                        {
                            DeviceId = deviceId,
                            IdNo = int.Parse(logItem.IdNo),
                            LogDate = DateTime.Parse(logItem.LogDate),
                            Mode = mode
                        });
                    }

                    List<tblDeviceLog> newLogs = logs.Where(x => !dbLogs.Any(y => y.IdNo == x.IdNo && y.LogDate == x.LogDate)).ToList();

                    context.tblDeviceLogs.InsertAllOnSubmit(newLogs.Distinct());
                    context.SubmitChanges();

                    res.Data = newLogs;
                }
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);
        }
               
        public ActionResult RelateLogs(int deviceId, DateTime startDate, DateTime endDate)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    List<tblEmployee_TimeLog> logs = context.vwDeviceLogs
                                                        .Where(x => x.DeviceId == deviceId && x.LogDate >= startDate && x.LogDate <= endDate)
                                                        .ToList()
                                                        .Where(x =>
                                                            !context.tblEmployee_TimeLogs.Any(y => y.EmployeeId == x.EmployeeId && y.TimeLog == x.LogDate && y.DeviceReference == x.GenerateReference())
                                                            )
                                                        .Select(x => new tblEmployee_TimeLog
                                                        {
                                                            EmployeeId = x.EmployeeId,
                                                            EntryType = (int)TimeLogEntryType.Auto,
                                                            TimeLog = x.LogDate,
                                                            Mode = x.Mode,
                                                            DeviceLogId = x.Id,
                                                            DeviceReference = x.GenerateReference()
                                                        })
                                                        .ToList();

                    context.tblEmployee_TimeLogs.InsertAllOnSubmit(logs);
                    context.SubmitChanges();

                    res.Data = logs;
                }
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);
        }

        public ActionResult DeviceLog(int deviceId, int enrollNo, DateTime timeLog, int mode)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                tblDeviceLog log = new tblDeviceLog
                {
                    DeviceId = deviceId,
                    IdNo = enrollNo,
                    LogDate = timeLog,
                    Mode = mode
                };

                using (dalDataContext context = new dalDataContext())
                {
                    if (!context.tblDeviceLogs.Any(x => x.DeviceId == log.DeviceId && x.IdNo == log.IdNo && x.LogDate == log.LogDate && x.Mode == log.Mode))
                    {
                        context.tblDeviceLogs.InsertOnSubmit(log);
                        context.SubmitChanges();

                        res.Data = log;
                    }

                    List<tblEmployee_DeviceIdNo> employees = context.tblEmployee_DeviceIdNos.Where(x => x.DeviceId == log.DeviceId && x.IdNo == log.IdNo).ToList();

                    List<tblEmployee_TimeLog> timelogs = new List<tblEmployee_TimeLog>();
                    foreach (var item in employees)
                    {
                        if (!context.tblEmployee_TimeLogs.Where(x =>
                            x.EmployeeId == item.EmployeeId &&
                            x.EntryType == (int)TimeLogEntryType.Auto &&
                            x.TimeLog == log.LogDate &&
                            x.DeviceReference == log.GenerateReference()
                        ).Any())
                        {
                            timelogs.Add(new tblEmployee_TimeLog
                            {
                                DeviceLogId = log.Id,
                                EmployeeId = item.EmployeeId,
                                EntryType = (int)TimeLogEntryType.Auto,
                                TimeLog = log.LogDate,
                                Mode = log.Mode,
                                DeviceReference = log.GenerateReference()
                            });
                        }
                    }

                    context.tblEmployee_TimeLogs.InsertAllOnSubmit(timelogs.Distinct());
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);
        }
    }
}