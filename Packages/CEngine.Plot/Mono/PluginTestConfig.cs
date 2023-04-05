//------------------------------------------------------------------------------
// PluginSysConsole.cs
// Copyright 2023 2023/1/21 
// Created by CYM on 2023/1/21
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using Sirenix.OdinInspector;

namespace CYM
{
    public partial class TestConfig : ScriptConfig<TestConfig>
    {
        [FoldoutGroup("Plot"), ShowInInspector]
        public static bool IsNoPlot = false;
    }
}