using UnityEngine;
namespace Gamelogic
{
    [CreateAssetMenu(menuName = "ScriptConfig/PluginConfig")]
    public partial class PluginConfig:CYM.PluginConfig
    {
        public static new PluginConfig Ins => CYM.PluginConfig.Ins as PluginConfig;
    }
}
