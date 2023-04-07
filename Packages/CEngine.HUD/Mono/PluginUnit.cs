using CYM.HUD;
namespace CYM
{
    public partial class BaseUnit : BaseCoreMono
    {
        static PluginUnit PluginHUD = new PluginUnit
        {
            OnPostAddComponet = (u, x) => {
                if (x is IHUDMgr)
                {
                    u.HUDMgr = x as IHUDMgr;
                }
            }
        };

        #region prop
        public IHUDMgr HUDMgr { get; protected set; }
        #endregion

    }
}
