//------------------------------------------------------------------------------
// CommonUIMgr.cs
// Created by CYM on 2021/9/13
// 填写类的描述...
//------------------------------------------------------------------------------

using CYM;
using CYM.UI;
namespace Gamelogic
{
    public class CommonUIMgr : BaseCommonUIMgr
    {
        #region prop
        public static ULoadingView LoadingView { get; private set; }
        public static SettingsView SettingsView { get; private set; }
        public static UModalBoxView ModalBoxView { get; private set; }
        #endregion

        #region life
        protected override void OnCreateUIView1()
        {
            base.OnCreateUIView1();
            LoadingView = CreateView<ULoadingView>("LoadingView");
            SettingsView = CreateView<SettingsView>("SettingsView");
            ModalBoxView = CreateView<UModalBoxView>("ModalBoxView");
        }
        #endregion
    }
}