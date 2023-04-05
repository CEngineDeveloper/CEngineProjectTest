//------------------------------------------------------------------------------
// PluginSysConsole.cs
// Copyright 2022 2022/12/26 
// Created by CYM on 2022/12/26
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using Sirenix.OdinInspector;

namespace CYM
{
    public partial class TestConfig : ScriptConfig<TestConfig>
    {
        [FoldoutGroup("Person"), ShowInInspector]
        public static bool IsFastPersonDeath = false;
    }
}