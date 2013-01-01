using System.Drawing;
using MegaMan.Common;

namespace MegaMan.Engine
{
    public class BossDoorHandler : JoinHandler
    {
        private IGameplayContainer container;
        private GameEntity doorOne;
        private GameEntity doorTwo;
        private bool open;
        private int triggerSize;

        private int doorOneX;
        private int doorOneY;
        private int doorTwoX;
        private int doorTwoY;

        public BossDoorHandler(Join join, IGameplayContainer container, int tileSize, int height, int width, string name)
            : base(join, tileSize, height, width, name)
        {
            this.container = container;

            if (direction == Direction.Down)
            {
                doorOneX = join.offsetOne * tileSize;
                doorOneY = height - tileSize;

                doorTwoX = join.offsetOne * tileSize;
                doorTwoY = height;
            }
            else if (direction == Direction.Left)
            {
                doorOneX = 0;
                doorOneY = join.offsetTwo * tileSize;

                doorTwoX = -tileSize;
                doorTwoY = join.offsetTwo * tileSize;
            }
            else if (direction == Direction.Right)
            {
                doorOneX = width - tileSize;
                doorOneY = join.offsetOne * tileSize;

                doorTwoX = width;
                doorTwoY = join.offsetOne * tileSize;
            }
            else if (direction == Direction.Up)
            {
                doorOneX = join.offsetTwo * tileSize;
                doorOneY = 0;

                doorTwoX = join.offsetTwo * tileSize;
                doorTwoY = -tileSize;
            }
        }

        public override void Start(ScreenHandler screen)
        {
            base.Start(screen);

            doorOne = GameEntity.Get(JoinInfo.bossEntityName, container);
            doorTwo = GameEntity.Get(JoinInfo.bossEntityName, container);

            doorOne.GetComponent<PositionComponent>().SetPosition(new PointF(doorOneX, doorOneY));
            doorTwo.GetComponent<PositionComponent>().SetPosition(new PointF(doorTwoX, doorTwoY));

            doorOne.Start();
            doorTwo.Start();

            doorOne.GetComponent<StateComponent>().StateChanged += s =>
            {
                if (s == "Open") open = true;
            };
        }

        public override void Stop()
        {
            doorOne.Die();
            doorTwo.Die();
        }

        public override bool Trigger(PointF position)
        {
            if (direction == Direction.Right || direction == Direction.Down)
            {
                if (JoinInfo.direction == JoinDirection.BackwardOnly) return false;
            }
            else if (JoinInfo.direction == JoinDirection.ForwardOnly) return false;

            return (doorOne.GetComponent<CollisionComponent>()).TouchedBy("Player");
        }

        public override void BeginScroll(ScreenHandler next, PointF playerPos)
        {
            doorOne.SendMessage(new StateMessage(null, "Opening"));
            if (direction == Direction.Down) triggerSize = (int)(height - playerPos.Y);
            else if (direction == Direction.Left) triggerSize = (int)playerPos.X;
            else if (direction == Direction.Right) triggerSize = (int)(width - playerPos.X);
            else triggerSize = (int)playerPos.Y;

            base.BeginScroll(next, playerPos);

            if (doorTwo != null)
            {
                doorTwo.SendMessage(new StateMessage(null, "Open"));
            }
        }

        public override void Update(PositionComponent playerPos)
        {
            if (open)
            {
                base.Update(playerPos);
            }
        }

        protected override void Finish(PositionComponent playerPos)
        {
            scrollDist = JoinInfo.type == JoinType.Vertical ? Game.CurrentGame.PixelsAcross : Game.CurrentGame.PixelsDown;

            doorTwo.SendMessage(new StateMessage(null, "Closing"));
            (doorTwo.GetComponent<StateComponent>()).StateChanged += s =>
            {
                if (s == "Start")
                {
                    base.Finish(playerPos);
                }
            };
            open = false;
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
