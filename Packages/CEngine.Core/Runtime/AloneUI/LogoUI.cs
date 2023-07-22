//------------------------------------------------------------------------------
// BaseLogoView.cs
// Copyright 2018 2018/3/17 
// Created by CYM on 2018/3/17
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CYM.UI;
#if UNITY_EDITOR
#endif

namespace CYM
{
    [AddComponentMenu(SysConst.STR_MenuAloneUI+nameof(LogoUI))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class LogoUI : BaseAloneUI<LogoUI>
    {
        [SerializeField]
        public Image BG;
        [SerializeField]
        public Image Image;
        [SerializeField]
        public UVideo Video;

        #region prop
        static Tweener tweener;
        static List<LogoData> Logos => BuildConfig.Ins.Logos;
        static Image LogoImage => Ins.Image;
        static Image LogoBG => Ins.BG;
        static UVideo LogoVideo => Ins?.Video;
        #endregion

        #region life
        void Update()
        {
            if (LogoVideo != null && LogoVideo.IsPlaying())
            {
                if (Input.anyKeyDown)
                {
                    LogoVideo.Stop();
                }
            }
        }
        #endregion

        #region loader
        public static IEnumerator Show()
        {
            Create("BaseLogoPlayer");
            bool IsNoLogo = Logos == null || Logos.Count == 0;
            if (VersionUtil.IsLogoEditorMode() && !IsNoLogo)
            {
                while (Ins == null)
                    yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(0.01f);
                for (int i = 0; i < Logos.Count; ++i)
                {
                    if (tweener != null) tweener.Kill();
                    if (Logos[i].IsImage())
                    {
                        LogoBG.color = Logos[i].BGColor;
                        LogoImage.color = new Color(1, 1, 1, 0);
                        tweener = DOTween.ToAlpha(() => LogoImage.color, x => LogoImage.color = x, 1.0f, Logos[i].InTime);
                        LogoImage.sprite = Logos[i].Logo;
                        LogoImage.SetNativeSize();
                        yield return new WaitForSeconds(Logos[i].WaitTime);
                        if (tweener != null) tweener.Kill();
                        tweener = DOTween.ToAlpha(() => LogoImage.color, x => LogoImage.color = x, 0.0f, Logos[i].OutTime);
                    }
                    else if (Logos[i].IsVideo())
                    {
                        LogoVideo.Play(Logos[i].Video);
                        while (LogoVideo.IsPreparing)
                            yield return new WaitForEndOfFrame();
                        LogoVideo.Show();
                        while (LogoVideo.IsPlaying())
                            yield return new WaitForEndOfFrame();
                        if (!IsNextVideo(i))
                            LogoVideo.Close();
                    }

                    if (i < Logos.Count - 1)
                        yield return new WaitForSeconds(Logos[i].OutTime);
                }
                LogoBG.color = Color.black;
                if (Logos.Count > 0)
                    yield return new WaitForSeconds(0.5f);
            }
            float dispearTime = 1.0f;
            DOTween.To(() => CanvasGroup.alpha, x => CanvasGroup.alpha = x, 0.0f, dispearTime).OnComplete(() => {
            });
            yield return new WaitForSeconds(dispearTime);
            Destroy(Ins.gameObject);
            IsShowedLogo = true;

            bool IsNextVideo(int i)
            {
                var index = i + 1;
                if (index < Logos.Count - 1)
                {
                    return Logos[index].IsVideo();
                }
                return false;
            }
        }
        #endregion

        #region is
        public static bool IsShowedLogo { get; private set; } = false;
        #endregion
    }
}