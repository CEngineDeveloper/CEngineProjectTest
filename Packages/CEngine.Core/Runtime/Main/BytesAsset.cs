using Sirenix.OdinInspector;
using System.IO;
using UnityEngine;
namespace CYM
{
    [HideMonoScript]
    public class BytesAsset : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
        string _extension;
        [SerializeField]
        [HideInInspector]
        string _filePath;
        [SerializeField]
        [HideInInspector]
        byte[] _bytes;

        public string Extension => _extension;
        public string FilePath => _filePath;
        public byte[] Bytes => _bytes;

        public void SetBytes(byte[] bytes,string filePath)
        {
            _bytes = bytes;
            _filePath = filePath;
            _extension = Path.GetExtension(filePath);
        }
    }
}