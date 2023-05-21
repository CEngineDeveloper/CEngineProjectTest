//------------------------------------------------------------------------------
// BaseLineRenderMgr.cs
// Copyright 2019 2019/4/20 
// Created by CYM on 2019/4/20
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using DigitalRuby.FastLineRenderer;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace CYM.Line
{
    public class BaseULineMgr : BaseMgr
    {
        #region prop
        FastLineRenderer LineRender;
        Color PathColor = Color.green;
        List<Vector3> PathVector = new List<Vector3>();
        #endregion

        #region life
        protected virtual List<Vector3> RawPathVector => throw new NotImplementedException();
        protected virtual float LineRadius => 0.025f;
        protected virtual float LineYOffset => 0.3f;
        protected virtual float LineEndCapScale => 5.0f;
        public sealed override MgrType MgrType => MgrType.Unit;

        public override void OnDeath()
        {
            base.OnDeath();
            ClearLine();
        }
        #endregion

        #region set
        // 一般上会在玩家选择的时候绘制
        public void SetColor(Color color)
        {
            PathColor = color;
        }

        public void DrawPath(List<Vector3> newPath)
        {
            BaseGlobal.PathRenderMgr.ClearPath(ref LineRender);
            LineRender = BaseGlobal.PathRenderMgr.DrawPath(newPath, PathColor, LineRadius, LineEndCapScale);
        }
        public void ClearLine()
        {
            IsForceDraw = false;
            BaseGlobal.PathRenderMgr.ClearPath(ref LineRender);
        }
        public void UpdateDrawLine()
        {
            //if (SelfBaseUnit.MoveMgr == null)
            //    return;
            if (
                IsNeedDraw ||
                IsForceDraw)
            {
                DrawPath(PathVector);
            }
            else
            {
                ClearLine();
            }
        }
        #endregion

        #region is
        public bool HasDraw => LineRender != null;
        public bool IsForceDraw { get; private set; } = false;
        public bool IsNeedDraw=> (IsForceDraw || BaseInputMgr.IsSelectUnit(SelfBaseUnit)) && PathVector.Count>0;
        #endregion

        #region set
        public void StartDrowLine()
        {
            PathVector.Clear();
            PathVector.AddRange(RawPathVector);
            for (int i = 0; i < PathVector.Count; i++)
            {
                Vector3 point = PathVector[i];
                float terrainY = TerrainObj.Ins.SampleHeight(point) + LineYOffset;
                float bakedPos = BaseSceneRoot.Ins.GetBakedColliderPos(point).y + LineYOffset;
                if (terrainY < bakedPos)
                    terrainY = bakedPos;
                // 避免地面小幅度起伏时先画在地面下面的问题
                point.y = terrainY;
                PathVector[i] = point;
            }
            UpdateDrawLine();
        }
        public void EndDrawLine()
        {
            ClearLine();
            PathVector.Clear();
        }
        public void StepDrawLine()
        {
            if (PathVector.Count > 4)
                PathVector.RemoveAt(0);
            UpdateDrawLine();
        }
        #endregion
    }
}