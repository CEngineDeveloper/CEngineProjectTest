using UnityEngine;
using System;
using System.Collections.Generic;
using Invoke;

namespace CYM
{
    public class Delayer : MonoBehaviour
    {
        #region data
        class DelayedEditorAction
        {
            internal double TimeToExecute;
            internal Action Action;
            internal MonoBehaviour ActionTarget;
            internal bool ForceEvenIfTargetIsGone;

            public DelayedEditorAction(double timeToExecute, Action action, MonoBehaviour actionTarget, bool forceEvenIfTargetIsGone = false)
            {
                TimeToExecute = timeToExecute;
                Action = action;
                ActionTarget = actionTarget;
                ForceEvenIfTargetIsGone = forceEvenIfTargetIsGone;
            }
        }
        class DelayedAction
        {
            public float timeToExecute;
            public Action action;
            public MonoBehaviour target;
            public bool forceEvenIfTargetIsInactive;
        }
        #endregion

        #region ins
        private static Delayer _ins;
        private static Delayer Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = GameObject.FindObjectOfType<Delayer>();

                    if (_ins == null && !IsQuitting)
                    {
                        var timerGO = new GameObject("Delayer");
                        _ins = timerGO.AddComponent<Delayer>();

                        GameObject.DontDestroyOnLoad(timerGO);
                    }
                }

                return _ins;
            }
        }
        #endregion

        #region prop
        public static bool IsFirstFrame=> Time.frameCount <= 1;
        private static bool IsQuitting { get; set; }
        List<DelayedAction> delayedActions { get; set; } = new List<DelayedAction>();
        #endregion


        #region life
        [RuntimeInitializeOnLoadMethod()]
        public static void OnLoad()
        {
            Application.quitting += () => IsQuitting = true;
        }
        #endregion

        #region get
        public static WaitForSecondsRealtime GetWaitForSecondsRealtimeInstruction(float seconds)
        {
            return new WaitForSecondsRealtime(seconds);
        }

        public static WaitForSeconds GetWaitForSecondsInstruction(float seconds)
        {
            return new WaitForSeconds(seconds);
        }
        #endregion

        #region update
        private void Update()
        {
            List<DelayedAction> actionsToExecute = null;
            foreach (var action in delayedActions)
            {
                if (Time.unscaledTime >= action.timeToExecute)
                {
                    if (actionsToExecute == null) actionsToExecute = new List<DelayedAction>();
                    actionsToExecute.Add(action);
                }
            }

            if (actionsToExecute == null || actionsToExecute.Count == 0) return;

            foreach (var action in actionsToExecute)
            {
                try
                {
                    if ((action.forceEvenIfTargetIsInactive)
                     || (action.target != null && action.target.gameObject.activeInHierarchy))
                    {
                        action.action.Invoke();
                    }
                }
                finally
                {
                    delayedActions.Remove(action);
                }
            }

            // stop calling update if we have nothing scheduled (DelayedCall will re-enable this)
            if (delayedActions.Count == 0) this.enabled = false;
        }
        #endregion

        #region delay
        /// <summary>
        /// Call Action 'action' after the specified delay, provided the 'actionTarget' is still present and active in the scene at that time.
        /// Can be used in both edit and play modes.
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="action"></param>
        /// <param name="actionTarget"></param>
        public static void DelayedCall(float delay, Action action, MonoBehaviour actionTarget, bool forceEvenIfObjectIsInactiveP = false)
        {
            if (Application.isPlaying)
            {
                Ins.delayedActions.Add(new DelayedAction { timeToExecute = Time.unscaledTime + delay, action = action, target = actionTarget, forceEvenIfTargetIsInactive = forceEvenIfObjectIsInactiveP });
            }
        }
        public static void DelayedCall(float delay, Action action,bool forceEvenIfObjectIsInactiveP = false)
        {
            if (Application.isPlaying)
            {
                Ins.delayedActions.Add(new DelayedAction { timeToExecute = Time.unscaledTime + delay, action = action, target = Ins, forceEvenIfTargetIsInactive = forceEvenIfObjectIsInactiveP });
            }
        }

        /// <summary>
        /// Shorthand for DelayedCall(0, action, actionTarget)
        /// </summary>
        /// <param name="action"></param>
        /// <param name="actionTarget"></param>
        public static void AtEndOfFrame(Action action, MonoBehaviour actionTarget, bool forceEvenIfObjectIsInactive = false)
        {
            DelayedCall(0, action, actionTarget, forceEvenIfObjectIsInactive);
        }
        public static void AtEndOfFrame(Action action, bool forceEvenIfObjectIsInactive = false)
        {
            DelayedCall(0, action, Ins, forceEvenIfObjectIsInactive);
        }
        #endregion

        #region utile
        public static IJob Invoke(Action action, float delay = 0.5f)
        {
            return SuperInvoke.Run(action, delay);
        }
        #endregion
    }
}

