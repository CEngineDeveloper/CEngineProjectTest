﻿using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CYM.UI
{
    public class UButtonData : UTextData
    {
        [HideInInspector]
        public Func<object, object> OnSorter = (x) => { return x; };
    }
    [AddComponentMenu(SysConst.STR_MenuUIControl + nameof(UButton))]
    [HideMonoScript]
    public class UButton : UPres<UButtonData>
    {
        #region 组建
        [FoldoutGroup("Inspector"),SerializeField, ChildGameObjectsOnly, Tooltip("可以位空")]
        public Text IName;
        [FoldoutGroup("Inspector"),SerializeField, ChildGameObjectsOnly, Tooltip("可以位空")]
        public Image IIcon;
        [FoldoutGroup("Inspector"),SerializeField, ChildGameObjectsOnly, Tooltip("可以位空")]
        public Image IBg;
        #endregion

        #region life
        public override bool IsAtom => true;
        public override void Refresh()
        {
            base.Refresh();
            if (Data != null)
            {
                if (IName != null)
                {
                    var temp = Data.GetName();
                    if (!temp.IsInv()) IName.text = temp;
                }
                if (IIcon != null)
                {
                    var temp = Data.GetIcon();
                    if (temp) IIcon.overrideSprite = temp;
                }
                if (IBg != null)
                {
                    var temp = Data.GetBg();
                    if (temp) IBg.overrideSprite = temp;
                }
            }
        }
        public void Refresh(string icon)
        {
            if(IIcon!=null)
                IIcon.overrideSprite = icon.GetIcon();
        }
        #endregion

        #region wrap
        public string NameText
        {
            get { return IName.text; }
            set { 
                if(IName) IName.text = value; 
            }
        }
        public Sprite IconSprite
        {
            get { return IIcon.sprite; }
            set { IIcon.sprite = value; }
        }
        public Sprite BgSprite
        {
            get { return IBg.sprite; }
            set { IBg.sprite = value; }
        }
        public Sprite IconOverrideSprite
        {
            get { return IIcon.overrideSprite; }
            set { IIcon.overrideSprite = value; }
        }
        public Sprite BgOverrideSprite
        {
            get { return IBg.overrideSprite; }
            set { IBg.overrideSprite = value; }
        }
        #endregion

        #region editor
        public override void AutoSetup()
        {
            base.AutoSetup();
            if (IName == null)
                IName = GetComponentInChildren<Text>();
            if (IBg == null)
                IBg = GetComponentInChildren<Image>();
        }
        #endregion
    }
}
