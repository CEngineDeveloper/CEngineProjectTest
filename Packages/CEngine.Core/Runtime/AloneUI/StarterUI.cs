//------------------------------------------------------------------------------
// StarterUI.cs
// Copyright 2022 2022/10/6 
// Created by CYM on 2022/10/6
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CYM.DLC;
namespace CYM
{
    [AddComponentMenu(SysConst.STR_MenuAloneUI + nameof(StarterUI))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(CanvasGroup))]
    [HideMonoScript]
    public class StarterUI : BaseAloneUI<StarterUI>
    {
        [SerializeField]
        Image BG;
        [SerializeField]
        Image BGLogo;
        [SerializeField]
        Image FillFg;


        #region prop
        static float CurPercent = 0.0f;
        #endregion

        #region set
        private void Awake()
        {
            BG.color = BuildConfig.Ins.LoadingBGColor;
            if (BuildConfig.Ins.LoadingBGLogo != null)
            {
                BGLogo.sprite = BuildConfig.Ins.LoadingBGLogo;
            }
        }
        private void Update()
        {
            if (FillFg)
            {
                FillFg.fillAmount = CurPercent;
            }
            CurPercent = Mathf.Lerp(CurPercent, CalcPercent(),Time.smoothDeltaTime* BuildConfig.Ins.LoadingSpeed);
        }

        float CalcPercent()
        { 
            if(DLCDownloader.IsNeedDownload())
                return DLCDownloader.DownloadProgress * 0.5f + BaseLoaderMgr.Percent * 0.5f;
            return BaseLoaderMgr.Percent;
        }

        #endregion

        #region set
        public static IEnumerator Show()
        {
            if (BuildConfig.Ins == null)
                yield break;
            if (BuildConfig.Ins?.StarterUIPrefab == null)
                yield break;
            CurPercent =0.0f;
            string key = BuildConfig.Ins?.StarterUIPrefab?.name;
            Create(key??"BaseStarterPlayer");
            CanvasGroup.alpha = 1.0f;
            Ins.FillFg.fillAmount = 0.0f;
            yield return new WaitForSeconds(BuildConfig.Ins.LoadingStartDelay);
        }
        public static IEnumerator Close()
        {
            if (Ins == null)
                yield break;
            float dispearTime = 0.5f;
            CanvasGroup.alpha = 1.0f;
            yield return new WaitUntil(() => CurPercent >= 0.99f);
            yield return new WaitForSeconds(BuildConfig.Ins.LoadingEndDelay);
            DOTween.To(() => CanvasGroup.alpha, x => CanvasGroup.alpha = x, 0.0f, dispearTime).OnComplete(() => {});
            yield return new WaitForSeconds(dispearTime);
            Ins.gameObject.SetActive(false);
        }
        #endregion

    }
}