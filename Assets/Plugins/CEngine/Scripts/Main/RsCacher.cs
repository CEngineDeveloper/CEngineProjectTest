//------------------------------------------------------------------------------
// RsCacher.cs
// Copyright 2023 2023/3/11 
// Created by CYM on 2023/3/11
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using UnityEngine;
using CYM.DLC;
using System.Collections.Generic;

namespace CYM
{
    public class RsCacher<T> : IRsCacher, IRsCacherT<T>
        where T : Object
    {
        public string Bundle { get; private set; }
        public ObjectRegister<T> Data { get; private set; } = new ObjectRegister<T>();
        public RsCacher(string bundleName)
        {
            Bundle = bundleName.ToLower();
            BaseGRMgr.BundleCachers.Add(bundleName, this);
        }

        public bool IsHave(string name)
        {
            if (DLCManager.IsHave(Bundle, name))
                return true;
            return false;
        }

        public T Get(string name, bool isLogError = true)
        {
            return GetResWithCache(Bundle, name, Data, isLogError);
        }
        public TObj GetT<TObj>(string name, bool isLogError = true) where TObj : class => Get(name, isLogError) as TObj;
        public T Spawn(string name)
        {
            return GameObject.Instantiate<T>(Get(name));
        }
        public TObj SpawnT<TObj>(string name) where TObj : class => Spawn(name) as TObj;

        public List<T> Get(List<string> names)
        {
            List<T> clips = new List<T>();
            if (names == null)
                return clips;
            for (int i = 0; i < names.Count; ++i)
            {
                if (names[i].IsInv())
                    continue;
                var temp = Get(names[i]);
                if (temp != null)
                    clips.Add(temp);
            }
            return clips;
        }
        public List<T> Get()
        {
            List<T> clips = new List<T>();
            foreach (var item in DLCManager.GetAssetsByBundle(Bundle))
            {
                var temp = Get(item);
                if (temp != null)
                    clips.Add(temp);
            }
            return clips;
        }
        public List<string> GetStrsByCategory(string category)
        {
            return DLCManager.GetAssetsByCategory(Bundle, category);
        }
        public List<string> GetStrs()
        {
            return DLCManager.GetAssetsByBundle(Bundle);
        }
        public List<T> GetByCategory(string category)
        {
            List<T> clips = new List<T>();
            foreach (var item in DLCManager.GetAssetsByCategory(Bundle, category))
            {
                var temp = Get(item);
                if (temp != null)
                    clips.Add(temp);
            }
            return clips;
        }
        private T GetResWithCache(string rulekey, string name, ObjectRegister<T> cache, bool logError = true)
        {
            //没有运行的时候无效，直接返回Null
            if (!Application.isPlaying)
                return null;
            DLCManager.IsNextLogError = logError;
            string bundleName = DLCManager.GetSourceBundleName(rulekey + name);
            if (name.IsInv())
                return null;
            //没有启动完毕的时候直接返回Null
            if (!BaseGlobal.IsAllLoadEnd)
            {
                return null;
            }
            if (cache.ContainsKey(name))
            {
                return cache[name];
            }
            else
            {
                var asset = DLCManager.LoadAsset<T>(bundleName, name);
                if (asset == null)
                    return null;
                T retGO = asset.Object as T;
                if (retGO == null)
                {
                    if (logError)
                        CLog.Error("no this res in bundle {0}, name = {1}", bundleName, name);
                }
                else
                {
                    if (!cache.ContainsKey(retGO.name)) cache.Add(retGO);
                    else cache[retGO.name] = retGO;
                }
                return retGO;
            }
        }

        public void RemoveNull()
        {
            Data.RemoveNull();
        }
    }
}