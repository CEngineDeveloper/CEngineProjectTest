//------------------------------------------------------------------------------
// PluginConsole.cs
// Copyright 2022 2022/10/3 
// Created by CYM on 2022/10/3
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using Sirenix.OdinInspector;
using UnityEngine;
namespace CYM
{
    public partial class TestConfig : ScriptConfig<TestConfig>
    {
        [FoldoutGroup("Camera"), SerializeField]
        public bool IsLockCamera = false;
    }
}