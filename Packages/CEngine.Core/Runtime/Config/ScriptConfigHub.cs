//------------------------------------------------------------------------------
// ScriptableObjectConfigMgr.cs
// Copyright 2023 2023/3/11 
// Created by CYM on 2023/3/11
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CYM
{
    public static class ScriptConfigHub
    {
#if UNITY_EDITOR
        public static Dictionary<SerializedScriptableObject, Editor> ConfigWindows { get; private set; } = new Dictionary<SerializedScriptableObject, Editor>();

        public static void InitOnEditor()
        {
            GenerateCode();
            LoadOrCreate();
        }

        public static void GenerateCode()
        {
            LocalConfig.GenerateCode();
            VersionConfig.GenerateCode();
            BuildConfig.GenerateCode();
            DLCConfig.GenerateCode();
            GameConfig.GenerateCode();
            CursorConfig.GenerateCode();
            UIConfig.GenerateCode();
            ImportConfig.GenerateCode();
            LogConfig.GenerateCode();
            PluginConfig.GenerateCode();
            TestConfig.GenerateCode();
            AssetDatabase.Refresh();
        }
#endif
        public static void Load()
        {
            Debug.Log("加载配置开始!!");
            LocalConfig.DoLoad();
            VersionConfig.DoLoad();
            BuildConfig.DoLoad();
            DLCConfig.DoLoad();
            GameConfig.DoLoad();
            CursorConfig.DoLoad();
            UIConfig.DoLoad();
            ImportConfig.DoLoad();
            LogConfig.DoLoad();
            PluginConfig.DoLoad();
            TestConfig.DoLoad();
            Debug.Log("加载配置完毕!!");
        }
        public static void LoadOrCreate()
        {
            LocalConfig.DoLoadOrCreate();
            VersionConfig.DoLoadOrCreate();
            BuildConfig.DoLoadOrCreate();
            DLCConfig.DoLoadOrCreate();
            GameConfig.DoLoadOrCreate();
            CursorConfig.DoLoadOrCreate();
            UIConfig.DoLoadOrCreate();
            ImportConfig.DoLoadOrCreate();
            LogConfig.DoLoadOrCreate();
            PluginConfig.DoLoadOrCreate();
            TestConfig.DoLoadOrCreate();
        }
        public static bool IsAllLoaded() =>
            VersionConfig.IsLoaded &&
            BuildConfig.IsLoaded &&
            DLCConfig.IsLoaded &&
            LocalConfig.IsLoaded &&
            CursorConfig.IsLoaded &&
            UIConfig.IsLoaded &&
            ImportConfig.IsLoaded &&
            LogConfig.IsLoaded &&
            GameConfig.IsLoaded &&
            PluginConfig.IsLoaded &&
            TestConfig.IsLoaded;

        public static void SaveConfig()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(LocalConfig.Ins);
            EditorUtility.SetDirty(VersionConfig.Ins);
            EditorUtility.SetDirty(BuildConfig.Ins);
            EditorUtility.SetDirty(DLCConfig.Ins);
            EditorUtility.SetDirty(GameConfig.Ins);
            EditorUtility.SetDirty(CursorConfig.Ins);
            EditorUtility.SetDirty(UIConfig.Ins);
            EditorUtility.SetDirty(ImportConfig.Ins);
            EditorUtility.SetDirty(LogConfig.Ins);
            EditorUtility.SetDirty(PluginConfig.Ins);
            EditorUtility.SetDirty(TestConfig.Ins);
            AssetDatabase.SaveAssets();
#endif
        }
    }
}