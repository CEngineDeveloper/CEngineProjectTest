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
namespace CYM.UI
{
    public class Fader : MonoBehaviour
    {
        static CanvasScaler CanvasScaler;
        static CanvasGroup BG;
        static Fader Ins;
        static Tweener alphaTween;

        public static void Show(float duration)
        {
            if (Ins == null)
            {
                Ins = GameObject.Instantiate<Fader>(Resources.Load<Fader>("BaseFade"));
                Ins.transform.hideFlags = HideFlags.HideInHierarchy;
                DontDestroyOnLoad(Ins);
                CanvasScaler = Ins.GetComponent<CanvasScaler>();
                BG = Ins.GetComponent<CanvasGroup>();
                CanvasScaler.referenceResolution = new Vector2(BuildConfig.Ins.Width, BuildConfig.Ins.Height);
            }
            BG.alpha = 1.0f;
            alphaTween?.Kill();
            alphaTween = DOTween.To(() => BG.alpha, x => BG.alpha = x, 0.0f, duration);
        }
    }
}