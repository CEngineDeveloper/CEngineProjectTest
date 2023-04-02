//------------------------------------------------------------------------------
// DBStruct.cs
// Created by CYM on 2021/6/6
// 填写类的描述...
//------------------------------------------------------------------------------
using CYM;
//using CYM.Unit;
using System;
using System.Collections.Generic;
namespace Gamelogic
{
    #region 游戏设置数据
    [Serializable]
    public class DBSettings : DBBaseSettings
    {

    }
    [Serializable]
    public class DBGameDiff : DBBaseGameDiff
    {

    }
    [Serializable]
    public class DBGame : DBBaseGame
    {

    }
    #endregion

    #region
    [Serializable]
    public class DBCrew : DBBaseUnit
    {
        //属性
        public Dictionary<CrewAttr, float> Attrs = new Dictionary<CrewAttr, float>();
        //Buff
        //public List<DBBaseBuff> Buffs = new List<DBBaseBuff>();
    }
    [Serializable]
    public class DBChara : DBBaseUnit
    { 
    
    }
    [Serializable]
    public class DBPlanet : DBBaseUnit
    {

    }
    [Serializable]
    public class DBShip : DBBaseUnit
    {

    }
    [Serializable]
    public class DBNation : DBBaseUnit
    {

    }
    #endregion
}