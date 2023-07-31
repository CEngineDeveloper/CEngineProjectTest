/**
 * BaseTextMgr.cs
 * Created by: CYM [as8506@qq.com]
 * Created on: 2023/7/29 (zh-CN)
 */

using CYM.DLC;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CYM
{
    public class BaseTextMgr : BaseGFlowMgr, ILoader
    {
        public event Callback Callback_OnParseStart;
        public event Callback Callback_OnParseEnd;
        public Dictionary<string, string> Data { get; private set; } = new Dictionary<string, string>();

        #region loader
        public IEnumerator Load()
        {
            Callback_OnParseStart?.Invoke();
            Data.Clear();
            foreach (var dlc in DLCManager.LoadedDLCItems.Values)
            {
                if (VersionUtil.IsEditorOrConfigMode)
                {
                    var files = dlc.GetAllText();
                    if (files == null)
                        continue;
                    foreach (var file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        Data.Add(fileName, File.ReadAllText(file));
                        BaseGlobal.LoaderMgr.ExtraLoadInfo = "Load text " + fileName;
                        yield return new WaitForEndOfFrame();
                    }
                }
                else
                {
                    var assetBundle = DLCManager.LoadRawBundle(dlc.Name, SysConst.BN_Text);
                    if (assetBundle != null)
                    {
                        foreach (var textAssets in assetBundle.LoadAllAssets<TextAsset>())
                        {
                            Data.Add(textAssets.name, textAssets.text);
                            BaseGlobal.LoaderMgr.ExtraLoadInfo = "Load text " + textAssets.name;
                        }
                    }
                }

            }
            Callback_OnParseEnd?.Invoke();
            yield break;
        }
        public string GetLoadInfo()
        {
            return "Load texts";
        }
        #endregion

        #region get
        public string GetText(string id)
        {
            if (!Data.ContainsKey(id))
                return null;
            return Data[id];
        }
        #endregion
    }
}