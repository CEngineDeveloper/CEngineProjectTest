using UnityEngine;
namespace Gamelogic
{
    [CreateAssetMenu(menuName = "ScriptConfig/LocalConfig")]
    public partial class LocalConfig:CYM.LocalConfig
    {
        public static new LocalConfig Ins => CYM.LocalConfig.Ins as LocalConfig;
    }
}
