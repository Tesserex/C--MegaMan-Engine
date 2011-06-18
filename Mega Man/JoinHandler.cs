using System;
using System.Drawing;
using MegaMan;

namespace Mega_Man
{
    public class JoinHandler
    {
        public Join JoinInfo { get; private set; }

        protected int threshXmin, threshXmax;
        protected int threshYmin, threshYmax;
        private readonly int size;
        protected readonly Direction direction;

        private int nextScreenX, nextScreenY;
        protected float scrollDist;
        private float offsetX, offsetY;

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
            ticks = ((JoinInfo.type == MegaMan.JoinType.Vertical) ? Game.CurrentGame.PixelsAcross : Game.CurrentGame.PixelsDown) / Const.ScrollSpeed;

            if (join.screenOne == currentScreen.Screen.Name)
            {
                NextScreenName = join.screenTwo;
                if (join.type == MegaMan.JoinType.Horizontal)   // bottom edge
                {
                    direction = Direction.Down;
                    threshXmin = join.offsetOne * currentScreen.Screen.Tileset.TileSize;
                    threshYmin = currentScreen.Screen.PixelHeight - Const.PlayerScrollTrigger;
                    threshXmax = threshXmin + size;
                    threshYmax = currentScreen.Screen.PixelHeight + 100;
                    nextScreenX = (join.offsetTwo - join.offsetOne) * currentScreen.Screen.Tileset.TileSize;
                }
                else // right edge
                {
                    direction = Direction.Right;
                    threshXmin = currentScreen.Screen.PixelWidth - Const.PlayerScrollTrigger;
                    threshYmin = join.offsetOne * currentScreen.Screen.Tileset.TileSize;
                    threshXmax = currentScreen.Screen.PixelWidth + 100;
                    threshYmax = threshYmin + size;
                    nextScreenY = (join.offsetTwo - join.offsetOne) * currentScreen.Screen.Tileset.TileSize;
                }
            }
            else
            {
                NextScreenName = join.screenOne;
                if (join.type == MegaMan.JoinType.Horizontal)   // top edge
                {
                    direction = Direction.Up;
                    threshXmin = join.offsetTwo * currentScreen.Screen.Tileset.TileSize;
                    threshYmin = -100;
                    threshXmax = threshXmin + size;
                    threshYmax = Const.PlayerScrollTrigger;
                    nextScreenX = (join.offsetOne - join.offsetTwo) * currentScreen.Screen.Tileset.TileSize;
                }
                else // left edge
                {
                    direction = Direction.Left;
                    threshXmin = -100;
                    threshYmin = join.offsetTwo * currentScreen.Screen.Tileset.TileSize;
                    threshXmax = Const.PlayerScrollTrigger;
                    threshYmax = threshYmin + size;
                    nextScreenY = (join.offsetOne - join.offsetTwo) * currentScreen.Screen.Tileset.TileSize;
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
            if (direction == Direction.Down) nextScreenY = -currentScreen.Screen.PixelHeight;
            else if (direction == Direction.Right) nextScreenX = -currentScreen.Screen.PixelWidth;
            else if (direction == Direction.Left) nextScreenX = next.Screen.PixelWidth;
            else if (direction == Direction.Up) nextScreenY = next.Screen.PixelHeight;
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
            if (direction == Direction.Right) { playerPos.SetPosition(new PointF(OffsetDist(), playerPos.Position.Y + nextScreenY)); }
            else if (direction == Direction.Left) { playerPos.SetPosition(new PointF(nextScreen.Screen.PixelWidth - OffsetDist(), playerPos.Position.Y + nextScreenY)); }
            else if (direction == Direction.Down) { playerPos.SetPosition(new PointF(playerPos.Position.X + nextScreenX, OffsetDist())); }
            else if (direction == Direction.Up) { playerPos.SetPosition(new PointF(playerPos.Position.X + nextScreenX, nextScreen.Screen.PixelHeight - OffsetDist())); }
        }

        public virtual void Update(PositionComponent playerPos)
        {
            scrollDist += Const.ScrollSpeed;
            if (JoinInfo.type == MegaMan.JoinType.Vertical && scrollDist >= Game.CurrentGame.PixelsAcross ||
                JoinInfo.type == MegaMan.JoinType.Horizontal && scrollDist >= Game.CurrentGame.PixelsDown)
            {
                FinalizePlayerPos(playerPos);
                Finish();
            }
            else
            {
                MovePlayer(playerPos);
            }
        }

        protected void Finish()
        {
            if (ScrollDone != null) ScrollDone(this, nextScreen);
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            float nextOffsetX = 0, nextOffsetY = 0;
            if (direction == Direction.Right)
            {
                offsetX = scrollDist;
                nextOffsetX = scrollDist - Game.CurrentGame.PixelsAcross;
                nextOffsetY += offsetY;
            }
            else if (direction == Direction.Left)
            {
                offsetX = -scrollDist;
                nextOffsetX = Game.CurrentGame.PixelsAcross - scrollDist;
                nextOffsetY += offsetY;
            }
            else if (direction == Direction.Down)
            {
                offsetY = scrollDist;
                nextOffsetY = scrollDist - Game.CurrentGame.PixelsDown;
                nextOffsetX += offsetX;
            }
            else if (direction == Direction.Up)
            {
                offsetY = -scrollDist;
                nextOffsetY = Game.CurrentGame.PixelsDown - scrollDist;
                nextOffsetX += offsetX;
            }

            currentScreen.Draw(batch, 0, 0, offsetX, offsetY);
            nextScreen.Draw(batch, nextScreenX, nextScreenY, nextOffsetX, nextOffsetY);
        }
    }
}
