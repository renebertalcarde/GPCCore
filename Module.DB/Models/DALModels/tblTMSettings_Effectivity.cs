using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Module.DB
{

    public partial class tblTMSettings_EffectivitiesMetaData
    { }
    [MetadataType(typeof(tblTMSettings_EffectivitiesMetaData))]
    public partial class tblTMSettings_Effectivity
    {
        public bool IsCustom
        {
            get
            {
                return Id != -1;
            }
        }

        public string Value
        {
            get
            {
                string ret = _Value;

                if (string.IsNullOrEmpty(_Value) && Setting != null)
                {
                    ret = Setting.SettingValue;
                }

                return ret;
            }
        }

        public tblTMSetting Setting
        {
            get
            {
                return Procs.Common.Global_CachedTables.TMSettings.Where(x => x.SettingId == SettingId).SingleOrDefault();
            }
        }
    }
}