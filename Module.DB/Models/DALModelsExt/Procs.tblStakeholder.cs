using System;
using System.Linq;

namespace Module.DB.Procs
{
    public class procs_tblStakeholder
    {
        tblStakeholder stakeholder;

        public procs_tblStakeholder(tblStakeholder employee)
        {
            this.stakeholder = employee;
        }

        public bool IsInCountry(int countryId)
        {
            bool ret = countryId == -1;

            if (!ret)
            {
                ret = stakeholder.Home_CountryId == countryId;
            }
            return ret;
        }

        public bool IsInProvince(int provinceId)
        {
            bool ret = provinceId == -1;

            if (!ret)
            {
                if (stakeholder.vwCity != null)
                {
                    ret = stakeholder.vwCity.ProvinceId == provinceId;
                }
            }
            return ret;
        }

        public bool IsInCity(int cityId)
        {
            bool ret = cityId == -1;

            if (!ret)
            {
                if (stakeholder.vwCity != null)
                {
                    ret = stakeholder.vwCity.Id == cityId;
                }
            }
            return ret;
        }

        public bool IsInBrgy(int brgyId)
        {
            bool ret = brgyId == -1;

            if (!ret)
            {
                ret = stakeholder.Home_BrgyId == brgyId;
            }
            return ret;
        }

        public virtual tblUserAccess Access()
        {
            return Common.Global_CachedTables.UserAccess.Where(x => x.StakeholderId == stakeholder.StakeholderId).SingleOrDefault() ?? new tblUserAccess { StakeholderId = stakeholder.StakeholderId };
        }
    }
}
