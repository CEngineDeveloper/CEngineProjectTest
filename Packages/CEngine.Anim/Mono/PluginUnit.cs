using CYM.AI;
using CYM.Anim;

namespace CYM
{
    public partial class BaseUnit : BaseCoreMono
    {
        static PluginUnit PluginAnim = new PluginUnit
        {
            OnPostAddComponet = (u, x) => {
                if (x is BaseAIMgr)
                {
                    u.AIMgr = x as BaseAIMgr;
                }
            }
        };
        public BaseAnimMgr AnimMgr { get; protected set; }
    }
}
