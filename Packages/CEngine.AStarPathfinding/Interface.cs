using CYM.Move;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;
namespace CYM.Pathfinding
{
    public interface IAStarTBMoveMgr : IAStarMoveMgr
    {
        BaseUnit MoveTarget_Unit { get; }
        bool ExcuteMoveTarget(bool isManual);
        //�Ƿ����ƶ�Ŀ�꣬���ڻغ�����Ϸ
        bool IsHaveMoveTarget();
        //�Ƿ�����Զ�ִ��MoveTarget����,���ڻغ�����Ϸ
        bool IsCanAutoExcuteMoveTarget();
    }
    public interface IAStarMoveMgr: IMoveMgr
    {
        #region is
        bool IsCanTraversal(GraphNode node);
        bool IsHaveValidPathData()
        {
            if (ABPath == null)
                return false;
            if (ABPath.vectorPath == null)
                return false;
            return true;
        }
        #endregion

        #region AStar
        ABPath ABPath { get; }
        Seeker Seeker { get; }
        BaseTraversal Traversal { get; }
        void SetToNode(GraphNode node);
        GraphNode PreNode { get; }
        GraphNode CurNode { get; }

        #endregion

        #region get
        List<Vector3> GetDetailPathPoints(ABPath path = null);
        #endregion
    }
}