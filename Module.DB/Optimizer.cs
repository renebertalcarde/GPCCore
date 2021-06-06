using Module.DB.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Module.DB
{    
    public class cachedTables : ICache, ICacheTables
    {
        OfflineDataSource offlineDS;
        bool useOffline
        {
            get
            {
                return offlineDS != null;
            }
        }

        public DateTime cacheDate { get; set; }

        public List<tblSetting> BaseSettings { get; set; } = new List<tblSetting>();
        public AddressData ADDR { get; set; } = new AddressData();
        public List<tblTMSetting> TMSettings { get; set; } = new List<tblTMSetting>();
        public List<tblTMSettings_Effectivity> TMSettings_Effectivities { get; set; } = new List<tblTMSettings_Effectivity>();
        public List<tblUserAccess> UserAccess { get; set; } = new List<tblUserAccess>();

        public List<UserPhoto> UserPhotos { get; set; } = new List<UserPhoto>();

        public cachedTables(OfflineDataSource offlineDS = null)
        {
            this.offlineDS = offlineDS;
            Refresh();
        }

        public void Refresh(bool _settings = false, bool _addr = false, bool _tmsettings = false, bool _tmsettings_effectivities = false, bool _user_access = false, bool _user_photos = false)
        {
            bool hasSpecified = _settings || _addr || _tmsettings || _tmsettings_effectivities || _user_access || _user_photos;

            if (useOffline)
            {
                if (!hasSpecified || _settings) BaseSettings = offlineDS.BaseSettings;
                if (!hasSpecified || _addr) ADDR = offlineDS.ADDR;
                if (!hasSpecified || _tmsettings) TMSettings = offlineDS.TMSettings;
                if (!hasSpecified || _tmsettings_effectivities) TMSettings_Effectivities = offlineDS.TMSettings_Effectivities;
                if (!hasSpecified || _user_access) UserAccess = offlineDS.UserAccess;
                if (!hasSpecified || _user_photos) UserPhotos = offlineDS.UserPhotos;

                cacheDate = DateTime.Now;
            }
            else
            {
                using (dalDataContext context = new dalDataContext())
                {
                    if (!hasSpecified || _settings) BaseSettings = context.tblSettings.OrderBy(x => x.Setting).ToList();
                    if (!hasSpecified || _addr) ADDR.Refresh();
                    if (!hasSpecified || _tmsettings) TMSettings = context.tblTMSettings.ToList();
                    if (!hasSpecified || _tmsettings_effectivities) TMSettings_Effectivities = context.tblTMSettings_Effectivities.ToList();
                    if (!hasSpecified || _user_access) UserAccess = context.tblUserAccesses.ToList();
                    if (!hasSpecified || _user_photos) UserPhotos = context.tblUserPhotos.Select(x => new UserPhoto { StakeholderId = x.StakeholderId, PhotoString = x.GetPhotoString() }).ToList();
                    cacheDate = context.getDateTime().Value;
                }
                
            }
        }
    }
}
