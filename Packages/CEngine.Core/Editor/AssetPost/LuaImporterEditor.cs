using UnityEditor;
using UnityEditor.AssetImporters;
namespace CYM
{
    [CustomEditor(typeof(LuaImporter))]
    public class LuaImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Lua file");
            ApplyRevertGUI();
        }
    }
}