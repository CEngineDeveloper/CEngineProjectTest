//------------------------------------------------------------------------------
// PluginPlatformSDK.cs
// Copyright 2023 2023/2/27 
// Created by CYM on 2023/2/27
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using CYM.Steam;
using System;

namespace CYM
{
    public partial class PlatformSDK  
    {
        public static Type Steam { get; private set; } = typeof(BaseSteamSDKMgr);
    }
}