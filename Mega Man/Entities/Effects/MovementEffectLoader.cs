using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            if (movement.Flying.HasValue)
            {
                bool f = movement.Flying.Value;
                action += entity => {
                    MovementComponent mov = entity.GetComponent<MovementComponent>();
                    if (mov != null) mov.Flying = f;
                };
            }

            if (movement.FlipSprite.HasValue)
            {
                var flip = movement.FlipSprite.Value;
                action += entity => {
                    MovementComponent mov = entity.GetComponent<MovementComponent>();
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

            float? mag = info.Magnitude;

            if (mag != 0)
            {
                switch (info.Direction)
                {
                    case MovementEffectDirection.Up:
                        action = entity => {
                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            if (mov != null) mov.VelocityY = -1 * (mag ?? Math.Abs(mov.VelocityY));
                        };
                        break;

                    case MovementEffectDirection.Down:
                        action = entity => {
                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            if (mov != null) mov.VelocityY = (mag ?? Math.Abs(mov.VelocityY));
                        };
                        break;

                    case MovementEffectDirection.Left:
                        action = entity => {
                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            if (mov != null) mov.VelocityX = -mag ?? -1 * Math.Abs(mov.VelocityX);
                        };
                        break;

                    case MovementEffectDirection.Right:
                        action = entity => {
                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            if (mov != null) mov.VelocityX = mag ?? Math.Abs(mov.VelocityX);
                        };
                        break;

                    case MovementEffectDirection.Same:
                        action = entity => {
                            if (mag == null) return;
                            float fmag = mag ?? 0;

                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            if (mov == null) return;
                            Direction dir = mov.Direction;

                            if (axis != Axis.Y) mov.VelocityX = (dir == Direction.Right) ? fmag : ((dir == Direction.Left) ? -fmag : 0);
                            if (axis != Axis.X) mov.VelocityY = (dir == Direction.Down) ? fmag : ((dir == Direction.Up) ? -fmag : 0);
                        };
                        break;

                    case MovementEffectDirection.Reverse:
                        action = entity => {
                            if (mag == null) return;
                            float fmag = mag ?? 0;

                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            if (mov == null) return;
                            Direction dir = mov.Direction;

                            if (axis != Axis.Y) mov.VelocityX = (dir == Direction.Left) ? fmag : ((dir == Direction.Right) ? -fmag : 0);
                            if (axis != Axis.X) mov.VelocityY = (dir == Direction.Up) ? fmag : ((dir == Direction.Down) ? -fmag : 0);
                        };
                        break;

                    case MovementEffectDirection.Inherit:
                        action = entity => {
                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            if (mov == null) return;
                            if (entity.Parent != null)
                            {
                                Direction dir = entity.Parent.Direction;

                                if (axis != Axis.Y) mov.VelocityX = (dir == Direction.Right) ? (mag ?? Math.Abs(mov.VelocityX)) : ((dir == Direction.Left) ? (-mag ?? -1 * Math.Abs(mov.VelocityX)) : 0);
                                if (axis != Axis.X) mov.VelocityY = (dir == Direction.Down) ? (mag ?? Math.Abs(mov.VelocityY)) : ((dir == Direction.Up) ? (-mag ?? -1 * Math.Abs(mov.VelocityY)) : 0);
                            }
                            else mov.VelocityY = 0;
                        };
                        break;

                    case MovementEffectDirection.Input:
                        action = entity => {
                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            InputComponent input = entity.GetComponent<InputComponent>();
                            if (mov == null || input == null) return;

                            if (axis != Axis.Y)
                            {
                                if (input.Left) mov.VelocityX = -mag ?? -1 * Math.Abs(mov.VelocityX);
                                else if (input.Right) mov.VelocityX = mag ?? Math.Abs(mov.VelocityX);
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
                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            PositionComponent pos = entity.GetComponent<PositionComponent>();
                            if (mov == null || pos == null) return;

                            GameEntity player = entity.Entities.GetEntityById("Player");

                            if (player == null)
                                return;

                            PositionComponent playerPos = player.GetComponent<PositionComponent>();

                            if (axis == Axis.X)
                            {
                                if (pos.Position.X > playerPos.Position.X) mov.VelocityX = -mag ?? -1 * Math.Abs(mov.VelocityX);
                                else if (pos.Position.X < playerPos.Position.X) mov.VelocityX = mag ?? Math.Abs(mov.VelocityX);
                            }
                            else if (axis == Axis.Y)
                            {
                                if (pos.Position.Y > playerPos.Position.Y) mov.VelocityY = -mag ?? -1 * Math.Abs(mov.VelocityY);
                                else if (pos.Position.Y < playerPos.Position.Y) mov.VelocityY = mag ?? Math.Abs(mov.VelocityY);
                            }
                            else
                            {
                                float dx = playerPos.Position.X - pos.Position.X;
                                float dy = playerPos.Position.Y - pos.Position.Y;
                                double hyp = Math.Pow(dx, 2) + Math.Pow(dy, 2);
                                hyp = Math.Pow(hyp, 0.5);

                                mov.VelocityX = (float)(mag * dx / hyp);
                                mov.VelocityY = (float)(mag * dy / hyp);
                            }
                        };
                        break;

                    default: action = new Effect(entity => { }); break;
                }
            }
            else
            {
                if (axis == Axis.X)
                    action = entity => {
                        MovementComponent mov = entity.GetComponent<MovementComponent>();
                        if (mov != null) mov.VelocityX = 0;
                    };
                else
                    action = entity => {
                        MovementComponent mov = entity.GetComponent<MovementComponent>();
                        if (mov != null) mov.VelocityY = 0;
                    };
            }

            return action;
        }
    }
}
