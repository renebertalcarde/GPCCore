using Module.DB.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Module.DB
{
    public static class TMSettings
    {
        public const string DEFAULT_EFFECTIVITY = "DEFAULT";

        public static List<tblTMSetting> Get(string asOf, int officeId)
        {
            if (asOf == DEFAULT_EFFECTIVITY)
            {
                return Procs.Common.Global_CachedTables.TMSettings.ToList();
            }
            else
            {
                DateTime dt = Convert.ToDateTime(asOf);

                return Get(dt, officeId);
            }
        }

        public static List<tblTMSetting> Get(DateTime asOf, int officeId)
        {
            List<tblTMSetting> ret = Module.DB.Procs.Common.Global_CachedTables.TMSettings.Select(x => x.Clone()).ToList();

            bool inTable = Module.DB.Procs.Common.Global_CachedTables.TMSettings_Effectivities.Any(x => x.OfficeId == officeId);

            List<DateTime> dates = Module.DB.Procs.Common.Global_CachedTables.TMSettings_Effectivities
                .Where(x => ((officeId == -1 || !inTable) && x.OfficeId == null) || (officeId != -1 && x.OfficeId == officeId))
                .Select(x => x.DateEffective)
                .Distinct()
                .OrderByDescending(x => x)
                .Where(x => x <= asOf)
                .ToList();

            if (dates.Count == 0)
            {
                return ret;
            }

            foreach (tblTMSetting setting in ret)
            {
                foreach (DateTime _dt in dates)
                {
                    tblTMSettings_Effectivity eff = Module.DB.Procs.Common.Global_CachedTables.TMSettings_Effectivities
                        .Where(x => (((officeId == -1 || !inTable) && x.OfficeId == null) || (officeId != -1 && x.OfficeId == officeId)) && x.DateEffective == _dt && x.SettingId == setting.SettingId)
                        .OrderByDescending(x => x.OfficeId)
                        .FirstOrDefault();

                    if (eff != null)
                    {
                        setting.SettingValue = eff._Value;
                        setting.DateUpdated = _dt;
                        break;
                    }
                }
            }

            return ret;
        }
    }

    public class TimeSettingsModel
    {
        public bool ApplyGSIS { get; set; }
        public bool ApplySSS { get; set; }
        public bool ApplyPH { get; set; }
        public bool ApplyPAGIBIG { get; set; }
        public bool ApplyWTAX { get; set; }
        public double WorkHoursPerDay { get; set; }
        public double WorkDaysPerYear { get; set; }
        public double WorkingDaysPerMonth { get; set; }
        public double FixedPHAmountForJO { get; set; }

        public int LateRoundOff { get; set; }
        public int UTRoundOff { get; set; }

        //Rates
        public DateTime NightPremium_Start { get; set; }
        public double NightPremium_Duration { get; set; }
        public int SundayRequiresXDaysWork { get; set; }

        public double NightPremium { get; set; } = 0;
        public double RestDay { get; set; } = 0;
        public double SpecialAndRegularHoliday { get; set; } = 0;
        public double SpecialAndRegularHoliday_RestDay { get; set; } = 0;
        public double SpecialHoliday { get; set; } = 0;
        public double SpecialHoliday_RestDay { get; set; } = 0;
        public double RegularHoliday { get; set; } = 0;
        public double RegularHoliday_RestDay { get; set; } = 0;
        public double OT { get; set; } = 0;
        public double OT_Special { get; set; } = 0;
        public double Sunday { get; set; } = 0;


        public double FlexiTime { get; set; }
        public double LateTolerance { get; set; }
        public double UTTolerance { get; set; }
        public bool AutoChargeAbsLateUTToVL { get; set; }
        public double MinNetNotification { get; set; }
        public double ACAPERA { get; set; }
        public double AgencyShareWTaxRate { get; set; }
        public double AgencyShareMaxDays { get; set; }
        public bool Faculty40_Enable { get; set; }
        public double Faculty40_MinHoursPerDay { get; set; }
        public bool EnableWorkspanUndertime { get; set; }


        public double FixedLatePenalty { get; set; }
        
        public bool CommissionIsTaxable { get; set; }
        public bool UsePAGIBIG_Percentage { get; set; }
        public bool RHRequirePrevDayAttendance { get; set; }

        public bool UseWorkingDaysPerMonth { get { return WorkingDaysPerMonth > 0; } }
        public bool UseFixedPHAmountForJO { get { return FixedPHAmountForJO > 0; } }
        public bool UseFixedLatePenalty { get { return FixedLatePenalty > 0; } }

        public double HalfDayWork
        {
            get
            {
                return WorkHoursPerDay / 2;
            }
        }

        public List<tblTMSetting> settings { get; set; }

        public TimeSettingsModel()
        { }

        public TimeSettingsModel(DateTime asOf, int officeId)
        {
            this.settings = TMSettings.Get(asOf, officeId);
            init();
        }

        public TimeSettingsModel(List<tblTMSetting> settings)
        {
            this.settings = settings;
            init();
        }

        void init()
        {
            //common
            WorkHoursPerDay = Convert.ToDouble(settings.Where(x => x.Setting == "WorkHoursPerDay").Single().SettingValue);
            WorkDaysPerYear = Convert.ToDouble(settings.Where(x => x.Setting == "WorkDaysPerYear").Single().SettingValue);
            WorkingDaysPerMonth = Convert.ToDouble(settings.Where(x => x.Setting == "WorkingDaysPerMonth").Single().SettingValue);

            //rates
            NightPremium = Convert.ToDouble(settings.Where(x => x.Setting == "NightPremium").Single().SettingValue);
            NightPremium_Start = Convert.ToDateTime(settings.Where(x => x.Setting == "NightPremium_Start").Single().SettingValue);
            NightPremium_Duration = Convert.ToDouble(settings.Where(x => x.Setting == "NightPremium_Duration").Single().SettingValue);
            RestDay = Convert.ToDouble(settings.Where(x => x.Setting == "RestDay").Single().SettingValue);
            SpecialAndRegularHoliday = Convert.ToDouble(settings.Where(x => x.Setting == "SpecialAndRegularHoliday").Single().SettingValue);
            SpecialAndRegularHoliday_RestDay = Convert.ToDouble(settings.Where(x => x.Setting == "SpecialAndRegularHoliday_RestDay").Single().SettingValue);
            SpecialHoliday = Convert.ToDouble(settings.Where(x => x.Setting == "SpecialHoliday").Single().SettingValue);
            SpecialHoliday_RestDay = Convert.ToDouble(settings.Where(x => x.Setting == "SpecialHoliday_RestDay").Single().SettingValue);
            RegularHoliday = Convert.ToDouble(settings.Where(x => x.Setting == "RegularHoliday").Single().SettingValue);
            RegularHoliday_RestDay = Convert.ToDouble(settings.Where(x => x.Setting == "RegularHoliday_RestDay").Single().SettingValue);
            Sunday = Convert.ToDouble(settings.Where(x => x.Setting == "Sunday").Single().SettingValue);
            SundayRequiresXDaysWork = Convert.ToInt16(settings.Where(x => x.Setting == "SundayRequiresXDaysWork").Single().SettingValue);
            OT = Convert.ToDouble(settings.Where(x => x.Setting == "OT").Single().SettingValue);
            OT_Special = Convert.ToDouble(settings.Where(x => x.Setting == "OT_Special").Single().SettingValue);

            ApplyGSIS = Convert.ToBoolean(settings.Where(x => x.Setting == "ApplyGSIS").Single().SettingValue);
            ApplySSS = Convert.ToBoolean(settings.Where(x => x.Setting == "ApplySSS").Single().SettingValue);
            ApplyPH = Convert.ToBoolean(settings.Where(x => x.Setting == "ApplyPH").Single().SettingValue);
            ApplyPAGIBIG = Convert.ToBoolean(settings.Where(x => x.Setting == "ApplyPAGIBIG").Single().SettingValue);
            ApplyWTAX = Convert.ToBoolean(settings.Where(x => x.Setting == "ApplyWTAX").Single().SettingValue);

            FixedPHAmountForJO = Convert.ToDouble(settings.Where(x => x.Setting == "FixedPHAmountForJO").Single().SettingValue);
            LateRoundOff = Convert.ToInt16(settings.Where(x => x.Setting == "LateRoundOff").Single().SettingValue);
            UTRoundOff = Convert.ToInt16(settings.Where(x => x.Setting == "UTRoundOff").Single().SettingValue);
            
            FlexiTime = Convert.ToDouble(settings.Where(x => x.Setting == "FlexiTime").Single().SettingValue);
            LateTolerance = Convert.ToDouble(settings.Where(x => x.Setting == "LateTolerance").Single().SettingValue);
            UTTolerance = Convert.ToDouble(settings.Where(x => x.Setting == "UTTolerance").Single().SettingValue);
            AutoChargeAbsLateUTToVL = Convert.ToBoolean(settings.Where(x => x.Setting == "AutoChargeAbsLateUTToVL").Single().SettingValue);
            MinNetNotification = Convert.ToDouble(settings.Where(x => x.Setting == "MinNetNotification").Single().SettingValue);
            ACAPERA = Convert.ToDouble(settings.Where(x => x.Setting == "ACAPERA").Single().SettingValue);
            AgencyShareWTaxRate = Convert.ToDouble(settings.Where(x => x.Setting == "AgencyShareWTaxRate").Single().SettingValue);
            AgencyShareMaxDays = Convert.ToDouble(settings.Where(x => x.Setting == "AgencyShareMaxDays").Single().SettingValue);

            Faculty40_Enable = Convert.ToBoolean(settings.Where(x => x.Setting == "Faculty40_Enable").Single().SettingValue);
            Faculty40_MinHoursPerDay = Convert.ToDouble(settings.Where(x => x.Setting == "Faculty40_MinHoursPerDay").Single().SettingValue);
            
            FixedLatePenalty = Convert.ToDouble(settings.Where(x => x.Setting == "FixedLatePenalty").Single().SettingValue);

            CommissionIsTaxable = Convert.ToBoolean(settings.Where(x => x.Setting == "CommissionIsTaxable").Single().SettingValue);
            UsePAGIBIG_Percentage = Convert.ToBoolean(settings.Where(x => x.Setting == "PAGIBIG_Percentage_Use").Single().SettingValue);

            RHRequirePrevDayAttendance = Convert.ToBoolean(settings.Where(x => x.Setting == "RHRequirePrevDayAttendance").Single().SettingValue);

            EnableWorkspanUndertime = Convert.ToBoolean(settings.Where(x => x.Setting == "EnableWorkspanUndertime").Single().SettingValue);
        }

        public string GetValueByName(string name)
        {
            return settings.Where(x => x.Setting == name).Single().SettingValue;
        }
    }

}