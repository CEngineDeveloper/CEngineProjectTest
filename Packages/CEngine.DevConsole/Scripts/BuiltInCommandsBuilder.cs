﻿using CYM.DevConsole.Command;
namespace CYM.DevConsole {
    public class BuiltInCommandsBuilder : Command.Unity.BuiltInCommandsBuilder {
        public BuiltInCommandsBuilder(CommandsManager manager) : base(manager) { }

        public override void Build() {
            int commandsBefore = manager.GetCommands().Length;

            if(DevConsole.settings.builtInCommands.analytics)
                Analytics();
            if(DevConsole.settings.builtInCommands.performanceReporting)
                PerformanceReporting();
            if(DevConsole.settings.builtInCommands.androidInput)
                AndroidInput();
            if(DevConsole.settings.builtInCommands.animator)
                Animator();
            if(DevConsole.settings.builtInCommands.appleReplayKit)
                AppleReplayKit();
            if(DevConsole.settings.builtInCommands.appleTvRemote)
                AppleTvRemote();
            if(DevConsole.settings.builtInCommands.application)
                Application();
            if(DevConsole.settings.builtInCommands.audioListener)
                AudioListener();
            if(DevConsole.settings.builtInCommands.audioSettings)
                AudioSettings();
            if(DevConsole.settings.builtInCommands.audioSource)
                AudioSource();
            if(DevConsole.settings.builtInCommands.caching)
                Caching();
            if(DevConsole.settings.builtInCommands.camera)
                Camera();
            if(DevConsole.settings.builtInCommands.canvas)
                Canvas();
            if(DevConsole.settings.builtInCommands.color)
                Color();
            if(DevConsole.settings.builtInCommands.color32)
                Color32();
            if(DevConsole.settings.builtInCommands.colorUtility)
                ColorUtility();
            if(DevConsole.settings.builtInCommands.crashReport)
                CrashReport();
            if(DevConsole.settings.builtInCommands.crashReportHandler)
                CrashReportHandler();
            if(DevConsole.settings.builtInCommands.cursor)
                Cursor();
            if(DevConsole.settings.builtInCommands.debug)
                Debug();
            if(DevConsole.settings.builtInCommands.playerConnection)
                PlayerConnection();
            if(DevConsole.settings.builtInCommands.display)
                Display();
            if(DevConsole.settings.builtInCommands.dynamicGI)
                DynamicGI();
            if(DevConsole.settings.builtInCommands.font)
                Font();
            if(DevConsole.settings.builtInCommands.gameObject)
                GameObject();
            if(DevConsole.settings.builtInCommands.hash128)
                Hash128();
            if(DevConsole.settings.builtInCommands.handheld)
                Handheld();
            if(DevConsole.settings.builtInCommands.humanTrait)
                HumanTrait();
            if(DevConsole.settings.builtInCommands.input)
                Input();
            if(DevConsole.settings.builtInCommands.compass)
                Compass();
            if(DevConsole.settings.builtInCommands.gyroscope)
                Gyroscope();
            if(DevConsole.settings.builtInCommands.locationService)
                LocationService();
            if(DevConsole.settings.builtInCommands.iOSDevice)
                IOSDevice();
            if(DevConsole.settings.builtInCommands.iOSNotificationServices)
                IOSNotificationServices();
            if(DevConsole.settings.builtInCommands.iOSOnDemandResources)
                IOSOnDemandResources();
            if(DevConsole.settings.builtInCommands.layerMask)
                LayerMask();
            if(DevConsole.settings.builtInCommands.lightmapSettings)
                LightmapSettings();
            if(DevConsole.settings.builtInCommands.lightProbeProxyVolume)
                LightProbeProxyVolume();
            if(DevConsole.settings.builtInCommands.lODGroup)
                LODGroup();
            if(DevConsole.settings.builtInCommands.masterServer)
                MasterServer();
            if(DevConsole.settings.builtInCommands.mathf)
                Mathf();
            if(DevConsole.settings.builtInCommands.microphone)
                Microphone();
            if(DevConsole.settings.builtInCommands.physics)
                Physics();
            if(DevConsole.settings.builtInCommands.physics2D)
                Physics2D();
            if(DevConsole.settings.builtInCommands.playerPrefs)
                PlayerPrefs();
            if(DevConsole.settings.builtInCommands.proceduralMaterial)
                ProceduralMaterial();
            //if(DevConsole.settings.builtInCommands.profiler)
            //    UnityEngine.Profiling.Profiler();
            if(DevConsole.settings.builtInCommands.qualitySettings)
                QualitySettings();
            if(DevConsole.settings.builtInCommands.quaternion)
                Quaternion();
            if(DevConsole.settings.builtInCommands.random)
                Random();
            if(DevConsole.settings.builtInCommands.rect)
                Rect();
            if(DevConsole.settings.builtInCommands.reflectionProbe)
                ReflectionProbe();
            if(DevConsole.settings.builtInCommands.remoteSettings)
                RemoteSettings();
            if(DevConsole.settings.builtInCommands.graphicsSettings)
                GraphicsSettings();
            if(DevConsole.settings.builtInCommands.renderSettings)
                RenderSettings();
            if(DevConsole.settings.builtInCommands.samsungTV)
                SamsungTV();
            if(DevConsole.settings.builtInCommands.sceneManager)
                SceneManager();
            if(DevConsole.settings.builtInCommands.sceneUtility)
                SceneUtility();
            if(DevConsole.settings.builtInCommands.screen)
                Screen();
            if(DevConsole.settings.builtInCommands.shader)
                Shader();
            if(DevConsole.settings.builtInCommands.sortingLayer)
                SortingLayer();
            if(DevConsole.settings.builtInCommands.systemInfo)
                SystemInfo();
            if(DevConsole.settings.builtInCommands.texture)
                Texture();
            if(DevConsole.settings.builtInCommands.time)
                Time();
            if(DevConsole.settings.builtInCommands.touchScreenKeyboard)
                TouchScreenKeyboard();
            if(DevConsole.settings.builtInCommands.vector2)
                Vector2();
            if(DevConsole.settings.builtInCommands.vector3)
                Vector3();
            if(DevConsole.settings.builtInCommands.vector4)
                Vector4();
            if(DevConsole.settings.builtInCommands.vRInputTracking)
                VRInputTracking();
            if(DevConsole.settings.builtInCommands.vRDevice)
                VRDevice();
            if(DevConsole.settings.builtInCommands.vRSettings)
                VRSettings();

            int commandsAfter = manager.GetCommands().Length;
            DevConsole.Ins.Log("Loaded " + (commandsAfter - commandsBefore) + " built-in commands");
        }
    }
}