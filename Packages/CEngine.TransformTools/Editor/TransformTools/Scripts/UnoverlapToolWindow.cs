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
using System.Linq;

namespace CYM.TransformTools
{

    public class UnoverlapToolWindow : BaseToolWindow
    {
        private TransformTools.UnoverlapData _data = new TransformTools.UnoverlapData();
        private System.Threading.Thread _unoverlapThread = null;
        private TransformTools.Unoverlapper _unoverlapper = null;
        private float _loadingProgress = 0f;
        private static bool _repaint = false;
        private (int objId, Vector3 offset)[] _offsets = null;
        private System.Collections.Generic.Dictionary<int, GameObject> _objDictionary
            = new System.Collections.Generic.Dictionary<int, GameObject>();
        private const int LARGEST_SELECTION_COUNT = 50;
#if UNITY_2020_1_OR_NEWER
        private int _progressId = -1;
#endif

        [UnityEditor.MenuItem("Tools/Transform Tools/Remove Overlaps", false, 1700)]
        public static void ShowWindow() => GetWindow<UnoverlapToolWindow>();

#if UNITY_2019_1_OR_NEWER
        public const string SHORTCUT_ID = "Transform Tools/Remove Overlaps";
        [UnityEditor.ShortcutManagement.Shortcut(SHORTCUT_ID)]
        public static void ShortcutAction() => GetWindow<UnoverlapToolWindow>().Apply();
        public static string shortcut => UnityEditor.ShortcutManagement.ShortcutManager.instance
            .GetShortcutBinding(SHORTCUT_ID).ToString();
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            minSize = new Vector2(280, 220);
            titleContent = new GUIContent("Remove Overlaps", null, "Remove Overlaps");
        }

        private GameObject[] selection
            => _data.topmostFilter ? SelectionManager.topLevelSelection : SelectionManager.selection;
        protected override void OnGUI()
        {
            base.OnGUI();

            if (_loadingProgress > 0f && _loadingProgress < 1f)
            {
#if UNITY_2020_1_OR_NEWER
                UnityEditor.Progress.Report(_progressId, _loadingProgress);
#else
                UnityEditor.EditorUtility.DisplayProgressBar("Removing Overlaps",
                    ((int)(_loadingProgress * 100)).ToString() + " %", _loadingProgress);
#endif
            }

            UnityEditor.EditorGUIUtility.labelWidth = 50;
            UnityEditor.EditorGUIUtility.fieldWidth = 70;
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                _data.topmostFilter = UnityEditor.EditorGUILayout.ToggleLeft("Topmost filter", _data.topmostFilter);
            }
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox)) //X
            {
                using (var toggleGroup = new UnityEditor.EditorGUILayout.ToggleGroupScope("Remove overlaps on X",
                    _data.x.unoverlap))
                {
                    _data.x.unoverlap = toggleGroup.enabled;
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        _data.x.minDistance = UnityEditor.EditorGUILayout.FloatField("Spacing:",
                            _data.x.minDistance, UnityEditor.EditorStyles.textField);
                    }
                }
            }
            GUILayout.Space(8);
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox)) //Y
            {
                using (var toggleGroup = new UnityEditor.EditorGUILayout.ToggleGroupScope("Remove overlaps on Y",
                    _data.y.unoverlap))
                {
                    _data.y.unoverlap = toggleGroup.enabled;
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        _data.y.minDistance = UnityEditor.EditorGUILayout.FloatField("Spacing:",
                            _data.y.minDistance, UnityEditor.EditorStyles.textField);
                    }
                }
            }
            GUILayout.Space(8);
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox)) //Z
            {
                using (var toggleGroup = new UnityEditor.EditorGUILayout.ToggleGroupScope("Remove overlaps on Z",
                    _data.z.unoverlap))
                {
                    _data.z.unoverlap = toggleGroup.enabled;
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        _data.z.minDistance = UnityEditor.EditorGUILayout.FloatField("Spacing:",
                            _data.z.minDistance, UnityEditor.EditorStyles.textField);
                    }
                }
            }
            GUILayout.Space(2);
            using (new GUILayout.HorizontalScope())
            {
                var statusStyle = new GUIStyle(UnityEditor.EditorStyles.label);
                GUILayout.Space(8);
                var statusMessage = "";
                if (selection.Length == 0
                || selection.Length > LARGEST_SELECTION_COUNT)
                {
                    statusMessage = selection.Length == 0
                    ? "No objects selected."
                    : selection.Length
                    + " objects. (max = " + LARGEST_SELECTION_COUNT + ")";
                    GUILayout.Label(new GUIContent(Resources.Load<Texture2D>("Sprites/Warning")),
                        new GUIStyle() { alignment = TextAnchor.LowerLeft });
                }
                else
                {
                    statusMessage = selection.Length + " objects selected.";
                }
                GUILayout.Label(statusMessage, statusStyle);
                GUILayout.FlexibleSpace();
                using (new UnityEditor.EditorGUI.DisabledScope(selection.Length == 0
                    || selection.Length > LARGEST_SELECTION_COUNT))
                {
                    if (GUILayout.Button(new GUIContent("Remove Overlaps", string.Empty
#if UNITY_2019_1_OR_NEWER
                            + shortcut
#endif
                            ), UnityEditor.EditorStyles.miniButtonRight)) Apply();
                }
            }

        }

        private void Apply()
        {
            _objDictionary = selection.ToDictionary(obj => obj.GetInstanceID());
            var bounds = selection.Select(obj => (obj.GetInstanceID(),
            BoundsUtils.GetBounds(obj.transform))).ToArray();
            _unoverlapper = new TransformTools.Unoverlapper(bounds, _data);
            _unoverlapper.progressChanged += OnProgress;
            _unoverlapper.OnDone += OnDone;
            var threadDelegate = new System.Threading.ThreadStart(_unoverlapper.RemoveOverlaps);
            _unoverlapThread = new System.Threading.Thread(threadDelegate);
            _unoverlapThread.Name = "Unoverlap";
            _unoverlapThread.Start();
            _loadingProgress = 0f;
            _offsets = null;
#if UNITY_2020_1_OR_NEWER
                        _progressId = UnityEditor.Progress.Start("Removing Overlaps");
#endif
        }
        private void OnProgress(float progress)
        {
            _loadingProgress = progress;
            _repaint = true;
        }

        private void OnDone((int objId, Vector3 offset)[] positions)
        {
            _unoverlapper.progressChanged -= OnProgress;
            _unoverlapper.OnDone -= OnDone;
            _unoverlapper = null;
            _loadingProgress = 1f;
            _offsets = positions;
        }

        private void Update()
        {
            if (_repaint)
            {
                Repaint();
                _repaint = false;
            }
            if (_offsets != null && _offsets.Length > 0)
            {
#if UNITY_2020_1_OR_NEWER
                _progressId = UnityEditor.Progress.Remove(_progressId);
#else
                UnityEditor.EditorUtility.ClearProgressBar();
#endif
                const string cmdName = "Remove Overlap";
                var parents = new Transform[_offsets.Length];
                if (!_data.topmostFilter)
                {
                    for (int i = 0; i < _offsets.Length; ++i)
                    {
                        var offsetObj = _offsets[i];
                        var transform = _objDictionary[_offsets[i].objId].transform;
                        UnityEditor.Undo.RecordObject(transform, cmdName);
                        parents[i] = transform.parent;
                        transform.SetParent(null);
                    }
                }
                for (int i = 0; i < _offsets.Length; ++i)
                {
                    var offsetObj = _offsets[i];
                    var transform = _objDictionary[_offsets[i].objId].transform;
                    if (_data.topmostFilter) UnityEditor.Undo.RecordObject(transform, cmdName);
                    transform.position += offsetObj.offset;
                }
                if (!_data.topmostFilter)
                {
                    for (int i = 0; i < _offsets.Length; ++i)
                    {
                        var offsetObj = _offsets[i];
                        var transform = _objDictionary[_offsets[i].objId].transform;
                        transform.SetParent(parents[i]);
                    }
                }
                _offsets = null;
            }
        }

        private void OnDestroy()
        {
#if UNITY_2020_1_OR_NEWER
            if (_progressId > 0)
            {
                _progressId = UnityEditor.Progress.Remove(_progressId);
            }
#else
            UnityEditor.EditorUtility.ClearProgressBar();
#endif
            if (_unoverlapper != null)
            {
                _unoverlapper.progressChanged -= OnProgress;
                _unoverlapper.OnDone -= OnDone;
                _unoverlapper.Cancel();
                _unoverlapper = null;
                _unoverlapThread.Abort();
                _repaint = false;
            }
        }
    }
}
