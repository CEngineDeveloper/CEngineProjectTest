﻿using CYM;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Gamelogic
{
    [CreateAssetMenu(menuName = "ScriptConfig/TestConfig")]
    public partial class TestConfig:CYM.TestConfig
    {
        public static new TestConfig Ins => CYM.TestConfig.Ins as TestConfig;

        [ConsoleUpdate]
        public static void OnGMUpdate()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                CLog.Error("测试log，测试log，测试log，测试log测试log测试log测试log测试log测试log测试log测试log测试log测试log测试log测试log测试log测试log测试log测试log测试log测试log测试log测试log-- \n 10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd10100sdaasdasdasdasd");
            }
        }

        [ConsoleCommand("hello")]
        public static void PrintHelloWorld() => Debug.Log("Hello World!");

        [ConsoleCommand]
        public static void Print(string text) => Debug.Log(text);

        [ConsoleCommand]
        public static void Add(int arg1, int arg2) => Debug.Log(arg1 + arg2);

        [ShowInInspector]
        public static bool IsTest = false;

        [Button("Test")]
        public void Test()
        {

        }
    }
}
