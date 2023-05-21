//------------------------------------------------------------------------------
// BuildLocalConfig.cs
// Copyright 2018 2018/5/3 
// Created by CYM on 2018/5/3
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------
using UnityEngine;
using CYM.Ver;

namespace CYM
{
    public partial class VersionConfig : ScriptConfig<VersionConfig>
    {
        public override bool IsHideInBuildWindow => false;

        public Version Version = new Version { major = 1, minor = 2, patch = 3 };

        [SerializeField]
        public int Data;

        [SerializeField]
        public int Prefs = 0;
        [SerializeField]
        public VersionTag Tag = VersionTag.Preview;

        #region get
        public override string ToString()
        {
            string str = string.Format("{0} {1} {2}", Version.ToString(),Tag, PlatformSDK.GetDistributionName());
            return str;
        }
        #endregion
    }
}