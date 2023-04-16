using UnityEngine;
namespace Gamelogic
{
    [CreateAssetMenu(menuName = "ScriptConfig/UIConfig")]
    public partial class UIConfig:CYM.UIConfig
    {
        public static new UIConfig Ins => CYM.UIConfig.Ins as UIConfig;
    }
}
