using UnityEngine;

namespace CYM.UI
{
    [AddComponentMenu(SysConst.STR_MenuUIRaycast + nameof(UIIgnoreRaycast))]
    public class UIIgnoreRaycast : MonoBehaviour, ICanvasRaycastFilter
    {
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return false;
        }
    }
}
