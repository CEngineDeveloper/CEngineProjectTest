//------------------------------------------------------------------------------
// PluginUnit.cs
// Copyright 2023 2023/1/20 
// Created by CYM on 2023/1/20
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------
using CYM.Diplomacy;
namespace CYM
{
    public partial class BaseUnit : BaseCoreMono
    {
        static PluginUnit PluginDiplomacy = new PluginUnit
        {
            OnPostAddComponet = (u, x) => {
                if (x is IDipMgr)
                {
                    u.DipMgr = x as IDipMgr;
                }
                else if (x is BaseTerritoryMgr)
                {
                    u.TerritoryMgr = x as BaseTerritoryMgr;
                }
                else if (x is IAlertMgr<TDBaseAlertData>)
                {
                    u.AlertMgr = x as IAlertMgr<TDBaseAlertData>;
                }
                else if (x is IEventMgr<TDBaseEventData>)
                {
                    u.EventMgr = x as IEventMgr<TDBaseEventData>;
                }
            }
        };
        public IDipMgr DipMgr { get; protected set; }
        public BaseTerritoryMgr TerritoryMgr { get; protected set; }
        public IAlertMgr<TDBaseAlertData> AlertMgr { get; protected set; }
        public IEventMgr<TDBaseEventData> EventMgr { get; protected set; }
    }
}