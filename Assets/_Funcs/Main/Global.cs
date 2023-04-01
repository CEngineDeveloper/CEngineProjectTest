//------------------------------------------------------------------------------
// Global.cs
// Created by CYM on 2022/7/22
// 填写类的描述...
//------------------------------------------------------------------------------
using CYM;
namespace Gamelogic
{
    public class Global : BaseGlobalT<UnitCrew,Config, DBSettings,Global>
    {
        #region Config
        public static TDBattle TDBattle { get; private set; } = new TDBattle();
        public static TDChara TDChara { get; private set; } = new TDChara();
        public static TDCrew TDCrew { get; private set; } = new TDCrew();
        public static TDDrama TDDrama { get; private set; } = new TDDrama();
        public static TDPlanet TDPlanet { get; private set; } = new TDPlanet();
        public static TDShip TDShip { get; private set; } = new TDShip();
        #endregion

        #region Mgr
        public static new SettingsMgr SettingsMgr { get; private set; }
        public static new DBMgr DBMgr { get; private set; }
        public static new BattleMgr BattleMgr { get; private set; }
        public static new CursorMgr CursorMgr { get; private set; }
        public static new ScreenMgr ScreenMgr { get; private set; }
        #endregion

        #region UI Mgr
        public static new CommonUIMgr CommonUIMgr { get; private set; }
        public static new MainUIMgr MainUIMgr { get; private set; }
        public static new HUDUIMgr HUDUIMgr { get; private set; }
        public static new BattleUIMgr BattleUIMgr { get; private set; }
        #endregion

        #region Unit Mgr
        public static PlanetMgr PlanetMgr { get; private set; }
        public static CharaMgr CharaMgr { get; private set; }
        public static CrewMgr CrewMgr { get; private set; }
        #endregion

        #region Life
        protected override void OnReplaceInnerComponet()
        {
            base.OnReplaceInnerComponet();
        }
        protected override void OnAttachComponet()
        {
            base.OnAttachComponet();

            #region mgr
            SettingsMgr = AddComponent<SettingsMgr>();
            DBMgr = AddComponent<DBMgr>();
            BattleMgr = AddComponent<BattleMgr>();
            CursorMgr = AddComponent<CursorMgr>();
            ScreenMgr = AddComponent<ScreenMgr>();
            #endregion

            #region ui mgr
            HUDUIMgr = AddComponent<HUDUIMgr>();
            MainUIMgr = AddComponent<MainUIMgr>();
            BattleUIMgr = AddComponent<BattleUIMgr>();
            CommonUIMgr = AddComponent<CommonUIMgr>();
            #endregion

            #region unit mgr
            PlanetMgr = AddComponent<PlanetMgr>();
            CharaMgr = AddComponent<CharaMgr>();
            CrewMgr = AddComponent<CrewMgr>();
            #endregion
        }
        #endregion

        #region Other
        protected override void OnRegisterLuaAssembly()
        {
            base.OnRegisterLuaAssembly();
        }
        protected override void OnAllLoadEnd2()
        {
            base.OnAllLoadEnd2();
            CLog.Green("这是一次热更新2222eeee2");
        }
        #endregion
    }
}