//------------------------------------------------------------------------------
// StarRate.cs
// Created by CYM on 2021/2/22
// 填写类的描述...
//------------------------------------------------------------------------------
using UnityEngine;
using System;
using UnityEngine.UI;
using Sirenix.OdinInspector;
namespace CYM.UI
{
    public class UStarRateData : UData
    {
        public Func<float> GetRate;
    }
    [AddComponentMenu(SysConst.STR_MenuUIControl + nameof(UStarRate))]
    [HideMonoScript]
    public class UStarRate : UPres<UStarRateData>
    {
        [SerializeField, ChildGameObjectsOnly]
        Image IFull;
        [SerializeField, ChildGameObjectsOnly]
        Image IHalf;
        [SerializeField]
        int MaxRate = 5;

        public override bool IsAtom => true;
        public override void Refresh()
        {
            base.Refresh();

            float step = 1.0f / MaxRate;
            if (Data != null)
            {
                if (Data.GetRate != null)
                {
                    float curRate = Data.GetRate.Invoke();
                    curRate = Mathf.Clamp(curRate,0,MaxRate);
                    if (IFull != null)
                    {
                        IFull.fillAmount = (int)(curRate) * step;
                    }
                    if (IHalf != null)
                    {
                        IHalf.fillAmount = Mathf.CeilToInt(curRate) * step;
                    }
                }
            }
        }
    }
}