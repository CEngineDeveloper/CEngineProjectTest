//------------------------------------------------------------------------------
// PluginGameConfig.cs
// Copyright 2023 2023/2/21 
// Created by CYM on 2023/2/21
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using CYM.SLGEnvirom;

namespace CYM
{
    public partial class GameConfig : ScriptConfig<GameConfig>
    {
        #region FastLine
        [SerializeField, FoldoutGroup("FastLine")]
        public string FastlineRender;
        #endregion
    }
}