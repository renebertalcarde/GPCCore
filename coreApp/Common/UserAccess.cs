using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using Module.DB;
using Module.DB.Procs;

namespace coreApp
{
    public class UserAccess
    {
        public string userName { get; set; }
        public DateTime? asOfDate { get; set; }

        public AspNetUser user { get; set; }
        public tblStakeholder stakeholder { get; set; }
        public string[] roles { get; set; } = new List<string>().ToArray();

        private tblUserAccess access { get; set; }

       
        public bool HasAnyAccess
        {
            get
            {
                bool ret = false;

                if (access != null)
                {

                    PropertyInfo[] pis = access.GetType().GetProperties();

                    foreach (PropertyInfo pi in pis)
                    {
                        if ("AccessId,StakeholderId".Split(',').Contains(pi.Name)) continue;

                        if (Convert.ToBoolean(pi.GetValue(access)))
                        {
                            ret = true;
                            break;
                        }
                    }
                }

                return ret;
            }
        }

        public bool HasAccess(string accessType)
        {
            if (IsAdmin)
            {
                return true;
            }
            else
            {
                return checkAccess(accessType);
            }
        }
        
        public bool IsAdmin
        {
            get
            {
                return roles.Contains("admin") || checkAccess("system_admin");
            }
        }


        private bool checkAccess(string accessType)
        {
            if (access == null)
            {
                return false;
            }
            else
            {
                PropertyInfo pi = access.GetType().GetProperty(accessType);
                if (pi == null)
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(pi.GetValue(access));
                }                
            }
        }

        public bool IsStaff
        {
            get
            {
                return roles.Contains("staff");
            }
        }

        public UserAccess(tblStakeholder stakeholder, DateTime? asOfDate = null)
        {
            user = Module.DB.Procs.Account.GetUser(id: stakeholder.UserId);
            init(asOfDate);
        }

        public UserAccess(string _userName = "", DateTime? asOfDate = null)
        {
            user = Module.DB.Procs.Account.GetUser(userName: _userName == "" ? HttpContext.Current.User.Identity.Name : _userName);
            init(asOfDate);
        }

        private void init(DateTime? asOfDate)
        {
            this.asOfDate = asOfDate;

            if (user != null)
            {
                userName = user.UserName;

                using (dalDataContext context = new dalDataContext())
                {
                    roles = context.AspNetUserRoles.Where(x => x.UserId == user.Id)
                            .Join(context.AspNetRoles, a => a.RoleId, b => b.Id, (a, b) => b.Name).ToArray();

                    stakeholder = context.tblStakeholders.Where(x => x.UserId == user.Id).SingleOrDefault();
                    if (stakeholder != null)
                    {
                        procs_tblStakeholder pStakeholder = new procs_tblStakeholder(stakeholder);



                        access = context.tblUserAccesses.Where(x => x.StakeholderId == stakeholder.StakeholderId).SingleOrDefault();
                        if (access == null)
                        {
                            access = new tblUserAccess { StakeholderId = stakeholder.StakeholderId };
                        }
                    }
                }
            }
        }
        

        public bool IsMe(int stakeholderId)
        {
            return stakeholder.StakeholderId == stakeholderId;
        }

        public bool AllowedStakeholder(int stakeholderId, bool forScheduling = false, bool disableMe = false, bool forLeave = false)
        {
            return true;
        }

    }
}