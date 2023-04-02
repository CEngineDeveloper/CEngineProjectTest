//------------------------------------------------------------------------------
// ChannelManager.cs
// Copyright 2023 2023/2/27 
// Created by CYM on 2023/2/27
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace CYM
{
    public partial class PlatformSDK
    {
        #region Distrubute
        public static Type Examine { get; private set; } = typeof(BaseExamineSDKMgr);
        public static Type Usual { get; private set; } = typeof(BaseUsualSDKMgr);
        public static Type Trial { get; private set; } = typeof(BaseTrialSDKMgr);
        #endregion

        #region 渠道
        public static string[] DistributionOptions { get; private set; }
        static Dictionary<string, Type> Distribution { get; set; } = new Dictionary<string, Type>();
        public static string GetDistributionName()
        {
            if (DistributionOptions == null)
                return "";
            var key = LocalConfig.Ins.Distribution;
            if (key >= DistributionOptions.Length)
                return "";
            return DistributionOptions[key];
        }
        public static Type GetDistributionType()
        {
            if (DistributionOptions == null)
                return null;
            var key = LocalConfig.Ins.Distribution;
            if (key >= DistributionOptions.Length)
                return null;
            return Distribution[DistributionOptions[key]];
        }
        public static void RefreshDistribution()
        {
            Distribution.Clear();
            List<string> tempList = new List<string>();
            var data = typeof(PlatformSDK).GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var item in data)
            {
                if (item == null)
                    continue;
                var val = item.GetValue(item) as Type;
                if (val != null && val.IsSubclassOf(typeof(BasePlatSDKMgr)))
                {
                    tempList.Add(item.Name);
                    Distribution.Add(item.Name, val);
                }
            }
            DistributionOptions = tempList.ToArray();
        }
        #endregion
    }
}