//------------------------------------------------------------------------------
// UTutorialView.cs
// Copyright 2022 2022/11/5 
// Created by CYM on 2022/11/5
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using UnityEngine;
using CYM;
using CYM.UI;
using System.Collections;
using System;
using System.Collections.Generic;

namespace CYM.Example
{
    public class CustomData
    {
        public string Name = "";
        public int Value = 0;
    }
    public class UExampleView : UStaticUIView<UExampleView> 
    {
        #region Inspector
        [SerializeField]
        UImage UImage;
        [SerializeField]
        UText UText;
        [SerializeField]
        UText UBgText;
        [SerializeField]
        UCustom UIconAttr;
        [SerializeField]
        UButton UButton;
        [SerializeField]
        UDropdown UDropdown;
        [SerializeField]
        UCheck UCheck;
        [SerializeField]
        UInput UInput;
        [SerializeField]
        UProgress UProgress;
        [SerializeField]
        USlider USlider;
        [SerializeField]
        UScroll UScroll;
        #endregion

        #region private
        List<CustomData> CustomDatas = new List<CustomData>();
        #endregion

        #region life
        protected override void OnCreatedView()
        {
            base.OnCreatedView();
            UImage.Init(new UImageData { IconStr = "" });
            UText.Init(new UTextData { Name =()=> "Name" });
            UBgText.Init(new UTextData { Name = ()=> "这是一段测试文字" });
            UIconAttr.Init(new UCustomData { Icon1 = ()=>null,Name1 = ()=>"生命",Name2 = ()=>"10".Green() });
            UButton.Init(new UButtonData { NameKey = "点击按钮" });
            UDropdown.Init(new UDropdownData { Opts =()=>new string[] { "选项1","水果","香蕉","娃哈哈" } });
            UCheck.Init(new UCheckData { NameKey = "背景音乐" });
            UInput.Init(new UInputData { });
            UProgress.Init(new UProgressData { Value = ()=>0.5f });
            USlider.Init(new USliderData { Value = ()=>0.3f });
            UScroll.Init(GetScrollData,OnScrollRefresh);

            for (int i = 0; i < 100; ++i)
            {
                CustomDatas.Add(new CustomData() { Name = "Name" + i, Value = i });
            }
        }

        private void OnScrollRefresh(object arg1, object arg2)
        {
            UCustom press = arg1 as UCustom;
            CustomData data = arg2 as CustomData;
            press.NameText1 = data.Name;
            press.Data.OnClick = (_, _) =>
            {
                if (MathUtil.IsOdd(data.Value))
                {
                    CLog.Yellow("点击了:" + data.Name);
                }
                else
                {
                    CLog.Cyan("点击了:" + data.Name);
                }
            };
        }

        private IList GetScrollData()
        {
            return CustomDatas;
        }
        #endregion
    }
}