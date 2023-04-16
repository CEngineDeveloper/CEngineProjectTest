using UnityEngine;
namespace Gamelogic
{
    [CreateAssetMenu(menuName = "ScriptConfig/DLCConfig")]
    public partial class DLCConfig:CYM.DLCConfig
    {
        public static new DLCConfig Ins => CYM.DLCConfig.Ins as DLCConfig;
    }
}
