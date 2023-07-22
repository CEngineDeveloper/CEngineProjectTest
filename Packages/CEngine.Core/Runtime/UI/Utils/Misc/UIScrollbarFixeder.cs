//------------------------------------------------------------------------------
// UIScrollbarFixeder.cs
// Created by CYM on 2021/11/7
// 填写类的描述...
//------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

namespace CYM.UI
{
    [ExecuteInEditMode][HideMonoScript]
    public class UIScrollbarFixeder : UIBehaviour
    {
        Scrollbar Scrollbar;

        protected override void Awake()
        {
            base.Awake();
            Scrollbar = GetComponent<Scrollbar>();
            if (Scrollbar)
            {
                var image = Scrollbar.handleRect.GetComponent<Image>();
                image.type = Image.Type.Simple;
                image.preserveAspect = true;
                if (Scrollbar)
                    Scrollbar.onValueChanged.AddListener(OnValueChange);
            }
        }
        void Update()
        {
            ForceUpdate();
        }
        //protected override void OnCanvasHierarchyChanged()
        //{
        //    base.OnCanvasHierarchyChanged();
        //    ForceUpdate();
        //}
        //protected override void OnCanvasGroupChanged()
        //{
        //    base.OnCanvasGroupChanged();
        //    ForceUpdate();
        //}
        private void OnValueChange(float arg0)
        {
            ForceUpdate();
        }

        public void ForceUpdate()
        {
            if (Scrollbar)
                Scrollbar.size = 0;

        }
        bool reverseValue { get { return Scrollbar.direction == UnityEngine.UI.Scrollbar.Direction.RightToLeft || Scrollbar.direction == UnityEngine.UI.Scrollbar.Direction.TopToBottom; } }
        public void UpdateVal()
        {
            Vector2 anchorMin = Vector2.zero;
            Vector2 anchorMax = Vector2.one;

            float movement = Mathf.Clamp01(0) * (1 - 0);
            if (reverseValue)
            {
                anchorMin[(int)0] = 1 - movement - 0;
                anchorMax[(int)0] = 1 - movement;
            }
            else
            {
                anchorMin[(int)1] = movement;
                anchorMax[(int)1] = movement + 0;
            }

            Scrollbar.handleRect.anchorMin = anchorMin;
            Scrollbar.handleRect.anchorMax = anchorMax;
        }
        public void Rebuild()
        {
            if (Scrollbar)
                Scrollbar.Rebuild(CanvasUpdate.Layout);
        }
    }
}