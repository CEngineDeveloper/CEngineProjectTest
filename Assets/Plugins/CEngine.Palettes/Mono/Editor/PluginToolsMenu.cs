//------------------------------------------------------------------------------
// PluginToolsMenu.cs
// Copyright 2023 2023/3/3 
// Created by CYM on 2023/3/3
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using UnityEngine;
using CYM;
using CYM.UI;
using UnityEditor;
using CYM.AssetPalette.Windows;

namespace CYM
{
    public partial class ToolsMenu : EditorWindow
    {
        [MenuItem("Tools/AssetsPlatte", priority = -10000)]
        static void ShowAssetsPlatte()
        {
            AssetPaletteWindow.ShowWindow();
        }
    }
}