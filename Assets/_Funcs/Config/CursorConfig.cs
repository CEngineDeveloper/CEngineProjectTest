using UnityEngine;
namespace Gamelogic
{
    [CreateAssetMenu(menuName = "ScriptConfig/CursorConfig")]
    public partial class CursorConfig:CYM.CursorConfig
    {
        public static new CursorConfig Ins => CYM.CursorConfig.Ins as CursorConfig;
    }
}
