using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Linq;

namespace Mega_Man
{
    [System.Diagnostics.DebuggerDisplay("Parent = {Parent.Name}, vx = {vx}, vy = {vy}")]
    public class MovementComponent : Component, IMovement
    {
        private float resistMultX, resistMultY, resistConstX, resistConstY;
        private float pushMultX, pushMultY, pushConstX, pushConstY;

        private float vx, vy, pendingVx, pendingVy;
        private bool newVx, newVy;
        public int ID;
        public bool Flying { get; set; }
        public float VelocityX
        {
            get { return vx; }
            set
            {
                pendingVx = value;
                newVx = true;
                if (pendingVx < 0) this.Direction = Direction.Left;
                else if (pendingVx > 0) this.Direction = Direction.Right;
            }
        }
        public float VelocityY
        {
            get { return vy; }
            set
            {
                pendingVy = value;
                newVy = true;
            }
        }
        public bool FlipSprite { get; set; }

        public bool CanMove { get; set; }
        public Direction Direction { get; private set; }

        private PositionComponent position;
        private CollisionComponent collision;

        public MovementComponent()
        {
            Direction = Direction.Right;
            Random rand = new Random();
            ID = rand.Next();
            CanMove = true;
        }

        public override Component Clone()
        {
            MovementComponent newone = new MovementComponent();
            newone.Flying = this.Flying;
            newone.CanMove = this.CanMove;

            return newone;
        }

        public override void Start()
        {
            Engine.Instance.GameThink += Think;
            Engine.Instance.GameAct += Update;

            resistConstX = resistConstY = pushConstX = pushConstY = 0;
            resistMultX = resistMultY = pushMultX = pushMultY = 1;
        }

        public override void Stop()
        {
            Engine.Instance.GameThink -= Think;
            Engine.Instance.GameAct -= Update;
        }

        public override void Message(IGameMessage msg)
        {
        }

        private void Think()
        {
            // modify my CURRENT velocity
            // not the pending changes
            vx = vx * resistMultX + resistConstX;
            vy = vy * resistMultY + resistConstY;

            resistConstX = resistConstY = 0;
            resistMultX = resistMultY = 1;
        }

        protected override void Update()
        {
            if (!CanMove || Parent.Paused) return;

            // set to the velocity i was thinking about
            // but only if there is something new --
            // don't want to overwrite a push effect
            if (newVx) vx = pendingVx;
            if (newVy) vy = pendingVy;
            newVx = newVy = false;

            vx = vx * pushMultX + pushConstX;
            vy = vy * pushMultY + pushConstY;

            pushConstX = pushConstY = 0;
            pushMultX = pushMultY = 1;

            if (!Flying)
            {
                int ts = this.Parent.Screen.Screen.Tileset.TileSize;
                int tx = (int)(this.position.Position.X / ts);
                int ty = (int)(this.position.Position.Y / ts);
                MegaMan.Tile tile = this.Parent.Screen.Screen.TileAt(tx, ty);
                float gmult = (tile != null)? tile.Properties.GravityMult : 1;
                if (Game.CurrentGame.GravityFlip)
                {
                    vy -= Game.CurrentGame.Gravity * gmult;
                    if (vy < -Const.TerminalVel) vy = -Const.TerminalVel;
                }
                else
                {
                    vy += Game.CurrentGame.Gravity * gmult;
                    if (vy > Const.TerminalVel) vy = Const.TerminalVel;
                }
            }

            if (FlipSprite)
            {
                SpriteComponent sprite = (SpriteComponent)Parent.GetComponent(typeof(SpriteComponent));
                if (sprite != null) sprite.HorizontalFlip = (this.Direction == Direction.Left);
            }

            if (position != null)
            {
                PointF pos = position.Position;
                if (collision == null)
                {
                    pos.X += vx;
                    pos.Y += vy;
                }
                else
                {
                    if ((!collision.BlockLeft && vx < 0) || (!collision.BlockRight && vx > 0)) pos.X += vx;
                    if ((!collision.BlockTop && vy < 0) || (!collision.BlockBottom && vy > 0)) pos.Y += vy;
                }
                position.SetPosition(pos);
            }
        }

        public override void RegisterDependencies(Component component)
        {
            if (component is PositionComponent) position = component as PositionComponent;
            else if (component is CollisionComponent) collision = component as CollisionComponent;
        }

        public void PushX(float mult, float add)
        {
            if (mult > pushMultX) pushMultX = mult;
            if (Math.Abs(add) > Math.Abs(pushConstX)) pushConstX = add;
        }

        public void PushY(float mult, float add)
        {
            if (mult > pushMultY) pushMultY = mult;
            if (Math.Abs(add) > Math.Abs(pushConstY)) pushConstY = add;
        }

        public void ResistX(float mult, float add)
        {
            if (mult < resistMultX) resistMultX = mult;
            if (Math.Abs(add) > Math.Abs(resistConstY)) resistConstX = add;
        }

        public void ResistY(float mult, float add)
        {
            if (mult < resistMultY) resistMultY = mult;
            if (Math.Abs(add) > Math.Abs(resistConstY)) resistConstY = add;
        }

        public override void LoadXml(XElement xmlNode)
        {
            // for now, just get the effect and then execute it on myself
            // later, refactor so it's the other way around
            ParseEffect(xmlNode)(Parent);
        }

        public override Effect ParseEffect(XElement child)
        {
            Effect action = new Effect((entity) => { });
            foreach (XElement prop in child.Elements())
            {
                switch (prop.Name.LocalName)
                {
                    case "Flying":
                        bool f;
                        if (!bool.TryParse(prop.Value, out f)) throw new EntityXmlException(prop, "Flying tag must be a valid bool (true or false).");
                        action += (entity) =>
                        {
                            MovementComponent mov = (MovementComponent)entity.GetComponent(typeof(MovementComponent));
                            if (mov != null) mov.Flying = f;
                        };
                        break;

                    case "FlipSprite":
                        bool flip;
                        if (!bool.TryParse(prop.Value, out flip)) throw new EntityXmlException(prop, "FlipSprite tag must be a valid bool (true or false).");
                        action += (entity) =>
                        {
                            MovementComponent mov = (MovementComponent)entity.GetComponent(typeof(MovementComponent));
                            if (mov != null) mov.FlipSprite = flip;
                        };
                        break;

                    case "X":
                        action += ParseMovementBehavior(prop, Axis.X);
                        break;

                    case "Y":
                        action += ParseMovementBehavior(prop, Axis.Y);
                        break;

                    case "Velocity":
                        action += ParseMovementBehavior(prop, Axis.Both);
                        break;
                }
            }
            return action;
        }

        private static Effect ParseMovementBehavior(XElement prop, Axis axis)
        {
            Effect action;

            XAttribute magAttr = prop.Attribute("magnitude");
            float? mag = null;
            if (magAttr != null)
            {
                float pmag;
                if (!float.TryParse(magAttr.Value, out pmag)) throw new EntityXmlException(magAttr, "Movement magnitude must be a number.");
                mag = pmag;
            }

            if (mag != 0)
            {
                XAttribute dirattr = prop.Attribute("direction");
                string direction;
                if (dirattr == null) direction = "Same";
                else direction = dirattr.Value;
                switch (direction)
                {
                    case "Up":
                        action = (entity) =>
                        {
                            MovementComponent mov = (MovementComponent)entity.GetComponent(typeof(MovementComponent));
                            if (mov != null) mov.VelocityY = -1 * (mag?? Math.Abs(mov.VelocityY));
                        };
                        break;

                    case "Down":
                        action = (entity) =>
                        {
                            MovementComponent mov = (MovementComponent)entity.GetComponent(typeof(MovementComponent));
                            if (mov != null) mov.VelocityY = (mag?? Math.Abs(mov.VelocityY));
                        };
                        break;

                    case "Left":
                        action = (entity) =>
                        {
                            MovementComponent mov = (MovementComponent)entity.GetComponent(typeof(MovementComponent));
                            if (mov != null) mov.VelocityX = -mag?? -1 * Math.Abs(mov.VelocityX);
                        };
                        break;

                    case "Right":
                        action = (entity) =>
                        {
                            MovementComponent mov = (MovementComponent)entity.GetComponent(typeof(MovementComponent));
                            if (mov != null) mov.VelocityX = mag?? Math.Abs(mov.VelocityX);
                        };
                        break;

                    case "Same":
                        action = (entity) =>
                        {
                            if (mag == null) return;
                            float fmag = mag ?? 0;

                            MovementComponent mov = (MovementComponent)entity.GetComponent(typeof(MovementComponent));
                            if (mov == null) return;
                            Direction dir = mov.Direction;

                            if (axis != Axis.Y) mov.VelocityX = (dir == Direction.Right) ? fmag : ((dir == Direction.Left) ? -fmag : 0);
                            if (axis != Axis.X) mov.VelocityY = (dir == Direction.Down) ? fmag : ((dir == Direction.Up) ? -fmag : 0);
                        };
                        break;

                    case "Reverse":
                        action = (entity) =>
                        {
                            if (mag == null) return;
                            float fmag = mag ?? 0;

                            MovementComponent mov = (MovementComponent)entity.GetComponent(typeof(MovementComponent));
                            if (mov == null) return;
                            Direction dir = mov.Direction;

                            if (axis != Axis.Y) mov.VelocityX = (dir == Direction.Left) ? fmag : ((dir == Direction.Right) ? -fmag : 0);
                            if (axis != Axis.X) mov.VelocityY = (dir == Direction.Up) ? fmag : ((dir == Direction.Down) ? -fmag : 0);
                        };
                        break;

                    case "Inherit":
                        action = (entity) =>
                        {
                            MovementComponent mov = (MovementComponent)entity.GetComponent(typeof(MovementComponent));
                            if (mov == null) return;
                            if (entity.Parent != null)
                            {
                                Direction dir = entity.Parent.Direction;

                                if (axis != Axis.Y) mov.VelocityX = (dir == Direction.Right) ? (mag?? Math.Abs(mov.VelocityX)) : ((dir == Direction.Left) ? (-mag?? -1 * Math.Abs(mov.VelocityX)) : 0);
                                if (axis != Axis.X) mov.VelocityY = (dir == Direction.Down) ? (mag?? Math.Abs(mov.VelocityY)) : ((dir == Direction.Up) ? (-mag?? -1 * Math.Abs(mov.VelocityY)) : 0);
                            }
                            else mov.VelocityY = 0;
                        };
                        break;

                    case "Input":
                        action = (entity) =>
                        {
                            MovementComponent mov = (MovementComponent)entity.GetComponent(typeof(MovementComponent));
                            InputComponent input = (InputComponent)entity.GetComponent(typeof(InputComponent));
                            if (mov == null || input == null) return;

                            if (axis != Axis.Y)
                            {
                                if (input.Left) mov.VelocityX = -mag?? -1 * Math.Abs(mov.VelocityX);
                                else if (input.Right) mov.VelocityX = mag?? Math.Abs(mov.VelocityX);
                            }
                            if (axis != Axis.X)
                            {
                                if (input.Down) mov.VelocityY = mag?? Math.Abs(mov.VelocityY);
                                else if (input.Up) mov.VelocityY = -mag?? -1 * Math.Abs(mov.VelocityY);
                                else mov.VelocityY = 0;
                            }
                        };
                        break;

                    case "Player":
                        action = (entity) =>
                        {
                            MovementComponent mov = (MovementComponent)entity.GetComponent(typeof(MovementComponent));
                            PositionComponent pos = (PositionComponent)entity.GetComponent(typeof(PositionComponent));
                            if (mov == null || pos == null) return;

                            GameEntity player = Game.CurrentGame.CurrentMap.Player;
                            PositionComponent playerPos = (PositionComponent)player.GetComponent(typeof(PositionComponent));

                            if (axis == Axis.X)
                            {
                                if (pos.Position.X > playerPos.Position.X) mov.VelocityX = -mag?? -1 * Math.Abs(mov.VelocityX);
                                else if (pos.Position.X < playerPos.Position.X) mov.VelocityX = mag?? Math.Abs(mov.VelocityX);
                            }
                            else if (axis == Axis.Y)
                            {
                                if (pos.Position.Y > playerPos.Position.Y) mov.VelocityY = -mag?? -1 * Math.Abs(mov.VelocityY);
                                else if (pos.Position.Y < playerPos.Position.Y) mov.VelocityY = mag?? Math.Abs(mov.VelocityY);
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

                    default: action = new Effect((entity) => { }); break;
                }
            }
            else
            {
                if (axis == Axis.X) action = (entity) =>
                {
                    MovementComponent mov = (MovementComponent)entity.GetComponent(typeof(MovementComponent));
                    if (mov != null) mov.VelocityX = 0;
                };
                else action = (entity) =>
                {
                    MovementComponent mov = (MovementComponent)entity.GetComponent(typeof(MovementComponent));
                    if (mov != null) mov.VelocityY = 0;
                };
            }

            return action;
        }
    }
}
