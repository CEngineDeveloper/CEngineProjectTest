/*
Copyright (c) 2020 Omar Duarte
Unauthorized copying of this file, via any medium is strictly prohibited.
Writen by Omar Duarte, 2020.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using UnityEngine;

namespace CYM.TransformTools
{
    public class PlaceOnSurfaceWindow : BaseToolWindow
    {
        private PlaceOnSurfaceUtils.PlaceOnSurfaceData _data = new PlaceOnSurfaceUtils.PlaceOnSurfaceData();
        private static Vector3[] _directions =
        {
            Vector3.right, Vector3.left,
            Vector3.up, Vector3.down,
            Vector3.forward, Vector3.back
        };
        private int _dirIdx = 3;
        private int _orientDirIdx = 3;

        [UnityEditor.MenuItem("Tools/Transform Tools/Place On The Surface", false, 1500)]
        public static void ShowWindow() => GetWindow<PlaceOnSurfaceWindow>();

#if UNITY_2019_1_OR_NEWER
        public const string SHORTCUT_ID = "Transform Tools/Place On Surface";
        [UnityEditor.ShortcutManagement.Shortcut(SHORTCUT_ID)]
        public static void ShortcutAction() => GetWindow<PlaceOnSurfaceWindow>().Apply();
        public static string shortcut
            => UnityEditor.ShortcutManagement.ShortcutManager.instance.GetShortcutBinding(SHORTCUT_ID).ToString();
#endif

        protected override void OnGUI()
        {
            base.OnGUI();
            minSize = new Vector2(244, 170);
            titleContent = new GUIContent("Place On The Surface", null, "Place On The Surface");

            UnityEditor.EditorGUIUtility.labelWidth = 120;
            UnityEditor.EditorGUIUtility.fieldWidth = 100;
            using (new GUILayout.VerticalScope())
            {
                using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        _data.projectionDirectionSpace = (Space)UnityEditor.EditorGUILayout.Popup("Projection Space:",
                            (int)_data.projectionDirectionSpace, new string[] { "Global", "Local" });
                        GUILayout.FlexibleSpace();
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        _dirIdx = UnityEditor.EditorGUILayout.Popup("Pojection Direction:", 
                            _dirIdx, new string[] { "+X", "-X", "+Y", "-Y", "+Z", "-Z" });
                        _data.projectionDirection = _directions[_dirIdx];
                        GUILayout.FlexibleSpace();
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        using (var changeCheck = new UnityEditor.EditorGUI.ChangeCheckScope())
                        {
                            var mask = UnityEditor.EditorGUILayout.MaskField("Surface Layer:",
                                EditorGUIUtils.LayerMaskToField(_data.mask), UnityEditorInternal.InternalEditorUtility.layers);
                            if(changeCheck.changed)
                            {
                                _data.mask = EditorGUIUtils.FieldToLayerMask(mask);
                            }
                        }
                        GUILayout.FlexibleSpace();
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        _data.placeOnColliders = !UnityEditor.EditorGUILayout.ToggleLeft("Place on meshes without colliders",
                            !_data.placeOnColliders);
                        GUILayout.FlexibleSpace();
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        _data.surfaceDistance = UnityEditor.EditorGUILayout.FloatField("Surface Distance:",
                            _data.surfaceDistance);
                        GUILayout.FlexibleSpace();
                    }
                    using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
                    {
                        using (var toggleGroup = new UnityEditor.EditorGUILayout.ToggleGroupScope("Rotate to the Surface",
                            _data.rotateToSurface))
                        {
                            _data.rotateToSurface = toggleGroup.enabled;
                            using (new GUILayout.HorizontalScope())
                            {
                                UnityEditor.EditorGUIUtility.labelWidth -= 4;
                                _orientDirIdx = UnityEditor.EditorGUILayout.Popup("Object axis:", _orientDirIdx,
                                    new string[] { "+X", "-X", "+Y", "-Y", "+Z", "-Z" });
                                UnityEditor.EditorGUIUtility.labelWidth += 4;
                                _data.objectOrientation = _directions[_orientDirIdx];
                                GUILayout.FlexibleSpace();
                            }
                        }
                    }
                }
                using (new GUILayout.HorizontalScope())
                {
                    var statusStyle = new GUIStyle(UnityEditor.EditorStyles.label);

                    GUILayout.Space(8);
                    var statusMessage = "";
                    if (SelectionManager.topLevelSelection.Length == 0)
                    {
                        statusMessage = "No objects selected.";
                        GUILayout.Label(_warningIcon, new GUIStyle() { alignment = TextAnchor.LowerLeft });
                    }
                    else
                    {
                        statusMessage = SelectionManager.topLevelSelection.Length.ToString() + " objects selected.";
                    }
                    GUILayout.Label(statusMessage, statusStyle);
                    GUILayout.FlexibleSpace();
                    using (new UnityEditor.EditorGUI.DisabledGroupScope(SelectionManager.topLevelSelection.Length == 0))
                    {
                        if (GUILayout.Button(new GUIContent("Apply", string.Empty
#if UNITY_2019_1_OR_NEWER
                            + shortcut
#endif
                            ), UnityEditor.EditorStyles.miniButtonRight)) Apply();
                    }
                }
            }
        }

        private void Apply() => TransformTools.PlaceOnSurface(SelectionManager.topLevelSelection, _data);
    }
}
