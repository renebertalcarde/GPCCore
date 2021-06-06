using System.Linq;
using System.Net;
using System.Web.Mvc;
using System;
using coreApp.Controllers;
using coreApp;
using coreLib.Objects;
using Module.DB;
using Module.Core;
using System.Collections.Generic;
using System.Data.Linq;

namespace coreApp.Areas.HRModule.Controllers
{
    [UserAccessAuthorize(allowedAccess: "hr_config")]
    public class OfficeStructureController : Base_NoCoreHRStaffController
    {
        public ActionResult Index()
        {
            OfficeStructureParameters model = new OfficeStructureParameters();
            return View(model);
        }

        public ActionResult Edit()
        {
            return View();
        }
        
        public ActionResult EditOfficeStructure()
        {
            OfficeStructureParameters model = new OfficeStructureParameters
            {
                Mode = Enums.OfficeStructureMode.Edit,
                RestrictAccess = false
            };

            return PartialView("_OfficeStructure", model);
        }

        public ActionResult ViewItem(string type, int id)
        {
            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            if (type == "branch")
            {
                tblBranch item = Cache.Get_Tables().Branches.Where(x => x.Id == id).SingleOrDefault();
                if (item == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Branch", item);
                }
            }
            else if (type == "office")
            {
                tblOffice item = Cache.Get_Tables().Offices.Where(x => x.OfficeId == id).SingleOrDefault();
                if (item == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Office", item);
                }
            }
            else
            {
                tblDepartment item = Cache.Get_Tables().Departments.Where(x => x.DepartmentId == id).SingleOrDefault();
                if (item == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Department", item);
                }
            }
        }

        public ActionResult CreateItem(string type, int parentId = -1)
        {
            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            if (type == "branch")
            {
                return PartialView("_Branch", new tblBranch());
            }
            else if (type == "office")
            {
                return PartialView("_Office", new tblOffice { BranchId = parentId });
            }
            else
            {
                return PartialView("_Department", new tblDepartment { OfficeId = parentId });
            }
        }

        [HttpPost]
        public JsonResult CreateBranch(tblBranch model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    if (context.tblBranches.Any(x => x.Description == model.Description))
                    {
                        AddError("Name already exists");
                    }

                    if (ModelState.IsValid)
                    {
                        tblBranch item = new tblBranch
                        {
                            Description = model.Description,
                            Address = model.Address,
                            CountryId = model.CountryId,
                            PostalCode = model.PostalCode,
                            RCC = model.RCC
                        };

                        context.tblBranches.InsertOnSubmit(item);                        
                        context.SubmitChanges();
                        Cache.Update_Tables(_branches: true);

                        res.Data = item;
                        res.Remarks = "Record was successfully added";

                        MvcApplication.HLLog.WriteLog("Office Structure", "Insert Branch",
                                 string.Format("Inserted record [{0}] {1}", item.Id, item.Description));
                    }
                    else
                    {
                        throw new Exception(coreProcs.ShowErrors(ModelState));
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

        [HttpPost]
        public JsonResult CreateOffice(tblOffice model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    if (context.tblOffices.Any(x => x.Office == model.Office))
                    {
                        AddError("Name already exists");
                    }

                    if (ModelState.IsValid)
                    {
                        tblOffice item = new tblOffice
                        {
                            BranchId = model.BranchId,
                            Office = model.Office,
                            TelephoneNo = model.TelephoneNo
                        };

                        context.tblOffices.InsertOnSubmit(item);
                        context.SubmitChanges();
                        Cache.Update_Tables(_offices: true);

                        res.Data = item;
                        res.Remarks = "Record was successfully added";

                        var data = new
                        {
                            Branch = string.Format("[{0}] {1}", item.Branch.Id, item.Branch.Description)
                        };

                        MvcApplication.HLLog.WriteLog("Office Structure", "Insert Office",
                                 string.Format("Inserted record [{0}] {1}", item.OfficeId, item.Office),
                                 data);
                    }
                    else
                    {
                        throw new Exception(coreProcs.ShowErrors(ModelState));
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

        [HttpPost]
        public JsonResult CreateDepartment(tblDepartment model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    if (context.tblDepartments.Any(x => x.Department == model.Department))
                    {
                        AddError("Name already exists");
                    }

                    if (ModelState.IsValid)
                    {
                        tblDepartment item = new tblDepartment
                        {
                            OfficeId = model.OfficeId,
                            Department = model.Department
                        };

                        context.tblDepartments.InsertOnSubmit(item);
                        context.SubmitChanges();
                        Cache.Update_Tables(_departments: true);

                        res.Data = item;
                        res.Remarks = "Record was successfully added";

                        var data = new
                        {
                            Branch = string.Format("[{0}] {1}", item.Office.Branch.Id, item.Office.Branch.Description),
                            Office = string.Format("[{0}] {1}", item.Office.OfficeId, item.Office.Office),
                        };

                        MvcApplication.HLLog.WriteLog("Office Structure", "Insert Department",
                                 string.Format("Inserted record [{0}] {1}", item.DepartmentId, item.Department),
                                 data);
                    }
                    else
                    {
                        throw new Exception(coreProcs.ShowErrors(ModelState));
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

        public ActionResult EditItem(string type, int id)
        {
            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            if (type == "branch")
            {
                tblBranch item = Cache.Get_Tables().Branches.Where(x => x.Id == id).Single();
                if (item == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Branch", item);
                }
            }
            else if (type == "office")
            {
                tblOffice item = Cache.Get_Tables().Offices.Where(x => x.OfficeId == id).Single();
                if (item == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Office", item);
                }
            }
            else
            {
                tblDepartment item = Cache.Get_Tables().Departments.Where(x => x.DepartmentId == id).Single();
                if (item == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Department", item);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBranch(tblBranch model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {

                    tblBranch item = context.tblBranches.Where(x => x.Id == model.Id).SingleOrDefault();
                    if (item == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }

                    if (context.tblBranches.Any(x => x.Id != model.Id && x.Description == model.Description))
                    {
                        AddError("Name already exists");
                    }

                    if (ModelState.IsValid)
                    {
                        item.Description = model.Description;
                        item.Address = model.Address;
                        item.CountryId = model.CountryId;
                        item.PostalCode = model.PostalCode;
                        item.RCC = model.RCC;

                        ModifiedMemberInfo[] mmi = context.tblBranches.GetModifiedMembers(item);
                        context.SubmitChanges();

                        Cache.Update_Tables(_branches: true, _offices: true, _departments: true);

                        res.Data = item;
                        res.Remarks = "Record was successfully updated";

                        object data = mmi.Select(x => new { FieldName = x.Member.Name, Orig = x.OriginalValue, Current = x.CurrentValue });

                        MvcApplication.HLLog.WriteLog("Office Structure", "Update Branch",
                                 string.Format("Updated record [{0}] {1}", item.Id, item.Description),
                                 data);
                    }
                    else
                    {
                        throw new Exception(coreProcs.ShowErrors(ModelState));
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOffice(tblOffice model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {

                    tblOffice item = context.tblOffices.Where(x => x.OfficeId == model.OfficeId).SingleOrDefault();
                    if (item == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }

                    if (context.tblOffices.Any(x => x.OfficeId != model.OfficeId && x.Office == model.Office))
                    {
                        AddError("Name already exists");
                    }

                    if (ModelState.IsValid)
                    {
                        item.Office = model.Office;
                        item.TelephoneNo = model.TelephoneNo;

                        ModifiedMemberInfo[] mmi = context.tblOffices.GetModifiedMembers(item);
                        context.SubmitChanges();

                        Cache.Update_Tables(_offices: true, _departments: true);

                        res.Data = item;
                        res.Remarks = "Record was successfully updated";

                        var data = new
                        {
                            Branch = string.Format("[{0}] {1}", item.Branch.Id, item.Branch.Description),
                            Changes = mmi.Select(x => new { FieldName = x.Member.Name, Orig = x.OriginalValue, Current = x.CurrentValue })
                        };

                        MvcApplication.HLLog.WriteLog("Office Structure", "Update Office",
                                 string.Format("Updated record [{0}] {1}", item.OfficeId, item.Office),
                                 data);
                    }
                    else
                    {
                        throw new Exception(coreProcs.ShowErrors(ModelState));
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDepartment(tblDepartment model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {

                    tblDepartment item = context.tblDepartments.Where(x => x.DepartmentId == model.DepartmentId).SingleOrDefault();
                    if (item == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }

                    if (context.tblDepartments.Any(x => x.DepartmentId != model.DepartmentId && x.Department == model.Department))
                    {
                        AddError("Name already exists");
                    }

                    if (ModelState.IsValid)
                    {
                        item.Department = model.Department;

                        ModifiedMemberInfo[] mmi = context.tblDepartments.GetModifiedMembers(item);
                        context.SubmitChanges();

                        Cache.Update_Tables(_departments: true);

                        res.Data = item;
                        res.Remarks = "Record was successfully updated";

                        var data = new
                        {
                            Branch = string.Format("[{0}] {1}", item.Office.Branch.Id, item.Office.Branch.Description),
                            Office = string.Format("[{0}] {1}", item.Office.OfficeId, item.Office.Office),
                            Changes = mmi.Select(x => new { FieldName = x.Member.Name, Orig = x.OriginalValue, Current = x.CurrentValue })
                        };

                        MvcApplication.HLLog.WriteLog("Office Structure", "Update Department",
                                 string.Format("Updated record [{0}] {1}", item.DepartmentId, item.Department),
                                 data);
                    }
                    else
                    {
                        throw new Exception(coreProcs.ShowErrors(ModelState));
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

        [HttpPost]
        public ActionResult DeleteBranch(int id)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    tblBranch item = context.tblBranches.Where(x => x.Id == id).SingleOrDefault();
                    if (item == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }

                    if (ModelState.IsValid)
                    {
                        foreach(vwEmployeeCareer e in item.GetEmployees())
                        {
                            tblEmployee_Career career = context.tblEmployee_Careers.Where(x => x.EmployeeId == e.EmployeeId).OrderByDescending(x => x.DateEffective).ThenByDescending(x => x.CareerId).FirstOrDefault();
                            if (career != null)
                            {
                                career.DepartmentId = -1;
                            }
                        }

                        context.tblBranches.DeleteOnSubmit(item);
                        context.SubmitChanges();

                        Cache.Update_Tables(_branches: true, _offices: true, _departments: true);

                        res.Data = id;
                        res.Remarks = "Record was successfully deleted";
                        
                        MvcApplication.HLLog.WriteLog("Office Structure", "Delete Branch",
                                 string.Format("Deleted record [{0}] {1}", item.Id, item.Description));
                    }
                    else
                    {
                        throw new Exception(coreProcs.ShowErrors(ModelState));
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

        [HttpPost]
        public ActionResult DeleteOffice(int id)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    tblOffice item = context.tblOffices.Where(x => x.OfficeId == id).SingleOrDefault();
                    if (item == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }

                    if (ModelState.IsValid)
                    {
                        foreach (vwEmployeeCareer e in item.GetEmployees())
                        {
                            tblEmployee_Career career = context.tblEmployee_Careers.Where(x => x.EmployeeId == e.EmployeeId).OrderByDescending(x => x.DateEffective).ThenByDescending(x => x.CareerId).FirstOrDefault();
                            if (career != null)
                            {
                                career.DepartmentId = -1;
                            }
                        }

                        context.tblOffices.DeleteOnSubmit(item);
                        context.SubmitChanges();

                        Cache.Update_Tables(_offices: true, _departments: true);

                        res.Data = id;
                        res.Remarks = "Record was successfully deleted";

                        var data = new
                        {
                            Branch = string.Format("[{0}] {1}", item.Branch.Id, item.Branch.Description)
                        };

                        MvcApplication.HLLog.WriteLog("Office Structure", "Delete Office",
                                 string.Format("Deleted record [{0}] {1}", item.OfficeId, item.Office),
                                 data);
                    }
                    else
                    {
                        throw new Exception(coreProcs.ShowErrors(ModelState));
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

        [HttpPost]
        public ActionResult DeleteDepartment(int id)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    tblDepartment item = context.tblDepartments.Where(x => x.DepartmentId == id).SingleOrDefault();
                    if (item == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }

                    if (ModelState.IsValid)
                    {
                        foreach (vwEmployeeCareer e in item.GetEmployees())
                        {
                            tblEmployee_Career career = context.tblEmployee_Careers.Where(x => x.EmployeeId == e.EmployeeId).OrderByDescending(x => x.DateEffective).ThenByDescending(x => x.CareerId).FirstOrDefault();
                            if (career != null)
                            {
                                career.DepartmentId = -1;
                            }
                        }

                        context.tblDepartments.DeleteOnSubmit(item);
                        context.SubmitChanges();

                        Cache.Update_Tables(_departments: true);

                        res.Data = id;
                        res.Remarks = "Record was successfully deleted";

                        var data = new
                        {
                            Branch = string.Format("[{0}] {1}", item.Office.Branch.Id, item.Office.Branch.Description),
                            Office = string.Format("[{0}] {1}", item.Office.OfficeId, item.Office.Office)
                        };

                        MvcApplication.HLLog.WriteLog("Office Structure", "Delete Department",
                                 string.Format("Deleted record [{0}] {1}", item.DepartmentId, item.Department),
                                 data);
                    }
                    else
                    {
                        throw new Exception(coreProcs.ShowErrors(ModelState));
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
    }
}