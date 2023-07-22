using UnityEditor.AssetImporters;
using UnityEngine;

namespace CYM
{

    [ScriptedImporter(1, ".xlsx")]
    public class XlsxImporter : ScriptedImporter
    {
        public sealed override void OnImportAsset(AssetImportContext ctx)
        {
            var bytes = System.IO.File.ReadAllBytes(ctx.assetPath);
            BytesAsset textAsset = ScriptableObject.CreateInstance<BytesAsset>();
            textAsset.SetBytes(bytes, ctx.assetPath);
            ctx.AddObjectToAsset("main obj", textAsset);
            ctx.SetMainObject(textAsset);
        }
    }
}