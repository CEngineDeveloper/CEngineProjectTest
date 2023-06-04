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
    public class SimulateGravityWindow : BaseToolWindow
    {
        private SimulateGravityData _data = new SimulateGravityData();
        

        [UnityEditor.MenuItem("Tools/Transform Tools/Simulate Gravity", false, 1600)]
        public static void ShowWindow() => GetWindow<SimulateGravityWindow>();

#if UNITY_2019_1_OR_NEWER
        public const string SHORTCUT_ID = "Transform Tools/Simulate Gravity";
        [UnityEditor.ShortcutManagement.Shortcut(SHORTCUT_ID)]
        public static void ShortcutAction() => GetWindow<SimulateGravityWindow>().Apply();
        public static string shortcut
            => UnityEditor.ShortcutManagement.ShortcutManager.instance.GetShortcutBinding(SHORTCUT_ID).ToString();
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            minSize = new Vector2(190, 235);
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            
            titleContent = new GUIContent("Simulate Gravity", null, "Simulate Gravity");

            UnityEditor.EditorGUIUtility.labelWidth = 120;
            UnityEditor.EditorGUIUtility.fieldWidth = 100;
            using (new GUILayout.VerticalScope())
            {
                using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
                {
                    _data.gravity = UnityEditor.EditorGUILayout.Vector3Field("Gravity:", _data.gravity);
                    _data.maxIterations = UnityEditor.EditorGUILayout.IntField("Max Iterations:", _data.maxIterations);
                    _data.maxSpeed = UnityEditor.EditorGUILayout.FloatField("Max Speed:", _data.maxSpeed);
                    _data.maxAngularSpeed = UnityEditor.EditorGUILayout.FloatField("Max Angular Speed:",
                        _data.maxAngularSpeed);
                    _data.mass = UnityEditor.EditorGUILayout.FloatField("Mass:", _data.mass);
                    _data.drag = UnityEditor.EditorGUILayout.FloatField("Drag:", _data.drag);
                    _data.angularDrag = UnityEditor.EditorGUILayout.FloatField("Angular Drag:", _data.angularDrag);
                    _data.ignoreSceneColliders = UnityEditor.EditorGUILayout.ToggleLeft("Ignore Scene Colliders",
                       _data.ignoreSceneColliders);
                    using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
                    {
                        _data.changeLayer = UnityEditor.EditorGUILayout.ToggleLeft("Change Layer Temporarily",
                            _data.changeLayer);
                        if (_data.changeLayer)
                            _data.tempLayer = UnityEditor.EditorGUILayout.LayerField("Temp layer:", _data.tempLayer);
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
                        if (GUILayout.Button(new GUIContent("Apply"), UnityEditor.EditorStyles.miniButtonRight)) Apply();
                    }
                }
            }
        }

        private void Apply() => GravityUtils.SimulateGravity(SelectionManager.topLevelSelection, _data, true);
    }
}
