using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
//------------------------------------------------------------------------------
// BuildConfig.cs
// Created by CYM on 2018/9/5
// 填写类的描述...
//------------------------------------------------------------------------------
namespace CYM
{
    public enum ResBuildType
    { 
        Config,
        Bundle,
    }
    public sealed class BuildConfig : ScriptConfig<BuildConfig> 
    {
        #region prop
        public override bool IsHideInBuildWindow => true;
        #endregion



        #region Inspector
        [FoldoutGroup("Resulution"), SerializeField]
        public int Width = 1920;
        [FoldoutGroup("Resulution"), SerializeField]
        public int Height = 1080;

        [FoldoutGroup("Logo"), SerializeField]
        public bool IsShowLogo;
        [FoldoutGroup("Logo"), SerializeField]
        public Sprite StartLogo;
        [FoldoutGroup("Logo"), SerializeField]
        public List<LogoData> Logos = new List<LogoData>();

        [FoldoutGroup("Progress"), SerializeField]
        public float ProgressWidth = 100;
        [FoldoutGroup("Progress"), SerializeField]
        public float ProgressHeight = 5;
        [FoldoutGroup("Progress"), SerializeField]
        public Texture2D ProgressBG;
        [FoldoutGroup("Progress"), SerializeField]
        public Texture2D ProgressFill;

        public string CompanyName = "CYM";
        [HideInInspector]
        public string NameSpace => "Gamelogic";
        [HideInInspector]
        public string MainUIView = "MainUIView";
        public bool IsShowWinClose = true;
        public bool IsShowConsoleBnt = false;
        public bool IgnoreChecker;
        public string Name = "MainTitle";

        //将所有的配置资源打成AssetBundle来读取，适合移动平台
        [SerializeField]
        public ResBuildType ResBuildType =  ResBuildType.Config;
        [SerializeField,ShowInInspector]
        public HashSet<LanguageType> Language = new HashSet<LanguageType>();
        [SerializeField]
        public bool IsTestLanguge = true;
        [SerializeField]
        public bool IsDiscreteShared = true; //是否为离散的共享包
        [SerializeField]
        public bool IsForceBuild = false;
        [SerializeField]
        public bool IsCompresse = true; //是否为压缩格式
        //是否初始化的时候加载所有的Directroy Bundle
        [SerializeField]
        public bool IsInitLoadDirBundle = true;
        //是否初始化的时候加载所有Shared Bundle
        [SerializeField]
        public bool IsInitLoadSharedBundle = true;
        [SerializeField]
        public bool IsWarmupAllShaders = true;
        //是否将_Bundle和Resource里共享依赖的资源打包到Shared包里面,如果要开发热更新游戏，此选项需要保持为true
        [SerializeField]
        public bool IsSafeBuild = true;

        public bool IsHotFix = false;
        public string PublicIP = "127.0.0.1:8001";
        public string TestIP = "127.0.0.1:8001";
        #endregion

        #region Job system
        [SerializeField, FoldoutGroup("Job System")]
        public int UnitJobPerFrame = 100;
        [SerializeField, FoldoutGroup("Job System")]
        public int GlobalJobPerFrame = 100;
        [SerializeField, FoldoutGroup("Job System")]
        public int ViewJobPerFrame = 10;
        [SerializeField, FoldoutGroup("Job System")]
        public int NormalJobPerFrame = 100;
        #endregion

#if UNITY_EDITOR
        public UnityEditor.ScriptingImplementation ScriptingImplementation = UnityEditor.ScriptingImplementation.Mono2x;
#endif
    }
}