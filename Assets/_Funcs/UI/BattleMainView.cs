//------------------------------------------------------------------------------
// BattleMainView.cs
// Created by CYM on 2022/7/24
// 填写类的描述...
//------------------------------------------------------------------------------
using UnityEngine;
using CYM;
using CYM.UI;
using UnityEngine.EventSystems;
namespace Gamelogic
{
    public class BattleMainView : MainUIView
    {
        #region Inspector
        [SerializeField]
        UDupplicate DPTimeOption;
        [SerializeField]
        UDupplicate DPMenu;
        [SerializeField]
        UDupplicate DPAttr;
        [SerializeField]
        UDupplicate DPMapOption;
        [SerializeField]
        UText DateTime;
        [SerializeField]
        UText CrewName;
        #endregion

        #region life
        protected override void OnCreatedView()
        {
            base.OnCreatedView();
            
            DPTimeOption.Init(new UDupplicateData { OnClickSelected  = OnSelectSpeed },new UCheckData { IsShow = (x)=>true });
            //DPMenu.Init(
            //    new UButtonData { IconStr = CIcon.Menu1 },
            //    new UButtonData { IconStr = CIcon.Menu2 },
            //    new UButtonData { IconStr = CIcon.Menu3 },
            //    new UButtonData { IconStr = CIcon.Menu4 },
            //    new UButtonData { IconStr = CIcon.Menu5 },
            //    new UButtonData { IconStr = CIcon.Menu6 },
            //    new UButtonData { IconStr = CIcon.Menu7 });
            //DPAttr.Init(
            //    new UTextData { IconStr = CIcon.黄金, Name = Get黄金 },
            //    new UTextData { IconStr = CIcon.食物, Name = Get食物 },
            //    new UTextData { IconStr = CIcon.材料, Name = Get材料 },
            //    new UTextData { IconStr = CIcon.医药, Name = Get医药 },
            //    new UTextData { IconStr = CIcon.威望, Name = Get威望 }
            //    );
            //DPMapOption.Init(
            //    new UButtonData { IconStr = CIcon.MinimapOption1,OnClick = OnClickSettings },
            //    new UButtonData { IconStr = CIcon.MinimapOption2},
            //    new UButtonData { IconStr = CIcon.MinimapOption3},
            //    new UButtonData { IconStr = CIcon.MinimapOption4}
            //    );
            DateTime.Init(new UTextData { Name = GetDateTimeStr });
            CrewName.Init(new UTextData { Name = GetCrewNameStr });
        }
        #endregion

        #region Callback
        private void OnClickSettings(UControl arg1, PointerEventData arg2)
        {
            CommonUIMgr.SettingsView.Toggle();
        }
        private void OnSelectSpeed(UControl arg1)
        {
            if (arg1.Index == 0)
            {

            }
            else if (arg1.Index == 1)
            {

            }
            else if (arg1.Index == 2)
            { 
            
            }
        }
        private string GetCrewNameStr()
        {
            return "帝国小队";
        }
        private string GetDateTimeStr()
        {
            return Global.DateTimeMgr.GetCurYearMonth() ;
        }
        private string Get威望()
        {
            return "99";
        }

        private string Get医药()
        {
            return "123";
        }

        private string Get能量()
        {
            return "2K";
        }

        private string Get食物()
        {
            return "221";
        }
        private string Get材料()
        {
            return "342";
        }

        private string Get黄金()
        {
            return "411";
        }
        #endregion
    }
}