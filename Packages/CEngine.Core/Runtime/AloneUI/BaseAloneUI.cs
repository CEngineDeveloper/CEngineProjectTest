/**
 * BaseAloneUI.cs
 * Created by: CYM [as8506@qq.com]
 * Created on: 2023/6/22 (zh-CN)
 */

using UnityEngine;
using UnityEngine.UI;

namespace CYM
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(CanvasGroup))]
    public class BaseAloneUI<T>:MonoBehaviour where T:BaseAloneUI<T>
	{
        protected static CanvasScaler CanvasScaler;
        protected static CanvasGroup CanvasGroup;
        protected static Canvas Canvas;

        public static T Ins { get; private set; }

        protected static void Create(string prefab)
        {
            if (IsInited())
                return;
            Ins = Util.CreateGlobalResourceObj<T>(prefab);
            DontDestroyOnLoad(Ins);
            CanvasScaler = Ins.GetComponent<CanvasScaler>();
            CanvasGroup = Ins.GetComponent<CanvasGroup>();
            Canvas = Ins.GetComponent<Canvas>();
            CanvasScaler.referenceResolution = new Vector2(BuildConfig.Ins.Width, BuildConfig.Ins.Height);
        }
        public static bool IsInited()
        {
            return Ins != null;
        }
	}
}