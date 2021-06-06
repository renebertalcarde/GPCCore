using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Module.DB.Interfaces
{
    public interface ICacheTables
    {
        List<tblSetting> BaseSettings { get; set; }
        AddressData ADDR { get; set; }
        List<tblTMSetting> TMSettings { get; set; }
        List<tblTMSettings_Effectivity> TMSettings_Effectivities { get; set; }
    }

    public interface ICache
    {
        DateTime cacheDate { get; set; }
        
    }

}
