//------------------------------------------------------------------------------
// MainUIMgr.cs
// Created by CYM on 2021/9/5
// 填写类的描述...
//------------------------------------------------------------------------------

using CYM;
using CYM.UI;
namespace Gamelogic
{
    public class MainUIMgr : BaseMainUIMgr
    {
        public static MainMenuView MainMenuView { get; private set; }
        public static UUIView TestLuaView { get; private set; }

        protected override void OnCreateUIView1()
        {
            base.OnCreateUIView1();
            MainMenuView = CreateView<MainMenuView>("MainMenuView");
            TestLuaView = CreateView<UUIView>("TestLuaView");
        }
    }
}