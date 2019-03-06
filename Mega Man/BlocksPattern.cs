using System.Collections.Generic;
using System.Linq;
using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.Engine.Entities;

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
        private readonly BlockPatternInfo info;
        private readonly List<BlockInfo> blocks;
        private readonly int leftBoundary;
        private readonly int rightBoundary;
        private bool running;
        private int frame;
        private bool stopped;
        private PositionComponent playerPos;
        private IGameplayContainer container;
        private IEntityPool _entityPool;

        public BlocksPattern(BlockPatternInfo info, IGameplayContainer container, IEntityPool entityPool)
        {
            this.info = info;
            length = info.Length;
            leftBoundary = info.LeftBoundary;
            rightBoundary = info.RightBoundary;
            blocks = new List<BlockInfo>();
            running = false;
            frame = 0;
            this.container = container;
            _entityPool = entityPool;
        }

        public void Start()
        {
            if (!blocks.Any())
            {
                foreach (var blockinfo in info.Blocks)
                {
                    var myInfo = new BlockInfo { entity = _entityPool.CreateEntity(info.Entity) };
                    // should always persist off screen
                    var pos = myInfo.entity.GetComponent<PositionComponent>();
                    pos.PersistOffScreen = true;
                    myInfo.pos = new PointF(blockinfo.Pos.X, blockinfo.Pos.Y);
                    myInfo.on = blockinfo.On;
                    myInfo.off = blockinfo.Off;
                    blocks.Add(myInfo);
                }
            }

            playerPos = container.Entities.GetEntityById("Player").GetComponent<PositionComponent>();
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

            var px = playerPos.X;
            if (px >= leftBoundary && px <= rightBoundary)
            {
                if (!running) Run();
                else
                {
                    frame++;
                    if (frame > length) frame = 0;
                    foreach (var info in blocks)
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
            foreach (var info in blocks)
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
            foreach (var info in blocks)
            {
                info.entity.SendMessage(new StateMessage(null, "Start"));
                info.entity.Start(container);
                var pos = info.entity.GetComponent<PositionComponent>();
                if (pos == null) continue;
                pos.SetPosition(info.pos);
                info.entity.SendMessage(new StateMessage(null, info.on > 0 ? "Hide" : "Show"));
            }
        }
    }
}
