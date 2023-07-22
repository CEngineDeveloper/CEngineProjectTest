//**********************************************
// Class Name	: BaseFadeView
// Discription	：None
// Author	：CYM
// Team		：BloodyMary
// Date		：#DATE#
// Copyright ©1995 [CYMCmmon] Powered By [CYM] Version 1.0.0 
//**********************************************
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
namespace CYM
{
    [AddComponentMenu(SysConst.STR_MenuAloneUI + nameof(FaderUI))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(CanvasGroup))]
    public class FaderUI : BaseAloneUI<FaderUI>
    {
        static Tweener alphaTween;

        public static void Show(float duration)
        {
            Create("BaseFade");
            Canvas.sortingOrder = 10000;
            CanvasGroup.alpha = 1.0f;
            alphaTween?.Kill();
            alphaTween = DOTween.To(() => CanvasGroup.alpha, x => CanvasGroup.alpha = x, 0.0f, duration);
        }
    }
}