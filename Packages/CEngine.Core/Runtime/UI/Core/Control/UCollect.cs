//------------------------------------------------------------------------------
// UItem.cs
// Created by CYM on 2022/8/27
// 填写类的描述...
//------------------------------------------------------------------------------
using UnityEngine;
using System;
using Sirenix.OdinInspector;
namespace CYM.UI
{
    [AddComponentMenu(SysConst.STR_MenuUIControl + nameof(UCollect))]
    public class UCollect : UPres<UData>
    {
        [Serializable]
        public class Collect
        {
            public UButton Button;
            public UText Text;
            public UImage Image;
            public UCheck Check;
            public UDropdown Dropdown;
            public USlider Slider;
        }

        #region Inspector
        [FoldoutGroup("Inspector"), SerializeField, ChildGameObjectsOnly]
        public Collect Items1;
        [FoldoutGroup("Inspector"), SerializeField, ChildGameObjectsOnly]
        public Collect Items2;
        [FoldoutGroup("Inspector"), SerializeField, ChildGameObjectsOnly]
        public Collect Items3;
        #endregion
    }
}