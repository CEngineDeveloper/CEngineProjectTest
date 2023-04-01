//------------------------------------------------------------------------------
// TDChara.cs
// Created by CYM on 2022/7/24
// 填写类的描述...
//------------------------------------------------------------------------------
using CYM;
using System;
using System.Linq;

namespace Gamelogic
{
    [Serializable]
    public class TDCharaData : TDBaseData
    {
        public string LihuiIcon { get; set; } = "";
        public string NameIcon { get; set; } = "";
        public string ChenghaoIcon { get; set; } = "";
        public string Quality { get; set; } = "";
        public string Frag { get; set; } = "";
        public string FragCount { get; set; } = "";
        public string Synth { get; set; } = "";
        public string Grade { get; set; } = "";
        public string GradeFrag { get; set; } = "";
        public string CustomLevel { get; set; } = "";
    }

    public class TDChara : TDBaseGlobalConfig<TDCharaData, TDChara>
    {
        public override void OnAllLoadEnd2()
        {
            base.OnAllLoadEnd2();
            CLog.Cyan("读取角色:"+Keys.Count);
        }
    }
}