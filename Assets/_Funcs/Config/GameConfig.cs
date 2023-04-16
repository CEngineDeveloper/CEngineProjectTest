using UnityEngine;
namespace Gamelogic
{
    [CreateAssetMenu(menuName = "ScriptConfig/GameConfig")]
    public partial class GameConfig:CYM.GameConfig
    {
        public static new GameConfig Ins => CYM.GameConfig.Ins as GameConfig;
    }
}
