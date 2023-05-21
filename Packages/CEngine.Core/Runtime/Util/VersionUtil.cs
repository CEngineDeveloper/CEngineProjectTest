//**********************************************
// Class Name	: GlobalComponet
// Discription	：None
// Author	：CYM
// Team		：MoBaGame
// Date		：#DATE#
// Copyright ©1995 [CYMCmmon] Powered By [CYM] Version 1.0.0 
//**********************************************

using System;
using System.IO;
using UnityEngine;

namespace CYM
{
    public class VersionUtil
    {
        #region Config
        static BuildConfig BuildConfig => BuildConfig.Ins;
        static VersionConfig VersionConfig => VersionConfig.Ins;
        static LocalConfig LocalConfig => LocalConfig.Ins;
        #endregion

        public static string GameName => BuildConfig.Name;
        public static string BuildTime => LocalConfig.LastBuildTime;
        public static string GameVersion => VersionConfig.ToString();

        public static bool IsTrialPlat => PlatformSDK.GetDistributionName() == nameof(PlatformSDK.Trial);
        public static int DataVersion => VersionConfig.Data;
        public static bool IsDevelop => LocalConfig.BuildType == BuildType.Develop;
        public static bool IsPublic => LocalConfig.BuildType == BuildType.Public;

        public static bool IsMustIL2CPP
        {
            get
            {
                if (BuildConfig.IsHotFix)
                    return true;
                if (LocalConfig.Platform == Platform.IOS)
                    return true;
                return false;
            }
        }
        public static bool IsMustResBundle
        {
            get
            {
                if (LocalConfig.Platform == Platform.IOS ||
                    LocalConfig.Platform == Platform.Android)
                    return true;
                return false;
            }
        }

        public static bool IsWindows =>
            Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.WindowsEditor;
        //编辑器模式或者纯配置模式
        public static bool IsEditorOrConfigMode => IsEditorMode || RealResBuildType == ResBuildType.Config;
        //编辑器模式或者AB配置模式
        public static bool IsEditorOrAssetBundleMode => IsEditorMode || RealResBuildType == ResBuildType.Bundle;
        //是否为编辑器模式
        public static bool IsEditorMode
        {
            get
            {
                if (!Application.isEditor) return false;
                if (Application.isEditor && LocalConfig.Ins.IsSimulationEditor) return true;
                return false;
            }
        }
        public static bool IsLogoEditorMode()
        {
            if (Application.isEditor)
                return BuildConfig.IsShowLogo;
            return true;
        }

        public static ResBuildType RealResBuildType
        {
            get
            {
                if (IsMustResBundle)
                    return ResBuildType.Bundle;
                return BuildConfig.ResBuildType;
            }
        }
        public static string FullVersion => string.Format("{0} {1} {2}", BuildConfig.Name, VersionConfig.ToString(), LocalConfig.Platform);
        public static string DirPath
        {
            get
            {
                if (IsPublic) return Path.Combine(SysConst.Path_Build, LocalConfig.Platform.ToString());//xxxx/Windows_x64
                else return Path.Combine(SysConst.Path_Build, FullVersion);//xxxx/BloodyMary v0.0 Preview1 Windows_x64 Steam
            }
        }
        public static string ExePath
        {
            get
            {
                if (LocalConfig.Platform == Platform.Windows64)
                    return Path.Combine(DirPath, BuildConfig.Name + ".exe");
                else if (LocalConfig.Platform == Platform.Android)
                    return Path.Combine(DirPath, BuildConfig.Name + ".apk");
                else if (LocalConfig.Platform == Platform.IOS)
                    return Path.Combine(DirPath, BuildConfig.Name + ".ipa");
                throw new Exception();
            }
        }

        #region isCan
        /// <summary>
        /// 数据库版本是否兼容
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsInData(int data)=> VersionConfig.Ins.Data == data;
        #endregion

        #region misc
        public static string GetPlatformName()
        {
#if UNITY_STANDALONE_OSX
			return "OSX";
#elif UNITY_STANDALONE_WIN
            return "WIN";
#elif UNITY_STANDALONE_LINUX
			return "LINUX";
#elif UNITY_STANDALONE
			return "STANDALONE";
#elif UNITY_WII
			return "WII";
#elif UNITY_IOS
			return "IOS";
#elif UNITY_IPHONE
			return "IPHONE";
#elif UNITY_ANDROID
            return "ANDROID";
#elif UNITY_PS3
			return "PS3";
#elif UNITY_PS4
			return "PS4";
#elif UNITY_SAMSUNGTV
			return "SAMSUNGTV";
#elif UNITY_XBOX360
			return "XBOX360";
#elif UNITY_XBOXONE
			return "XBOXONE";
#elif UNITY_TIZEN
			return "TIZEN";
#elif UNITY_TVOS
			return "TVOS";
#elif UNITY_WP_8_1
			return "WP_8_1";
#elif UNITY_WSA_10_0
			return "WSA_10_0";
#elif UNITY_WSA_8_1
			return "WSA_8_1";
#elif UNITY_WSA
			return "WSA";
#elif UNITY_WINRT_10_0
			return "WINRT_10_0";
#elif UNITY_WINRT_8_1
			return "WINRT_8_1";
#elif UNITY_WINRT
			return "WINRT";
#elif UNITY_WEBGL
			return "WEBGL";
#else
            return "UNKNOWNHW";
#endif
        }
        #endregion

        #region info
        public static string SimpleSystemInfo
        {
            get
            {
                string info =
                       "OS:" + SystemInfo.operatingSystem +
                       "\nProcessor:" + SystemInfo.processorType +
                       "\nMemory:" + SystemInfo.systemMemorySize +
                       "\nGraphics API:" + SystemInfo.graphicsDeviceType +
                       "\nGraphics Processor:" + SystemInfo.graphicsDeviceName +
                       "\nGraphics Memory:" + SystemInfo.graphicsMemorySize +
                       "\nGraphics Vendor:" + SystemInfo.graphicsDeviceVendor;
                return info;
            }
        }
        // 基本系统信息
        public static string BaseSystemInfo
        {
            get
            {
                string systemInfo =
                "DeviceModel：" + SystemInfo.deviceModel +
                "\nDeviceName：" + SystemInfo.deviceName +
                "\nDeviceType：" + SystemInfo.deviceType +
                "\nGraphicsDeviceName：" + SystemInfo.graphicsDeviceName +
                "\nGraphicsDeviceVersion:" + SystemInfo.graphicsDeviceVersion +
                "\nGraphicsMemorySize（M）：" + SystemInfo.graphicsMemorySize +
                "\nGraphicsShaderLevel：" + SystemInfo.graphicsShaderLevel +
                "\nMaxTextureSize：" + SystemInfo.maxTextureSize +
                "\nOperatingSystem：" + SystemInfo.operatingSystem +
                "\nProcessorCount：" + SystemInfo.processorCount +
                "\nProcessorType：" + SystemInfo.processorType +
                "\nSystemMemorySize：" + SystemInfo.systemMemorySize;

                return systemInfo;
            }
        }
        // 高级系统信息
        public static string AdvSystemInfo
        {
            get
            {
                string systemInfo =
                "DeviceModel：" + SystemInfo.deviceModel +
                "\nDeviceName：" + SystemInfo.deviceName +
                "\nDeviceType：" + SystemInfo.deviceType +
                "\nDeviceUniqueIdentifier：" + SystemInfo.deviceUniqueIdentifier +
                "\nGraphicsDeviceID：" + SystemInfo.graphicsDeviceID +
                "\nGraphicsDeviceName：" + SystemInfo.graphicsDeviceName +
                "\nGraphicsDeviceVendor：" + SystemInfo.graphicsDeviceVendor +
                "\nGraphicsDeviceVendorID:" + SystemInfo.graphicsDeviceVendorID +
                "\nGraphicsDeviceVersion:" + SystemInfo.graphicsDeviceVersion +
                "\nGraphicsMemorySize（M）：" + SystemInfo.graphicsMemorySize +
                "\nGraphicsShaderLevel：" + SystemInfo.graphicsShaderLevel +
                "\nMaxTextureSize：" + SystemInfo.maxTextureSize +
                "\nNpotSupport：" + SystemInfo.npotSupport +
                "\nOperatingSystem：" + SystemInfo.operatingSystem +
                "\nProcessorCount：" + SystemInfo.processorCount +
                "\nProcessorType：" + SystemInfo.processorType +
                "\nSupportedRenderTargetCount：" + SystemInfo.supportedRenderTargetCount +
                "\nSupports3DTextures：" + SystemInfo.supports3DTextures +
                "\nSupportsAccelerometer：" + SystemInfo.supportsAccelerometer +
                "\nSupportsComputeShaders：" + SystemInfo.supportsComputeShaders +
                "\nSupportsGyroscope：" + SystemInfo.supportsGyroscope +
                "\nSupportsInstancing：" + SystemInfo.supportsInstancing +
                "\nSupportsLocationService：" + SystemInfo.supportsLocationService +
                "\nSupportsShadows：" + SystemInfo.supportsShadows +
                "\nSupportsSparseTextures：" + SystemInfo.supportsSparseTextures +
                "\nSupportsVibration：" + SystemInfo.supportsVibration +
                "\nSystemMemorySize：" + SystemInfo.systemMemorySize;

                return systemInfo;
            }
        }
        #endregion
    }
}