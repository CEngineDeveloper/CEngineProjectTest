/**
 * EditorTitle.cs
 * Created by: CYM [as8506@qq.com]
 * Created on: 2023/6/29 (zh-CN)
 */

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CYM
{
    [InitializeOnLoad]
    public class EditorTitle
	{
        private const BindingFlags UPDATE_MAIN_WINDOW_TITLE_ATTR = BindingFlags.Static | BindingFlags.NonPublic;
        private const string APPLICATION_TITLE_DESCRIPTOR_FULL_NAME = "UnityEditor.ApplicationTitleDescriptor";

        private static readonly Type EDITOR_APPLICATION_TYPE = typeof(EditorApplication);
        private static readonly Assembly EDITOR_ASSEMBLY = EDITOR_APPLICATION_TYPE.Assembly;
        private static readonly Type[] EDITOR_TYPES = EDITOR_ASSEMBLY.GetTypes();

        private static readonly Type APPLICATION_TITLE_DESCRIPTOR_TYPE = EDITOR_TYPES
            .FirstOrDefault(c => c.FullName == APPLICATION_TITLE_DESCRIPTOR_FULL_NAME);

        private static readonly EventInfo UPDATE_MAIN_WINDOW_TITLE_EVENT_INFO =
            EDITOR_APPLICATION_TYPE.GetEvent("updateMainWindowTitle", UPDATE_MAIN_WINDOW_TITLE_ATTR);

        private static readonly MethodInfo UPDATE_MAIN_WINDOW_TITLE_METHOD_INFO =
            EDITOR_APPLICATION_TYPE.GetMethod("UpdateMainWindowTitle", UPDATE_MAIN_WINDOW_TITLE_ATTR);

        private static string m_title;

        static EditorTitle()
        {
            EditorApplication.projectWindowItemOnGUI += OnFocusChanged;   

        }
        void DisableEditorApi()
        {
            EditorApplication.projectWindowItemOnGUI -= OnFocusChanged;
        }

        private static void OnFocusChanged(string guid, Rect selectionRect)
        {
            var folderName = Environment.CurrentDirectory.Split('\\').Last();
            string platform = Util.PlatformName;
            string curSceneName = SceneManager.GetActiveScene().name;
            SetTitle($"{folderName} <{platform}> <{curSceneName}>");
        }

        static void SetTitle(string title)
        {
            m_title = title;

            var delegateType = typeof(Action<>).MakeGenericType(APPLICATION_TITLE_DESCRIPTOR_TYPE);
            var methodInfo = ((Action<object>)UpdateMainWindowTitle).Method;
            var del = Delegate.CreateDelegate(delegateType, null, methodInfo);
            var nonPublic = true;
            var parameters = new object[] { del };

            UPDATE_MAIN_WINDOW_TITLE_EVENT_INFO.GetAddMethod(nonPublic).Invoke(null, parameters);
            UPDATE_MAIN_WINDOW_TITLE_METHOD_INFO.Invoke(null, new object[0]);
            UPDATE_MAIN_WINDOW_TITLE_EVENT_INFO.GetRemoveMethod(nonPublic).Invoke(null, parameters);
        }

        private static void UpdateMainWindowTitle(object desc)
        {
            var fieldInfo = APPLICATION_TITLE_DESCRIPTOR_TYPE
                .GetField("title", BindingFlags.Instance | BindingFlags.Public);

            fieldInfo.SetValue(desc, m_title);
        }
    }
}