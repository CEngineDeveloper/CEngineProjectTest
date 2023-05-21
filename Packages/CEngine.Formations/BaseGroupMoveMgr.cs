//------------------------------------------------------------------------------
// BaseGroupMoveMgr.cs
// Created by CYM on 2022/3/18
// 填写类的描述...
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using CYM.Pool;

namespace CYM.Formations
{
    public class BaseGroupMoveMgr : BaseGFlowMgr
    {
        #region class
        public class MoveData
        {
            #region private
            BaseUnit unit;
            #endregion

            public BaseUnit TargetUnit { get; set; }
            public BaseUnit Unit
            {
                set
                {
                    unit = value;
                    Pos = unit.Pos;
                }
                get => unit;
            }
            public Vector3 Offset { get; set; }
            public Vector3 Pos { get; set; }
            public bool IsTargetSet => TargetOffset.HasValue;
            public Vector3? TargetOffset { get; set; }

            public void Clear()
            {
                unit = null;
                Pos = Vector3.zero;
                Offset = Vector3.zero;
                TargetOffset = null;
            }
        }
        #endregion

        #region private
        List<Vector3> offsets = new List<Vector3>();
        List<MoveData> moveUnits = new List<MoveData>();
        static readonly ObjPool<MoveData> moveDataPool = new ObjPool<MoveData>();
        #endregion

        #region override
        protected virtual float ForceSpacing => 2.0f;
        //移动到某个位置点
        protected virtual void MoveToPos(MoveData moveData)
        {
            throw new System.NotImplementedException();
        }
        //移动到某个静态单位,比如建筑,城池
        protected virtual void MoveToStatic(MoveData moveData)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region Group move
        public void GroupToPos(IEnumerable<BaseUnit> selUnits, Vector3 pos)
        {
            GenerateMoveData(selUnits);
            Vector3 center = CalcAvgPos();
            CalcOffset(center);
            Vector3 dir = (pos - center).normalized;
            GetFormationOffsets(offsets, moveUnits.Count, dir, ForceSpacing);
            CalcTargetOffset();
            for (int i = 0; i < moveUnits.Count; ++i)
            {
                moveUnits[i].Pos = pos + moveUnits[i].TargetOffset.Value;
                MoveToPos(moveUnits[i]);
            }
            RecycleMoveUnitsData();


            void CalcOffset(Vector3 center)
            {
                foreach (var unit in moveUnits)
                {
                    Vector3 delta = unit.Pos - center;
                    delta.y = 0;
                    unit.Offset = delta;
                }

            }
            void CalcTargetOffset()
            {
                for (int i = 0; i < moveUnits.Count; i++)
                {
                    Vector3 targetOffset = offsets[i];
                    float min = float.MaxValue;
                    int index = -1;
                    for (int j = 0; j < moveUnits.Count; j++)
                    {
                        MoveData unit = moveUnits[j];
                        if (!unit.IsTargetSet)
                        {
                            Vector3 delta = unit.Offset - targetOffset;
                            float sqr = delta.sqrMagnitude;
                            if (sqr < min)
                            {
                                min = sqr;
                                index = j;
                            }
                        }
                    }
                    moveUnits[index].TargetOffset = targetOffset;
                }
            }
            void GetFormationOffsets(List<Vector3> offsets, int count, Vector3 dir, float scaler = 3.0f)
            {
                offsets.Clear();
                Quaternion rot = Quaternion.LookRotation(dir);
                int colCount = Mathf.CeilToInt(Mathf.Sqrt(count));
                int rowCount = count / colCount;
                float centerRow = (rowCount - 1) / 2f;
                float centerCol = (colCount - 1) / 2f;
                for (int i = 0; i < count; i++)
                {
                    int row = i / colCount;
                    int col = i - row * colCount;
                    Vector3 offset = new Vector3(col - centerCol, 0, -(row - centerRow));
                    offsets.Add(rot * (offset * scaler));
                }
            }
        }
        public void GroupToStatic(IEnumerable<BaseUnit> selUnits, BaseUnit staticUnit)
        {
            GenerateMoveData(selUnits);
            Vector3 center = CalcAvgPos();
            Vector3 target = staticUnit.Pos;
            Vector3 dir = (center - target).normalized;
            for (int i = 0; i < moveUnits.Count; ++i)
            {
                Quaternion rotation = Quaternion.Euler(0f, (MathUtil.IsOdd(i) ? i : -i) * 30, 0f);
                moveUnits[i].TargetUnit = staticUnit;
                moveUnits[i].Pos = target + rotation * dir * 3;
                MoveToStatic(moveUnits[i]);
            }
            RecycleMoveUnitsData();
        }
        #endregion

        #region set
        void GenerateMoveData(IEnumerable<BaseUnit> selUnits)
        {
            if (selUnits == null)
                return;
            moveUnits.Clear();
            foreach (var selUnit in selUnits)
            {
                if (selUnit == null)
                {
                    continue;
                }
                if (selUnit.MoveMgr == null)
                {
                    continue;
                }
                MoveData unit = moveDataPool.Get();
                unit.Clear();
                unit.Unit = selUnit;
                moveUnits.Add(unit);
            }
            if (moveUnits.Count == 0)
            {
                return;
            }
        }
        void RecycleMoveUnitsData()
        {
            foreach (var unit in moveUnits)
            {
                moveDataPool.Recycle(unit);
            }
            moveUnits.Clear();
        }
        #endregion

        #region util
        Vector3 CalcAvgPos()
        {
            if (moveUnits.Count == 0)
                return Vector3.zero;
            Vector3 sum = Vector3.zero;
            foreach (var unit in moveUnits)
            {
                sum += unit.Pos;
            }
            return sum / moveUnits.Count;
        }
        #endregion
    }
}