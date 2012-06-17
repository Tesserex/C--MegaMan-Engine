using System;
using System.Linq;
using System.Drawing;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.Engine
{
    [System.Diagnostics.DebuggerDisplay("Parent = {Parent.Name}, vx = {vx}, vy = {vy}")]
    public class MovementComponent : Component
    {
        private float resistX, resistY, dragX, dragY;
        private float pushX, pushY;

        private float vx, vy, pendingVx, pendingVy;
        public bool Flying { get; set; }
        public float VelocityX
        {
            get { return vx; }
            set
            {
                pendingVx = value;

                if (pendingVx < 0) Direction = Direction.Left;
                else if (pendingVx > 0) Direction = Direction.Right;
            }
        }
        public float VelocityY
        {
            get { return vy; }
            set
            {
                pendingVy = value;
            }
        }
        public bool FlipSprite { get; set; }

        public bool CanMove { get; set; }
        public Direction Direction { get; set; }

        private PositionComponent position;
        private CollisionComponent collision;

        private Tile overTile;

        public string TileType
        {
            get {
                if (overTile != null) return overTile.Properties.Name;
                return "";
            }
        }

        public MovementComponent()
        {
            CanMove = true;
        }

        public override Component Clone()
        {
            MovementComponent newone = new MovementComponent {Flying = this.Flying, CanMove = this.CanMove};

            return newone;
        }

        public override void Start()
        {
            if (Direction == Direction.Unknown)
            {
                Direction = Direction.Right;
            }

            Parent.Container.GameAct += Update;

            pushX = pushY = 0;
            dragX = dragY = resistX = resistY = 1;

            if (Parent.Screen != null)
            {
                overTile = Parent.Screen.TileAt(position.Position.X, position.Position.Y);
            }
        }

        public override void Stop()
        {
            Parent.Container.GameAct -= Update;
        }

        public override void Message(IGameMessage msg)
        {
        }

        protected override void Update()
        {
            if (!CanMove || Parent.Paused) return;

            float accelX = (pendingVx - vx) * dragX;
            vx += accelX;

            float accelY = (pendingVy - vy) * dragY;
            vy += accelY;

            if (!Flying)
            {
                float gmult = (overTile != null) ? overTile.Properties.GravityMult : 1;
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
                SpriteComponent sprite = Parent.GetComponent<SpriteComponent>();
                if (sprite != null) sprite.HorizontalFlip = (Direction == Direction.Left);
            }

            if (position != null)
            {
                float deltaX = vx + pushX;
                float deltaY = vy + pushY;

                PointF pos = position.Position;
                if (collision == null)
                {
                    pos.X += deltaX;
                    pos.Y += deltaY;
                }
                else
                {
                    if ((!collision.BlockLeft && deltaX < 0) || (!collision.BlockRight && deltaX > 0)) pos.X += deltaX;
                    if ((!collision.BlockTop && deltaY < 0) || (!collision.BlockBottom && deltaY > 0)) pos.Y += deltaY;
                }
                position.SetPosition(pos);
            }

            Tile nextOverTile = Parent.Screen.TileAt(position.Position.X, position.Position.Y);

            if (Parent.Name == "Player")
            {
                if (overTile != null && nextOverTile != null && nextOverTile.Properties.Name != overTile.Properties.Name)
                {
                    if (overTile.Properties.OnLeave != null) EffectParser.GetLateBoundEffect(overTile.Properties.OnLeave)(Parent);
                    if (nextOverTile.Properties.OnEnter != null) EffectParser.GetLateBoundEffect(nextOverTile.Properties.OnEnter)(Parent);
                }

                if (nextOverTile != null && nextOverTile.Properties.OnOver != null)
                {
                    EffectParser.GetLateBoundEffect(nextOverTile.Properties.OnOver)(Parent);
                }
            }

            vx *= resistX;
            vy *= resistY;
            resistX = resistY = 1;

            pendingVx = vx;
            pendingVy = vy;

            overTile = nextOverTile;
            pushX = pushY = 0;
            dragX = dragY = 1;
        }

        public override void RegisterDependencies(Component component)
        {
            if (component is PositionComponent) position = component as PositionComponent;
            else if (component is CollisionComponent) collision = component as CollisionComponent;
        }

        public void PushX(float add)
        {
            if (Math.Sign(add) == Math.Sign(pushX))
            {
                // take highest magnitude
                pushX = Math.Max(Math.Abs(pushX), Math.Abs(add)) * Math.Sign(pushX);
            }
            else
            {
                pushX += add;
            }
        }

        public void PushY(float add)
        {
            if (Math.Sign(add) == Math.Sign(pushY))
            {
                // take highest magnitude
                pushY = Math.Max(Math.Abs(pushY), Math.Abs(add)) * Math.Sign(pushY);
            }
            else
            {
                pushY += add;
            }
        }

        public void ResistX(float mult)
        {
            if (mult < resistX) resistX = mult;
        }

        public void ResistY(float mult)
        {
            if (mult < resistY) resistY = mult;
        }

        public void DragX(float mult)
        {
            if (mult < dragX) dragX = mult;
        }

        public void DragY(float mult)
        {
            if (mult < dragY) dragY = mult;
        }

        // these next two functions exist for the sake of xml expressions
        public void SetX(double x)
        {
            VelocityX = (float)x;
        }

        public void SetY(double y)
        {
            VelocityY = (float)y;
        }

        public override void LoadXml(XElement xmlNode)
        {
            // for now, just get the effect and then execute it on myself
            // later, refactor so it's the other way around
            ParseEffect(xmlNode)(Parent);
        }

        public static Effect ParseEffect(XElement child)
        {
            Effect action = entity => { };
            foreach (XElement prop in child.Elements())
            {
                switch (prop.Name.LocalName)
                {
                    case "Flying":
                        bool f = prop.GetBool();
                        action += entity =>
                        {
                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            if (mov != null) mov.Flying = f;
                        };
                        break;

                    case "FlipSprite":
                        bool flip = prop.GetBool();
                        action += entity =>
                        {
                            MovementComponent mov = entity.GetComponent<MovementComponent>();
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
                if (!magAttr.Value.TryParse(out pmag)) throw new GameXmlException(magAttr, "Movement magnitude must be a number.");
                mag = pmag;
            }

            if (mag != 0)
            {
                XAttribute dirattr = prop.Attribute("direction");
                string direction;
                direction = dirattr == null ? "Same" : dirattr.Value;
                switch (direction)
                {
                    case "Up":
                        action = entity =>
                        {
                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            if (mov != null) mov.VelocityY = -1 * (mag?? Math.Abs(mov.VelocityY));
                        };
                        break;

                    case "Down":
                        action = entity =>
                        {
                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            if (mov != null) mov.VelocityY = (mag?? Math.Abs(mov.VelocityY));
                        };
                        break;

                    case "Left":
                        action = entity =>
                        {
                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            if (mov != null) mov.VelocityX = -mag?? -1 * Math.Abs(mov.VelocityX);
                        };
                        break;

                    case "Right":
                        action = entity =>
                        {
                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            if (mov != null) mov.VelocityX = mag?? Math.Abs(mov.VelocityX);
                        };
                        break;

                    case "Same":
                        action = entity =>
                        {
                            if (mag == null) return;
                            float fmag = mag ?? 0;

                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            if (mov == null) return;
                            Direction dir = mov.Direction;

                            if (axis != Axis.Y) mov.VelocityX = (dir == Direction.Right) ? fmag : ((dir == Direction.Left) ? -fmag : 0);
                            if (axis != Axis.X) mov.VelocityY = (dir == Direction.Down) ? fmag : ((dir == Direction.Up) ? -fmag : 0);
                        };
                        break;

                    case "Reverse":
                        action = entity =>
                        {
                            if (mag == null) return;
                            float fmag = mag ?? 0;

                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            if (mov == null) return;
                            Direction dir = mov.Direction;

                            if (axis != Axis.Y) mov.VelocityX = (dir == Direction.Left) ? fmag : ((dir == Direction.Right) ? -fmag : 0);
                            if (axis != Axis.X) mov.VelocityY = (dir == Direction.Up) ? fmag : ((dir == Direction.Down) ? -fmag : 0);
                        };
                        break;

                    case "Inherit":
                        action = entity =>
                        {
                            MovementComponent mov = entity.GetComponent<MovementComponent>();
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
                        action = entity =>
                        {
                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            InputComponent input = entity.GetComponent<InputComponent>();
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
                        action = entity =>
                        {
                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            PositionComponent pos = entity.GetComponent<PositionComponent>();
                            if (mov == null || pos == null) return;

                            GameEntity player = entity.Screen.GetEntities("Player").Single();
                            PositionComponent playerPos = player.GetComponent<PositionComponent>();

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

                    default: action = new Effect(entity => { }); break;
                }
            }
            else
            {
                if (axis == Axis.X) action = entity =>
                {
                    MovementComponent mov = entity.GetComponent<MovementComponent>();
                    if (mov != null) mov.VelocityX = 0;
                };
                else action = entity =>
                {
                    MovementComponent mov = entity.GetComponent<MovementComponent>();
                    if (mov != null) mov.VelocityY = 0;
                };
            }

            return action;
        }
    }
}
