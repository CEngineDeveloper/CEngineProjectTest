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
    public class BuildConfig : ScriptConfig<BuildConfig> 
    {
        #region prop
        public override bool IsHideInBuildWindow => false;
        #endregion

        #region Manual config
        [FoldoutGroup("Resulution"), SerializeField]
        public int Width = 1920;
        [FoldoutGroup("Resulution"), SerializeField]
        public int Height = 1080;

        [FoldoutGroup("Starter Logo"), SerializeField]
        [HideInInspector]
        public bool IsShowLogo;
        [FoldoutGroup("Starter Logo"), SerializeField]
        public List<LogoData> Logos = new List<LogoData>();

        [FoldoutGroup("Starter Loading"), SerializeField,AssetsOnly]
        public GameObject StarterUIPrefab;
        [FoldoutGroup("Starter Loading"), SerializeField]
        public float LoadingStartDelay = 0.2f;
        [FoldoutGroup("Starter Loading"), SerializeField]
        public float LoadingEndDelay = 2.0f;
        [FoldoutGroup("Starter Loading")]
        public float LoadingSpeed = 2;
        [FoldoutGroup("Starter Loading")]
        public Color LoadingBGColor = Color.black;
        [FoldoutGroup("Starter Loading")]
        public Sprite LoadingBGLogo;
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


        #region Inspector
        [ReadOnly, FoldoutGroup("Other")]
        public string CompanyName = "CYM";
        [ReadOnly, FoldoutGroup("Other")]
        public static string NameSpace => "Gamelogic";
        [ReadOnly, FoldoutGroup("Other")]
        public string MainUIView = "MainUIView";
        [ReadOnly, FoldoutGroup("Other")]
        public bool IsShowWinClose = true;
        [ReadOnly, FoldoutGroup("Other")]
        public bool IsShowConsoleBnt = false;
        [ReadOnly, FoldoutGroup("Other")]
        public bool IgnoreChecker;
        [ReadOnly, FoldoutGroup("Other")]
        public string Name = "MainTitle";
        //将所有的配置资源打成AssetBundle来读取，适合移动平台
        [SerializeField]
        [ReadOnly, FoldoutGroup("Other")]
        public ResBuildType ResBuildType =  ResBuildType.Config;
        [SerializeField, ReadOnly, FoldoutGroup("Other")]
        public HashSet<LanguageType> Language = new HashSet<LanguageType>();
        [SerializeField]
        [ReadOnly, FoldoutGroup("Other")]
        public bool IsTestLanguge = true;
        [SerializeField]
        [ReadOnly, FoldoutGroup("Other")]
        public bool IsDiscreteShared = true; //是否为离散的共享包
        [SerializeField]
        [ReadOnly, FoldoutGroup("Other")]
        public bool IsForceBuild = false;
        [SerializeField]
        [ReadOnly, FoldoutGroup("Other")]
        public bool IsCompresse = true; //是否为压缩格式
        //是否初始化的时候加载所有的Directroy Bundle
        [SerializeField]
        [ReadOnly, FoldoutGroup("Other")]
        public bool IsInitLoadDirBundle = true;
        //是否初始化的时候加载所有Shared Bundle
        [SerializeField]
        [ReadOnly, FoldoutGroup("Other")]
        public bool IsInitLoadSharedBundle = true;
        [SerializeField]
        [ReadOnly, FoldoutGroup("Other")]
        public bool IsWarmupAllShaders = true;
        //是否将_Bundle和Resource里共享依赖的资源打包到Shared包里面,如果要开发热更新游戏，此选项需要保持为true
        [SerializeField]
        [ReadOnly, FoldoutGroup("Other")]
        public bool IsSafeBuild = true;
        [ReadOnly, FoldoutGroup("Other")]
        public bool IsHotFix = false;
        [ReadOnly, FoldoutGroup("Other")]
        public string PublicIP = "127.0.0.1:8001";
        [ReadOnly, FoldoutGroup("Other")]
        public string TestIP = "127.0.0.1:8001";
        #endregion

#if UNITY_EDITOR
        [ReadOnly, FoldoutGroup("Other")]
        public UnityEditor.ScriptingImplementation ScriptingImplementation = UnityEditor.ScriptingImplementation.Mono2x;
#endif
    }
}