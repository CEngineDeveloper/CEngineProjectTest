using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace CYM
{
    public class ScriptConfigWindow : EditorWindow
    {
        private Editor editor;
        public static void ShowConfigWindow(SerializedScriptableObject config)
        {
            var window = EditorWindow.GetWindow<ScriptConfigWindow>(true, "ConfigWindow", true);
            // ֱ�Ӹ���ScriptableObject����һ��Editor
            window.editor = Editor.CreateEditor(config);
        }

        private void OnGUI()
        {
            // ֱ�ӵ���Inspector�Ļ�����ʾ
            this.editor.OnInspectorGUI();
        }
    }
}
