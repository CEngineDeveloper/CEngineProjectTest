using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace CYM.DevConsole
{
    public static class ToolsMenu 
    {
        [MenuItem("Tools/CreateDevConsoleConfig")]
        public static void CreateConfig()
        {
            AssetDatabase.CopyAsset("Packages/com.cengine.devconsole/ConfigTemp/DevConsoleSettings.asset", "Assets/Resources/DevConsoleSettings.asset");
        }
    }
}