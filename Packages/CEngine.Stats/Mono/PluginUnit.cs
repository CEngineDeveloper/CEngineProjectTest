using CYM.Stats;
namespace CYM
{
    public partial class BaseUnit : BaseCoreMono
    {
        static PluginUnit PluginUnit = new PluginUnit
        {
            OnPostAddComponet = (u, x) => {
                if (x is IAttrMgr)
                {
                    u.AttrMgr = x as IAttrMgr;
                }
                else if (x is IBuffMgr)
                {
                    u.BuffMgr = x as IBuffMgr;
                }
                else if (x is BaseImmuneMgr)
                {
                    u.ImmuneMgr = x as BaseImmuneMgr;
                }
            }
        };

        #region prop
        public BaseImmuneMgr ImmuneMgr { get; protected set; }
        public IAttrMgr AttrMgr { get; protected set; }
        public IBuffMgr BuffMgr { get; protected set; }
        #endregion

    }
}
