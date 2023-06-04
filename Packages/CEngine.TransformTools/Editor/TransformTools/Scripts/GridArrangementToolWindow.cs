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
    public class GridArrangementToolWindow : BaseToolWindow
    {
        #region WINDOW
        private TransformTools.ArrangeData _data = new TransformTools.ArrangeData();
        private GUIStyle _buttonStyle = null;

        private readonly System.Collections.Generic.Dictionary<AxesUtils.Axis, GUIContent[]> _alignBtnDict
            = new System.Collections.Generic.Dictionary<AxesUtils.Axis, GUIContent[]>
        {
            {AxesUtils.Axis.X,  new GUIContent[3]},
            {AxesUtils.Axis.Y,  new GUIContent[3]},
            {AxesUtils.Axis.Z,  new GUIContent[3]}
        };

        private readonly string[] _priorityOptions = new string[] { "1", "2", "3" };
        private readonly string[] _directionOptionsX = new string[] { "+X", "-X" };
        private readonly string[] _directionOptionsY = new string[] { "+Y", "-Y" };
        private readonly string[] _directionOptionsZ = new string[] { "+Z", "-Z" };
        private readonly string[] _widthOptions = new string[] { "Widest object per column",
            "Widest object selected", "Custom" };
        private readonly string[] _heightOptions = new string[] { "Tallest object per column",
            "Tallest object selected", "Custom" };
        private readonly string[] _sortByOptions = new string[] { "Selection Order", "Current Position",
            "Hierarchy Order" };
        private readonly string[] _arrangeRelativeToOptions = new string[] { "Selection Bounds", "First Object" };
        private readonly string[] _alingObjPropOptions = new string[] { "Bounding Box", "Pivot" };

        [UnityEditor.MenuItem("Tools/Transform Tools/Grid Arrangement", false, 1200)]
        public static void ShowWindow() => GetWindow<GridArrangementToolWindow>();

#if UNITY_2019_1_OR_NEWER
        public const string SHORTCUT_ID = "Transform Tools/Grid Arrangement";
        [UnityEditor.ShortcutManagement.Shortcut(SHORTCUT_ID)]
        public static void GridArrange() => GetWindow<GridArrangementToolWindow>().Arrange();
        public static string shortcut
            => UnityEditor.ShortcutManagement.ShortcutManager.instance.GetShortcutBinding(SHORTCUT_ID).ToString();
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            _buttonStyle = _skin.GetStyle("AlignmentToggle");
            _alignBtnDict[AxesUtils.Axis.X][0]
                = new GUIContent(Resources.Load<Texture2D>("Sprites/AlignmentLeft"), "Align Left");
            _alignBtnDict[AxesUtils.Axis.X][1]
                = new GUIContent(Resources.Load<Texture2D>("Sprites/AlignmentCenterX"), "Center on X");
            _alignBtnDict[AxesUtils.Axis.X][2]
                = new GUIContent(Resources.Load<Texture2D>("Sprites/AlignmentRight"), "Align Right");
            _alignBtnDict[AxesUtils.Axis.Y][0]
                = new GUIContent(Resources.Load<Texture2D>("Sprites/AlignmentBottom"), "Align Bottom");
            _alignBtnDict[AxesUtils.Axis.Y][1]
                = new GUIContent(Resources.Load<Texture2D>("Sprites/AlignmentCenterY"), "Center on Y");
            _alignBtnDict[AxesUtils.Axis.Y][2]
                = new GUIContent(Resources.Load<Texture2D>("Sprites/AlignmentTop"), "Align Top");
            _alignBtnDict[AxesUtils.Axis.Z][0]
                = new GUIContent(Resources.Load<Texture2D>("Sprites/AlignmentBack"), "Align Back");
            _alignBtnDict[AxesUtils.Axis.Z][1]
                = new GUIContent(Resources.Load<Texture2D>("Sprites/AlignmentCenterZ"), "Center on Z");
            _alignBtnDict[AxesUtils.Axis.Z][2]
                = new GUIContent(Resources.Load<Texture2D>("Sprites/AlignmentFront"), "Align Front");

#if UNITY_2019_1_OR_NEWER
            maxSize = minSize = new Vector2(536, 306);
#else
            maxSize = minSize = new Vector2(552, 306);
#endif
            titleContent = new GUIContent("Grid Arrangement");
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                using (new GUILayout.HorizontalScope())
                {
                    UnityEditor.EditorGUIUtility.labelWidth = 125;
                    UnityEditor.EditorGUIUtility.fieldWidth = 110;
                    _data.sortBy = (TransformTools.SortBy)UnityEditor.EditorGUILayout.Popup("Arrange according to:",
                        (int)_data.sortBy, _sortByOptions);
                    GUILayout.FlexibleSpace();
                    using (var check = new UnityEditor.EditorGUI.ChangeCheckScope())
                    {
                        UnityEditor.EditorGUIUtility.labelWidth = 85;
                        UnityEditor.EditorGUIUtility.fieldWidth = 120;
                        _data.alignProperty = UnityEditor.EditorGUILayout.Popup("Align property:",
                            _data.alignProperty == BoundsUtils.ObjectProperty.BOUNDING_BOX ? 0 : 1,
                            _alingObjPropOptions) == 0 ? BoundsUtils.ObjectProperty.BOUNDING_BOX
                            : BoundsUtils.ObjectProperty.PIVOT;
                        if (check.changed && _data.alignProperty == BoundsUtils.ObjectProperty.PIVOT)
                        {
                            _data.z.cellSizeType = _data.y.cellSizeType = _data.x.cellSizeType
                                = TransformTools.CellSizeType.CUSTOM;
                            _data.z.cellSize = _data.y.cellSize = _data.x.cellSize = 1f;
                        }
                    }
                }
            }
            UnityEditor.EditorGUIUtility.labelWidth = 55;
            UnityEditor.EditorGUIUtility.fieldWidth = 100;
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(4);
                AxisGUI(AxesUtils.Axis.X, _data.x);
                AxisGUI(AxesUtils.Axis.Y, _data.y);
                AxisGUI(AxesUtils.Axis.Z, _data.z);
            }
            GUILayout.Space(2);
            using (new GUILayout.HorizontalScope())
            {
                var statusStyle = new GUIStyle(UnityEditor.EditorStyles.label);
                GUILayout.Space(8);
                var statusMessage = "";
                if (SelectionManager.topLevelSelection.Length < 2)
                    statusMessage = "No objects selected.";
                else if (SelectionManager.topLevelSelection.Length > _data.x.cells * _data.y.cells * _data.z.cells)
                    statusMessage = SelectionManager.topLevelSelection.Length
                        + " objects selected. Selection don't fit. Add more rows or columns.";
                else statusMessage = SelectionManager.topLevelSelection.Length + " objects selected.";
                if (SelectionManager.topLevelSelection.Length < 2
                    || SelectionManager.topLevelSelection.Length > _data.x.cells * _data.y.cells * _data.z.cells)
                    GUILayout.Label(_warningIcon, new GUIStyle() { alignment = TextAnchor.LowerLeft });
                GUILayout.Label(statusMessage, statusStyle);
                GUILayout.FlexibleSpace();
                UnityEditor.EditorGUI.BeginDisabledGroup(
                    SelectionManager.topLevelSelection.Length > _data.x.cells * _data.y.cells * _data.z.cells
                    || SelectionManager.topLevelSelection.Length < 2);
                if (GUILayout.Button("Arrange", UnityEditor.EditorStyles.miniButtonRight)) Arrange();
                UnityEditor.EditorGUI.EndDisabledGroup();
            }
        }

        private void AxisGUI(AxesUtils.Axis axis, TransformTools.ArrangeAxisData axisData)
        {
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox, GUILayout.Width(170)))
            {
                UnityEditor.EditorGUI.BeginChangeCheck();
                axisData.overwrite = UnityEditor.EditorGUILayout.BeginToggleGroup(axis.ToString(), axisData.overwrite);
                if (UnityEditor.EditorGUI.EndChangeCheck()) _data.UpdatePriorities(axis);

                using (new UnityEditor.EditorGUI.DisabledGroupScope(_data.sortBy == TransformTools.SortBy.POSITION))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        UnityEditor.EditorGUI.BeginChangeCheck();
                        axisData.priority = UnityEditor.EditorGUILayout.Popup("Priority:", axisData.priority, _priorityOptions);
                        GUILayout.FlexibleSpace();
                        if (UnityEditor.EditorGUI.EndChangeCheck()) _data.UpdatePriorities(axis);
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        var dirOptions = axis == AxesUtils.Axis.X ? _directionOptionsX
                            : axis == AxesUtils.Axis.Y ? _directionOptionsY : _directionOptionsZ;
                        axisData.direction = UnityEditor.EditorGUILayout.Popup("Direction:", axisData.direction == 1 ? 0 : 1,
                            dirOptions) == 0 ? 1 : -1;
                        GUILayout.FlexibleSpace();
                    }
                }
                using (new GUILayout.HorizontalScope())
                {
                    axisData.cells = Stepper("Columns:", axisData.cells, 1,
                        SelectionManager.topLevelSelection.Length);
                }

                GUILayout.Label(axis == AxesUtils.Axis.Y ? "Height" : "Width:", UnityEditor.EditorStyles.label);
                using (new GUILayout.HorizontalScope())
                {
                    UnityEditor.EditorGUIUtility.fieldWidth = 100;
                    var sizeOptions = new System.Collections.Generic.List<string>(axis == AxesUtils.Axis.Y
                        ? _heightOptions : _widthOptions);
                    using (new UnityEditor.EditorGUI.DisabledGroupScope
                        (_data.alignProperty == BoundsUtils.ObjectProperty.PIVOT))
                        axisData.cellSizeType = (TransformTools.CellSizeType)
                            UnityEditor.EditorGUILayout.Popup((int)axisData.cellSizeType, sizeOptions.ToArray());
                    GUILayout.FlexibleSpace();
                }
                using (new GUILayout.HorizontalScope())
                {
                    using (new UnityEditor.EditorGUI.DisabledGroupScope(axisData.cellSizeType
                        != TransformTools.CellSizeType.CUSTOM))
                    {
                        using (new GUILayout.HorizontalScope(UnityEditor.EditorStyles.helpBox))
                        {
                            UnityEditor.EditorGUIUtility.labelWidth = 51;
                            UnityEditor.EditorGUIUtility.fieldWidth = 90;
                            axisData.cellSize
                                = UnityEditor.EditorGUILayout.FloatField("Value:", axisData.cellSize,
                                UnityEditor.EditorStyles.numberField);
                            GUILayout.FlexibleSpace();
                        }
                    }
                    GUILayout.FlexibleSpace();
                }

                UnityEditor.EditorGUIUtility.labelWidth = 55;
                UnityEditor.EditorGUIUtility.fieldWidth = 100;
                GUILayout.Label("Alignment:", UnityEditor.EditorStyles.label);
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Toggle(axisData.aligment == TransformTools.Bound.MIN,
                        _alignBtnDict[axis][0], _buttonStyle)) axisData.aligment = TransformTools.Bound.MIN;
                    if (GUILayout.Toggle(axisData.aligment == TransformTools.Bound.CENTER,
                        _alignBtnDict[axis][1], _buttonStyle)) axisData.aligment = TransformTools.Bound.CENTER;
                    if (GUILayout.Toggle(axisData.aligment == TransformTools.Bound.MAX,
                        _alignBtnDict[axis][2], _buttonStyle)) axisData.aligment = TransformTools.Bound.MAX;
                    GUILayout.FlexibleSpace();
                }



                axisData.autoSpacing = UnityEditor.EditorGUILayout.ToggleLeft("Auto-spacing:", axisData.autoSpacing);
                using (new UnityEditor.EditorGUI.DisabledGroupScope(axisData.autoSpacing))
                {
                    using (new GUILayout.HorizontalScope(UnityEditor.EditorStyles.helpBox))
                    {
                        UnityEditor.EditorGUIUtility.labelWidth = 53;
                        UnityEditor.EditorGUIUtility.fieldWidth = 90;
                        axisData.spacing = UnityEditor.EditorGUILayout.FloatField("Spacing:",
                            axisData.spacing, UnityEditor.EditorStyles.numberField);
                        GUILayout.FlexibleSpace();
                    }
                }


                GUILayout.Space(4);
                UnityEditor.EditorGUILayout.EndToggleGroup();
            }
        }
        private void Arrange() => TransformTools.Arrange(SelectionManager.topLevelSelection, _data);
        #endregion //WINDOW

        #region UTILS
        private int Stepper(string label, int value, int min, int max)
        {
            var retVal = value;
            using (new GUILayout.HorizontalScope())
            {
                UnityEditor.EditorGUIUtility.fieldWidth = 63;
                retVal = Mathf.Clamp(UnityEditor.EditorGUILayout.IntField(label, retVal,
                    UnityEditor.EditorStyles.numberField, GUILayout.Height(18)), min, max);
                GUILayout.Space(-4);
                if (GUILayout.Button("-", UnityEditor.EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    --retVal;
                    if (retVal < min) retVal = min;
                }
                GUILayout.Space(-4);
                if (GUILayout.Button("+", UnityEditor.EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    ++retVal;
                    if (retVal > max) retVal = max;
                }
                GUILayout.FlexibleSpace();
            }
            return retVal;
        }
        #endregion
    }
}
