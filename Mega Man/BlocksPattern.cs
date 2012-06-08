using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MegaMan.Common;

namespace MegaMan.Engine
{
    /// <summary>
    /// Controls a sequence of disappearing blocks on the screen
    /// </summary>
    public class BlocksPattern
    {
        private class BlockInfo
        {
            public GameEntity entity;
            public PointF pos;
            public int on;
            public int off;
        }

        private readonly int length;
        private readonly List<BlockInfo> blocks;
        private readonly int leftBoundary;
        private readonly int rightBoundary;
        private bool running;
        private int frame;
        private bool stopped;
        private PositionComponent playerPos;
        private IGameplayContainer container;

        public BlocksPattern(BlockPatternInfo info, IGameplayContainer container)
        {
            length = info.Length;
            leftBoundary = info.LeftBoundary;
            rightBoundary = info.RightBoundary;
            blocks = new List<BlockInfo>();
            foreach (BlockPatternInfo.BlockInfo blockinfo in info.Blocks)
            {
                BlockInfo myInfo = new BlockInfo {entity = GameEntity.Get(info.Entity, container)};
                // should always persist off screen
                PositionComponent pos = myInfo.entity.GetComponent<PositionComponent>();
                pos.PersistOffScreen = true;
                myInfo.pos = blockinfo.pos;
                myInfo.on = blockinfo.on;
                myInfo.off = blockinfo.off;
                blocks.Add(myInfo);
            }
            running = false;
            frame = 0;
            this.container = container;
        }

        public void Start()
        {
            this.playerPos = container.Entities.GetEntities("Player").Single().GetComponent<PositionComponent>();
            container.GameThink += Update;
            stopped = false;
        }

        public void Stop()
        {
            stopped = true;
            container.GameThink -= Update;
            Halt();
        }

        private void Update()
        {
            if (stopped) return;

            float px = playerPos.Position.X;
            if (px >= leftBoundary && px <= rightBoundary)
            {
                if (!running) Run();
                else
                {
                    frame++;
                    if (frame > length) frame = 0;
                    foreach (BlockInfo info in blocks)
                    {
                        if (info.on == frame) info.entity.SendMessage(new StateMessage(null, "Show"));
                        else if (info.off == frame) info.entity.SendMessage(new StateMessage(null, "Hide"));
                    }
                }
            }
            else if (running) Halt();
        }

        private void Halt()
        {
            foreach (BlockInfo info in blocks)
            {
                info.entity.Stop();
            }
            running = false;
            frame = 0;
        }

        private void Run()
        {
            running = true;
            frame = 0;
            foreach (BlockInfo info in blocks)
            {
                info.entity.SendMessage(new StateMessage(null, "Start"));
                info.entity.Start();
                PositionComponent pos = info.entity.GetComponent<PositionComponent>();
                if (pos == null) continue;
                pos.SetPosition(info.pos);
                info.entity.SendMessage(new StateMessage(null, info.on > 0 ? "Hide" : "Show"));
            }
        }
    }
}
