using System;
using MegaMan.Common;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common.Geometry;

namespace MegaMan.Engine.Entities.Effects
{
    public class PositionEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(PositionEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var posInfo = (PositionEffectPartInfo)info;

            Effect action = entity => { };
            if (posInfo.X != null)
                action += ParsePositionBehavior(posInfo.X, Axis.X);

            if (posInfo.Y != null)
                action += ParsePositionBehavior(posInfo.Y, Axis.Y);
            
            return action;
        }

        private Effect ParsePositionBehavior(PositionEffectAxisInfo axisInfo, Axis axis)
        {
            Effect action = e => { };

            var baseVar = axisInfo.BaseVar;
            var offsetVar = axisInfo.OffsetVar;

            if (baseVar != null)
            {
                if (axis == Axis.X)
                    action = entity => {
                        var x = CheckNumericVar(entity, baseVar);
                        if (x.HasValue)
                        {
                            PositionComponent pos = entity.GetComponent<PositionComponent>();
                            if (pos != null) pos.SetPosition(new PointF(x.Value, pos.Position.Y));
                        }
                    };
                else
                    action = entity => {
                        var y = CheckNumericVar(entity, baseVar);
                        if (y.HasValue)
                        {
                            PositionComponent pos = entity.GetComponent<PositionComponent>();
                            if (pos != null) pos.SetPosition(new PointF(pos.Position.X, y.Value));
                        }
                    };
            }
            else if (axisInfo.Base == null)
            {
                action = entity => {
                    PositionComponent pos = entity.GetComponent<PositionComponent>();
                    if (pos != null && entity.Parent != null)
                    {
                        PositionComponent parentPos = entity.Parent.GetComponent<PositionComponent>();
                        if (parentPos != null)
                        {
                            pos.SetPosition(axis == Axis.X
                                ? new PointF(parentPos.Position.X, pos.Position.Y)
                                : new PointF(pos.Position.X, parentPos.Position.Y));
                        }
                    }
                };
            }
            else
            {
                if (axis == Axis.X)
                    action = entity =>
                    {
                        PositionComponent pos = entity.GetComponent<PositionComponent>();
                        if (pos != null) pos.SetPosition(new PointF(axisInfo.Base.Value, pos.Position.Y));
                    };
                else
                    action = entity =>
                    {
                        PositionComponent pos = entity.GetComponent<PositionComponent>();
                        if (pos != null) pos.SetPosition(new PointF(pos.Position.X, axisInfo.Base.Value));
                    };
            }

            if (axisInfo.Offset != null || offsetVar != null)
            {
                switch (axisInfo.OffsetDirection)
                {
                    case OffsetDirection.Inherit:
                        action += entity => {
                            var offset = axisInfo.Offset ?? CheckNumericVar(entity, offsetVar) ?? 0;
                            PositionComponent pos = entity.GetComponent<PositionComponent>();
                            if (pos != null && entity.Parent != null)
                            {
                                Direction offdir = entity.Parent.Direction;
                                switch (offdir)
                                {
                                    case Direction.Down: pos.SetPosition(new PointF(pos.Position.X, pos.Position.Y + offset)); break;
                                    case Direction.Up: pos.SetPosition(new PointF(pos.Position.X, pos.Position.Y - offset)); break;
                                    case Direction.Left: pos.SetPosition(new PointF(pos.Position.X - offset, pos.Position.Y)); break;
                                    case Direction.Right: pos.SetPosition(new PointF(pos.Position.X + offset, pos.Position.Y)); break;
                                }
                            }
                        };
                        break;

                    case OffsetDirection.Input:
                        action += entity => {
                            var offset = axisInfo.Offset ?? CheckNumericVar(entity, offsetVar) ?? 0;
                            PositionComponent pos = entity.GetComponent<PositionComponent>();
                            InputComponent input = entity.GetComponent<InputComponent>();
                            if (input != null && pos != null)
                            {
                                if (axis == Axis.Y)
                                {
                                    if (input.Down) pos.SetPosition(new PointF(pos.Position.X, pos.Position.Y + offset));
                                    else if (input.Up) pos.SetPosition(new PointF(pos.Position.X, pos.Position.Y - offset));
                                } else
                                {
                                    if (input.Left) pos.SetPosition(new PointF(pos.Position.X - offset, pos.Position.Y));
                                    else if (input.Right || (!input.Up && !input.Down)) pos.SetPosition(new PointF(pos.Position.X + offset, pos.Position.Y));
                                }
                            }
                        };
                        break;

                    case OffsetDirection.Left:
                        action += entity =>
                        {
                            var offset = axisInfo.Offset ?? CheckNumericVar(entity, offsetVar) ?? 0;
                            PositionComponent pos = entity.GetComponent<PositionComponent>();
                            if (pos != null) pos.SetPosition(new PointF(pos.Position.X - offset, pos.Position.Y));
                        };
                        break;

                    case OffsetDirection.Right:
                        action += entity =>
                        {
                            var offset = axisInfo.Offset ?? CheckNumericVar(entity, offsetVar) ?? 0;
                            PositionComponent pos = entity.GetComponent<PositionComponent>();
                            if (pos != null) pos.SetPosition(new PointF(pos.Position.X + offset, pos.Position.Y));
                        };
                        break;

                    case OffsetDirection.Down:
                        action += entity =>
                        {
                            var offset = axisInfo.Offset ?? CheckNumericVar(entity, offsetVar) ?? 0;
                            PositionComponent pos = entity.GetComponent<PositionComponent>();
                            if (pos != null) pos.SetPosition(new PointF(pos.Position.X, pos.Position.Y + offset));
                        };
                        break;

                    case OffsetDirection.Up:
                        action += entity =>
                        {
                            var offset = axisInfo.Offset ?? CheckNumericVar(entity, offsetVar) ?? 0;
                            PositionComponent pos = entity.GetComponent<PositionComponent>();
                            if (pos != null) pos.SetPosition(new PointF(pos.Position.X, pos.Position.Y - offset));
                        };
                        break;
                }
            }
            return action;
        }

        private static float? CheckNumericVar(IEntity entity, string numVar)
        {
            if (numVar != null)
            {
                var varsComp = entity.GetComponent<VarsComponent>();
                if (varsComp != null)
                {
                    var numStr = varsComp.Get(numVar);
                    if (string.IsNullOrEmpty(numStr))
                        return null;

                    float tmpNum;
                    if (float.TryParse(numStr, out tmpNum))
                        return tmpNum;
                    else
                        throw new GameRunException(string.Format("Entity {0} attempted to set position using local variable {1}, but the value it contained was not a number.", entity.Name, numVar));
                }
            }

            return null;
        }
    }
}
