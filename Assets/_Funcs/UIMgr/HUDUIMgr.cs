//------------------------------------------------------------------------------
// HUDUIMgr.cs
// Created by CYM on 2022/3/31
// 填写类的描述...
//------------------------------------------------------------------------------

using CYM;
using CYM.UI;
namespace Gamelogic
{
    public class HUDUIMgr : BaseHUDUIMgr
    {
        #region prop
        public static UHUDView CastleHUDView { get; protected set; }
        public static UHUDView LegionHUDView { get; protected set; }
        #endregion

        #region life
        protected override void OnCreateUIView1()
        {
            base.OnCreateUIView1();
            LegionHUDView = CreateHUDView("Legion");
            CastleHUDView = CreateHUDView("Castle");
        }
        #endregion
    }
}