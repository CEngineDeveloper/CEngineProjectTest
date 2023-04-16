//------------------------------------------------------------------------------
// PluginGlobal.cs
// Copyright 2022 2022/9/26 
// Created by CYM on 2022/9/26
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------
using CYM.Stats;

namespace CYM
{
    public partial class BaseGlobal : BaseCoreMono
    {
        static PluginGlobal PluginUnit = new PluginGlobal
        {
        };

        public static IAttrMgr AttrMgr { get; protected set; }
        public static IBuffMgr BuffMgr { get; protected set; }
    }
}