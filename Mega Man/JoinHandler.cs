using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MegaMan;

namespace Mega_Man
{
    public class JoinHandler
    {
        public Join JoinInfo { get; protected set; }

        protected int threshXmin, threshXmax;
        protected int threshYmin, threshYmax;
        protected int size;
        protected Direction direction;

        private int nextScreenX = 0, nextScreenY = 0;
        protected float scrollDist = 0;
        private float offsetX, offsetY;

        protected ScreenHandler currentScreen;
        protected ScreenHandler nextScreen;

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
            this.JoinInfo = join;
            this.currentScreen = currentScreen;

            size = join.Size * currentScreen.Screen.Tileset.TileSize;
            if (join.screenOne == currentScreen.Screen.Name)
            {
                this.NextScreenName = join.screenTwo;
                if (join.type == MegaMan.JoinType.Horizontal)   // bottom edge
                {
                    this.direction = Direction.Down;
                    this.threshXmin = join.offsetOne * currentScreen.Screen.Tileset.TileSize;
                    this.threshYmin = currentScreen.Screen.PixelHeight - Const.PlayerScrollTrigger;
                    this.threshXmax = this.threshXmin + size;
                    this.threshYmax = currentScreen.Screen.PixelHeight + 100;
                    this.nextScreenX = (join.offsetTwo - join.offsetOne) * currentScreen.Screen.Tileset.TileSize;
                }
                else // right edge
                {
                    this.direction = Direction.Right;
                    this.threshXmin = currentScreen.Screen.PixelWidth - Const.PlayerScrollTrigger;
                    this.threshYmin = join.offsetOne * currentScreen.Screen.Tileset.TileSize;
                    this.threshXmax = currentScreen.Screen.PixelWidth + 100;
                    this.threshYmax = this.threshYmin + size;
                    this.nextScreenY = (join.offsetTwo - join.offsetOne) * currentScreen.Screen.Tileset.TileSize;
                }
            }
            else
            {
                this.NextScreenName = join.screenOne;
                if (join.type == MegaMan.JoinType.Horizontal)   // top edge
                {
                    this.direction = Direction.Up;
                    this.threshXmin = join.offsetTwo * currentScreen.Screen.Tileset.TileSize;
                    this.threshYmin = -100;
                    this.threshXmax = this.threshXmin + size;
                    this.threshYmax = Const.PlayerScrollTrigger;
                    this.nextScreenX = (join.offsetOne - join.offsetTwo) * currentScreen.Screen.Tileset.TileSize;
                }
                else // left edge
                {
                    this.direction = Direction.Left;
                    this.threshXmin = -100;
                    this.threshYmin = join.offsetTwo * currentScreen.Screen.Tileset.TileSize;
                    this.threshXmax = Const.PlayerScrollTrigger;
                    this.threshYmax = this.threshYmin + size;
                    this.nextScreenY = (join.offsetOne - join.offsetTwo) * currentScreen.Screen.Tileset.TileSize;
                }
            }
        }

        public virtual void Start(ScreenHandler currentScreen)
        {
        }

        public virtual void Stop()
        {
        }

        public virtual bool Trigger(PointF position)
        {
            if (this.direction == Direction.Right || this.direction == Direction.Down)
            {
                if (this.JoinInfo.direction == JoinDirection.BackwardOnly) return false;
            }
            else if (this.JoinInfo.direction == JoinDirection.ForwardOnly) return false;

            return (position.X >= threshXmin && position.X <= threshXmax &&
                    position.Y >= threshYmin && position.Y <= threshYmax);
        }

        public virtual void BeginScroll(ScreenHandler next, PointF playerPos)
        {
            this.nextScreen = next;
            if (this.direction == Direction.Down) this.nextScreenY = -next.Screen.PixelHeight;
            else if (this.direction == Direction.Right) this.nextScreenX = -next.Screen.PixelWidth;
            else if (this.direction == Direction.Left) this.nextScreenX = next.Screen.PixelWidth;
            else if (this.direction == Direction.Up) this.nextScreenY = next.Screen.PixelHeight;
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
            int scrollDist = this.TriggerSize() + this.OffsetDist();
            float ticks = ((JoinInfo.type == MegaMan.JoinType.Vertical) ? Game.CurrentGame.PixelsAcross : Game.CurrentGame.PixelsDown) / Const.ScrollSpeed;
            float tickdist = scrollDist / ticks;
            if (this.direction == Direction.Right) playerPos.SetPosition(new PointF(playerPos.Position.X + tickdist, playerPos.Position.Y));
            else if (this.direction == Direction.Left) playerPos.SetPosition(new PointF(playerPos.Position.X - tickdist, playerPos.Position.Y));
            else if (this.direction == Direction.Down) playerPos.SetPosition(new PointF(playerPos.Position.X, playerPos.Position.Y + tickdist));
            else if (this.direction == Direction.Up) playerPos.SetPosition(new PointF(playerPos.Position.X, playerPos.Position.Y - tickdist));
        }

        protected void FinalizePlayerPos(PositionComponent playerPos)
        {
            if (this.direction == Direction.Right) { playerPos.SetPosition(new PointF(this.OffsetDist(), playerPos.Position.Y + nextScreenY)); }
            else if (this.direction == Direction.Left) { playerPos.SetPosition(new PointF(nextScreen.Screen.PixelWidth - this.OffsetDist(), playerPos.Position.Y + nextScreenY)); }
            else if (this.direction == Direction.Down) { playerPos.SetPosition(new PointF(playerPos.Position.X + nextScreenX, this.OffsetDist())); }
            else if (this.direction == Direction.Up) { playerPos.SetPosition(new PointF(playerPos.Position.X + nextScreenX, nextScreen.Screen.PixelHeight - this.OffsetDist())); }
        }

        public virtual void Update(PositionComponent playerPos)
        {
            scrollDist += Const.ScrollSpeed;
            if (JoinInfo.type == MegaMan.JoinType.Vertical && scrollDist >= Game.CurrentGame.PixelsAcross ||
                JoinInfo.type == MegaMan.JoinType.Horizontal && scrollDist >= Game.CurrentGame.PixelsDown)
            {
                FinalizePlayerPos(playerPos);
                Finish();
                return;
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

        public void Draw(Graphics g, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            int width = currentScreen.Screen.PixelWidth;
            int height = currentScreen.Screen.PixelHeight;

            float nextOffsetX = 0, nextOffsetY = 0;
            if (this.direction == Direction.Right)
            {
                offsetX = scrollDist;
                nextOffsetX = scrollDist - Game.CurrentGame.PixelsAcross;
                nextOffsetY += offsetY;
            }
            else if (this.direction == Direction.Left)
            {
                offsetX = -scrollDist;
                nextOffsetX = Game.CurrentGame.PixelsAcross - scrollDist;
                nextOffsetY += offsetY;
            }
            else if (this.direction == Direction.Down)
            {
                offsetY = scrollDist;
                nextOffsetY = scrollDist - Game.CurrentGame.PixelsDown;
                nextOffsetX += offsetX;
            }
            else if (this.direction == Direction.Up)
            {
                offsetY = -scrollDist;
                nextOffsetY = Game.CurrentGame.PixelsDown - scrollDist;
                nextOffsetX += offsetX;
            }

            currentScreen.Draw(g, 0, 0, offsetX, offsetY);
            nextScreen.Draw(g, this.nextScreenX, this.nextScreenY, nextOffsetX, nextOffsetY);

            currentScreen.Draw(batch, 0, 0, offsetX, offsetY);
            nextScreen.Draw(batch, this.nextScreenX, this.nextScreenY, nextOffsetX, nextOffsetY);
        }
    }
}
