//**********************************************
// Class Name	: LoaderManager
// Discription	：None
// Author	：CYM
// Team		：MoBaGame
// Date		：#DATE#
// Copyright ©1995 [CYMCmmon] Powered By [CYM] Version 1.0.0 
//**********************************************

using CYM.DLC;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CYM
{
    /// <summary>
    /// $LocalPlayer表示动态ID
    /// </summary>
    public sealed class BaseGRMgr : BaseGFlowMgr, ILoader
    {
#region member variable
        private DLCConfig DLCConfig => DLCConfig.Ins;
        public static Dictionary<string, IRsCacher> BundleCachers { get; private set; } = new Dictionary<string, IRsCacher>();
#endregion

#region get
        public GameObject GetResources(string path, bool instance = false)
        {
            var temp = Resources.Load<GameObject>(path);
            if (temp == null)
            {
                CLog.Error("错误,没有这个对象:"+ path);
                return null;
            }
            if (instance)
            {
                var ret = GameObject.Instantiate(temp);
                ret.transform.SetParent(SelfBaseGlobal.Trans);
                return ret;
            }
            else return temp;
        }
        public T GetResources<T>(string path) where T: Object
        {
            var temp = Resources.Load<T>(path);
            if (temp == null)
            {
                CLog.Error("错误,没有这个对象:" + path);
                return null;
            }
            return temp;
        }
#endregion

#region Callback
        protected override void OnBattleUnLoaded()
        {
            base.OnBattleUnLoaded();
            Resources.UnloadUnusedAssets();
            GlobalMonoManager.RemoveAllNull();
            BundleCachers.ForEach(x => x.Value.RemoveNull());
            BaseGlobal.GCCollect();
        }
        protected override void OnSubBattleUnLoaded()
        {
            base.OnSubBattleUnLoaded();
        }
        public IEnumerator Load()
        {
            var dlcItemConfigs = GetDLCItemConfigs();

            foreach (var item in dlcItemConfigs)
            {
                DLCManager.LoadDLC(item);
                yield return new WaitForEndOfFrame();
            }

            //初始化的加载所有Bundle
            if (!VersionUtil.IsEditorMode)
            {
                var shaderData = DLCManager.LoadAllShaderBundle();
                foreach (var bundle in shaderData)
                {
                    while (!bundle.IsDone)
                        yield return new WaitForEndOfFrame();
                }
                if (BuildConfig.Ins.IsInitLoadSharedBundle)
                {
                    var data = DLCManager.LoadAllSharedBundle();
                    foreach (var bundle in data)
                    {
                        while (!bundle.IsDone)
                            yield return new WaitForEndOfFrame();
                    }
                }
                if (BuildConfig.Ins.IsInitLoadDirBundle)
                {
                    var data = DLCManager.LoadAllDirBundle();
                    foreach (var bundle in data)
                    {
                        while (!bundle.IsDone)
                            yield return new WaitForEndOfFrame();
                    }
                }
            }

            if(BuildConfig.Ins.IsWarmupAllShaders)
                Shader.WarmupAllShaders(); 
            yield break;

            // 获得DLC的根目录
            List<DLCItemConfig> GetDLCItemConfigs()
            {
                if (VersionUtil.IsEditorOrAssetBundleMode)
                {
                    return DLCConfig.ConfigAll;
                }
                else
                {
                    List<DLCItemConfig> ret = new List<DLCItemConfig>();
                    string[] files = Directory.GetFiles(SysConst.Path_StreamingAssets, SysConst.STR_DLCItem + ".json", SearchOption.AllDirectories);
                    foreach (var item in files)
                    {
                        var dlcJson = FileUtil.LoadJson<DLCItemConfig>(item);
                        if(dlcJson.IsActive)
                            ret.Add(dlcJson);
                    }
                    return ret;
                }
            }
        }
        public string GetLoadInfo()
        {
            return "Load Resources";
        }
#endregion

#region 语法糖
        public Material FontRendering =>BaseGlobal.RsMaterial.Get("FontRendering");
        public Material ImageGrey =>BaseGlobal.RsMaterial.Get("ImageGrey");
#endregion
    }

}