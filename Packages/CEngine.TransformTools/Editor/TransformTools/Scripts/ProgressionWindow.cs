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
    public abstract class ProgressionWindow : BaseToolWindow
    {
        protected TransformTools.ProgressionData _data = new TransformTools.ProgressionData();
        protected enum Attribute
        {
            POSITION,
            ROTATION,
            SCALE
        }
        protected Attribute _attribute = Attribute.POSITION;
        
        protected override void OnGUI()
        {
            base.OnGUI();
            
            GUILayout.BeginVertical();
            {
                ToolSettings();
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                {
                    var statusStyle = new GUIStyle(UnityEditor.EditorStyles.label);

                    GUILayout.Space(8);
                    var statusMessage = "";
                    if (SelectionManager.topLevelSelection.Length == 0)
                    {
                        statusMessage = "No objects selected.";
                        GUILayout.Label(_warningIcon,
                            new GUIStyle() { alignment = TextAnchor.LowerLeft });
                    }
                    else
                    {
                        statusMessage = SelectionManager.topLevelSelection.Length + " objects selected.";
                    }
                    GUILayout.Label(statusMessage, statusStyle);
                    GUILayout.FlexibleSpace();
                    UnityEditor.EditorGUI.BeginDisabledGroup(SelectionManager.topLevelSelection.Length == 0);
                    if (GUILayout.Button("Apply", UnityEditor.EditorStyles.miniButtonRight))
                    {
                        Apply();
                    }
                    UnityEditor.EditorGUI.EndDisabledGroup();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        protected virtual void ToolSettings()
        {
            UnityEditor.EditorGUIUtility.labelWidth = 74;
            UnityEditor.EditorGUIUtility.fieldWidth = 110;
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                _data.arrangeOrder = (TransformTools.ArrangeBy)UnityEditor.EditorGUILayout.Popup("Arrange by:",
                    (int)_data.arrangeOrder, new string[] { "Selection order", "Hierarchy order" });
            }
            GUILayout.EndHorizontal();
            UnityEditor.EditorGUIUtility.fieldWidth = 100;
            GUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    _data.type = (TransformTools.IncrementalDataType)UnityEditor.EditorGUILayout.Popup("Value Type:",
                        (int)_data.type,
                        _attribute == Attribute.POSITION ? new string[] { "Constant delta", "Curve", "Object Size"}
                        : new string[] { "Constant delta", "Curve"});
                }
                GUILayout.EndHorizontal();
                if (_data.type == TransformTools.IncrementalDataType.CONSTANT_DELTA)
                {
                    minSize += new Vector2(0, 40);
                    GUILayout.BeginHorizontal();
                    {
                        _data.constantDelta = UnityEditor.EditorGUILayout.Vector3Field("Value:", _data.constantDelta,
                            GUILayout.Width(200));
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                }
                else if (_data.type == TransformTools.IncrementalDataType.CURVE)
                {
                    minSize  += new Vector2(0, 120);
                    GUILayout.BeginHorizontal();
                    {
                        _data.curveRangeMin = UnityEditor.EditorGUILayout.Vector3Field("Min: ", _data.curveRangeMin,
                            GUILayout.Width(200));
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    {
                        _data.curveRangeSize = UnityEditor.EditorGUILayout.Vector3Field("Size: ", _data.curveRangeSize,
                            GUILayout.Width(200));
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Label("Value:");
                    GUILayout.BeginHorizontal();
                    {
                        UnityEditor.EditorGUIUtility.labelWidth = 10;
                        UnityEditor.EditorGUIUtility.fieldWidth = 44;
                        GUILayout.Space(15);
                        _data.x.curve = UnityEditor.EditorGUILayout.CurveField("X", _data.x.curve, Color.red,
                            _data.GetRect(AxesUtils.Axis.X));
                        GUILayout.Space(1);
                        _data.y.curve = UnityEditor.EditorGUILayout.CurveField("Y", _data.y.curve, Color.green,
                            _data.GetRect(AxesUtils.Axis.Y));
                        GUILayout.Space(1);
                        _data.z.curve = UnityEditor.EditorGUILayout.CurveField("Z", _data.z.curve, Color.blue,
                            _data.GetRect(AxesUtils.Axis.Z));
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.Label("Ovewrite:");
                GUILayout.BeginHorizontal();
                {
                    UnityEditor.EditorGUIUtility.labelWidth = 10;
                    UnityEditor.EditorGUIUtility.fieldWidth = 44;
                    GUILayout.Space(15);
                    _data.x.overwrite = UnityEditor.EditorGUILayout.Toggle("X", _data.x.overwrite);
                    GUILayout.Space(31);
                    _data.y.overwrite = UnityEditor.EditorGUILayout.Toggle("Y", _data.y.overwrite);
                    GUILayout.Space(31);
                    _data.z.overwrite = UnityEditor.EditorGUILayout.Toggle("Z", _data.z.overwrite);
                    GUILayout.FlexibleSpace();
                    
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(4);
            }
            GUILayout.EndVertical();
        }

        protected abstract void Apply();
    }

    public class PositionProgressionWindow : ProgressionWindow
    {
        private bool _orientToPath = false;
        private static Vector3[] _directions =
        {
            Vector3.right, Vector3.left,
            Vector3.up, Vector3.down,
            Vector3.forward, Vector3.back
        };
        private int _dirIdx = 0;


        [UnityEditor.MenuItem("Tools/Transform Tools/Position Progression", false, 1300)]
        public static void ShowWindow() => GetWindow<PositionProgressionWindow>();

#if UNITY_2019_1_OR_NEWER
        public const string SHORTCUT_ID = "Transform Tools/Progression - Position";
        [UnityEditor.ShortcutManagement.Shortcut(SHORTCUT_ID)]
        public static void ShortcutAction() => GetWindow<PositionProgressionWindow>().Apply();
        public static string shortcut
            => UnityEditor.ShortcutManagement.ShortcutManager.instance.GetShortcutBinding(SHORTCUT_ID).ToString();
#endif

        protected override void Apply() => TransformTools.IncrementalPosition(SelectionManager.topLevelSelection,
            _data, _orientToPath, _directions[_dirIdx]);

        protected override void OnEnable()
        {
            base.OnEnable();
            _attribute = ProgressionWindow.Attribute.POSITION;
            titleContent = new GUIContent("Position Progression");
        }

        protected override void ToolSettings()
        {
            minSize = new Vector2(220, 155);
            base.ToolSettings();
            GUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
            {
                UnityEditor.EditorGUIUtility.labelWidth = 74;
                UnityEditor.EditorGUIUtility.fieldWidth = 44;
                _orientToPath = UnityEditor.EditorGUILayout.BeginToggleGroup("Orient to the path", _orientToPath);
                {
                    GUILayout.BeginHorizontal();
                    {
                        _dirIdx = UnityEditor.EditorGUILayout.Popup("Object axis:", _dirIdx,
                            new string[] { "+X", "-X", "+Y", "-Y", "+Z", "-Z" });
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                }
                UnityEditor.EditorGUILayout.EndToggleGroup();
            }
            GUILayout.EndVertical();
        }
    }
    public class RotationProgressionWindow : ProgressionWindow
    {
        [UnityEditor.MenuItem("Tools/Transform Tools/Rotation Progression", false, 1300)]
        public static void ShowWindow() => GetWindow<RotationProgressionWindow>();

#if UNITY_2019_1_OR_NEWER
        public const string SHORTCUT_ID = "Transform Tools/Progression - Rotation";
        [UnityEditor.ShortcutManagement.Shortcut(SHORTCUT_ID)]
        public static void ShortcutAction() => GetWindow<RotationProgressionWindow>().Apply();
        public static string shortcut
            => UnityEditor.ShortcutManagement.ShortcutManager.instance.GetShortcutBinding(SHORTCUT_ID).ToString();
#endif

        protected override void Apply() => TransformTools.IncrementalRotation(SelectionManager.topLevelSelection,
            _data);

        protected override void OnEnable()
        {
            base.OnEnable();
            _attribute = ProgressionWindow.Attribute.ROTATION;
            titleContent = new GUIContent("Rotation Progression");
        }
        protected override void ToolSettings()
        {
            minSize = new Vector2(220, 110);
            base.ToolSettings();
        }
    }

    public class ScaleProgressionWindow : ProgressionWindow
    {
        [UnityEditor.MenuItem("Tools/Transform Tools/Scale Progression", false, 1300)]
        public static void ShowWindow() => GetWindow<ScaleProgressionWindow>();

#if UNITY_2019_1_OR_NEWER
        public const string SHORTCUT_ID = "Transform Tools/Progression - Scale";
        [UnityEditor.ShortcutManagement.Shortcut(SHORTCUT_ID)]
        public static void ShortcutAction() => GetWindow<ScaleProgressionWindow>().Apply();
        public static string shortcut
            => UnityEditor.ShortcutManagement.ShortcutManager.instance.GetShortcutBinding(SHORTCUT_ID).ToString();
#endif
        protected override void Apply() => TransformTools.IncrementalScale(SelectionManager.topLevelSelection, _data);

        protected override void OnEnable()
        {
            base.OnEnable();
            _attribute = ProgressionWindow.Attribute.SCALE;
            titleContent = new GUIContent("Scale Progression");
        }

        protected override void ToolSettings()
        {
            minSize = new Vector2(220, 110);
            base.ToolSettings();
        }
    }
}
