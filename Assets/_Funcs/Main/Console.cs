//------------------------------------------------------------------------------
// Options.cs
// Created by CYM on 2022/1/5
// 填写类的描述...
//------------------------------------------------------------------------------
using CYM;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Gamelogic
{
    [HideMonoScript]
    public class Console : SysConsole
    {
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