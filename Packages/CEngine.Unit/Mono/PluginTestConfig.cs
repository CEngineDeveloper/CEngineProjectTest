//------------------------------------------------------------------------------
// PluginConsole.cs
// Copyright 2022 2022/10/3 
// Created by CYM on 2022/10/3
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using UnityEngine;
using Sirenix.OdinInspector;

namespace CYM
{
    public partial class TestConfig : ScriptConfig<TestConfig>
    {
        [FoldoutGroup("Unit"), ShowInInspector]
        public static bool IsAllAlert = false;
        [FoldoutGroup("Unit"), ShowInInspector]
        public static bool IsNoEvent = false;
        [FoldoutGroup("Unit"), ShowInInspector]
        public static bool IsMustEvent = false;
    }
}