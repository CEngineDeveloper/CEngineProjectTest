//------------------------------------------------------------------------------
// LogConfig.cs
// Copyright 2020 2020/7/9 
// Created by CYM on 2020/7/9
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static CYM.CLog;

namespace CYM
{
    public sealed class LogConfig : ScriptConfig<LogConfig>
    {
        [HideInInspector]
        public bool Enable = true;
        [HideInInspector]
        public LogLevel Level = LogLevel.Warn;
        [HideInInspector]
        public Dictionary<string, TagInfo> Tags = new Dictionary<string, TagInfo>();

        [SerializeField]
        [HideReferenceObjectPicker]
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout,KeyLabel = "")]
        SerializableDic<string, LogItem> Items = new SerializableDic<string, LogItem>();

        public static LogItem Get(string key)
        {
            if (Ins.Items.ContainsKey(key))
            {
                Ins.Items[key].OnFetch(key);
                return Ins.Items[key];
            }
            else
            {
                Ins.Items.Add(key,new LogItem(key));
                return Get(key);
            }
        }

        #region life
        public override bool IsHideInBuildWindow => true;
        [RuntimeInitializeOnLoadMethod()]
        static void Initlized()
        {
            foreach (var item in Ins.Tags)
            {
                item.Value.Init();
            }
            Init(Ins.Enable, Ins.Level, Ins.Tags);
        }
        public override void OnCreate()
        {
            base.OnCreate();
        }
        public override void OnInited()
        {
            base.OnInited();

        }
        #endregion
    }
}