//------------------------------------------------------------------------------
// PluginSysConsole.cs
// Copyright 2022 2022/12/26 
// Created by CYM on 2022/12/26
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using Sirenix.OdinInspector;
using UnityEngine;

namespace CYM
{
    public partial class TestConfig : ScriptConfig<TestConfig>
    {
        [FoldoutGroup("Person"), SerializeField]
        public bool IsFastPersonDeath = false;
    }
}