using Module.DB;

namespace coreApp.Interfaces
{
    public interface IMyController
    {
        tblStakeholder stakeholder { get; set; }
    }

    public interface IStakeholderController
    {
        tblStakeholder stakeholder { get; set; }
    }

    
    public interface IEffectivityController
    {
        string effectivity { get; set; }
    }
    
}