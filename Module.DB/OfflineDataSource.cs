using System;
using System.Collections.Generic;
using System.Linq;

namespace Module.DB
{
    public class UserPhoto
    {
        public int StakeholderId;
        public string PhotoString;

        public byte[] Photo
        {
            get
            {
                if (string.IsNullOrEmpty(PhotoString))
                {
                    return null;
                }
                else
                {
                    return Convert.FromBase64String(PhotoString);
                }
            }
        }
    }

    public class OfflineDataSource
    {
        public DateTime Version;
        public string AgencyName;
        public List<AspNetUserRole> AspNetUserRoles;
        public List<tblStakeholder> Stakeholders;

        public List<tblUserAccess> UserAccess;
        public List<UserPhoto> UserPhotos = new List<UserPhoto>();

        public List<tblSetting> BaseSettings;
        public AddressData ADDR = new AddressData();
        public List<tblTMSetting> TMSettings;
        public List<tblTMSettings_Effectivity> TMSettings_Effectivities;
        public OfflineDataSource() { }

        public OfflineDataSource(bool fill, bool _roles = false, bool _stakeholders = false, bool _user_acess = false, bool _user_photos = false, 
            bool _basesettings = false, bool _addr = false, bool _tmsettings = false, bool _tmsettings_effectivities = false)
        {
            if (fill)
            {
                this.Fill(_roles, _stakeholders, _user_acess, _user_photos, 
            _basesettings, _addr, _tmsettings, _tmsettings_effectivities);
            }
        }

        public void Fill_Inverse(bool _roles = true, bool _stakeholders = true, bool _user_acess = true, bool _user_photos = true, 
            bool _basesettings = true, bool _addr = true, bool _tmsettings = true, bool _tmsettings_effectivities = true)
        {
            this.Fill(_roles, _stakeholders, _user_acess, _user_photos, 
            _basesettings, _addr, _tmsettings, _tmsettings_effectivities);
        }

        public void Fill(bool _roles = false, bool _stakeholders = false, bool _user_acess = false, bool _user_photos = false, 
            bool _basesettings = false, bool _addr = false, bool _tmsettings = false, bool _tmsettings_effectivities = false)
        {

            bool hasSpecified = _roles || _stakeholders || _user_acess || 
            _basesettings || _addr || _tmsettings || _tmsettings_effectivities || _user_photos; 

            using (dalDataContext context = new dalDataContext())
            {
                if (!hasSpecified || _roles) AspNetUserRoles = context.AspNetUserRoles.ToList();
                if (!hasSpecified || _stakeholders) Stakeholders = context.tblStakeholders.ToList();
                if (!hasSpecified || _user_acess) UserAccess = context.tblUserAccesses.ToList();
                if (!hasSpecified || _user_photos) UserPhotos = context.tblUserPhotos.Select(x => new UserPhoto { StakeholderId = x.StakeholderId, PhotoString = x.GetPhotoString() }).ToList();
                
                if (!hasSpecified || _basesettings) BaseSettings = context.tblSettings.OrderBy(x => x.Setting).ToList();
                if (!hasSpecified || _addr) ADDR.Refresh();
                if (!hasSpecified || _tmsettings) TMSettings = context.tblTMSettings.ToList();
                if (!hasSpecified || _tmsettings_effectivities) TMSettings_Effectivities = context.tblTMSettings_Effectivities.ToList();

                Version = context.getDateTime().Value;
            }
        }
    }
}