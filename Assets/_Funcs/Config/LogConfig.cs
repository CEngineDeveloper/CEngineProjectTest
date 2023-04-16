using UnityEngine;
namespace Gamelogic
{
    [CreateAssetMenu(menuName = "ScriptConfig/LogConfig")]
    public partial class LogConfig:CYM.LogConfig
    {
        public static new LogConfig Ins => CYM.LogConfig.Ins as LogConfig;
    }
}
