using System;
using System.Drawing;
using MegaMan.Common;

namespace MegaMan.Engine
{
    public class JoinHandler
    {
        public Join JoinInfo { get; private set; }

        protected int threshXmin, threshXmax;
        protected int threshYmin, threshYmax;
        private readonly int size;
        protected readonly Direction direction;

        public int NextScreenX { get; private set; }
        public int NextScreenY { get; private set; }

        protected float scrollDist;

        public float OffsetX { get; private set; }
        public float OffsetY { get; private set; }
        public float NextOffsetX { get; private set; }
        public float NextOffsetY { get; private set; }

        protected readonly ScreenHandler currentScreen;
        protected ScreenHandler nextScreen;
        private readonly float ticks;
        private float tickdist;

        public string NextScreenName { get; private set; }

        public event Action<JoinHandler, ScreenHandler> ScrollDone;

        public static JoinHandler Create(Join join, ScreenHandler currentScreen)
        {
            if (join.bossDoor)
            {
                GameEntity door = GameEntity.Get(join.bossEntityName);
                if (door != null)
                {
                    return new BossDoorHandler(door, join, currentScreen);
                }
            }
            return new JoinHandler(join, currentScreen);
        }
        
        protected JoinHandler(Join join, ScreenHandler currentScreen)
        {
            JoinInfo = join;
            this.currentScreen = currentScreen;

            size = join.Size * currentScreen.Screen.Tileset.TileSize;
            ticks = ((JoinInfo.type == JoinType.Vertical) ? Game.CurrentGame.PixelsAcross : Game.CurrentGame.PixelsDown) / Const.ScrollSpeed;

            if (join.screenOne == currentScreen.Screen.Name)
            {
                NextScreenName = join.screenTwo;
                if (join.type == JoinType.Horizontal)   // bottom edge
                {
                    direction = Direction.Down;
                    threshXmin = join.offsetOne * currentScreen.Screen.Tileset.TileSize;
                    threshYmin = currentScreen.Screen.PixelHeight - Const.PlayerScrollTrigger;
                    threshXmax = threshXmin + size;
                    threshYmax = currentScreen.Screen.PixelHeight + 100;
                    NextScreenX = (join.offsetTwo - join.offsetOne) * currentScreen.Screen.Tileset.TileSize;
                }
                else // right edge
                {
                    direction = Direction.Right;
                    threshXmin = currentScreen.Screen.PixelWidth - Const.PlayerScrollTrigger;
                    threshYmin = join.offsetOne * currentScreen.Screen.Tileset.TileSize;
                    threshXmax = currentScreen.Screen.PixelWidth + 100;
                    threshYmax = threshYmin + size;
                    NextScreenY = (join.offsetTwo - join.offsetOne) * currentScreen.Screen.Tileset.TileSize;
                }
            }
            else
            {
                NextScreenName = join.screenOne;
                if (join.type == JoinType.Horizontal)   // top edge
                {
                    direction = Direction.Up;
                    threshXmin = join.offsetTwo * currentScreen.Screen.Tileset.TileSize;
                    threshYmin = -100;
                    threshXmax = threshXmin + size;
                    threshYmax = Const.PlayerScrollTrigger;
                    NextScreenX = (join.offsetOne - join.offsetTwo) * currentScreen.Screen.Tileset.TileSize;
                }
                else // left edge
                {
                    direction = Direction.Left;
                    threshXmin = -100;
                    threshYmin = join.offsetTwo * currentScreen.Screen.Tileset.TileSize;
                    threshXmax = Const.PlayerScrollTrigger;
                    threshYmax = threshYmin + size;
                    NextScreenY = (join.offsetOne - join.offsetTwo) * currentScreen.Screen.Tileset.TileSize;
                }
            }
        }

        public virtual void Start(ScreenHandler screen)
        {
        }

        public virtual void Stop()
        {
        }

        public virtual bool Trigger(PointF position)
        {
            if (direction == Direction.Right || direction == Direction.Down)
            {
                if (JoinInfo.direction == JoinDirection.BackwardOnly) return false;
            }
            else if (JoinInfo.direction == JoinDirection.ForwardOnly) return false;

            return (position.X >= threshXmin && position.X <= threshXmax &&
                    position.Y >= threshYmin && position.Y <= threshYmax);
        }

        public virtual void BeginScroll(ScreenHandler next, PointF playerPos)
        {
            tickdist = (TriggerSize() + OffsetDist()) / ticks;
            nextScreen = next;
            if (direction == Direction.Down) NextScreenY = -currentScreen.Screen.PixelHeight;
            else if (direction == Direction.Right) NextScreenX = -currentScreen.Screen.PixelWidth;
            else if (direction == Direction.Left) NextScreenX = next.Screen.PixelWidth;
            else if (direction == Direction.Up) NextScreenY = next.Screen.PixelHeight;
        }

        protected virtual int TriggerSize()
        {
            return Const.PlayerScrollTrigger;
        }

        protected virtual int OffsetDist()
        {
            return Const.PlayerScrollOffset;
        }

        protected void MovePlayer(PositionComponent playerPos)
        {
            if (direction == Direction.Right) playerPos.SetPosition(new PointF(playerPos.Position.X + tickdist, playerPos.Position.Y));
            else if (direction == Direction.Left) playerPos.SetPosition(new PointF(playerPos.Position.X - tickdist, playerPos.Position.Y));
            else if (direction == Direction.Down) playerPos.SetPosition(new PointF(playerPos.Position.X, playerPos.Position.Y + tickdist));
            else if (direction == Direction.Up) playerPos.SetPosition(new PointF(playerPos.Position.X, playerPos.Position.Y - tickdist));
        }

        protected void FinalizePlayerPos(PositionComponent playerPos)
        {
            if (direction == Direction.Right) { playerPos.SetPosition(new PointF(OffsetDist(), playerPos.Position.Y + NextScreenY)); }
            else if (direction == Direction.Left) { playerPos.SetPosition(new PointF(nextScreen.Screen.PixelWidth - OffsetDist(), playerPos.Position.Y + NextScreenY)); }
            else if (direction == Direction.Down) { playerPos.SetPosition(new PointF(playerPos.Position.X + NextScreenX, OffsetDist())); }
            else if (direction == Direction.Up) { playerPos.SetPosition(new PointF(playerPos.Position.X + NextScreenX, nextScreen.Screen.PixelHeight - OffsetDist())); }
        }

        public virtual void Update(PositionComponent playerPos)
        {
            scrollDist += Const.ScrollSpeed;
            if (JoinInfo.type == JoinType.Vertical && scrollDist >= Game.CurrentGame.PixelsAcross ||
                JoinInfo.type == JoinType.Horizontal && scrollDist >= Game.CurrentGame.PixelsDown)
            {
                FinalizePlayerPos(playerPos);
                Finish();
            }
            else
            {
                MovePlayer(playerPos);
            }

            Calculate();
        }

        protected void Finish()
        {
            if (ScrollDone != null) ScrollDone(this, nextScreen);
        }

        private void Calculate()
        {
            NextOffsetX = 0;
            NextOffsetY = 0;

            if (direction == Direction.Right)
            {
                OffsetX = scrollDist;
                NextOffsetX = scrollDist - Game.CurrentGame.PixelsAcross;
                NextOffsetY += OffsetY;
            }
            else if (direction == Direction.Left)
            {
                OffsetX = -scrollDist;
                NextOffsetX = Game.CurrentGame.PixelsAcross - scrollDist;
                NextOffsetY += OffsetY;
            }
            else if (direction == Direction.Down)
            {
                OffsetY = scrollDist;
                NextOffsetY = scrollDist - Game.CurrentGame.PixelsDown;
                NextOffsetX += OffsetX;
            }
            else if (direction == Direction.Up)
            {
                OffsetY = -scrollDist;
                NextOffsetY = Game.CurrentGame.PixelsDown - scrollDist;
                NextOffsetX += OffsetX;
            }

            
        }
    }
}
