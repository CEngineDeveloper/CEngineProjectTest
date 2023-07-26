//------------------------------------------------------------------------------
// AssetBundleConfig.cs
// Copyright 2018 2018/5/18 
// Created by CYM on 2018/5/18
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Sirenix.OdinInspector;
using CYM.DLC;
using UnityEditor;
using System;
using System.Reflection;
#pragma warning disable CS0414
namespace CYM
{
    public partial class DLCConfig : ScriptConfig<DLCConfig>
    {
        #region private inspector
        [SerializeField,ReadOnly, HideInInspector] 
        List<BuildRuleConfig> InnerBuildRule = new List<BuildRuleConfig>();
        [SerializeField, ReadOnly, HideInInspector]
        List<string> IgnoreConst = new List<string>();
        #endregion

        #region Config inspector
        //配置资源
        [DLCCopy][SerializeField, FoldoutGroup("Copy Rule"), ReadOnly] BuildRuleType Config = BuildRuleType.Whole;
        [DLCCopy][SerializeField, FoldoutGroup("Copy Rule"), ReadOnly] BuildRuleType Lua = BuildRuleType.Whole;
        [DLCCopy][SerializeField, FoldoutGroup("Copy Rule"), ReadOnly] BuildRuleType Language = BuildRuleType.Whole;
        [DLCCopy][SerializeField, FoldoutGroup("Copy Rule"), ReadOnly] BuildRuleType Excel = BuildRuleType.Whole;
        [DLCCopy][SerializeField, FoldoutGroup("Copy Rule"), ReadOnly] BuildRuleType Bytes = BuildRuleType.Whole;
        //图片资源
        [DLCBundle][SerializeField, FoldoutGroup("System Rule")] BuildRuleType BG = BuildRuleType.Whole;
        [DLCBundle][SerializeField, FoldoutGroup("System Rule")] BuildRuleType Icon = BuildRuleType.Whole;
        [DLCBundle][SerializeField, FoldoutGroup("System Rule")] BuildRuleType Head = BuildRuleType.Whole;
        [DLCBundle][SerializeField, FoldoutGroup("System Rule")] BuildRuleType Texture = BuildRuleType.Whole;
        [DLCBundle][SerializeField, FoldoutGroup("System Rule")] BuildRuleType Illustration = BuildRuleType.Whole;
        //其他资源
        [DLCBundle][SerializeField, FoldoutGroup("System Rule")] BuildRuleType Audio = BuildRuleType.Whole;
        [DLCBundle][SerializeField, FoldoutGroup("System Rule")] BuildRuleType Narration = BuildRuleType.Whole;
        [DLCBundle][SerializeField, FoldoutGroup("System Rule")] BuildRuleType Material = BuildRuleType.Whole;
        [DLCBundle][SerializeField, FoldoutGroup("System Rule")] BuildRuleType Music = BuildRuleType.Whole;
        [DLCBundle][SerializeField, FoldoutGroup("System Rule")] BuildRuleType Video = BuildRuleType.Whole;
        [DLCBundle][SerializeField, FoldoutGroup("System Rule")] BuildRuleType Animator = BuildRuleType.Whole;
        //Prefab资源
        [DLCBundle][SerializeField, FoldoutGroup("System Rule")] BuildRuleType Prefab = BuildRuleType.Whole;
        [DLCBundle][SerializeField, FoldoutGroup("System Rule")] BuildRuleType Perform = BuildRuleType.Whole;
        [DLCBundle][SerializeField, FoldoutGroup("System Rule")] BuildRuleType UI = BuildRuleType.Whole;
        [DLCBundle][SerializeField, FoldoutGroup("System Rule"), ReadOnly] BuildRuleType System = BuildRuleType.Whole;
        [DLCBundle][SerializeField, FoldoutGroup("System Rule"), ReadOnly] BuildRuleType Scene = BuildRuleType.File;
        #endregion

        #region inspector
        //自定义规则
        [SerializeField, FoldoutGroup("Custom Rule")] List<BuildRuleConfig> BuildRule = new List<BuildRuleConfig>();
        //其他
        [HideReferenceObjectPicker]
        [SerializeField, FoldoutGroup("Custom DLC")] List<DLCItemConfig> DLC = new List<DLCItemConfig>();
        #endregion

        #region dlc config JSON配置文件
        //默认DLC
        public DLCItemConfig ConfigNative { get; private set; }
        //扩展DLC 不包含 Native
        public List<DLCItemConfig> ConfigExtend { get; private set; } = new List<DLCItemConfig>();
        public List<DLCItemConfig> ConfigAll { get; private set; } = new List<DLCItemConfig>();
        #endregion

        #region editor dlc item DLC配置资源,由配置文件生成
        public DLCItem EditorNative { get; private set; }
        public List<DLCItem> EditorExtend { get; private set; } = new List<DLCItem>();
        public List<DLCItem> EditorAll { get; private set; } = new List<DLCItem>();
        #endregion

        #region runtime 资源打包规则
        public HashSet<string> RuntimeAllDirectory { get; private set; } = new HashSet<string>();
        public List<BuildRuleConfig> RuntimeConfig { get; private set; } = new List<BuildRuleConfig>();
        public List<string> RuntimeCopyDirectory { get; private set; } = new List<string>();
        public HashSet<string> RuntimeIgnoreConstSet { get; private set; } = new HashSet<string>();
        public HashSet<string> RuntimeHashBuildRuleConfig { get; private set; } = new HashSet<string>();
        #endregion

        #region life
        public override bool IsHideInBuildWindow => true;
        //资源创建的时候刷新一次
        public override void OnCreate()
        {
            base.OnCreate();
            Refresh();
        }
        //DLC加载的时候刷新一次
        public override void OnInited()
        {
            base.OnInited();
            Refresh();
        }
        [Button(nameof(Refresh))]
        public void Refresh()
        {
            InnerBuildRule.Clear();
            RuntimeCopyDirectory.Clear();
            RuntimeHashBuildRuleConfig.Clear();
            IgnoreConst.Clear();

            Relection();

            //添加自定义规则
            foreach (var item in BuildRule)
            {
                AddBuildRule(InnerBuildRule, item.Clone() as BuildRuleConfig);
            }
            //忽略的Const
            IgnoreConst.Add("CONFIG_DLCItem");
            IgnoreConst.Add("CONFIG_DLCManifest");

            //刷新DLC
            RuntimeConfig.Clear();
            RuntimeAllDirectory.Clear();
            RuntimeCopyDirectory.Clear();
            RuntimeIgnoreConstSet.Clear();
            RuntimeHashBuildRuleConfig.Clear();
            ConfigExtend.Clear();
            ConfigAll.Clear();
            EditorExtend.Clear();
            EditorAll.Clear();

            //添加BuildConfig
            foreach (var item in InnerBuildRule)
                AddBuildConfig(RuntimeConfig, RuntimeCopyDirectory, item);
            //添加IgnoreConst
            foreach (var item in IgnoreConst)
                AddIgnoreConst(item);

            //加载原生dlc
            ConfigNative = new DLCItemConfig(SysConst.STR_NativeDLC);
            EditorNative = new DLCItem(ConfigNative);

            //加载其他额外DLC
            foreach (var item in DLC)
            {
                var dlcItem = new DLCItemConfig(item.Name);
                ConfigExtend.Add(dlcItem);
                EditorExtend.Add(new DLCItem(dlcItem));
            }

            ConfigAll = new List<DLCItemConfig>(ConfigExtend)
            {
                ConfigNative
            };

            EditorAll = new List<DLCItem>(EditorExtend)
            {
                EditorNative
            };

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                foreach (var item in RuntimeAllDirectory)
                {
                    FileUtil.EnsureDirectory(Path.Combine(SysConst.Path_Bundles, item));
                }

                foreach (var item in DLC)
                {
                    foreach (var dic in RuntimeAllDirectory)
                    {
                        if (item.Name.ToLower() == SysConst.BN_System)
                            continue;
                        FileUtil.EnsureDirectory(Path.Combine(SysConst.Path_Dlc, item.Name, dic));
                    }
                }
                AssetDatabase.Refresh();
            }
#endif
        }

        void Relection()
        {
            var data = typeof(DLCConfig).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var item in data)
            {
                if (item == null)
                    continue;
                var name = item.Name;
                var attrCopy = item.GetCustomAttribute<DLCCopyAttribute>();
                var attrBundle = item.GetCustomAttribute<DLCBundleAttribute>();
                if (attrCopy != null)
                {
                    var val = (BuildRuleType)item.GetValue(this);
                    AddBuildRule(InnerBuildRule, new BuildRuleConfig(name, val, true));
                }
                else if (attrBundle != null)
                {
                    var val = (BuildRuleType)item.GetValue(this);
                    AddBuildRule(InnerBuildRule, new BuildRuleConfig(name, val, false));
                }
            }
        }

        void AddBuildRule(List<BuildRuleConfig> runtimedata, BuildRuleConfig config)
        {
            if (RuntimeHashBuildRuleConfig.Contains(config.Name))
            {
                CLog.Error($"错误！发现重复的打包规则：{config.Name}");
                return;
            }
            RuntimeHashBuildRuleConfig.Add(config.Name);
            BuildRuleConfig data = config.Clone() as BuildRuleConfig;
            runtimedata.Add(data);
        }
        void AddBuildConfig(List<BuildRuleConfig> runtimedata,List<string> copyDir,BuildRuleConfig config)
        {
            if (RuntimeHashBuildRuleConfig.Contains(config.Name))
            {
                CLog.Error($"错误！发现重复的打包规则：{config.Name}");
                return;
            }
            RuntimeHashBuildRuleConfig.Add(config.Name);

            BuildRuleConfig data = config.Clone() as BuildRuleConfig;

            if (data.IsCopyDirectory)
            {
                if (VersionUtil.RealResBuildType == ResBuildType.Bundle)
                {
                    runtimedata.Add(data);
                }
                else if (VersionUtil.RealResBuildType == ResBuildType.Config)
                {
                    copyDir.Add(data.Name);
                }
            }
            else
            {
                runtimedata.Add(data);
            }
            if (!RuntimeAllDirectory.Contains(data.Name))
                RuntimeAllDirectory.Add(data.Name);
        }
        void AddIgnoreConst(string name)
        {
            if (!RuntimeIgnoreConstSet.Contains(name))
                RuntimeIgnoreConstSet.Add(name);
        }
        public void RemoveDLC(string name)
        {
            DLC.RemoveAll(x => x.Name == name);
            Refresh();
        }
        public void AddDLC(string name)
        {
            DLC.Add(new DLCItemConfig(name));
            Refresh();
        }
        #endregion

        #region is
        public bool IsInIgnoreConst(string name)
        {
            return RuntimeIgnoreConstSet.Contains(name);
        }
        #endregion
    }
}