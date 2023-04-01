//------------------------------------------------------------------------------
// SystemRes.cs
// Copyright 2023 2023/3/11 
// Created by CYM on 2023/3/11
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using UnityEngine;
using CYM.DLC;
using System.Linq;
using UnityEditor;
using System.IO;

namespace CYM
{
    public class SystemRes
    {
        public static T LoadConfig<T>(string fileName) where T : ScriptableObject
        {
            return Load<T>(fileName + ".asset");
        }
        public static GameObject LoadPrefab(string fileName)
        {
            return Load<GameObject>(fileName + ".prefab");
        }
        static T Load<T>(string fileName) where T : Object
        {
            string rawFileName = fileName.Split(".").FirstOrDefault();
            T _ins = Resources.Load<T>(SysConst.Dir_System + "/" + rawFileName);
            if (_ins == null)
            {
                _ins = Resources.Load<T>(SysConst.Dir_Temp + "/" + rawFileName);
            }
            if (_ins == null)
            {
                _ins = Resources.Load<T>(rawFileName);
            }
            //从Bundle加载
            if (_ins == null)
            {
                _ins = LoadFromBundle<T>(fileName);
            }
            if (_ins == null)
            {
                CLog.Error($"System:{fileName}没有！！！");
            }
            return _ins;
        }
        public static GameObject Spawn(string fileName)
        {
            GameObject go = LoadPrefab(fileName);
            if (go != null)
            {
                var ret = GameObject.Instantiate(go);
                ret.transform.SetParent(BaseGlobal.Ins.Trans);
                return ret;
            }
            return null;
        }

        static T LoadFromBundle<T>(string fileName) where T : Object
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                var path = string.Format($"Assets/{SysConst.Dir_Bundles}/{SysConst.Dir_System}/{fileName}");
                var path2 = string.Format($"{SysConst.Dir_Bundles}/{SysConst.Dir_System}/{fileName}");
                var fullPath = Path.Combine(Application.dataPath, path2);
                if (File.Exists(fullPath))
                {
                    return AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
                }
#endif
            }
            else
            {
                string endFileName = fileName.Split("/").Last();
                return DLCManager.LoadRawAsset<T>(SysConst.STR_NativeDLC, SysConst.BN_System, endFileName);
            }
            return default;
        }
    }
}