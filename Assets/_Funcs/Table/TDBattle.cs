//------------------------------------------------------------------------------
// TDBattle.cs
// Created by CYM on 2022/7/23
// 填写类的描述...
//------------------------------------------------------------------------------
using CYM;
using System;
namespace Gamelogic
{
    [Serializable]
    public class TDBattleData : TDBaseBattleData
    {

    }

    public class TDBattle: TDBaseGlobalConfig<TDBattleData,TDBattle>
    {
        public TDBattle()
        {
            AddFromObj(new TDBattleData { 
                TDID = "Battle",
                UseAssetBundle = false,
            });
        }
    }
}