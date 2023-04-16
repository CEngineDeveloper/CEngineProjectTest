//------------------------------------------------------------------------------
// GameConfig.cs
// Copyright 2018 2018/12/14 
// Created by CYM on 2018/12/14
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using CYM;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CYM
{
    [Serializable]
    public class GameDateTime
    {
        public GameDateTime(GameDateTimeType type,int year,int month,int day)
        {
            Type = type;
            Year = year;
            Month = month;
            Day = day;
        }
        public GameDateTimeType Type = GameDateTimeType.BC;
        [Range(1,9999)]
        public int Year = 770;
        [Range(1, 12)]
        public int Month =1;
        [Range(1, 31)]
        public int Day =1;
    }
    public partial class GameConfig : ScriptConfig<GameConfig> 
    {
        #region version
        [SerializeField, FoldoutGroup("Version")]
        public int Major;
        [SerializeField, FoldoutGroup("Version")]
        public int Minor;
        [SerializeField, FoldoutGroup("Version")]
        public int Data;
        [SerializeField, FoldoutGroup("Version")]
        public int Suffix = 1;
        [SerializeField, FoldoutGroup("Version")]
        public int Prefs = 0;
        [SerializeField, FoldoutGroup("Version")]
        public VersionTag Tag = VersionTag.Preview;
        #endregion

        #region input assets
        [SerializeField, FoldoutGroup("Input"), PropertyOrder(-100)]
        public int TouchDPI = 1;
        [SerializeField, FoldoutGroup("Input"), PropertyOrder(-100)]
        public int DragDPI = 800;
        [SerializeField, FoldoutGroup("Input"),PropertyOrder(-100)]
        public InputActionAsset InputActionAsset;
        #endregion

        #region reference
        [SerializeField, FoldoutGroup("Reference")]
        SerializableDic<string, Texture2D> RefTexture2D = new SerializableDic<string, Texture2D>();
        [SerializeField, FoldoutGroup("Reference")]
        SerializableDic<string, GameObject> RefGameObject = new SerializableDic<string, GameObject>();
        [SerializeField, FoldoutGroup("Reference")]
        SerializableDic<string, AnimationCurve> RefAnimationCurve = new SerializableDic<string, AnimationCurve>();
        #endregion

        #region DateTime
        [SerializeField, FoldoutGroup("DateTime")]
        public GameDateTime StartDateTime = new GameDateTime(GameDateTimeType.BC, 770, 1, 1);
        #endregion

        #region Music
        [SerializeField, FoldoutGroup("Music")]
        public string StartMusics  = "MainMenu";
        [SerializeField, FoldoutGroup("Music")]
        public string BattleMusics  = "Battle";
        [SerializeField, FoldoutGroup("Music")]
        public string CreditsMusics  = "Credits";
        #endregion

        #region DB
        [SerializeField, FoldoutGroup("DB")]
        public bool DBCompressed = false;
        [SerializeField, FoldoutGroup("DB")]
        public bool DBHash = false;
        [SerializeField, FoldoutGroup("DB")]
        public bool DBSaveAsyn = true;
        [SerializeField, FoldoutGroup("DB")]
        public bool DBLoadAsyn = true;
        #endregion

        #region Realtime
        [SerializeField, FoldoutGroup("Realtime")]
        public List<float> GameSpeed = new List<float>
        {
            1,2,3,4
        };
        #endregion

        #region Url
        [FoldoutGroup("URL")]
        public string URLWebsite = "https://store.steampowered.com/developer/Yiming";
        #endregion

        #region Ref Func
        public Texture2D GetTexture2D(string id)
        {
            if (RefTexture2D.ContainsKey(id)) return RefTexture2D[id];
            return null;
        }
        public GameObject GetGameObject(string id)
        {
            if (RefGameObject.ContainsKey(id)) return RefGameObject[id];
            return null;
        }
        public AnimationCurve GetAnimationCurve(string id)
        {
            if (RefAnimationCurve.ContainsKey(id)) return RefAnimationCurve[id];
            return null;
        }
        #endregion

        #region get
        public override string ToString()
        {
            string str = string.Format("v{0}.{1} {2}{3} {4}", Major, Minor, Tag, Suffix, PlatformSDK.GetDistributionName());
            return str;
        }
        #endregion
    }
}