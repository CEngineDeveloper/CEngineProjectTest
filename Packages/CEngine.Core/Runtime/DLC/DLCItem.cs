//------------------------------------------------------------------------------
// DLCConfig.cs
// Copyright 2018 2018/11/7 
// Created by CYM on 2018/11/7
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace CYM.DLC
{    
     
    // DLC配置文件
    [Serializable][HideReferenceObjectPicker]
    public class DLCItemConfig
    {
        public string Name;
        public bool IsActive = true;
        public int Version = 0;
        public DLCItemConfig(string name)
        {
            Name = name;
        }
    }
    // 打包规则
    public enum BuildRuleType
    {
        Whole,  //根据文件夹打包
        Path,   //根据完整路径打包
        File,   //根据文件打包
        Fold,   //根据子文件夹打包
    }
    // AB加载方式
    public enum AssetBundleLoadType
    {
        Normal,
        Async,
        Scene,
    }
    // 打包规则数据
    [Serializable]
    public class BuildRuleConfig : ICloneable
    {
        public BuildRuleConfig(string name, BuildRuleType type,bool copyDir=false)
        {
            Name = name;
            BuildRuleType = type;
            IsCopyDirectory = copyDir;
        }
        //搜索路径
        public string Name;
        //打包规则
        public BuildRuleType BuildRuleType = BuildRuleType.Whole;
        public bool IsCopyDirectory = false;
        public bool IsSystem => Name.ToLower() == SysConst.BN_System;
        //自定义根目录
        [HideInInspector]
        public string CustomRootPath { get; set; } = null;
        //完整路径 eg. Level/Data = Assets/Bundles/Native/Level/Data
        public string FullSearchPath { get; set; }
        public object Clone()=> MemberwiseClone();
    }
    //资源路径数据
    [Serializable]
    public class AssetPathData
    {
        //BuildRule名称
        public string BuildRuleName;
        //原始Bundle名称,比如原先这个资源是放在Icon下面的,在打包过程中被整理到Shared下面
        public string SourceBundleName;
        //完整路径 eg. Assets/CYMCommon/Plugins/CYM/_Bundle/Icon/Logo/Logo_巴西龟.png
        public string FullPath;
        //文件名称 eg. Logo_巴西龟
        public string FileName;
        //分类
        public string Category;
    }
    //Bundle数据
    [Serializable]
    public class BundleData
    {
        //所在DLC的名称 eg. Native
        public string DLCName;
        //资源Bundle的完整名称
        public string BundleName;
        //共享Bundle?
        public bool IsShared;
        //资源完整路径
        public List<AssetPathData> AssetFullPaths = new List<AssetPathData>();
    }
    //资源清单
    [Serializable]
    public class DLCManifest
    {
        [SerializeField]
        //Bundle单位 eg. Icon,Music,Video,Shared的散包
        public List<BundleData> Data = new List<BundleData>();
    }
    public class DownloadData
    {
        public bool IsPredownload = false;
        public string HashCode = "";
    }


    /// <summary>
    /// dlc 资源配置
    /// </summary>
    [Serializable]
    public partial class DLCItem
    {
        #region static
        //public readonly static Dictionary<string, AssetBundle> RawBundles = new Dictionary<string, AssetBundle>();
        //public readonly static Dictionary<string, UnityEngine.Object> RawAssets = new Dictionary<string, UnityEngine.Object>();
        //DLC配置文件
        static private DLCConfig DLCConfig => DLCConfig.Ins;
        static bool IsEditorMode => VersionUtil.IsEditorMode;
        #endregion

        #region res path
        //生成目标路径
        public string TargetPath { get; private set; }
        //完整Root路径
        public string AbsRootPath { get; private set; }
        string AbsRootPathBuild { get; set; }
        //根目录
        public string AssetsRootPath { get; private set; }
        string AssetsRootPathBuild { get; set; }
        //Manifest 路径
        public string ManifestPath { get; private set; }
        //const 路径
        public string ConstPathBuild { get; private set; }
        public string ManifestPathBuild { get; private set; }
        public string ConfigPathBuild { get; private set; }
        #endregion

        #region prop
        public DLCItemConfig Config { get; private set; }
        //内置DLC
        public DLCItem InternalDLC { get; set; }
        //依赖文件
        private AssetBundleManifest ABManifest { get; set; }
        public List<BuildRuleConfig> FinalBuildRule { get; protected set; } = new List<BuildRuleConfig>();
        public List<string> FinalCopyDirectory { get; protected set; } = new List<string>();
        //需拷贝的文件的完整路径
        private List<string> AbsCopyDirectory { get; set; } = new List<string>();
        #endregion

        #region inspector
        //DLC的名称 eg. Native
        public string Name => Config.Name;
        //DLC是否激活
        public bool IsActive => Config.IsActive;
        public DLCItem(DLCItemConfig config, AssetBundleManifest assetBundleManifest=null)
        {
            Config = config;
            ABManifest = assetBundleManifest;

            FinalBuildRule.Clear();
            FinalCopyDirectory.Clear();

            foreach (var item in DLCConfig.RuntimeConfig)
                FinalBuildRule.Add(item.Clone() as BuildRuleConfig);
            FinalCopyDirectory.AddRange(DLCConfig.RuntimeCopyDirectory.ToArray());

            //计算DLC的跟目录
            AssetsRootPath = CalcAssetRootPath(Name,true);
            //计算出绝对路径(拷贝文件使用)
            AbsRootPath = Path.Combine(Application.dataPath, AssetsRootPath.Replace("Assets/", ""));
            //计算出目标路径
            TargetPath = Path.Combine(SysConst.Path_StreamingAssets, Name);

            //计算Build路径
            ConstPathBuild = Path.Combine(SysConst.RPath_Funcs, SysConst.Dir_Main, Name + "Const.cs");
            ManifestPathBuild = Path.Combine(AssetsRootPath, SysConst.Dir_Config, SysConst.STR_DLCManifest + ".json");
            ConfigPathBuild = Path.Combine(AssetsRootPath, SysConst.Dir_Config, SysConst.STR_DLCItem + ".json");
            //计算Manifest路径
            if (IsEditorMode) ManifestPath = Path.Combine(AssetsRootPath, SysConst.Dir_Config);
            else ManifestPath = Path.Combine(TargetPath, SysConst.Dir_Config);
        }
        #endregion

        #region set
        public void PreBuild()
        {
            AssetsRootPathBuild = CalcAssetRootPath(Name,true);
            AbsRootPathBuild = Path.Combine(Application.dataPath, AssetsRootPathBuild.Replace("Assets/", ""));
            AbsCopyDirectory.Clear();
            if (FinalCopyDirectory != null)
            {
                for (int i = 0; i < FinalCopyDirectory.Count; ++i)
                {
                    AbsCopyDirectory.Add(Path.Combine(AbsRootPathBuild, FinalCopyDirectory[i]));
                }
            }
            foreach (var item in FinalBuildRule)
            {
                string tempRootPath = AssetsRootPathBuild;
                if (!item.CustomRootPath.IsInv())
                    tempRootPath = item.CustomRootPath;
                item.FullSearchPath = tempRootPath + "/" + item.Name;
            }
        }
        public void OnPostBuildBundle()
        {
            //修改文件名
            string path = Path.Combine(Application.streamingAssetsPath, Name) +"\\";
            string source = path + Name;
            string target = path + Name + SysConst.Extention_AssetBundle;
            if (File.Exists(target))
            {
                File.Delete(target);
            }
            if(File.Exists(source))
                File.Move(source, target);
            //资源包模式不拷贝
            if (VersionUtil.RealResBuildType == ResBuildType.Config)
            {
                //拷贝非打包资源到指定目录
                for (int i = 0; i < AbsCopyDirectory.Count; ++i)
                {
                    string absPath = AbsCopyDirectory[i];
                    if (!Directory.Exists(absPath))
                        continue;
                    string dir = FinalCopyDirectory[i];
                    string finalTargetPath = Path.Combine(TargetPath, dir);
                    FileUtil.EnsureDirectory(finalTargetPath);
                    FileUtil.CopyDir(absPath, finalTargetPath, false, true);
                }
            }
        }
        #endregion

        #region get all path
        public string[] GetAllLanguages() => GetAllFilies(SysConst.Dir_Language, "*.*");
        public string[] GetAllLuas() => GetAllFilies(SysConst.Dir_Lua, "*.lua");
        public string[] GetAllBytes() => GetAllFilies(SysConst.Dir_Bytes, "*.*");
        public string[] GetAllExcel() => GetAllFilies(SysConst.Dir_Excel, "*.*");
        public string[] GetAllText() => GetAllFilies(SysConst.Dir_Text, "*.*");
        string[] GetAllFilies(string dir,string extension)
        {
            string fullPath;
            if (IsEditorMode)
                fullPath = Path.Combine(AssetsRootPath, dir);
            else
                fullPath = Path.Combine(TargetPath, dir);

            List<string> fileList = new List<string>();
            var ret = FileUtil.GetFiles(fullPath, extension, SearchOption.AllDirectories)?.Where(x=>!x.EndsWith(".meta"));
            return ret?.ToArray();
        }
        #endregion

        #region get file
        public string GetManifest() => Path.Combine(ManifestPath, SysConst.STR_DLCManifest + ".json");
        public DLCManifest GetManifestFile()
        {
            DLCManifest reader = null;
            if (VersionUtil.IsEditorOrConfigMode)
            {
                reader = FileUtil.LoadJson<DLCManifest>(GetManifest());
            }
            else
            {
                var asset = DLCManager.LoadRawAsset<TextAsset>(Name,SysConst.BN_Config, "DLCManifest");
                if (asset != null)
                {
                    reader = JsonUtility.FromJson<DLCManifest>(asset.text);
                }
            }
            return reader;
        }
        #endregion

        #region get
        public string[] GetAllDependencies(string assetBundleName)
        {
            if (ABManifest == null) return null;
            return ABManifest.GetAllDependencies(assetBundleName);
        }
        private string CalcAssetRootPath(string dlcName,bool force=false)
        {
            if (IsEditorMode || force)
            {
                string editorDLCPath = "";
                if (IsNative)
                {
                    editorDLCPath = SysConst.RPath_Bundles;
                    return editorDLCPath;
                }
                else
                {
                    editorDLCPath = SysConst.RPath_Dlc;
                    return Path.Combine(editorDLCPath, dlcName);
                }

            }
            else return Path.Combine(SysConst.Path_StreamingAssets, dlcName);
        }
        #endregion

        #region is
        public bool IsNative => Name == SysConst.STR_NativeDLC;
        #endregion
    }

}