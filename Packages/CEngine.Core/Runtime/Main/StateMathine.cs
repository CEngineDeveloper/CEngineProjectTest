//**********************************************
// Class Name	: CYMStateMathine
// Discription	：State Mathine. Useful calss for AI
// Author	：CYM
// Team		：MoBaGame
// Date		：#DATE#
// Copyright ©1995 [CYMCmmon] Powered By [CYM] Version 1.0.0 
//**********************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CYM
{
    public interface IStateMachine
    {
        BaseUnit SelfBaseUnit { get; }
        void Init(BaseUnit owner);
        void OnUpdate();
        void OnFixedUpdate();
        void RevertToPreState();
    }
    public class StateMachine<TStateData> : IStateMachine 
        where TStateData : StateMachine<TStateData>.State
    {
        #region prop
        public TStateData CurStateData { get; private set; }
        public TStateData PreStateData { get; private set; }
        public TStateData GlobalStateData { get; private set; }
        public BaseUnit SelfBaseUnit { get; private set; }
        #endregion

        public StateMachine()
        {
            SelfBaseUnit = null;
            CurStateData = null;
            PreStateData = null;
            GlobalStateData = null;
        }
        public virtual void Init(BaseUnit self)
        {
            this.SelfBaseUnit = self;
        }
        public bool IsInState(TStateData state)
        {
            return CurStateData == state;
        }
        public virtual void OnUpdate()
        {
            if (GlobalStateData != null) GlobalStateData.OnUpdate();
            if (CurStateData != null) CurStateData.OnUpdate();
        }
        public virtual void OnFixedUpdate()
        {
            if (GlobalStateData != null) GlobalStateData.OnFixedUpdate();
            if (CurStateData != null) CurStateData.OnFixedUpdate();
        }
        public virtual void OnJobUpdate()
        {
            if (GlobalStateData != null) GlobalStateData.OnJobUpdate();
            if (CurStateData != null) CurStateData.OnJobUpdate();
        }
        public void SetPreState(TStateData state) { PreStateData = state; }
        public void SetGlobalState(TStateData state) { GlobalStateData = state; }
        public virtual void SetCurState(TStateData state, bool isManual = true)
        {
            if (state == null) return;
            state.StateMachine = this;
            CurStateData = state;
            CurStateData.IsManual = isManual;
            CurStateData.OnEnter();
        }
        public virtual void ChangeState(TStateData state, bool isForce = true, bool isManual = true)
        {
            if (state == null) return;
            state.StateMachine = this;
            PreStateData = CurStateData;
            if (CurStateData != null) CurStateData.OnExit();
            CurStateData = state;
            CurStateData.IsForce = isForce;
            CurStateData.IsManual = isManual;
            CurStateData.OnEnter();
        }
        public void RevertToPreState()
        {
            ChangeState(PreStateData);
        }

        public class State
        {
            public StateMachine<TStateData> StateMachine { get; set; }
            public bool IsManual { get; set; } = false;
            public bool IsForce { get; set; } = false;
            public float UpdateTime { get; set; } = 0.0f;
            public virtual void OnFixedUpdate() { }
            public virtual void OnJobUpdate() { }
            public virtual void OnUpdate()
            {
                UpdateTime += Time.deltaTime;
            }
            public virtual void OnEnter()
            {
                UpdateTime = 0;
            }
            public virtual void OnExit() { }
        }
    }

    public class CharaStateMachine<TState, TUnit, TStateData,TBalckboard> : StateMachine<TStateData> 
        where TState : Enum 
        where TUnit : BaseUnit 
        where TStateData : CharaStateMachine<TState, TUnit, TStateData, TBalckboard>.State, new()
        where TBalckboard : class,new()
    {
        #region Callback
        public event Callback<TState, TState> Callback_OnChangeState;
        #endregion

        #region prop
        public Dictionary<int, TStateData> States = new Dictionary<int, TStateData>();
        public TUnit SelfUnit { get; protected set; }
        public TBalckboard Balckboard { get; protected set; } = new TBalckboard();
        #endregion

        #region Val
        public TState CurState { get; private set; }
        public TState PreState { get; private set; }
        #endregion

        #region life
        public override void Init(BaseUnit selfUnit)
        {
            if (selfUnit == null)
            {
                CLog.Error("CharaStateMachine.Init:selfUnit 不能为空");
                return;
            }
            ClearState();
            base.Init(selfUnit);
            SelfUnit = selfUnit as TUnit;
            EnumUtil<TState>.For((x) =>
            {
                AddState(x, new TStateData());
            });
        }
        #endregion

        #region set
        /// <summary>
        ///直接改变一个状态
        ///target:目标
        ///isForce:是否为强制性
        ///isManual:是否为手动改变(非系统性操作)
        /// </summary>
        /// <param name="state"></param>
        public virtual TStateData ChangeState(TState state, bool isForce = false, bool isManual = true)
        {
            int intCurState = EnumUtil<TState>.Int(CurState);
            int intNextState = EnumUtil<TState>.Int(state);
            if (!isForce && intCurState == intNextState) return null;
            PreState = CurState;
            CurState = state;
            var newStateData = States[intNextState];
            newStateData.StateMachine = this;
            newStateData.SelfUnit = SelfUnit;
            newStateData.Blackboard = Balckboard;
            base.ChangeState(newStateData, isForce, isManual);
            Callback_OnChangeState?.Invoke(CurState, PreState);
            return newStateData;
        }
        /// <summary>
        /// 设置状态
        /// </summary>
        public void SetCurState(TState state, bool isManual = true)
        {
            int intstate = EnumUtil<TState>.Int(state);
            if (!States.ContainsKey(intstate)) return;
            var newStateData = States[intstate];
            newStateData.StateMachine = this;
            newStateData.SelfUnit = SelfUnit;
            newStateData.Blackboard = Balckboard;
            SetCurState(newStateData, isManual);
            CurState = state;
        }
        /// <summary>
        /// 添加一个状态
        /// </summary>
        /// <param name="type"></param>
        /// <param name="state"></param>
        public void AddState(TState type, TStateData state)
        {
            int key = EnumUtil<TState>.Int(type);
            state.SelfUnit = SelfUnit;
            state.StateType = CurState;
            state.OnBeAdded();
            if (States.ContainsKey(key))
            {
                States[key] = state;
            }
            else
            {
                States.Add(key, state);
            }

        }
        /// <summary>
        /// 清空状态
        /// </summary>
        public void ClearState()
        {
            States.Clear();
        }
        #endregion

        #region get
        public TStateData GetState(TState state)
        {
            int key = EnumUtil<TState>.Int(state);
            if (States.ContainsKey(key))
                return States[key];
            return null;
        }
        #endregion

        #region is
        // 是否在指定状态
        public bool IsIn(TState state)
        {
            int intState = EnumUtil<TState>.Int(CurState);
            int intstate = EnumUtil<TState>.Int(state);
            return intState == intstate;
        }
        // 判断上一个状态
        public bool IsInPreState(TState state, int index)
        {
            int intState = EnumUtil<TState>.Int(CurState);
            int intstate = EnumUtil<TState>.Int(state);
            return intState == intstate;
        }
        #endregion


        public new class State : StateMachine<TStateData>.State
        {
            public new CharaStateMachine<TState, TUnit, TStateData, TBalckboard> StateMachine { get; set; }
            public TBalckboard Blackboard { get; set; }
            public float Wait { get; set; } = 0.0f;
            public TUnit SelfUnit { get; set; }
            public TState StateType{ get; set; }
            public State() : base() { }
            public override void OnUpdate()
            {
                base.OnUpdate();
                if (UpdateTime >= Wait) { }
            }
            public virtual void OnBeAdded() { }

            #region is
            protected bool IsLocalPlayer()
            {
                return SelfUnit.IsPlayer();
            }
            #endregion

        }
    }
}
