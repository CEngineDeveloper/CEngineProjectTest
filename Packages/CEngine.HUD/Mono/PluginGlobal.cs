using CYM.HUD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CYM
{
    public partial class BaseGlobal : BaseCoreMono
    {
        static PluginGlobal PluginHUD = new PluginGlobal
        {
            OnInstall = (g) => {
                HUDUIMgr = g.AddComponent<BaseHUDUIMgr>();
            }
        };

        #region Componet
        public static BaseHUDUIMgr HUDUIMgr { get; protected set; }
        #endregion
    }

}