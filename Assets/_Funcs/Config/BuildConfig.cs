using UnityEngine;
namespace Gamelogic
{
    [CreateAssetMenu(menuName = "ScriptConfig/BuildConfig")]
    public partial class BuildConfig:CYM.BuildConfig
    {
        public static new BuildConfig Ins => CYM.BuildConfig.Ins as BuildConfig;
    }
}
