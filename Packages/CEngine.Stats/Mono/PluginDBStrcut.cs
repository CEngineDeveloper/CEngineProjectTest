//------------------------------------------------------------------------------
// DBBaseStrcut.cs
// Copyright 2022 2022/11/6 
// Created by CYM on 2022/11/6
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using UnityEngine;
using CYM;
using CYM.UI;
using System;

namespace CYM.Stats
{
    [Serializable]
    public class DBBaseBuff : DBBase
    {
        public float CD = 0;
        public float Input;
        public bool Valid = true;
    }
}