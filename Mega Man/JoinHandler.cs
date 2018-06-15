using System;
using MegaMan.Common;
using MegaMan.Common.Geometry;

namespace MegaMan.Engine
{
    public class JoinHandler
    {
        public Join JoinInfo { get; private set; }

        private int threshXmin, threshXmax;
        private int threshYmin, threshYmax;
        private readonly int size;
        protected readonly Direction direction;

        protected int width, height, nextWidth, nextHeight, tileSize;

        private readonly float ticks;
        private float tickdist;

        protected float scrollDist;

        public float OffsetX { get; private set; }
        public float OffsetY { get; private set; }
        public float NextOffsetX { get; private set; }
        public float NextOffsetY { get; private set; }
        public int NextScreenX { get; private set; }
        public int NextScreenY { get; private set; }
        public string NextScreenName { get; private set; }

        public event Action<JoinHandler> ScrollDone;

        public JoinHandler(Join join, int tileSize, int height, int width, string name)
        {
            JoinInfo = join;

            this.height = height;
            this.width = width;
            this.tileSize = tileSize;

            size = join.Size * tileSize;
            ticks = ((JoinInfo.type == JoinType.Vertical) ? Game.CurrentGame.PixelsAcross : Game.CurrentGame.PixelsDown) / Const.ScrollSpeed;

            if (join.screenOne == name)
            {
                NextScreenName = join.screenTwo;
                if (join.type == JoinType.Horizontal)   // bottom edge
                {
                    direction = Direction.Down;
                    threshXmin = join.offsetOne * tileSize;
                    threshYmin = height - Const.PlayerScrollTrigger;
                    threshXmax = threshXmin + size;
                    threshYmax = height + 100;
                    NextScreenX = (join.offsetTwo - join.offsetOne) * tileSize;
                }
                else // right edge
                {
                    direction = Direction.Right;
                    threshXmin = width - Const.PlayerScrollTrigger;
                    threshYmin = join.offsetOne * tileSize;
                    threshXmax = width + 100;
                    threshYmax = threshYmin + size;
                    NextScreenY = (join.offsetTwo - join.offsetOne) * tileSize;
                }
            }
            else
            {
                NextScreenName = join.screenOne;
                if (join.type == JoinType.Horizontal)   // top edge
                {
                    direction = Direction.Up;
                    threshXmin = join.offsetTwo * tileSize;
                    threshYmin = -100;
                    threshXmax = threshXmin + size;
                    threshYmax = Const.PlayerScrollTrigger;
                    NextScreenX = (join.offsetOne - join.offsetTwo) * tileSize;
                }
                else // left edge
                {
                    direction = Direction.Left;
                    threshXmin = -100;
                    threshYmin = join.offsetTwo * tileSize;
                    threshXmax = Const.PlayerScrollTrigger;
                    threshYmax = threshYmin + size;
                    NextScreenY = (join.offsetOne - join.offsetTwo) * tileSize;
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
            scrollDist = 0;

            tickdist = (TriggerSize() + OffsetDist()) / ticks;

            nextHeight = next.Screen.PixelHeight;
            nextWidth = next.Screen.PixelWidth;

            if (direction == Direction.Down) NextScreenY = -height;
            else if (direction == Direction.Right) NextScreenX = -width;
            else if (direction == Direction.Left) NextScreenX = nextWidth;
            else if (direction == Direction.Up) NextScreenY = nextHeight;

            Calculate();
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

        protected virtual void Finish(PositionComponent playerPos)
        {
            if (direction == Direction.Right) { playerPos.SetPosition(new PointF(OffsetDist(), playerPos.Position.Y + NextScreenY)); }
            else if (direction == Direction.Left) { playerPos.SetPosition(new PointF(nextWidth - OffsetDist(), playerPos.Position.Y + NextScreenY)); }
            else if (direction == Direction.Down) { playerPos.SetPosition(new PointF(playerPos.Position.X + NextScreenX, OffsetDist())); }
            else if (direction == Direction.Up) { playerPos.SetPosition(new PointF(playerPos.Position.X + NextScreenX, nextHeight - OffsetDist())); }

            if (ScrollDone != null) ScrollDone(this);
        }

        public virtual void Update(PositionComponent playerPos)
        {
            scrollDist += Const.ScrollSpeed;
            if (JoinInfo.type == JoinType.Vertical && scrollDist >= Game.CurrentGame.PixelsAcross ||
                JoinInfo.type == JoinType.Horizontal && scrollDist >= Game.CurrentGame.PixelsDown)
            {
                Finish(playerPos);
            }
            else
            {
                MovePlayer(playerPos);
            }

            Calculate();
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
