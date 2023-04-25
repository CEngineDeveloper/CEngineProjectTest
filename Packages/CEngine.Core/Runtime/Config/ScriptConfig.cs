//------------------------------------------------------------------------------
// BaseScriptableObjectConfig.cs
// Copyright 2018 2018/3/28 
// Created by CYM on 2018/3/28
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using UnityEngine;
using Sirenix.OdinInspector;
using System.IO;
#if UNITY_EDITOR
using CodegenCS;
using UnityEditor;
#endif

namespace CYM
{
    public interface IScriptConfig
    {
        void OnCreate();
        void OnCreated();
        void OnInited();
        void OnUse();
        bool IsHideInBuildWindow { get; }
    }
    [HideMonoScript]
    public class ScriptConfig<T> : SerializedScriptableObject, ISerializationCallbackReceiver, IScriptConfig 
        where T : SerializedScriptableObject, IScriptConfig
    {
        public static bool IsLoaded => _ins != null;
        public virtual bool IsHideInBuildWindow => false;

        static T _ins;
        public static T Ins
        {
            get
            {
                if (_ins == null)
                {
                    DoLoad();
                }
                _ins?.OnUse();
                return _ins;
            }
        }
        public virtual void OnCreate() { }
        public virtual void OnCreated() { }
        public virtual void OnInited() { }
        public virtual void OnUse()
        {
        }
        public static void DoLoadOrCreate()
        {
            DoLoad();
            DoCreate();
        }
        public static void DoLoad()
        {
            string fileName = typeof(T).Name;
            _ins = SystemRes.LoadConfig<T>(fileName);
            _ins?.OnInited();
#if UNITY_EDITOR
            if (_ins)
            {
                if (!ScriptConfigHub.ConfigWindows.ContainsKey(_ins))
                {
                    ScriptConfigHub.ConfigWindows.Add(_ins, Editor.CreateEditor(_ins));
                }
            }
#endif 
        }
        public static void DoCreate()
        {
            string fileName = typeof(T).Name;
#if UNITY_EDITOR
            if (_ins == null)
            {
                if (Application.isEditor)
                {
                    _ins = CreateInstance<T>();
                    _ins.OnCreate();
                    _ins.OnCreated();
                    CLog.Green($"{typeof(T).Name}配置文件不存在，将自动创建，请查看Resources/Temp目录");
                }
                else
                {
                    CLog.Green($"{typeof(T).Name}配置文件不存在");
                }
                var path = string.Format(SysConst.Format_ResourcesTempAssetPath, fileName);
                AssetDatabase.CreateAsset(_ins, path);
            }
#endif
        }

#if UNITY_EDITOR
        [Button]
        public static void GenerateCode()
        {
            string configName = typeof(T).Name;
            string configFullName = typeof(T).FullName;
            string fileName = Path.Combine(SysConst.Path_FuncsConfig, configName + ".cs");
            FileUtil.EnsureDirectory(SysConst.Path_FuncsConfig);
            if (File.Exists(fileName))
            {
                return;
            }
            var w = new CodegenTextWriter($"{fileName}", System.Text.Encoding.UTF8);
            w.WriteLine($"using UnityEngine;");
            w.WithCurlyBraces($"namespace {BuildConfig.NameSpace}", () =>
            {
                w.WriteLine($"[CreateAssetMenu(menuName = \"ScriptConfig/{configName}\")]");
                w.WithCurlyBraces($"public partial class {configName}:{configFullName}", () =>
                {
                    w.WriteLine($"public static new {configName} Ins => {configFullName}.Ins as {configName};");
                });
            });
            w.Flush();
            w.Dispose();          
        }
#endif  
    }
}