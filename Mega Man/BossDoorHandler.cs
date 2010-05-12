using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MegaMan;

namespace Mega_Man
{
    public class BossDoorHandler : JoinHandler
    {
        private GameEntity door;
        private GameEntity otherDoor;
        private bool open = false;
        private int triggerSize;

        public BossDoorHandler(GameEntity door, MegaMan.Join join, ScreenHandler currentScreen) : base(join, currentScreen)
        {
            this.door = door;

            if (this.direction == Direction.Down) this.threshYmin -= currentScreen.Screen.Tileset.TileSize;
            else if (this.direction == Direction.Left)
            {
                this.threshXmin = 0;
                this.threshXmax += currentScreen.Screen.Tileset.TileSize;
            }
            else if (this.direction == Direction.Right) this.threshXmin -= currentScreen.Screen.Tileset.TileSize;
            else if (this.direction == Direction.Up)
            {
                this.threshYmin = 0;
                this.threshYmax += currentScreen.Screen.Tileset.TileSize;
            }

            PositionComponent pos = door.GetComponent<PositionComponent>();
            pos.SetPosition(new System.Drawing.PointF(this.threshXmin, this.threshYmin));
        }

        public override void Start(ScreenHandler currentScreen)
        {
            door.Start();
            door.Screen = currentScreen;

            door.GetComponent<StateComponent>().StateChanged += (s) =>
            {
                if (s == "Open") open = true;
            };
        }

        public override void Stop()
        {
            door.Die();
        }

        public override bool Trigger(PointF position)
        {
            if (this.direction == Direction.Right || this.direction == Direction.Down)
            {
                if (this.JoinInfo.direction == JoinDirection.BackwardOnly) return false;
            }
            else if (this.JoinInfo.direction == JoinDirection.ForwardOnly) return false;

            return (door.GetComponent<CollisionComponent>()).TouchedBy("Player");
        }

        public override void BeginScroll(ScreenHandler next, PointF playerPos)
        {
            door.SendMessage(new StateMessage(null, "Opening"));
            if (this.direction == Direction.Down) triggerSize = (int)(this.currentScreen.Screen.PixelHeight - playerPos.Y);
            else if (this.direction == Direction.Left) triggerSize = (int)playerPos.X;
            else if (this.direction == Direction.Right) triggerSize = (int)(this.currentScreen.Screen.PixelWidth - playerPos.X);
            else triggerSize = (int)playerPos.Y;

            base.BeginScroll(next, playerPos);

            JoinHandler otherSide = next.GetJoinHandler(this.JoinInfo);
            if (otherSide != null && otherSide.JoinInfo.bossDoor) // damn well better be true!
            {
                otherDoor = ((BossDoorHandler)otherSide).door;
                otherDoor.SendMessage(new StateMessage(null, "Open"));
            }
        }

        public override void Update(PositionComponent playerPos)
        {
            if (!open) return;

            scrollDist += Const.ScrollSpeed;
            if (JoinInfo.type == MegaMan.JoinType.Vertical && scrollDist >= Game.CurrentGame.PixelsAcross ||
                JoinInfo.type == MegaMan.JoinType.Horizontal && scrollDist >= Game.CurrentGame.PixelsDown)
            {
                if (JoinInfo.type == MegaMan.JoinType.Vertical) scrollDist = Game.CurrentGame.PixelsAcross;
                else scrollDist = Game.CurrentGame.PixelsDown;

                otherDoor.SendMessage(new StateMessage(null, "Closing"));
                (otherDoor.GetComponent<StateComponent>()).StateChanged += (s) =>
                {
                    if (s == "Start")
                    {
                        Finish();
                        FinalizePlayerPos(playerPos);
                    }
                };
                open = false;
                return;
            }
            else
            {
                MovePlayer(playerPos);
            }
        }

        protected override int TriggerSize()
        {
            return triggerSize;
        }

        protected override int OffsetDist()
        {
            return triggerSize + Const.PlayerScrollOffset;
        }
    }
}
