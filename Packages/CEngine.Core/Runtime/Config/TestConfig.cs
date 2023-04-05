//------------------------------------------------------------------------------
// BuildLocalConfig.cs
// Copyright 2018 2018/5/3 
// Created by CYM on 2018/5/3
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using Sirenix.OdinInspector;
using UnityEngine;

namespace CYM
{
    public partial class TestConfig : ScriptConfig<TestConfig>
    {
        public override bool IsHideInBuildWindow => false;

        [FoldoutGroup("Core"), ShowInInspector]
        public static bool IsIgnoreCondition = false;
    }
}