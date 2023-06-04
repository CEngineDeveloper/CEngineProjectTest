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
    public abstract class HomogenizeToolWindow : BaseToolWindow
    {
        protected TransformTools.HomogenizeData _data = new TransformTools.HomogenizeData();

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            UnityEditor.EditorGUIUtility.labelWidth = 60;

            OnGUIValue();
            GUILayout.Space(8);
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
                    statusMessage = SelectionManager.topLevelSelection.Length + " objects selected.";
                }
                GUILayout.Label(statusMessage, statusStyle);
                GUILayout.FlexibleSpace();
                using (new UnityEditor.EditorGUI.DisabledGroupScope(SelectionManager.topLevelSelection.Length == 0))
                {
                    if (GUILayout.Button(new GUIContent("Homogenize", string.Empty
#if UNITY_2019_1_OR_NEWER
                            + (this is HomogenizeSpacingWindow ? HomogenizeSpacingWindow.shortcut
                            : this is HomogenizeRotationWindow ? HomogenizeRotationWindow.shortcut
                            : this is HomogenizeScaleWindow ? HomogenizeScaleWindow.shortcut : string.Empty)
#endif
                        ), UnityEditor.EditorStyles.miniButtonRight)) Homogenize();
                }
            }
        }

        protected virtual void OnGUIValue()
        {
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox)) //X
            {
                using (var toggleGroup = new UnityEditor.EditorGUILayout.ToggleGroupScope("Homogenize X", _data.x.homogenize))
                {
                    _data.x.homogenize = toggleGroup.enabled;
                    _data.x.strength = UnityEditor.EditorGUILayout.Slider("Strength:", _data.x.strength, 0f, 1f);
                }
            }
            GUILayout.Space(8);
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox)) //Y
            {
                using (var toggleGroup = new UnityEditor.EditorGUILayout.ToggleGroupScope("Homogenize Y", _data.y.homogenize))
                {
                    _data.y.homogenize = toggleGroup.enabled;
                    _data.y.strength = UnityEditor.EditorGUILayout.Slider("Strength:", _data.y.strength, 0f, 1f);
                }
            }
            GUILayout.Space(8);
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox)) //Z
            {
                using (var toggleGroup = new UnityEditor.EditorGUILayout.ToggleGroupScope("Homogenize Z", _data.z.homogenize))
                {
                    _data.z.homogenize = toggleGroup.enabled;
                    _data.z.strength = UnityEditor.EditorGUILayout.Slider("Strength:", _data.z.strength, 0f, 1f);
                }
            }
        }
        protected abstract void Homogenize();
    }

    public class HomogenizeSpacingWindow : HomogenizeToolWindow
    {
        [UnityEditor.MenuItem("Tools/Transform Tools/Homogenize Spacing", false, 1450)]
        public static void ShowWindow() => GetWindow<HomogenizeSpacingWindow>();

        protected override void OnEnable()
        {
            base.OnEnable();
            titleContent = new GUIContent("Homogenize Spacing");
            minSize = new Vector2(240, 185);
        }

#if UNITY_2019_1_OR_NEWER
        public const string SHORTCUT_ID = "Transform Tools/Homogenize Spacing";
        [UnityEditor.ShortcutManagement.Shortcut(SHORTCUT_ID)]
        public static void ShortcutAction() => GetWindow<HomogenizeSpacingWindow>().Homogenize();
        public static string shortcut
            => UnityEditor.ShortcutManagement.ShortcutManager.instance.GetShortcutBinding(SHORTCUT_ID).ToString();
#endif
        protected override void Homogenize() 
            => TransformTools.HomogenizeSpacing(SelectionManager.topLevelSelection, _data);
    }

    public class HomogenizeRotationWindow : HomogenizeToolWindow
    {
        [UnityEditor.MenuItem("Tools/Transform Tools/Homogenize Rotation", false, 1450)]
        public static void ShowWindow() => GetWindow<HomogenizeRotationWindow>();

        protected override void OnEnable()
        {
            base.OnEnable();
            titleContent = new GUIContent("Homogenize Rotation");
            minSize = new Vector2(240, 185);
        }

#if UNITY_2019_1_OR_NEWER
        public const string SHORTCUT_ID = "Transform Tools/Homogenize Rotation";
        [UnityEditor.ShortcutManagement.Shortcut(SHORTCUT_ID)]
        public static void ShortcutAction() => GetWindow<HomogenizeRotationWindow>().Homogenize();
        public static string shortcut
            => UnityEditor.ShortcutManagement.ShortcutManager.instance.GetShortcutBinding(SHORTCUT_ID).ToString();
#endif
        protected override void Homogenize()
            => TransformTools.HomogenizeRotation(SelectionManager.topLevelSelection, _data);
    }

    public class HomogenizeScaleWindow : HomogenizeToolWindow
    {
        private bool _separateAxes = false;

        [UnityEditor.MenuItem("Tools/Transform Tools/Homogenize Scale", false, 1450)]
        public static void ShowWindow() => GetWindow<HomogenizeScaleWindow>();

        protected override void OnEnable()
        {
            base.OnEnable();
            titleContent = new GUIContent("Homogenize Scale");
        }

#if UNITY_2019_1_OR_NEWER
        public const string SHORTCUT_ID = "Transform Tools/Homogenize Scale";
        [UnityEditor.ShortcutManagement.Shortcut(SHORTCUT_ID)]
        public static void ShortcutAction() => GetWindow<HomogenizeScaleWindow>().Homogenize();
        public static string shortcut
            => UnityEditor.ShortcutManagement.ShortcutManager.instance.GetShortcutBinding(SHORTCUT_ID).ToString();
#endif
        protected override void Homogenize()
            => TransformTools.HomogenizeScale(SelectionManager.topLevelSelection, _data);

        protected override void OnGUIValue()
        {
            UnityEditor.EditorGUIUtility.labelWidth = 90;
            _separateAxes = UnityEditor.EditorGUILayout.Toggle("Separate Axes", _separateAxes);
            UnityEditor.EditorGUIUtility.labelWidth = 60;

            if (_separateAxes)
            {
                minSize = new Vector2(240, 205);
                base.OnGUIValue();
            }
            else
            {
                minSize = new Vector2(240, 80);
                using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        _data.x.strength = _data.y.strength = _data.z.strength 
                            = UnityEditor.EditorGUILayout.Slider("Strength:", _data.x.strength, 0f, 1f);
                    }
                }
            }
        }
    }
}
