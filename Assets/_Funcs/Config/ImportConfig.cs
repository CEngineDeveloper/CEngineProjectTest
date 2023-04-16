using UnityEngine;
namespace Gamelogic
{
    [CreateAssetMenu(menuName = "ScriptConfig/ImportConfig")]
    public partial class ImportConfig:CYM.ImportConfig
    {
        public static new ImportConfig Ins => CYM.ImportConfig.Ins as ImportConfig;
    }
}
