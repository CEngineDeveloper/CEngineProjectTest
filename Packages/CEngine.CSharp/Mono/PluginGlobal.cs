namespace CYM
{
    public partial class BaseGlobal : BaseCoreMono
    {
        static PluginGlobal PluginCS = new PluginGlobal
        {
            OnInstall = (g) => {
                CSMgr = g.AddComponent<BaseCSMgr>();
            }
        };
        public static BaseCSMgr CSMgr { get; private set; } 
    }
}