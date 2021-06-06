using System;
using System.Linq;

namespace Module.DB.Procs
{
    public class offline_procs_tblStakeholder : procs_tblStakeholder
    {
        OfflineDataSource offlineDS;
        bool useOffline
        {
            get
            {
                return offlineDS != null;
            }
        }

        tblStakeholder stakeholder;

        public offline_procs_tblStakeholder(tblStakeholder stakeholder, ref OfflineDataSource offlineDS) : base(stakeholder)
        {
            this.offlineDS = offlineDS;
            this.stakeholder = stakeholder;
        }


        public override tblUserAccess Access()
        {
            return offlineDS.UserAccess.Where(x => x.StakeholderId == stakeholder.StakeholderId).SingleOrDefault() ?? new tblUserAccess { StakeholderId = stakeholder.StakeholderId };
        }
    }
}
