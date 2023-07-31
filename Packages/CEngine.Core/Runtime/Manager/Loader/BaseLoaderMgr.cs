//**********************************************
// Class Name	: LoaderManager
// Discription	：None
// Author	：CYM
// Team		：MoBaGame
// Date		：#DATE#
// Copyright ©1995 [CYMCmmon] Powered By [CYM] Version 1.0.0 
// 先读取Lua配置，再读取Excel配置（没有则添加，有则修改）
//**********************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CYM
{
    public sealed class BaseLoaderMgr : BaseGFlowMgr
    {
        #region Callback Val
        /// <summary>
        /// 当一个Loader加载完成
        /// </summary>
        public event Callback<LoadEndType, string> Callback_OnLoadEnd;
        /// <summary>
        /// 加载开始
        /// </summary>
        public event Callback Callback_OnStartLoad;
        /// <summary>
        /// 当所有的loader都加载完成
        /// </summary>
        public event Callback Callback_OnAllLoadEnd1;
        /// <summary>
        /// 当所有的loader都加载完成
        /// </summary>
        public event Callback Callback_OnAllLoadEnd2;
        #endregion

        #region member variable
        readonly List<ILoader> loderList = new List<ILoader>();
        private string LoadInfo;
        public bool IsLoadEnd { get; private set; } = false;
        public string ExtraLoadInfo { get; set; }
        #endregion

        #region property
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        public static float Percent { get; set; }
        public ILoader CurLoader { get; private set; }
        static List<ILoader> CustomLoaders = new List<ILoader>();
        #endregion

        #region life
        protected override void OnSetNeedFlag()
        {
            base.OnSetNeedFlag();
            NeedGUI = true;
        }
        public override void OnStart()
        {
            base.OnStart();

        }
        public override void OnGUIPaint()
        {
            if (!IsLoadEnd)
            {
            }
        }
        public IEnumerator StartAllLoader()
        {
            var loaders = new List<ILoader>() {
                BaseGlobal.GRMgr,
                BaseGlobal.LangMgr,
                BaseGlobal.LuaMgr,
                BaseGlobal.ExcelMgr,
                BaseGlobal.TextMgr,
                BaseGlobal.BytesMgr,
            };
            CustomLoaders.RemoveAll(x => loaders.Contains(x));
            loaders.AddRange(CustomLoaders);
            if (loaders == null || loaders.Count == 0)
            {
                CLog.Error("错误,没有Loader");
                yield break;
            }
            foreach (var item in loaders)
            {
                loderList.Add(item);
            }
            IsLoadEnd = false;
            yield return SelfMono.StartCoroutine(IEnumerator_Load());
        }
        #endregion

        #region utile
        public void AddCustomLoader(ILoader loader)
        {
            CustomLoaders.Add(loader);
        }
        IEnumerator IEnumerator_Load()
        {
            Percent = 0;
            yield return new WaitForEndOfFrame();
            Callback_OnStartLoad?.Invoke();
            for (int i = 0; i < loderList.Count; ++i)
            {
                stopwatch.Start();
                LoadInfo = loderList[i].GetLoadInfo();
                CurLoader = loderList[i];
                CLog.Info(LoadInfo);
                yield return loderList[i].Load();
                Percent = (i / (float)loderList.Count);
                stopwatch.Stop();
                CLog.Yellow($"{LoadInfo} Loading time:{ stopwatch.Elapsed.TotalSeconds}");
            }
            yield return new WaitForEndOfFrame();
            Percent = 1.0f;
            IsLoadEnd = true;
            Callback_OnLoadEnd?.Invoke(LoadEndType.Success, LoadInfo);
            Callback_OnAllLoadEnd1?.Invoke();
            Callback_OnAllLoadEnd2?.Invoke();
            CurLoader = null;
            CLog.Info("All loaded!!");
        }
        #endregion
    }

}

