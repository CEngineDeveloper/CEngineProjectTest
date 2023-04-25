using UnityEditor;
using Sirenix.OdinInspector.Editor;
namespace CYM
{
    [InitializeOnLoad]
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BaseMono), true)]
    public class BaseMonoEditor : OdinEditor
    {
        BaseMono preFabOverride;

        protected override void OnEnable()
        {
            base.OnEnable();
            preFabOverride = target as BaseMono;

        }

        public override void OnInspectorGUI()
        {

            base.OnInspectorGUI();
            if (preFabOverride == null)
                return;

        }
    }
}
