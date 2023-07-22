//------------------------------------------------------------------------------
// SunObj.cs
// Copyright 2021 2021/1/10 
// Created by CYM on 2021/1/10
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using UnityEngine;
namespace CYM
{
    [AddComponentMenu(SysConst.STR_MenuSceneObj + nameof(SunObj))]
    public sealed class SunObj : SceneObj<Light, SunObj> 
    {
    }
}