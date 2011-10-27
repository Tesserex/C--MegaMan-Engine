using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common;

namespace MegaMan.Engine
{
    public class ScreenFactory
    {
        public static ScreenHandler CreateScreen(Screen screen, PositionComponent playerPos, IEnumerable<Join> mapJoins)
        {
            List<BlocksPattern> blockPatterns = new List<BlocksPattern>();
            foreach (BlockPatternInfo info in screen.BlockPatternInfo)
            {
                BlocksPattern pattern = new BlocksPattern(info);
                blockPatterns.Add(pattern);
            }

            var joinList = new List<JoinHandler>();
            foreach (Join join in mapJoins)
            {
                if (join.screenOne == screen.Name || join.screenTwo == screen.Name)
                {
                    //JoinHandler handler = CreateJoin(join);
                    //joinList.Add(handler);
                }
            }

            Music music = null;

            string intropath = (screen.MusicIntroPath != null) ? screen.MusicIntroPath.Absolute : null;
            string looppath = (screen.MusicLoopPath != null) ? screen.MusicLoopPath.Absolute : null;
            if (intropath != null || looppath != null) music = Engine.Instance.SoundSystem.LoadMusic(intropath, looppath, 1);

            return new ScreenHandler(screen, music, playerPos, joinList, blockPatterns);
        }

        private static JoinHandler CreateJoin(Join join, ScreenHandler currentScreen)
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
    }
}
