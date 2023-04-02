//------------------------------------------------------------------------------
// GlobalTutorial.cs
// Copyright 2022 2022/11/5 
// Created by CYM on 2022/11/5
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using CYM.Example;
namespace CYM
{
    public class GlobalExample : BaseGlobalT<BaseUnit, SysConfig,DBBaseSettings, GlobalExample> 
    {
        public override bool IsLoadLua => false;
        protected override void OnAttachComponet()
        {
            base.OnAttachComponet();
            AddComponent<ExampleUIMgr>();
        }
    }
}