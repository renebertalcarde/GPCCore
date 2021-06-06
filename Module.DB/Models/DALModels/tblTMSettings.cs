using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Module.DB
{

    public partial class tblTMSettingsMetaData
    {
        [Display(Name = "Id")]
        public int SettingId { get; set; }

        [Display(Name = "Value")]
        public string SettingValue { get; set; }

        [Display(Name = "Value Type")]
        public string ValueType { get; set; }

    }

    [MetadataType(typeof(tblTMSettingsMetaData))]
    public partial class tblTMSetting
    {
        public DateTime DateUpdated { get; set; }

        public tblTMSetting Clone()
        {
            return new tblTMSetting
            {
                SettingId = this.SettingId,
                Setting = this.Setting,
                SettingValue = this.SettingValue,
                Category = this.Category,
                Description = this.Description,
                ValueType = this.ValueType
            };
        }

        public bool IsFx
        {
            get
            {
                return Category == "Functions";
            }
        }

        //public bool IsLogicalFx
        //{
        //    get
        //    {
        //        return IsFx && Setting.StartsWith("logic_");
        //    }
        //}
    }
}