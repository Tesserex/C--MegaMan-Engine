using System.Drawing;
using MegaMan.Common;

namespace MegaMan.Engine
{
    public class BossDoorHandler : JoinHandler
    {
        private readonly GameEntity door;
        private GameEntity otherDoor;
        private bool open;
        private int triggerSize;

        public BossDoorHandler(GameEntity door, Join join, ScreenHandler currentScreen) : base(join, currentScreen)
        {
            this.door = door;

            if (direction == Direction.Down) threshYmin -= currentScreen.Screen.Tileset.TileSize;
            else if (direction == Direction.Left)
            {
                threshXmin = 0;
                threshXmax += currentScreen.Screen.Tileset.TileSize;
            }
            else if (direction == Direction.Right) threshXmin -= currentScreen.Screen.Tileset.TileSize;
            else if (direction == Direction.Up)
            {
                threshYmin = 0;
                threshYmax += currentScreen.Screen.Tileset.TileSize;
            }

            PositionComponent pos = door.GetComponent<PositionComponent>();
            pos.SetPosition(new PointF(threshXmin, threshYmin));
        }

        public override void Start(ScreenHandler screen)
        {
            door.Start();
            door.Screen = screen;

            door.GetComponent<StateComponent>().StateChanged += s =>
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
            if (direction == Direction.Right || direction == Direction.Down)
            {
                if (JoinInfo.direction == JoinDirection.BackwardOnly) return false;
            }
            else if (JoinInfo.direction == JoinDirection.ForwardOnly) return false;

            return (door.GetComponent<CollisionComponent>()).TouchedBy("Player");
        }

        public override void BeginScroll(ScreenHandler next, PointF playerPos)
        {
            door.SendMessage(new StateMessage(null, "Opening"));
            if (direction == Direction.Down) triggerSize = (int)(currentScreen.Screen.PixelHeight - playerPos.Y);
            else if (direction == Direction.Left) triggerSize = (int)playerPos.X;
            else if (direction == Direction.Right) triggerSize = (int)(currentScreen.Screen.PixelWidth - playerPos.X);
            else triggerSize = (int)playerPos.Y;

            base.BeginScroll(next, playerPos);

            JoinHandler otherSide = next.GetJoinHandler(JoinInfo);
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
            if (JoinInfo.type == JoinType.Vertical && scrollDist >= Game.CurrentGame.PixelsAcross ||
                JoinInfo.type == JoinType.Horizontal && scrollDist >= Game.CurrentGame.PixelsDown)
            {
                scrollDist = JoinInfo.type == JoinType.Vertical ?
                    Game.CurrentGame.PixelsAcross : Game.CurrentGame.PixelsDown;

                otherDoor.SendMessage(new StateMessage(null, "Closing"));
                (otherDoor.GetComponent<StateComponent>()).StateChanged += s =>
                {
                    if (s == "Start")
                    {
                        Finish();
                        FinalizePlayerPos(playerPos);
                    }
                };
                open = false;
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
