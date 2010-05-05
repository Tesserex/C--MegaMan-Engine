using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Drawing;
using MegaMan;

namespace Mega_Man
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

        private int length;
        private List<BlockInfo> blocks;
        private int leftBoundary, rightBoundary;
        private bool running;
        private int frame;

        public BlocksPattern(MegaMan.BlockPatternInfo info)
        {
            this.length = info.Length;
            this.leftBoundary = info.LeftBoundary;
            this.rightBoundary = info.RightBoundary;
            blocks = new List<BlockInfo>();
            foreach (MegaMan.BlockPatternInfo.BlockInfo blockinfo in info.Blocks)
            {
                BlockInfo myInfo = new BlockInfo();
                myInfo.entity = GameEntity.Get(info.Entity);
                // should always persist off screen
                PositionComponent pos = (PositionComponent)myInfo.entity.GetComponent(typeof(PositionComponent));
                pos.PersistOffScreen = true;
                myInfo.pos = blockinfo.pos;
                myInfo.on = blockinfo.on;
                myInfo.off = blockinfo.off;
                blocks.Add(myInfo);
            }
            running = false;
            frame = 0;
        }

        public void Start()
        {
            Engine.Instance.GameThink += Update;
        }

        public void Stop()
        {
            Halt();
            Engine.Instance.GameThink -= Update;
        }

        private void Update()
        {
            float px = Game.CurrentGame.CurrentMap.PlayerPos.Position.X;
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
                PositionComponent pos = (PositionComponent)info.entity.GetComponent(typeof(PositionComponent));
                if (pos == null) continue;
                pos.SetPosition(info.pos);
                if (info.on > 0) // not turned on yet
                {
                    info.entity.SendMessage(new StateMessage(null, "Hide"));
                }
                else
                    info.entity.SendMessage(new StateMessage(null, "Show"));
            }
        }
    }
}
