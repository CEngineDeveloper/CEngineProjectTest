using CYM.Perform;
using CYM.Stats;

namespace CYM
{
    public partial class BaseUnit : BaseCoreMono
    {
        static PluginUnit PluginPerform = new PluginUnit
        {
            OnPostAddComponet = (u, x) => {
                if (x is BasePerformMgr)
                {
                    u.PerformMgr = x as BasePerformMgr;
                }
            }
        };
        public BasePerformMgr PerformMgr { get; protected set; }
    }
}