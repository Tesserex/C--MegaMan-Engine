using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.Engine.Entities;

namespace MegaMan.Engine
{
    public class BossDoorHandler : JoinHandler
    {
        private readonly IGameplayContainer container;
        private readonly IEntityPool _entityPool;

        private GameEntity doorOne;
        private GameEntity doorTwo;
        private bool open;
        private int triggerSize;

        private int doorOneX;
        private int doorOneY;
        private int doorTwoX;
        private int doorTwoY;

        public BossDoorHandler(Join join, IGameplayContainer container, IEntityPool entityPool, int tileSize, int height, int width, string name)
            : base(join, tileSize, height, width, name)
        {
            this.container = container;
            _entityPool = entityPool;

            if (direction == Direction.Down)
            {
                doorOneX = join.OffsetOne * tileSize;
                doorOneY = height - tileSize;

                doorTwoX = join.OffsetOne * tileSize;
                doorTwoY = height;
            }
            else if (direction == Direction.Left)
            {
                doorOneX = 0;
                doorOneY = join.OffsetTwo * tileSize;

                doorTwoX = -tileSize;
                doorTwoY = join.OffsetTwo * tileSize;
            }
            else if (direction == Direction.Right)
            {
                doorOneX = width - tileSize;
                doorOneY = join.OffsetOne * tileSize;

                doorTwoX = width;
                doorTwoY = join.OffsetOne * tileSize;
            }
            else if (direction == Direction.Up)
            {
                doorOneX = join.OffsetTwo * tileSize;
                doorOneY = 0;

                doorTwoX = join.OffsetTwo * tileSize;
                doorTwoY = -tileSize;
            }
        }

        public override void Start(ScreenHandler screen)
        {
            base.Start(screen);

            doorOne = _entityPool.CreateEntity(JoinInfo.BossEntityName);
            doorTwo = _entityPool.CreateEntity(JoinInfo.BossEntityName);

            doorOne.GetComponent<PositionComponent>().SetPosition(new PointF(doorOneX, doorOneY));
            doorTwo.GetComponent<PositionComponent>().SetPosition(new PointF(doorTwoX, doorTwoY));

            doorOne.Start(container);
            doorTwo.Start(container);

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
                if (JoinInfo.Direction == JoinDirection.BackwardOnly) return false;
            }
            else if (JoinInfo.Direction == JoinDirection.ForwardOnly) return false;

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
            scrollDist = JoinInfo.Type == JoinType.Vertical ? Game.CurrentGame.PixelsAcross : Game.CurrentGame.PixelsDown;

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
