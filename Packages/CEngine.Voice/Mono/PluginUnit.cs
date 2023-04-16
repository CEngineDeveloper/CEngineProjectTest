using CYM.Voice;
namespace CYM
{
    public partial class BaseUnit : BaseCoreMono
    {
        static PluginUnit PluginVoice = new PluginUnit
        {
            OnPostAddComponet = (u, x) => {
                if (x is BaseVoiceMgr)
                {
                    u.VoiceMgr = x as BaseVoiceMgr;
                }
            }
        };

        #region prop
        public BaseVoiceMgr VoiceMgr { get; protected set; }
        #endregion

    }
}
