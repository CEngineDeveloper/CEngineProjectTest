﻿
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using CYM.DLC;
using CYM.Excel;
//**********************************************
// Class Name	: CYMBaseLanguage
// Discription	：None
// Author	：CYM
// Team		：MoBaGame
// Date		：#DATE#
// Copyright ©1995 [CYMCmmon] Powered By [CYM] Version 1.0.0 
//**********************************************

namespace CYM
{
    public sealed class BaseLangMgr : BaseGFlowMgr, ILoader
    {
        #region Callback Val
        /// <summary>
        /// 切换语言的时候
        /// </summary>
        public event Callback Callback_OnSwitchLanguage;
        public event Callback Callback_OnParseStart;
        public event Callback Callback_OnParseEnd;
        #endregion

        #region Prop
        public static LanguageType LanguageType { get;private set; }
        static Dictionary<int, Dictionary<string, string>> data = new Dictionary<int, Dictionary<string, string>>();
        static Dictionary<string, string> curDic = new Dictionary<string, string>();
        static Dictionary<string, string> defualtDic = new Dictionary<string, string>();
        static Dictionary<string, Func<string>> dynamicDic = new Dictionary<string, Func<string>>();
        static HashSet<string> lanKeys = new HashSet<string>();
        static Dictionary<string, HashList<string>> categoryKey = new Dictionary<string, HashList<string>>();
        public static HashSet<string> AllLastNames { get; private set; } = new HashSet<string>();
        public static List<string> LoadTips => GetCategoryKeyList(SysConst.Lang_Category_LoadTip);
        static Dictionary<string, LanguageType?> FirstRowStrs = new Dictionary<string, LanguageType?>();
        public string this[string key] => Get(key);
        BasePlatSDKMgr PlatSDKMgr => BaseGlobal.PlatSDKMgr;
        BuildConfig BuildConfig => BuildConfig.Ins;
        #endregion

        #region life
        public override void OnCreate()
        {
            base.OnCreate();
            FirstRowStrs.Clear();
            foreach (var x in BuildConfig.Language)
            {
                FirstRowStrs.Add(x.ToString(), x);
            }
            FirstRowStrs.Add("ID", null);
        }
        protected override void OnAllLoadEnd1()
        {
            base.OnAllLoadEnd1();
            if (!Prefers.IsCustomLanguage() &&
                PlatSDKMgr.IsSuportPlatformLanguage())
            {
                Switch(PlatSDKMgr.GetLanguageType());
            }
        }
        #endregion

        #region property
        public static string Space
        {
            get
            {
                if (LanguageType == LanguageType.English ||
                    LanguageType == LanguageType.Spanish)
                    return SysConst.UCD_NoBreakingSpace;
                return "";
            }
        }
        #endregion

        #region get
        // 当前的语言
        public LanguageType CurLangType => LanguageType;
        // 获得翻译
        public static string Get(string key)
        {
            if (key.IsInv()) return "";
            if (curDic == null) return SysConst.STR_Unkown;
            if (data.Count <= 0) return "";
            int lanIndex = EnumUtil<LanguageType>.Int(LanguageType);
            //获得当前的语言包
            if (curDic.Count == 0)
            {
                if (data.ContainsKey(lanIndex))
                    curDic = data[lanIndex];
            }
            //获得默认语言包
            if (defualtDic.Count == 0)
            {
                defualtDic = data[0];
            }
            if (curDic.ContainsKey(key))
            {
                return curDic[key];
            }
            else if (dynamicDic.ContainsKey(key))
            {
                return dynamicDic[key].Invoke();
            }
            return GetWrapKey();

            //当没有找到翻译内容时候，给key加个标记，提示开发者
            string GetWrapKey()
            {
                if(BuildConfig.Ins.IsTestLanguge)
                    return "*" + key;
                return key;
            }
        }
        public static string Get(string key, params object[] param)=> string.Format(Get(key), param);
        public static string Joint(string key, params object[] param)
        {
            string title = Get(key);
            foreach (var item in param)
            {
                title += item.ToString();
            }
            return title;
        }
        public static string JointColon(string key, params object[] param) => Joint(key,":", param);
        public static string RandLoadTip()=> Get(LoadTips.Rand());
        public static List<string> GetCategoryKeyList(string category)
        {
            if (!categoryKey.ContainsKey(category)) return new List<string>();
            return categoryKey[category];
        }
        public static string RandCategory(string category)
        {
            return Get(GetCategoryKeyList(category).Rand());
        }
        public static string[] GetAllLanguages()
        {
            List<string> data = new List<string>();
            foreach (var item in BuildConfig.Ins.Language)
            {
                data.Add(BaseLangMgr.Get(item.GetFull()));
            }
            return data.ToArray();
        }
        #endregion

        #region is
        /// <summary>
        /// 是否包含这个翻译
        /// </summary>
        /// <returns></returns>
        public static bool IsContain(string key)
        {
            if (lanKeys == null)
                return false;
            if (lanKeys.Contains(key))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region set
        // 切换语言
        public void Switch(LanguageType type)
        {
            LanguageType = type;
            int lanIndex = EnumUtil<LanguageType>.Int(LanguageType);
            if (data.ContainsKey(lanIndex))
            {
                curDic = data[lanIndex];
            }
            Callback_OnSwitchLanguage?.Invoke();
            if(BaseGlobal.SettingsMgr!=null)
                BaseGlobal.SettingsMgr.Settings.LanguageType = type;
            GlobalUITextMgr.RefreshFont();
        }
        public void Next()
        {
            int nextLang = (int)LanguageType;
            nextLang += 1;
            Array arrays = Enum.GetValues(typeof(LanguageType));
            if (nextLang >= arrays.Length)
                nextLang = 0;
            Switch((LanguageType)nextLang);
        }
        public void Prev()
        {
            int preLang = (int)LanguageType;
            preLang -= 1;
            Array arrays = Enum.GetValues(typeof(LanguageType));
            if (preLang < 0) preLang = arrays.Length - 1;
            Switch((LanguageType)preLang);
        }

        // 添加语言
        public void Add(LanguageType? type, string key, string desc, string fileName = "", string category = "")
        {
            //跳过无效的key和Desc
            if (type == null) return;
            if (key.IsInv() || desc.IsInv()) return;
            int lanIndex = EnumUtil<LanguageType>.Int(type.Value);
            if (!lanKeys.Contains(key)) lanKeys.Add(key);
            if (!data.ContainsKey(lanIndex)) data.Add(lanIndex, new Dictionary<string, string>());
            //添加到Key
            if (!data[lanIndex].ContainsKey(key))
            {
                data[lanIndex].Add(key, desc);
                if (key.StartsWith(SysConst.Prefix_Lang_Surname))
                    AllLastNames.Add(key);
                //自定义分类
                if (category != "")
                {
                    if (!categoryKey.ContainsKey(category))
                        categoryKey.Add(category, new HashList<string>());
                    categoryKey[category].Add(key);
                }
            }
            else
            {
                if (Application.isEditor)
                    CLog.Error("错误!重复的组建key:" + key + " 当前语言:" + type.ToString() + "  file Name:" + fileName);
                data[lanIndex][key] = desc;
            }
        }
        #endregion

        #region Life
        // 添加动态翻译
        private void OnAddDynamicDic() { }
        public IEnumerator Load()
        {
            Callback_OnParseStart?.Invoke();
            //Load Resources Language
            var textAssets = Resources.Load<TextAsset>("SysLanguage");
            LoadLangugeData(textAssets.bytes,textAssets.name);

            //加载DLC 的 Language
            foreach (var dlc in DLCManager.LoadedDLCItems.Values)
            {
                if (VersionUtil.IsEditorOrConfigMode)
                {
                    string[] fileList = dlc.GetAllLanguages();
                    if (fileList == null)
                        continue;
                    foreach (var item in fileList)
                    {
                        LoadLangugeData(File.ReadAllBytes(item), item);
                        yield return new WaitForEndOfFrame();
                    }
                }
                else
                {
                    var assetBundle = DLCManager.LoadRawBundle(dlc.Name,SysConst.BN_Language);
                    if (assetBundle != null)
                    {
                        foreach (var txt in assetBundle.LoadAllAssets<TextAsset>())
                        {
                            LoadLangugeData(txt.bytes,txt.name);
                            yield return new WaitForEndOfFrame();
                        }
                        foreach (var txt in assetBundle.LoadAllAssets<BytesAsset>())
                        {
                            LoadLangugeData(txt.Bytes, txt.name);
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }
            }

            OnAddDynamicDic();
            Callback_OnParseEnd?.Invoke();
            yield break;
        }

        void LoadLangugeData(byte[] buffer,string name)
        {
            if (ExcelUtil.IsBinary(buffer))
            {
                var workbook = ExcelUtil.ReadWorkbook(buffer);
                if (workbook == null)
                {
                    CLog.Error($"无法读取Excel文件:{name}");
                    return;
                }
                //读取每一个Sheet
                foreach (var table in workbook)
                {
                    CLog.Info($"读取xlsx:{name}");
                    ParseTable(table, name);
                }
            }
            else
            {
                var doc = ExcelUtil.ReadCSV(buffer);
                if (doc == null)
                {
                    CLog.Error($"无法读取CSV文件:{name}");
                    return;
                }
                CLog.Info("开始解析csv:" + name);
                ParseTable(doc, name);
            }
        }

        void ParseTable(Table table,string name)
        {
            if (table.Count <= 0)
                return;
            if (table is WorkSheet sheet)
            {
                if (sheet.Name.StartsWith(SysConst.Prefix_Notes))
                    return;
            }
            string lastedCategory = "";
            int NumberOfRows = table.Rows.Count;
            int NumberOfCols = table.Rows[0].Count;
            if (NumberOfRows <= 0 || NumberOfCols <= 0) return;
            //读取行
            for (int rowIndex = 0; rowIndex < NumberOfRows; ++rowIndex)
            {
                #region key
                //读取第一行
                if (rowIndex == 0)
                {
                    for (int colIndex = 0; colIndex < NumberOfCols; ++colIndex)
                    {
                        string tempStr = table.Rows[rowIndex][colIndex].ToString();
                        if (tempStr.IsInv()) continue;
                        if (!FirstRowStrs.ContainsKey(tempStr))
                        {
                            continue;
                        }
                    }
                }
                #endregion
                #region 读取翻译
                //读取非首行
                else
                {
                    string key = "";
                    for (int colIndex = 0; colIndex < NumberOfCols; ++colIndex)
                    {
                        if (table.Rows[rowIndex].Count <= colIndex) continue;
                        if (colIndex - 1 >= BuildConfig.Language.Count) continue;
                        //读取key
                        if (colIndex == 0)
                        {
                            key = table.Rows[rowIndex][colIndex].ToString();
                            //跳过注释
                            if (key.StartsWith(SysConst.Prefix_Notes))
                            {
                                continue;
                            }
                            //跳过分类
                            else if (key.StartsWith(SysConst.Prefix_Lang_Category))
                            {
                                lastedCategory = key.Remove(0, 1);
                                continue;
                            }
                            //自动ID
                            else if (key == SysConst.Prefix_Lang_AutoID)
                            {
                                key = IDUtil.GenString();
                            }
                            //跳过无效的字符
                            else if (key.IsInv())
                            {
                                continue;
                            }
                        }
                        //读取desc
                        else
                        {
                            string firstRowKey = table.Rows[0][colIndex].ToString();
                            if (!FirstRowStrs.ContainsKey(firstRowKey))
                                CLog.Error("语言包错误!无效的首行:" + firstRowKey);
                            string desc = table.Rows[rowIndex][colIndex].ToString();
                            if (desc.IsInv()) continue;
                            Add(FirstRowStrs[firstRowKey], key, desc, "", lastedCategory);
                        }
                    }
                }
                #endregion
            }
        }

        public string GetLoadInfo() => "Load language";
        #endregion
    }

}

