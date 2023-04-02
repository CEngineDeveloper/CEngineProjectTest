using UnityEngine;

namespace CYM.AssetPalette.Runtime.Macros
{
    /// <summary>
    /// Macros for dealing with save data.
    /// </summary>
    public static class SaveDataMacros 
    {
        public static void ResetSaveData()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorUtility.DisplayDialog(
                "Reset Save Data", "Are you sure you want to reset your save data?", "Yes"))
            {
                PlayerPrefs.DeleteAll();
            }
#endif
        }
        
        public static void ResetEditorSaveData()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorUtility.DisplayDialog(
                "Reset Editor Save Data", "Are you sure you want to reset your editor save data?" +
                    "This may reset a lot of preferences for Unity and its extensions.",
                "Yes, I know what I'm doing."))
            {
                UnityEditor.EditorPrefs.DeleteAll();
            }
#endif
        }
    }
}
