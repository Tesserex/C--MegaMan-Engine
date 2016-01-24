using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MegaMan.Common;
using MegaMan.Engine.Entities;

namespace MegaMan.Engine
{
    public class StageFactory
    {
        private Dictionary<string, StageHandler> _loadedStages = new Dictionary<string, StageHandler>();
        private readonly IEntityPool _entityPool;
        private readonly IEntityRespawnTracker _respawnTracker;

        public StageFactory(IEntityPool entityPool, IEntityRespawnTracker respawnTracker)
        {
            _entityPool = entityPool;
            _respawnTracker = respawnTracker;
        }

        public StageHandler Get(string name)
        {
            if (!_loadedStages.ContainsKey(name))
            {
                throw new GameRunException(String.Format("I couldn't find a stage called {0}. Sorry.", name));
            }

            return _loadedStages[name];
        }

        public void Load(StageLinkInfo info)
        {
            try
            {
                TryLoad(info);
            }
            catch (XmlException e)
            {
                throw new GameRunException(String.Format("The map file for stage {0} has badly formatted XML:\n\n{1}", info.Name, e.Message));
            }
        }

        public void TryLoad(StageLinkInfo info)
        {
            var stageReader = Game.CurrentGame.FileReaderProvider.GetStageReader(info.StagePath);
            StageInfo map = stageReader.Load(info.StagePath);

            var handler = new StageHandler(map);

            var joins = new Dictionary<ScreenInfo, Dictionary<Join, JoinHandler>>();

            foreach (var screen in map.Screens.Values)
            {
                joins[screen] = new Dictionary<Join, JoinHandler>();
            }

            foreach (Join join in map.Joins)
            {
                var screenOne = map.Screens[join.screenOne];
                var screenTwo = map.Screens[join.screenTwo];

                JoinHandler handlerOne = CreateJoin(join, handler, screenOne);

                joins[screenOne].Add(join, handlerOne);

                JoinHandler handlerTwo = CreateJoin(join, handler, screenTwo);

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

            _loadedStages[info.Name] = handler;
        }

        private ScreenHandler CreateScreen(StageHandler stage, ScreenInfo screen, IEnumerable<JoinHandler> joins)
        {
            var patterns = new List<BlocksPattern>(screen.BlockPatterns.Count);

            foreach (BlockPatternInfo info in screen.BlockPatterns)
            {
                BlocksPattern pattern = new BlocksPattern(info, stage, _entityPool);
                patterns.Add(pattern);
            }

            var layers = new List<ScreenLayer>();
            foreach (var layerInfo in screen.Layers)
            {
                layers.Add(new ScreenLayer(layerInfo, stage, _respawnTracker));
            }

            return new ScreenHandler(screen, layers, joins, patterns, stage);
        }

        private JoinHandler CreateJoin(Join join, StageHandler stage, ScreenInfo screen)
        {
            if (join.bossDoor)
            {
                return new BossDoorHandler(join, stage, _entityPool, screen.Tileset.TileSize, screen.PixelHeight, screen.PixelWidth, screen.Name);
            }

            return new JoinHandler(join, screen.Tileset.TileSize, screen.PixelHeight, screen.PixelWidth, screen.Name);
        }
    }
}
