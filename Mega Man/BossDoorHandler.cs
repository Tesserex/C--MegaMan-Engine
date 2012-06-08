using System.Drawing;
using MegaMan.Common;

namespace MegaMan.Engine
{
    public class BossDoorHandler : JoinHandler
    {
        public GameEntity Door { get; private set; }
        private GameEntity otherDoor;
        private bool open;
        private int triggerSize;

        public BossDoorHandler(GameEntity door, GameEntity otherDoor, Join join, int tileSize, int height, int width, string name)
            : base(join, tileSize, height, width, name)
        {
            this.otherDoor = otherDoor;

            this.Door = door;

            if (direction == Direction.Down) threshYmin -= tileSize;
            else if (direction == Direction.Left)
            {
                threshXmin = 0;
                threshXmax += tileSize;
            }
            else if (direction == Direction.Right) threshXmin -= tileSize;
            else if (direction == Direction.Up)
            {
                threshYmin = 0;
                threshYmax += tileSize;
            }

            PositionComponent pos = door.GetComponent<PositionComponent>();
            pos.SetPosition(new PointF(threshXmin, threshYmin));
        }

        public override void Start(ScreenHandler screen)
        {
            base.Start(screen);

            Door.Start();

            Door.GetComponent<StateComponent>().StateChanged += s =>
            {
                if (s == "Open") open = true;
            };
        }

        public override void Stop()
        {
            Door.Die();
        }

        public override bool Trigger(PointF position)
        {
            if (direction == Direction.Right || direction == Direction.Down)
            {
                if (JoinInfo.direction == JoinDirection.BackwardOnly) return false;
            }
            else if (JoinInfo.direction == JoinDirection.ForwardOnly) return false;

            return (Door.GetComponent<CollisionComponent>()).TouchedBy("Player");
        }

        public override void BeginScroll(ScreenHandler next, PointF playerPos)
        {
            Door.SendMessage(new StateMessage(null, "Opening"));
            if (direction == Direction.Down) triggerSize = (int)(height - playerPos.Y);
            else if (direction == Direction.Left) triggerSize = (int)playerPos.X;
            else if (direction == Direction.Right) triggerSize = (int)(width - playerPos.X);
            else triggerSize = (int)playerPos.Y;

            base.BeginScroll(next, playerPos);

            if (otherDoor != null)
            {
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
