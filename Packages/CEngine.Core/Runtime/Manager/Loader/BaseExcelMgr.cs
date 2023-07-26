//------------------------------------------------------------------------------
// BaseExcelMgr.cs
// Copyright 2019 2019/1/30 
// Created by CYM on 2019/1/30
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using CYM.DLC;
using CYM.Excel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CYM
{
    public sealed class BaseExcelMgr : BaseGFlowMgr, ILoader
    {
        #region prop
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        //默认从第二行读取配置字段
        private int StartRowCount => 2;
        #endregion

        #region Callback val
        public event Callback Callback_OnParseEnd;
        public event Callback Callback_OnParseStart;
        #endregion

        #region loader
        public IEnumerator Load()
        {
            Callback_OnParseStart?.Invoke();

            //加载DLC Excel
            foreach (var dlc in DLCManager.LoadedDLCItems.Values)
            {
                if (VersionUtil.IsEditorOrConfigMode)
                {
                    string[] fileList = dlc.GetAllExcel();
                    if (fileList == null)
                        continue;
                    foreach (var item in fileList)
                    {
                        LoadExcelData(File.ReadAllBytes(item), Path.GetFileNameWithoutExtension(item));
                        yield return new WaitForEndOfFrame();
                    }
                }
                else
                {
                    var assetBundle = DLCManager.LoadRawBundle(dlc.Name,SysConst.BN_Excel);
                    if (assetBundle != null)
                    {
                        foreach (var txt in assetBundle.LoadAllAssets<TextAsset>())
                        {
                            LoadExcelData(txt.bytes, txt.name);
                            yield return new WaitForEndOfFrame();
                        }
                        foreach (var txt in assetBundle.LoadAllAssets<BytesAsset>())
                        {
                            LoadExcelData(txt.Bytes, txt.name);
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }
                yield return new WaitForEndOfFrame();
            }
            Callback_OnParseEnd?.Invoke();
            yield break;
        }
        public string GetLoadInfo()
        {
            return "Load Excel";
        }

        private void LoadExcelData(byte[] buffer,string tableName)
        {
            var luaMgr = BaseLuaMgr.GetTDConfig(tableName);
            if (luaMgr == null)
                return;
            stopwatch.Start();
            if (ExcelUtil.IsBinary(buffer))
            {
                WorkBook book = ExcelUtil.ReadWorkbook(buffer);
                if (book == null || book.Count <= 0)
                {
                    CLog.Error("错误！无法读取Excel：" + tableName);
                }
                foreach (var item in book)
                {
                    if (!item.Name.StartsWith(SysConst.Prefix_Notes))
                        OnConvert(item, tableName);
                }
            }
            else
            {
                var doc = ExcelUtil.ReadCSV(buffer);
                OnConvert(doc, tableName);
            }
            stopwatch.Stop();
            CLog.Info($"加载数据表：{tableName},Time：{stopwatch.Elapsed.TotalSeconds}");
        }
        private void OnConvert(Table table,string tableName)
        {
            int excludeRow = StartRowCount;
            var luaMgr = BaseLuaMgr.GetTDConfig(tableName);
            if (luaMgr!=null)
            {
                IEnumerable<object> data;
                if (luaMgr.TableMapper != null)
                {
                    var map = luaMgr.TableMapper.Exclude(excludeRow);
                    map.SafeMode = true;
                    data = table.Convert(map);
                }
                else
                {
                    data = table.Convert(luaMgr.DataType, excludeRow, true) ;
                }
                luaMgr.AddAlterRangeFromObj(data);
            }
        }
        #endregion
    }
}