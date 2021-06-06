using Module.DB;
using Module.DB.Procs;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace coreApp.Models
{
    public class EmployeeSearchCriteria
    {
        public string lastName = "";
        public string firstName = "";
        public int mfoId = -1;
        public string departmentIds = "";
        public int positionId = -1;
        public int groupId = -1;
        public int employmentType = -1;
        public bool excludeNoCareer = false;
        public bool excludeNoOffice = false;
        public string altSource = "";
        public string active = "";
        public string nodepartment = "";
        public bool forScheduling = false;
        public bool includeLocations = false;
        public int country = -1;
        public int province = -1;
        public int city = -1;
        public int brgy = -1;

        public int[] deptIds
        {
            get
            {
                int[] ret = new int[] { };

                if (!string.IsNullOrEmpty(departmentIds))
                {
                    ret = departmentIds.Split(',').Select(x => int.Parse(x)).ToArray();
                }

                return ret;
            }
        }
    }

    public class EmployeeSearch
    {
        public string altSource { get; set; } = "";

        public EmployeeSearch(string altSource = "")
        {
            this.altSource = altSource;
        }

        public EmployeeSearchCriteria criteria { get; set; }

        public EmployeeSearch()
        { }

        public EmployeeSearch(string altSource = "", string lastName = "", string firstName = "", int mfoId = -1, string departmentIds = "", int positionId = -1, int groupId = -1, int employmentType = -1, bool excludeNoCareer = false, bool excludeNoOffice = false, string active = "", bool forScheduling = false, string nodepartment = "", bool includeLocations = false, int country = -1, int province = -1, int city = -1, int brgy = -1)
        {
            criteria = new EmployeeSearchCriteria
            {
                lastName = lastName,
                firstName = firstName,
                mfoId = mfoId,
                departmentIds = departmentIds,
                positionId = positionId,
                groupId = groupId,
                employmentType = employmentType,
                excludeNoCareer = excludeNoCareer,
                excludeNoOffice = excludeNoOffice,
                active = active,
                nodepartment = nodepartment,
                forScheduling = forScheduling,
                includeLocations = includeLocations,
                country = country,
                province = province,
                city = city,
                brgy = brgy
            };
        }

        public List<tblEmployee> GetEmployees(string lastName = "", string firstName = "", int mfoId = -1, string departmentIds = "", int positionId = -1, int groupId = -1, int employmentType = -1, bool excludeNoCareer = false, bool excludeNoOffice = false, string active = "", bool forScheduling = false, string nodepartment = "", bool includeLocations = false, int country = -1, int province = -1, int city = -1, int brgy = -1)
        {
            criteria = new EmployeeSearchCriteria
            {
                lastName = lastName,
                firstName = firstName,
                mfoId = mfoId,
                departmentIds = departmentIds,
                positionId = positionId,
                groupId = groupId,
                employmentType = employmentType,
                excludeNoCareer = excludeNoCareer,
                excludeNoOffice = excludeNoOffice,
                active = active,
                nodepartment = nodepartment,
                forScheduling = forScheduling,
                includeLocations = includeLocations,
                country = country,
                province = province,
                city = city,
                brgy = brgy
            };

            return GetEmployees(criteria);
        }

        public List<tblEmployee> GetEmployees()
        {
            return GetEmployees(criteria);
        }

        public List<tblEmployee> GetEmployees(EmployeeSearchCriteria criteria)
        {
            List<tblEmployee> model = new List<tblEmployee>();

            using (dalDataContext context = new dalDataContext())
            {
                List<tblEmployee> filter1 = new List<tblEmployee>();

                if (string.IsNullOrEmpty(altSource))
                {
                    filter1 = context.tblEmployees.ToList();
                }
                else
                {
                    filter1 = context.tblEmployees.Where(x => altSource.Split(',').Contains(x.EmployeeId.ToString())).ToList();
                }

                filter1 = filter1
                    .Where(x =>
                    (string.IsNullOrEmpty(criteria.lastName) || x.LastName.ToLower().Contains(criteria.lastName.ToLower())) &&
                    (string.IsNullOrEmpty(criteria.firstName) || x.FirstName.ToLower().Contains(criteria.firstName.ToLower()))
                    )
                    .ToList();


                filter1.ForEach(x => x._tmpCareer = new procs_tblEmployee(x).LatestCareer());

                if (criteria.excludeNoCareer)
                {
                    filter1 = filter1.Where(x => x._tmpCareer != null).ToList();
                }

                if (criteria.excludeNoOffice || criteria.nodepartment == "exclude-no-dept")
                {
                    filter1 = filter1.Where(x => x._tmpCareer != null).ToList();
                    filter1 = filter1.Where(x => x._tmpCareer._Department != null).ToList();
                    filter1 = filter1.Where(x => new procs_tblEmployee(x).IsInDepartment(criteria.deptIds)).ToList();
                }
                else if (criteria.nodepartment == "include-no-dept")
                {
                    filter1 = filter1.Where(x => new procs_tblEmployee(x).IsInDepartment(criteria.deptIds, includeUnassigned: true)).ToList();
                }
                else if (criteria.nodepartment == "no-dept-only")
                {
                    filter1 = filter1.Where(x => new procs_tblEmployee(x).NoDepartment()).ToList();
                }
                else
                {
                    filter1 = filter1.Where(x => new procs_tblEmployee(x).IsInDepartment(criteria.deptIds)).ToList();
                }

                if (criteria.groupId != -1)
                {
                    filter1 = filter1.Where(x => new procs_tblEmployee(x).IsInGroup(criteria.groupId)).ToList();
                }

                if (criteria.mfoId != -1)
                {
                    filter1 = filter1.Where(x => new procs_tblEmployee(x).IsInMFO(criteria.mfoId)).ToList();
                }

                if (criteria.positionId != -1)
                {
                    filter1 = filter1.Where(x => new procs_tblEmployee(x).IsInPosition(criteria.positionId)).ToList();
                }

                if (criteria.employmentType != -1)
                {
                    filter1 = filter1.Where(x => new procs_tblEmployee(x).IsInEmploymentType(criteria.employmentType)).ToList();
                }

                if (criteria.includeLocations)
                {
                    if (criteria.country != -1)
                    {
                        filter1 = filter1.Where(x => new procs_tblEmployee(x).IsInCountry(criteria.country)).ToList();
                    }

                    if (criteria.province != -1)
                    {
                        filter1 = filter1.Where(x => new procs_tblEmployee(x).IsInProvince(criteria.province)).ToList();
                    }

                    if (criteria.city != -1)
                    {
                        filter1 = filter1.Where(x => new procs_tblEmployee(x).IsInCity(criteria.city)).ToList();
                    }

                    if (criteria.brgy != -1)
                    {
                        filter1 = filter1.Where(x => new procs_tblEmployee(x).IsInBrgy(criteria.brgy)).ToList();
                    }
                }

                UserAccess access = Cache.Get().userAccess;

                filter1 = filter1.Where(x => access.AllowedEmployee(x.EmployeeId, criteria.forScheduling)).ToList();

                if (criteria.active == "")
                {
                    model = filter1
                    .OrderBy(x => x.FullName)
                    .ToList();
                }
                else
                {
                    model = filter1
                    .Where(x => x.IsActive() == (criteria.active == "active"))
                    .OrderBy(x => x.FullName)
                    .ToList();
                }

                return model;
            }
        }
    }

    public class EmployeeSearchUIModel
    {
        public string Title { get; set; } = "Search Employees";
        public string DataUrl { get; set; }
        public bool MinimalView { get; set; }
        public bool MultiSelect { get; set; } = false;
        public bool ExcludeNoCareer { get; set; } = false;
        public bool ExcludeNoOffice { get; set; } = false;
        public bool IncludeLocations { get; set; } = false;
        public string AltSource { get; set; } = null;
        public bool ShowSearchBtn { get; set; } = true;
        public bool ShowResult { get; set; } = true;
        public bool LessParameters { get; set; } = true;
        public int[] SelectedItems { get; set; }
        public string SearchCallback { get; set; }
        public string SortCallback { get; set; }
        public bool AllowSingleSelection { get; set; } = true;
        public bool NoState { get; set; } = false;
        public bool NoPaging { get; set; } = false;
        public bool ForScheduling { get; set; } = false;
        public bool DisableShowPanelTrigger { get; set; } = false;
        public int[] InitialParam_DepartmentIds { get; set; } = new int[] { };
        public bool NoInitialSearch { get; set; } = false;

        public string strSelectedItems
        {
            get
            {
                string ret = "";

                if (SelectedItems != null)
                {
                    ret = string.Join(";", SelectedItems);
                }

                return ret;
            }
        }
    }
}