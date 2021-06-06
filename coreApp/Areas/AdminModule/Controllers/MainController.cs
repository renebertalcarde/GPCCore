using coreApp.Areas.AdminModule.Models;
using coreApp.Areas.Procurement;
using coreApp.Areas.Procurement.DAL;
using coreApp.Controllers;
using coreLib.Enums;
using coreLib.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace coreApp.Areas.AdminModule.Controllers
{
    public class MainController : Controller
    {
        //public ActionResult ForceUpdate()
        //{
        //    try
        //    {
        //        string myPath = Server.MapPath("~/");

        //        ProcessStartInfo start = new ProcessStartInfo();
        //        start.FileName = Properties.Settings.Default.UpdateClientApplicationPath;
        //        start.Arguments = string.Format("{0} {1} {2}",
        //            Properties.Settings.Default.ClientId,
        //            Properties.Settings.Default.ApplicationName,
        //            myPath
        //            );

        //        start.WindowStyle = ProcessWindowStyle.Hidden;
        //        start.CreateNoWindow = true;
        //        int exitCode;
                
        //        using (Process proc = Process.Start(start))
        //        {
        //            proc.WaitForExit();
        //            exitCode = proc.ExitCode;
        //        }
                
        //        return RedirectToAction("UpdateLogs");
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(coreLib.Procs.ShowErrors(e));
        //    }
        //}

        //public ActionResult UpdateLogs(string startDate, string endDate)
        //{
        //    PeriodModel pm = new PeriodModel(PeriodModelInitType.Today);
        //    DateTime tmp;

        //    if (DateTime.TryParse(startDate, out tmp))
        //    {
        //        pm.StartDate = tmp;
        //    }

        //    if (DateTime.TryParse(endDate, out tmp))
        //    {
        //        pm.EndDate = tmp;
        //    }

        //    ViewBag.Period = pm;

        //    string UpdateClientPath = Path.Combine(
        //        Properties.Settings.Default.UpdateClientPath,
        //        Properties.Settings.Default.ClientId,
        //        Properties.Settings.Default.ApplicationName
        //        );

        //    string LogsPath = Path.Combine(UpdateClientPath, "Logs");

        //    DirectoryInfo di = new DirectoryInfo(LogsPath);
        //    if (di.Exists)
        //    {
        //        List<objDayLog> model = di.GetFiles()
        //            .Where(x => pm.Within(DateTime.Parse(x.Name.Substring(0, x.Name.LastIndexOf('.')).Replace(".", ":"))))
        //            .Select(x => GetDayLog(x))
        //            .ToList();

        //        return View(model);
        //    }
        //    else
        //    {
        //        return View();
        //    }
            
        //}

        //objDayLog GetDayLog(FileInfo fi)
        //{
        //    string n = fi.Name.Substring(0, fi.Name.LastIndexOf('.')).Replace(".", ":");
        //    objDayLog ret = new objDayLog { Date = DateTime.Parse(n) };

        //    foreach (string ln in System.IO.File.ReadAllLines(fi.FullName))
        //    {
        //        if (string.IsNullOrEmpty(ln))
        //        {
        //            continue;
        //        }

        //        string[] s = ln.Split('\t');
        //        ret.Logs.Add(new objLog
        //        {
        //            Time = DateTime.Parse(s[0]),
        //            Description = s[1]
        //        });
        //    }

        //    return ret;
        //}

        public ActionResult UpdateProgress(int? year, bool force = false)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
            }

            using (coreApp.Areas.Procurement.DAL.procurementDataContext context = new Procurement.DAL.procurementDataContext())
            {
                Dictionary<string, object> errors = new Dictionary<string, object>();

                temporaryCache cache = new temporaryCache(year.Value);

                var tmp = context.tblPPMPs.Where(x => x.Year == year.Value && (force || x._perc == null || x._wperc == null)).ToList();
                
                
                tmp.ForEach(x =>
                {
                    try
                    {
                        x.UpdateProgress(cache);
                    }
                    catch (Exception e)
                    {
                        errors.Add(x.Id.ToString(), e);
                    }

                    
                });

                if (errors.Any())
                {
                    return Content(string.Join("<br />", errors.Select(x => x.Key + " - " + ((Exception)x.Value).Message)));
                } 
                else
                {
                    return Content("done");
                }
            }
        }

        public ActionResult UpdateDocNo_LogStatus(int? year)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
            }

            using (coreApp.Areas.Procurement.DAL.procurementDataContext context = new Procurement.DAL.procurementDataContext())
            {
                using (coreApp.Areas.SAM.samDataContext _context = new SAM.samDataContext())
                {
                    Dictionary<string, object> errors = new Dictionary<string, object>();

                    int id = -1;

                    try
                    {
                        var tmp = context.tblPPMPs.Where(x => x.Year == year.Value);

                        
                        
                        foreach (var x in tmp)
                        {
                            id = x.Id;
                            x.DocNo = x.getDocNo;

                            Log docLog = x.docLog;
                            objLogs l = docLog.Logs();

                            if (l.logs.Any())
                            {
                                x._LogStatus = Newtonsoft.Json.JsonConvert.SerializeObject(docLog.GetStates(l));
                                x._LogStatusVal = l.Current.LogType;
                            }

                            
                        }
                    }
                    catch (Exception e)
                    {
                        errors.Add("PPMP " + id.ToString(), e);
                    }

                    try
                    {
                        var tmp = context.tblPPMP_Trails.Where(x => x.Year == year.Value);
                        
                        
                        foreach (var x in tmp)
                        {
                            id = x.Id;
                            x.DocNo = x.getDocNo;

                            Log docLog = x.docLog;
                            objLogs l = docLog.Logs();

                            if (l.logs.Any())
                            {
                                x._LogStatus = Newtonsoft.Json.JsonConvert.SerializeObject(docLog.GetStates(l));
                                x._LogStatusVal = l.Current.LogType;
                            }

                            
                        }
                    }
                    catch (Exception e)
                    {
                        errors.Add("PPMP Trail " + id.ToString(), e);
                    }
                    try
                    {
                        var tmp = context.tblAPPs.Where(x => x.Year == year.Value);
                        
                        
                        foreach (var x in tmp)
                        {
                            id = x.Id;
                            x.DocNo = x.getDocNo;

                            Log docLog = x.docLog;
                            objLogs l = docLog.Logs();

                            if (l.logs.Any())
                            {
                                x._LogStatus = Newtonsoft.Json.JsonConvert.SerializeObject(docLog.GetStates(l));
                                x._LogStatusVal = l.Current.LogType;
                            }

                            
                        }
                    }
                    catch (Exception e)
                    {
                        errors.Add("APP " + id.ToString(), e);
                    }
                    try
                    {
                        var tmp = context.tblAPRs.Where(x => x.Year == year.Value);
                        
                        
                        foreach (var x in tmp)
                        {
                            id = x.Id;
                            x.DocNo = x.getDocNo;

                            Log docLog = x.docLog;
                            objLogs l = docLog.Logs();

                            if (l.logs.Any())
                            {
                                x._LogStatus = Newtonsoft.Json.JsonConvert.SerializeObject(docLog.GetStates(l));
                                x._LogStatusVal = l.Current.LogType;
                            }

                            
                        }
                    }
                    catch (Exception e)
                    {
                        errors.Add("APR " + id.ToString(), e);
                    }
                    try
                    {
                        var tmp = context.tblPRs.Where(x => x.Year == year.Value);
                        
                        
                        foreach (var x in tmp)
                        {
                            id = x.Id;
                            x.DocNo = x.getDocNo;

                            Log docLog = x.docLog;
                            objLogs l = docLog.Logs();

                            if (l.logs.Any())
                            {
                                x._LogStatus = Newtonsoft.Json.JsonConvert.SerializeObject(docLog.GetStates(l));
                                x._LogStatusVal = l.Current.LogType;
                            }

                            
                        }
                    }
                    catch (Exception e)
                    {
                        errors.Add("PR " + id.ToString(), e);
                    }
                    try
                    {
                        var tmp = context.tblCPRs.Where(x => x.Year == year.Value);
                        
                        
                        foreach (var x in tmp)
                        {
                            id = x.Id;
                            x.DocNo = x.getDocNo;

                            Log docLog = x.docLog;
                            objLogs l = docLog.Logs();

                            if (l.logs.Any())
                            {
                                x._LogStatus = Newtonsoft.Json.JsonConvert.SerializeObject(docLog.GetStates(l));
                                x._LogStatusVal = l.Current.LogType;
                            }

                            
                        }
                    }
                    catch (Exception e)
                    {
                        errors.Add("CPR " + id.ToString(), e);
                    }
                    try
                    {
                        var tmp = context.tblRFQs.Where(x => x.Year == year.Value);
                        
                        
                        foreach (var x in tmp)
                        {
                            id = x.Id;
                            x.DocNo = x.getDocNo;

                            Log docLog = x.docLog;
                            objLogs l = docLog.Logs();

                            if (l.logs.Any())
                            {
                                x._LogStatus = Newtonsoft.Json.JsonConvert.SerializeObject(docLog.GetStates(l));
                                x._LogStatusVal = l.Current.LogType;
                            }

                            
                        }
                    }
                    catch (Exception e)
                    {
                        errors.Add("RFQ " + id.ToString(), e);
                    }
                    try
                    {
                        var tmp = context.tblAOBs.Where(x => x.Year == year.Value);
                        
                        
                        foreach (var x in tmp)
                        {
                            id = x.Id;
                            x.DocNo = x.getDocNo;

                            Log docLog = x.docLog;
                            objLogs l = docLog.Logs();

                            if (l.logs.Any())
                            {
                                x._LogStatus = Newtonsoft.Json.JsonConvert.SerializeObject(docLog.GetStates(l));
                                x._LogStatusVal = l.Current.LogType;
                            }

                            
                        }
                    }
                    catch (Exception e)
                    {
                        errors.Add("AOB " + id.ToString(), e);
                    }
                    try
                    {
                        var tmp = context.tblPOs.Where(x => x.Year == year.Value);
                        
                        
                        foreach (var x in tmp)
                        {
                            id = x.Id;
                            x.DocNo = x.getDocNo;

                            Log docLog = x.docLog;
                            objLogs l = docLog.Logs();

                            if (l.logs.Any())
                            {
                                x._LogStatus = Newtonsoft.Json.JsonConvert.SerializeObject(docLog.GetStates(l));
                                x._LogStatusVal = l.Current.LogType;
                            }

                            
                        }
                    }
                    catch (Exception e)
                    {
                        errors.Add("PO " + id.ToString(), e);
                    }
                    try
                    {
                        var tmp = _context.tblBacklogs.Where(x => x.Year == year.Value);
                        
                        
                        foreach (var x in tmp)
                        {
                            id = x.Id;
                            x.DocNo = x.getDocNo;

                            Log docLog = x.docLog;
                            objLogs l = docLog.Logs();

                            if (l.logs.Any())
                            {
                                x._LogStatus = Newtonsoft.Json.JsonConvert.SerializeObject(docLog.GetStates(l));
                                x._LogStatusVal = l.Current.LogType;
                            }

                            
                        }
                    }
                    catch (Exception e)
                    {
                        errors.Add("BL " + id.ToString(), e);
                    }
                    try
                    {
                        var tmp = _context.tblDeliveries.Where(x => x.Year == year.Value);
                        
                        
                        foreach (var x in tmp)
                        {
                            id = x.Id;
                            x.DocNo = x.getDocNo;

                            Log docLog = x.docLog;
                            objLogs l = docLog.Logs();

                            if (l.logs.Any())
                            {
                                x._LogStatus = Newtonsoft.Json.JsonConvert.SerializeObject(docLog.GetStates(l));
                                x._LogStatusVal = l.Current.LogType;
                            }

                            
                        }
                    }
                    catch (Exception e)
                    {
                        errors.Add("DEL " + id.ToString(), e);
                    }

                    context.SubmitChanges();
                    _context.SubmitChanges();

                    if (errors.Any())
                    {
                        return Content(string.Join("<br />", errors.Select(x => x.Key + " - " + ((Exception)x.Value).Message)));
                    }
                    else
                    {
                        return Content("done");
                    }
                }
            }
        }
    }
}