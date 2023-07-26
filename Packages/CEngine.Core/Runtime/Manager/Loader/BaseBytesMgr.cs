
using CYM.DLC;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CYM
{
    public sealed class BaseBytesMgr : BaseGFlowMgr, ILoader
    {
        public Dictionary<string, byte[]> Data { get; private set; } = new Dictionary<string, byte[]>();
        public event Callback Callback_OnParseStart;
        public event Callback Callback_OnParseEnd;

        #region loader
        public IEnumerator Load()
        {
            Callback_OnParseStart?.Invoke();
            Data.Clear();
            foreach (var dlc in DLCManager.LoadedDLCItems.Values)
            {
                if (VersionUtil.IsEditorOrConfigMode)
                {
                    var files = dlc.GetAllBytes();
                    if (files == null)
                        continue;
                    foreach (var file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        Data.Add(fileName, File.ReadAllBytes(file));
                        BaseGlobal.LoaderMgr.ExtraLoadInfo = "Load bytes " + fileName;
                        yield return new WaitForEndOfFrame();
                    }
                }
                else
                {
                    var assetBundle = DLCManager.LoadRawBundle(dlc.Name,SysConst.BN_Bytes);
                    if (assetBundle != null)
                    {
                        foreach (var textAssets in assetBundle.LoadAllAssets<TextAsset>())
                        {
                            Data.Add(textAssets.name, textAssets.bytes);
                            BaseGlobal.LoaderMgr.ExtraLoadInfo = "Load bytes " + textAssets.name;
                        }
                        foreach (var textAssets in assetBundle.LoadAllAssets<BytesAsset>())
                        {
                            Data.Add(textAssets.name, textAssets.Bytes);
                            BaseGlobal.LoaderMgr.ExtraLoadInfo = "Load bytes " + textAssets.name;
                        }
                    }
                }

            }
            Callback_OnParseEnd?.Invoke();
            yield break;
        }
        public string GetLoadInfo()
        {
            return "Load bytes";
        }
        #endregion

        #region get
        /// <summary>
        /// 获得text
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes(string id)
        {
            if (!Data.ContainsKey(id))
                return null;
            return Data[id];
        }
        #endregion
    }

}