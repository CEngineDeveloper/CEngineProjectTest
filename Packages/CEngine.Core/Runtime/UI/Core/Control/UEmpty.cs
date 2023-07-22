using Sirenix.OdinInspector;
using UnityEngine;

namespace CYM.UI
{
    [AddComponentMenu(SysConst.STR_MenuUIControl + nameof(UEmpty))]
    [HideMonoScript]
    public class UEmpty : UPres<UData>
    {
        public override bool IsAtom => true;

    }

}