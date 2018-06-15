using System;
using System.Diagnostics;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Geometry;
using MegaMan.Engine.Entities.Effects;

namespace MegaMan.Engine
{
    [DebuggerDisplay("Parent = {Parent.Name}, vx = {vx}, vy = {vy}")]
    public class MovementComponent : Component
    {
        private float resistX, resistY, dragX, dragY;
        private float pushX, pushY;

        private float vx, vy, pendingVx, pendingVy;
        public bool Floating { get; set; }
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
            get
            {
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
            MovementComponent newone = new MovementComponent { Floating = Floating, CanMove = CanMove};

            return newone;
        }

        public override void Start(IGameplayContainer container)
        {
            if (Direction == Direction.Unknown)
            {
                Direction = Direction.Right;
            }

            container.GameAct += Update;

            pushX = pushY = 0;
            dragX = dragY = resistX = resistY = 1;

            if (Parent.Screen != null && position != null)
            {
                overTile = Parent.Screen.TileAt(position.Position.X, position.Position.Y);
            }
        }

        public override void Stop(IGameplayContainer container)
        {
            container.GameAct -= Update;
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

            if (!Floating)
            {
                float gmult = (overTile != null) ? overTile.Properties.GravityMult : 1;

                if (Parent.Container.IsGravityFlipped)
                {
                    vy -= Parent.Container.Gravity * gmult;
                    if (vy < -Const.TerminalVel) vy = -Const.TerminalVel;
                }
                else
                {
                    vy += Parent.Container.Gravity * gmult;
                    if (vy > Const.TerminalVel) vy = Const.TerminalVel;
                }
            }

            if (FlipSprite)
            {
                SpriteComponent sprite = Parent.GetComponent<SpriteComponent>();
                if (sprite != null) sprite.HorizontalFlip = (Direction == Direction.Left);
            }

            Tile nextOverTile = null;

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

                nextOverTile = Parent.Screen.TileAt(position.Position.X, position.Position.Y);
            }

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

        public void LoadInfo(MovementComponentInfo info)
        {
            var loader = new MovementEffectLoader();
            var effect = loader.Load(info.EffectInfo);
            effect(Parent);
        }
    }
}
