//------------------------------------------------------------------------------
// PluginGlobal.cs
// Copyright 2022 2022/9/26 
// Created by CYM on 2022/9/26
// Owner: CYM
// ÌîÐ´ÀàµÄÃèÊö...
//------------------------------------------------------------------------------
using CYM.Perform;

namespace CYM
{
    public partial class BaseGlobal : BaseCoreMono
    {
        static PluginGlobal PluginPerform = new PluginGlobal
        {
            OnInstall = (g) =>
            {
                PerformMgr = g.AddComponent<BasePerformMgr>();
            }
        };
        public static BasePerformMgr PerformMgr { get; protected set; }
    }
}