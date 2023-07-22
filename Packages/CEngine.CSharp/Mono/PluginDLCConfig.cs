//------------------------------------------------------------------------------
// PluginDLCConfig.cs
// Copyright 2023 2023/2/27 
// Created by CYM on 2023/2/27
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using UnityEngine;
using CYM.DLC;
using Sirenix.OdinInspector;
#pragma warning disable CS0414
namespace CYM
{
    public partial class DLCConfig : ScriptConfig<DLCConfig>
    {
        [DLCCopy][SerializeField, FoldoutGroup("Copy Rule"), ReadOnly] BuildRuleType CSharp = BuildRuleType.Whole;
    }
}