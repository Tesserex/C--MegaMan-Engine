﻿using System;
using MegaMan.Common;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class MovementEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(MovementEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var movement = (MovementEffectPartInfo)info;

            Effect action = entity => { };

            if (movement.Floating.HasValue)
            {
                var f = movement.Floating.Value;
                action += entity => {
                    var mov = entity.GetComponent<MovementComponent>();
                    if (mov != null) mov.Floating = f;
                };
            }

            if (movement.FlipSprite.HasValue)
            {
                var flip = movement.FlipSprite.Value;
                action += entity => {
                    var mov = entity.GetComponent<MovementComponent>();
                    if (mov != null) mov.FlipSprite = flip;
                };
            }

            if (movement.X != null)
                action += ParseMovementBehavior(movement.X, Axis.X);

            if (movement.Y != null)
                action += ParseMovementBehavior(movement.Y, Axis.Y);

            if (movement.Both != null)
                action += ParseMovementBehavior(movement.Both, Axis.Both);

            return action;
        }

        private Effect ParseMovementBehavior(VelocityEffectInfo info, Axis axis)
        {
            Effect action;

            var mag = info.Magnitude;
            var magVar = info.MagnitudeVarName;

            switch (info.Direction)
            {
                case MovementEffectDirection.Up:
                    action = entity => {
                        mag = CheckMagnitudeVar(entity, magVar) ?? mag;
                        var mov = entity.GetComponent<MovementComponent>();
                        if (mov != null) mov.VelocityY = -1 * (mag ?? Math.Abs(mov.VelocityY));
                    };
                    break;

                case MovementEffectDirection.Down:
                    action = entity => {
                        mag = CheckMagnitudeVar(entity, magVar) ?? mag;
                        var mov = entity.GetComponent<MovementComponent>();
                        if (mov != null) mov.VelocityY = (mag ?? Math.Abs(mov.VelocityY));
                    };
                    break;

                case MovementEffectDirection.Left:
                    action = entity => {
                        mag = CheckMagnitudeVar(entity, magVar) ?? mag;
                        var mov = entity.GetComponent<MovementComponent>();
                        if (mov != null) mov.VelocityX = -mag ?? -1 * Math.Abs(mov.VelocityX);
                        if (mov.VelocityX == 0) mov.Direction = Direction.Left;
                    };
                    break;

                case MovementEffectDirection.Right:
                    action = entity => {
                        mag = CheckMagnitudeVar(entity, magVar) ?? mag;
                        var mov = entity.GetComponent<MovementComponent>();
                        if (mov != null) mov.VelocityX = mag ?? Math.Abs(mov.VelocityX);
                        if (mov.VelocityX == 0) mov.Direction = Direction.Right;
                    };
                    break;

                case MovementEffectDirection.Same:
                    action = entity => {
                        mag = CheckMagnitudeVar(entity, magVar) ?? mag;
                        if (mag == null) return;
                        var fmag = mag ?? 0;

                        var mov = entity.GetComponent<MovementComponent>();
                        if (mov == null) return;
                        var dir = mov.Direction;

                        if (axis != Axis.Y) mov.VelocityX = (dir == Direction.Right) ? fmag : ((dir == Direction.Left) ? -fmag : 0);
                        if (axis != Axis.X) mov.VelocityY = (dir == Direction.Down) ? fmag : ((dir == Direction.Up) ? -fmag : 0);
                    };
                    break;

                case MovementEffectDirection.Reverse:
                    action = entity => {
                        mag = CheckMagnitudeVar(entity, magVar) ?? mag;
                        if (mag == null) return;
                        var fmag = mag ?? 0;

                        var mov = entity.GetComponent<MovementComponent>();
                        if (mov == null) return;
                        var dir = mov.Direction;

                        if (axis != Axis.Y)
                        {
                            mov.VelocityX = (dir == Direction.Left) ? fmag : ((dir == Direction.Right) ? -fmag : 0);
                            if (mov.VelocityX == 0) mov.Direction = (dir == Direction.Left) ? Direction.Right : Direction.Left;
                        }

                        if (axis != Axis.X) mov.VelocityY = (dir == Direction.Up) ? fmag : ((dir == Direction.Down) ? -fmag : 0);
                    };
                    break;

                case MovementEffectDirection.Inherit:
                    action = entity => {
                        var mov = entity.GetComponent<MovementComponent>();
                        if (mov == null) return;
                        if (entity.Parent != null)
                        {
                            var dir = entity.Parent.Direction;
                            mag = CheckMagnitudeVar(entity, magVar) ?? mag;

                            if (axis != Axis.Y)
                            {
                                mov.VelocityX = (dir == Direction.Right) ? (mag ?? Math.Abs(mov.VelocityX)) : ((dir == Direction.Left) ? (-mag ?? -1 * Math.Abs(mov.VelocityX)) : 0);
                                if (mov.VelocityX == 0) mov.Direction = dir;
                            }

                            if (axis != Axis.X) mov.VelocityY = (dir == Direction.Down) ? (mag ?? Math.Abs(mov.VelocityY)) : ((dir == Direction.Up) ? (-mag ?? -1 * Math.Abs(mov.VelocityY)) : 0);
                        }
                        else mov.VelocityY = 0;
                    };
                    break;

                case MovementEffectDirection.Input:
                    action = entity => {
                        var mov = entity.GetComponent<MovementComponent>();
                        var input = entity.GetComponent<InputComponent>();
                        if (mov == null || input == null) return;

                        mag = CheckMagnitudeVar(entity, magVar) ?? mag;

                        if (axis != Axis.Y)
                        {
                            if (input.Left) mov.VelocityX = -mag ?? -1 * Math.Abs(mov.VelocityX);
                            else if (input.Right) mov.VelocityX = mag ?? Math.Abs(mov.VelocityX);

                            if (mov.VelocityX == 0 && input.Left) mov.Direction = Direction.Left;
                            else if (mov.VelocityX == 0 && input.Right) mov.Direction = Direction.Right;
                        }
                        if (axis != Axis.X)
                        {
                            if (input.Down) mov.VelocityY = mag ?? Math.Abs(mov.VelocityY);
                            else if (input.Up) mov.VelocityY = -mag ?? -1 * Math.Abs(mov.VelocityY);
                            else mov.VelocityY = 0;
                        }
                    };
                    break;

                case MovementEffectDirection.Player:
                    action = entity => {
                        var mov = entity.GetComponent<MovementComponent>();
                        var pos = entity.GetComponent<PositionComponent>();
                        if (mov == null || pos == null) return;

                        var player = entity.Entities.GetEntityById("Player");

                        if (player == null)
                            return;

                        var playerPos = player.GetComponent<PositionComponent>();
                        mag = CheckMagnitudeVar(entity, magVar) ?? mag;

                        if (axis == Axis.X)
                        {
                            if (pos.X > playerPos.X)
                            {
                                mov.VelocityX = -mag ?? -1 * Math.Abs(mov.VelocityX);
                                if (mov.VelocityX == 0) mov.Direction = Direction.Left;
                            }
                            else if (pos.X < playerPos.X)
                            {
                                mov.VelocityX = mag ?? Math.Abs(mov.VelocityX);
                                if (mov.VelocityX == 0) mov.Direction = Direction.Right;
                            }
                        }
                        else if (axis == Axis.Y)
                        {
                            if (pos.Y > playerPos.Y) mov.VelocityY = -mag ?? -1 * Math.Abs(mov.VelocityY);
                            else if (pos.Y < playerPos.Y) mov.VelocityY = mag ?? Math.Abs(mov.VelocityY);
                        }
                        else
                        {
                            var dx = playerPos.X - pos.X;
                            var dy = playerPos.Y - pos.Y;
                            var hypsq = Math.Pow(dx, 2) + Math.Pow(dy, 2);
                            var hyp = Math.Pow(hypsq, 0.5);

                            mov.VelocityX = (float)(mag * dx / hyp);
                            mov.VelocityY = (float)(mag * dy / hyp);
                        }
                    };
                    break;

                default: action = entity => { }; break;
            }

            return action;
        }

        private static float? CheckMagnitudeVar(IEntity entity, string magVar)
        {
            if (magVar != null)
            {
                var varsComp = entity.GetComponent<VarsComponent>();
                if (varsComp != null)
                {
                    var magStr = varsComp.Get(magVar);
                    if (string.IsNullOrEmpty(magStr))
                        return null;

                    float tmpMag;
                    if (float.TryParse(magStr, out tmpMag))
                        return tmpMag;
                    throw new GameRunException(string.Format("Entity {0} attempted to set movement using local variable {1}, but the value it contained was not a number.", entity.Name, magVar));
                }
            }

            return null;
        }
    }
}
