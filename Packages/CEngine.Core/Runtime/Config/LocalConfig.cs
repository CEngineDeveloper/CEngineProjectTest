//------------------------------------------------------------------------------
// BuildLocalConfig.cs
// Copyright 2018 2018/5/3 
// Created by CYM on 2018/5/3
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using UnityEngine;

namespace CYM
{
    public class LocalConfig : ScriptConfig<LocalConfig>
    {
        public override bool IsHideInBuildWindow => true;

        #region editor setting
        public bool FoldLang = false;
        public bool FoldInfo = false;
        public bool FoldVersion = false;
        public bool FoldSetting = false;
        public bool FoldLog = false;
        public bool FoldSettingSteam = false;
        public bool FoldSettingBuild = false;
        public bool FoldDLC = false;
        public bool FoldBuild = false;
        public bool FoldHotFix = false;
        public bool FoldHotFixGenerate = false;
        public bool FoldExplorer = false;
        public bool FoldSceneList = false;
        public bool FoldOther = false;
        public bool FoldSubWindow = false;
        public bool FoldConfigWindow = false;
        public bool FoldPluginWindow = false;

        public string ABDownloadPath = "";
        #endregion

        #region UI prefab
        public bool FoldRawComponent = false;
        public bool FoldComponent = false;
        public bool FoldView = false;
        #endregion

        #region opetion
        [SerializeField]
        public bool IsSimulationEditor = true;
        public bool IsUnityDevelopmentBuild;
        public BuildType BuildType = BuildType.Develop;
        public string LastBuildTime;
        public Platform Platform = Platform.Windows64;
        public int Distribution;
        #endregion
    }
}