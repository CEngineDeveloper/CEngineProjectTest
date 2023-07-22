//**********************************************
// Class Name	: BaseBone
// Discription	：None
// Author	：CYM
// Team		：BloodyMary
// Date		：#DATE#
// Copyright ©1995 [CYMCmmon] Powered By [CYM] Version 1.0.0 
//**********************************************
using UnityEngine;

namespace CYM
{
    [AddComponentMenu(SysConst.STR_MenuSceneObj + nameof(BoneObj))]
    public sealed class BoneObj : BaseMono
    {
        public NodeType Type;
        public string ExtendName = SysConst.STR_Inv;

    }
}