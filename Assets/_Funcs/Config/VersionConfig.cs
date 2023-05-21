using UnityEngine;
namespace Gamelogic
{
    [CreateAssetMenu(menuName = "ScriptConfig/VersionConfig")]
    public partial class VersionConfig:CYM.VersionConfig
    {
        public static new VersionConfig Ins => CYM.VersionConfig.Ins as VersionConfig;
    }
}
