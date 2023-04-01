//------------------------------------------------------------------------------
// PluginGameConfig.cs
// Copyright 2023 2023/3/7 
// Created by CYM on 2023/3/7
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using UnityEngine;
using CYM;
using CYM.UI;
using Sirenix.OdinInspector;
using HighlightPlus;

namespace CYM
{
    public partial class GameConfig : ScriptConfig<GameConfig>
    {
        #region FastLine
        [SerializeField, FoldoutGroup("Surf")]
        public HighlightProfile HighlightProfile;
        #endregion
    }
}