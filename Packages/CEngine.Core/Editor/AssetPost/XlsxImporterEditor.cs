using UnityEditor;
using UnityEditor.AssetImporters;

namespace CYM
{
    [CustomEditor(typeof(XlsxImporter))]
    public class XlsxImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Excel file");
            ApplyRevertGUI();
        }
    }
}