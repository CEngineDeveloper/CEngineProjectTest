using UnityEngine;
namespace CYM.Move
{
    public interface IMoveMgr
    {
        #region Callback val
        Callback Callback_OnMoveStart { get; set; }
        Callback Callback_OnMovingAlone { get; set; }
        Callback Callback_OnMoveEnd { get; set; }
        Callback Callback_OnFirstMovingAlone { get; set; }
        Callback Callback_OnMovingStep { get; set; }
        Callback Callback_OnMoveDestination { get; set; }
        #endregion

        #region prop
        BaseUnit GetSelfBaseUnit();
        Quaternion NewQuateration { get; }
        //BaseUnit FaceTarget { get; }
        float SearchedSpeed { get; }
        Vector3 SearchedPos { get; }
        Vector3 Destination { get; }
        #endregion

        #region val
        float BaseMoveSpeed { get; }
        #endregion

        #region is
        bool IsPositionChange { get; }
        bool IsRotationChange { get; }
        bool IsMoving { get; }
        bool IsCanMove { get; }
        bool IsInDestination() => MathUtil.Approximately(Destination, GetSelfBaseUnit().Pos);
        bool IsNoInDestination() => !IsInDestination();
        bool IsInPos(Vector3 targetPos, float k = 0) => MathUtil.Approximately(GetSelfBaseUnit().Pos, targetPos, k);
        #endregion

        #region set
        bool RePath()
        {
            if (IsMoving)
                return StartPath(SearchedPos, SearchedSpeed);
            return false;
        }
        bool StartPath(Vector3 pos, float speed);
        void StopPath();
        void PreviewPath(Vector3 pos);
        #endregion

        #region rotate
        void GrabNewQuateration(Quaternion? qua = null);
        void Look(BaseUnit unit);
        void SetRotationY(float rot);
        void RandRotationY()
        {
            RandUtil.RandForwardY(GetSelfBaseUnit(), Vector3.up);
            GrabNewQuateration();
        }
        #endregion
    }
}