//------------------------------------------------------------------------------
// ToolsMenu.cs
// Created by CYM on 2022/1/2
// 填写类的描述...
//------------------------------------------------------------------------------
using UnityEditor;
using CYM.AssetPalette.Windows;

namespace CYM
{
    public partial class ToolsMenu : EditorWindow
    {
        [MenuItem("Tools/Build  &`", priority = -10000)]
        static void ShowBuildWindow()
        {
            BuildWindow.ShowBuildWindow();
        }
    }
}