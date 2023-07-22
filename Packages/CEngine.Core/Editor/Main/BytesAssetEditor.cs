using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace CYM
{
    [CustomEditor(typeof(BytesAsset))]
    public class BytesAssetEditor : Editor
    {
        [SerializeField] Texture2D imageIcon;
        public BytesAsset Target
        {
            get
            {
                return this.target as BytesAsset;
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
        {
            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
        }

    }
}