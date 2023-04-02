using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace CYM
{
    public class AssembHub : MonoSingleton<AssembHub>
    {
        #region prop
        static Dictionary<string, Assembly> Assemblies = new Dictionary<string, Assembly>();
        static Dictionary<string, Assembly> AssemblyEditors = new Dictionary<string, Assembly>();
        #endregion

        #region set
        protected override void OnInit()
        {
            base.OnInit();
            var plugins = PluginConfig.Ins.Datas;
            foreach (var item in plugins)
            { 
            
            }
        }
        #endregion

    }
}
