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
    public class RadialArrangeToolWindow : BaseToolWindow
    {
        private TransformTools.RadialArrangeData _data = new TransformTools.RadialArrangeData();
        private static readonly Vector3[] _axes =
        {
            Vector3.right, Vector3.left,
            Vector3.up, Vector3.down,
            Vector3.forward, Vector3.back
        };
        private static readonly string[] _axesOptions = { "+X", "-X", "+Y", "-Y", "+Z", "-Z" };
        private static readonly string[] _spaceOptions = { "Global", "Local" };

        private int _axisIdx = 4;
        private int _orientDirIdx = 0;
        private System.Collections.Generic.List<Vector3> _parallelAxes = null;
        private int _parallelDirIdx = 0;
        private System.Collections.Generic.List<string> _parallelAxesOptions = null;
        private bool _lastUpdateSpacing = true;

        [UnityEditor.MenuItem("Tools/Transform Tools/Radial Arrangement", false, 1201)]
        public static void ShowWindow() => GetWindow<RadialArrangeToolWindow>();

#if UNITY_2019_1_OR_NEWER
        public const string SHORTCUT_ID = "Transform Tools/Radial Arrangement";
        [UnityEditor.ShortcutManagement.Shortcut(SHORTCUT_ID)]
        public static void ShortcutAction() => GetWindow<RadialArrangeToolWindow>().Arrange();
        public static string shortcut
            => UnityEditor.ShortcutManagement.ShortcutManager.instance.GetShortcutBinding(SHORTCUT_ID).ToString();
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            titleContent = new GUIContent("Radial Arrangement");
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            minSize = new Vector2(250, 305);

            UnityEditor.EditorGUIUtility.labelWidth = 74;
            UnityEditor.EditorGUIUtility.fieldWidth = 110;

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                _data.arrangeBy = (TransformTools.ArrangeBy)UnityEditor.EditorGUILayout.Popup("Arrange by:",
                    (int)_data.arrangeBy, new string[] { "Selection order", "Hierarchy order" });
            }

            UnityEditor.EditorGUIUtility.labelWidth = 90;
            UnityEditor.EditorGUIUtility.fieldWidth = 140;
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                using (new GUILayout.HorizontalScope())
                {
                    _data.rotateAround = (TransformTools.RotateAround)UnityEditor.EditorGUILayout.Popup("Rotate around:",
                        (int)_data.rotateAround,new string[] { "Selection Center", "Transform position",
                            "Object bounds center", "Custom position" });
                    GUILayout.FlexibleSpace();
                }

                var disableCenterField = _data.rotateAround != TransformTools.RotateAround.CUSTOM_POSITION;
                if (_data.rotateAround == TransformTools.RotateAround.TRANSFORM_POSITION
                    || _data.rotateAround == TransformTools.RotateAround.OBJECT_BOUNDS_CENTER)
                {
                    minSize += new Vector2(0, 40);
                    using (new GUILayout.HorizontalScope())
                    {
                        _data.centerTransform = (Transform)UnityEditor.EditorGUILayout.ObjectField("Transform:",
                            _data.centerTransform, typeof(Transform), true);
                        GUILayout.FlexibleSpace();
                    }
                    UnityEditor.EditorGUIUtility.fieldWidth = 100;
                    _data.space = (Space)UnityEditor.EditorGUILayout.Popup("space:",
                        (int)_data.space, _spaceOptions, GUILayout.Width(235));
                }
                else if (_data.rotateAround == TransformTools.RotateAround.SELECTION_CENTER)
                    _data.UpdateCenter(SelectionManager.topLevelSelection);
                using (new UnityEditor.EditorGUI.DisabledGroupScope(disableCenterField))
                {
                    _data.center = UnityEditor.EditorGUILayout.Vector3Field("Center", _data.center);
                }
                UnityEditor.EditorGUIUtility.fieldWidth = 140;
                _axisIdx = UnityEditor.EditorGUILayout.Popup("Rotation axis:", _axisIdx, _axesOptions, GUILayout.Width(235));
                _data.axis = _axes[_axisIdx];

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Ovewrite:", GUILayout.Width(87));
                    UnityEditor.EditorGUIUtility.labelWidth = 10;
                    _data.overwriteX = UnityEditor.EditorGUILayout.Toggle("X", _data.overwriteX);
                    _data.overwriteY = UnityEditor.EditorGUILayout.Toggle("Y", _data.overwriteY);
                    _data.overwriteZ = UnityEditor.EditorGUILayout.Toggle("Z", _data.overwriteZ);
                    GUILayout.FlexibleSpace();
                    UnityEditor.EditorGUIUtility.labelWidth = 90;
                }
            }

            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                using (new GUILayout.HorizontalScope())
                {
                    _data.shape = (TransformTools.Shape)UnityEditor.EditorGUILayout.Popup("Shape:",
                        (int)_data.shape, new string[] { "Circle", "Circular Spiral",
                            "Ellipse", "Elliptical Spiral" });
                    GUILayout.FlexibleSpace();
                }
                switch (_data.shape)
                {
                    case TransformTools.Shape.CIRCLE:
                        minSize += new Vector2(0, 40);
                        using (var check = new UnityEditor.EditorGUI.ChangeCheckScope())
                        {
                            _data.endEllipseAxes = _data.startEllipseAxes = Vector2.one
                                * UnityEditor.EditorGUILayout.FloatField("Radius:", _data.startEllipseAxes.x,
                                GUILayout.Width(235));
                            if (check.changed)
                            {
                                _data.UpdateCircleSpacing(SelectionManager.topLevelSelection.Length);
                                _lastUpdateSpacing = true;
                            }
                        }
                        using (var check = new UnityEditor.EditorGUI.ChangeCheckScope())
                        {
                            _data.spacing = UnityEditor.EditorGUILayout.FloatField("Spacing:", _data.spacing,
                                GUILayout.Width(235));
                            if (check.changed)
                            {
                                _data.UpdateCircleRadius(SelectionManager.topLevelSelection.Length);
                                _lastUpdateSpacing = false;
                            }
                        }
                        break;
                    case TransformTools.Shape.CIRCULAR_SPIRAL:
                        minSize += new Vector2(0, 40);
                        _data.startEllipseAxes = Vector2.one * UnityEditor.EditorGUILayout.FloatField("Start Radius:",
                            _data.startEllipseAxes.x, GUILayout.Width(235));
                        _data.endEllipseAxes = Vector2.one * UnityEditor.EditorGUILayout.FloatField("End Radius:",
                            _data.endEllipseAxes.x, GUILayout.Width(235));
                        break;
                    case TransformTools.Shape.ELLIPSE:
                        minSize += new Vector2(0, 40);
                        _data.endEllipseAxes = _data.startEllipseAxes = UnityEditor.EditorGUILayout.Vector2Field("Ellipse axes:",
                            _data.startEllipseAxes, GUILayout.Width(235));
                        break;
                    case TransformTools.Shape.ELLIPTICAL_SPIRAL:
                        _data.startEllipseAxes = UnityEditor.EditorGUILayout.Vector2Field("Start ellipse axes:",
                            _data.startEllipseAxes, GUILayout.Width(235));
                        _data.endEllipseAxes = UnityEditor.EditorGUILayout.Vector2Field("End ellipse axes:",
                            _data.endEllipseAxes, GUILayout.Width(235));
                        minSize += new Vector2(0, 80);
                        break;
                }
            }

            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                _data.startAngle = UnityEditor.EditorGUILayout.FloatField("Start angle:", _data.startAngle, GUILayout.Width(235));
                using (var check = new UnityEditor.EditorGUI.ChangeCheckScope())
                {
                    _data.maxArcAngle = UnityEditor.EditorGUILayout.FloatField("Max arc angle:", _data.maxArcAngle,
                        GUILayout.Width(235));
                    UnityEditor.EditorGUIUtility.labelWidth = 170;
                    _data.lastSpotEmpty = UnityEditor.EditorGUILayout.ToggleLeft("Add an empty spot at the end",
                        _data.lastSpotEmpty);
                    if (check.changed) UpdateCircleRadiusAndSpacing();
                }
                UnityEditor.EditorGUIUtility.labelWidth = 90;
            }

            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                using (var toggleGroup = new UnityEditor.EditorGUILayout.ToggleGroupScope("Orient to the center",
                    _data.orientToRadius))
                {
                    _data.orientToRadius = toggleGroup.enabled;
                    UnityEditor.EditorGUI.BeginChangeCheck();
                    _orientDirIdx = UnityEditor.EditorGUILayout.Popup("Radial axis:", _orientDirIdx, _axesOptions,
                        GUILayout.Width(235));
                    _data.orientDirection = _axes[_orientDirIdx];
                    if (UnityEditor.EditorGUI.EndChangeCheck() || _parallelAxes == null)
                    {
                        _parallelAxes = new System.Collections.Generic.List<Vector3>(_axes);
                        _parallelAxesOptions = new System.Collections.Generic.List<string>(_axesOptions);
                        if (_orientDirIdx < 2)
                        {
                            _parallelAxes.RemoveAt(0);
                            _parallelAxes.RemoveAt(0);
                            _parallelAxesOptions.RemoveAt(0);
                            _parallelAxesOptions.RemoveAt(0);
                        }
                        else if (_orientDirIdx < 4)
                        {
                            _parallelAxes.RemoveAt(2);
                            _parallelAxes.RemoveAt(2);
                            _parallelAxesOptions.RemoveAt(2);
                            _parallelAxesOptions.RemoveAt(2);
                        }
                        else
                        {
                            _parallelAxes.RemoveAt(4);
                            _parallelAxes.RemoveAt(4);
                            _parallelAxesOptions.RemoveAt(4);
                            _parallelAxesOptions.RemoveAt(4);
                        }
                        _parallelDirIdx = 0;
                    }
                    _parallelDirIdx = UnityEditor.EditorGUILayout.Popup("Parallel axis:", _parallelDirIdx,
                        _parallelAxesOptions.ToArray(), GUILayout.Width(235));
                    _data.parallelDirection = _parallelAxes[_parallelDirIdx];
                }
            }

            GUILayout.Space(2);
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(8);
                var statusMessage = "";
                if (SelectionManager.topLevelSelection.Length == 0)
                {
                    statusMessage = "No objects selected.";
                    GUILayout.Label(_warningIcon, new GUIStyle() { alignment = TextAnchor.LowerLeft });
                }
                else statusMessage = SelectionManager.topLevelSelection.Length + " objects selected.";
                GUILayout.Label(statusMessage, UnityEditor.EditorStyles.label);
                GUILayout.FlexibleSpace();
                using (new UnityEditor.EditorGUI.DisabledGroupScope(SelectionManager.topLevelSelection.Length == 0))
                {
                    if (GUILayout.Button("Apply", UnityEditor.EditorStyles.miniButtonRight)) Arrange();
                }
            }
        }

        private void Arrange() => TransformTools.RadialArrange(SelectionManager.topLevelSelection, _data);

        private void UpdateCircleRadiusAndSpacing()
        {
            if (_data.shape == TransformTools.Shape.CIRCLE)
            {
                if (_lastUpdateSpacing) _data.UpdateCircleSpacing(SelectionManager.topLevelSelection.Length);
                else _data.UpdateCircleRadius(SelectionManager.topLevelSelection.Length);
            }
        }

        protected void OnSelectionChange()
        {
            _data.UpdateCenter(SelectionManager.topLevelSelection);
            UpdateCircleRadiusAndSpacing();
        }

        private void Update() => _data.UpdateCenter();
    }
}
