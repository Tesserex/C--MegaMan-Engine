using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common;

namespace MegaMan.Engine
{
    public class StageFactory
    {
        private StageHandler handler;

        public StageHandler CreateMap(StageLinkInfo info)
        {
            StageInfo map = new StageInfo(info.StagePath);

            handler = new StageHandler(map);

            var joins = new Dictionary<ScreenInfo, Dictionary<Join, JoinHandler>>();
            var bossDoors = new Dictionary<ScreenInfo, Dictionary<Join, GameEntity>>();

            foreach (var screen in map.Screens.Values)
            {
                joins[screen] = new Dictionary<Join, JoinHandler>();
                bossDoors[screen] = new Dictionary<Join, GameEntity>();

                foreach (Join join in map.Joins)
                {
                    GameEntity door = null;
                    if (join.bossDoor)
                    {
                        door = GameEntity.Get(join.bossEntityName, handler);
                    }
                    bossDoors[screen][join] = door;
                }
            }

            foreach (Join join in map.Joins)
            {
                var screenOne = map.Screens[join.screenOne];
                var screenTwo = map.Screens[join.screenTwo];

                JoinHandler handlerOne = CreateJoin(join,
                    screenOne,
                    bossDoors[screenOne][join],
                    bossDoors[screenTwo][join]);

                joins[screenOne].Add(join, handlerOne);

                JoinHandler handlerTwo = CreateJoin(join,
                    screenTwo,
                    bossDoors[screenTwo][join],
                    bossDoors[screenOne][join]);

                joins[screenTwo].Add(join, handlerTwo);
            }

            var screens = new Dictionary<string, ScreenHandler>();
            foreach (var screen in map.Screens.Values)
            {
                screens[screen.Name] = CreateScreen(handler, screen, joins[screen].Values.ToList());
            }

            handler.InitScreens(screens);

            handler.WinHandler = info.WinHandler;

            if (info.LoseHandler == null)
            {
                // repeat this stage
                handler.LoseHandler = new HandlerTransfer { Fade = true, Type = HandlerType.Stage, Name = info.Name };
            }
            else
            {
                handler.LoseHandler = info.LoseHandler;
            }

            return handler;
        }

        private ScreenHandler CreateScreen(StageHandler stage, ScreenInfo screen, IEnumerable<JoinHandler> joins)
        {
            var patterns = new List<BlocksPattern>(screen.BlockPatterns.Count);

            foreach (BlockPatternInfo info in screen.BlockPatterns)
            {
                BlocksPattern pattern = new BlocksPattern(info, handler);
                patterns.Add(pattern);
            }

            var layers = new List<ScreenLayer>();
            foreach (var layerInfo in screen.Layers)
            {
                layers.Add(new ScreenLayer(layerInfo, stage));
            }

            return new ScreenHandler(screen, layers, joins, patterns, handler);
        }

        private JoinHandler CreateJoin(Join join, ScreenInfo screen, GameEntity door, GameEntity otherDoor)
        {
            if (join.bossDoor)
            {
                return new BossDoorHandler(door, otherDoor, join, screen.Tileset.TileSize, screen.PixelHeight, screen.PixelWidth, screen.Name);
            }

            return new JoinHandler(join, screen.Tileset.TileSize, screen.PixelHeight, screen.PixelWidth, screen.Name);
        }
    }
}
