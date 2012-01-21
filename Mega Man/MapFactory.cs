using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common;

namespace MegaMan.Engine
{
    public class MapFactory
    {
        public GamePlay GamePlay { get; private set; }

        public MapHandler CreateMap(StageInfo info, Common.PauseScreen pauseScreenInfo)
        {
            Map map = new Map(info.StagePath);

            GamePlay = new GamePlay();

            var Player = GameEntity.Get("Player", GamePlay);

            // TODO: Remove this circular dependency
            GamePlay.Player = Player;

            PauseScreen pauseScreen = null;
            if (pauseScreenInfo != null)
            {
                pauseScreen = new PauseScreen(pauseScreenInfo, GamePlay);
            }

            var joins = new Dictionary<Screen, Dictionary<Join, JoinHandler>>();
            var bossDoors = new Dictionary<Screen, Dictionary<Join, GameEntity>>();

            foreach (var screen in map.Screens.Values)
            {
                joins[screen] = new Dictionary<Join, JoinHandler>();
                bossDoors[screen] = new Dictionary<Join, GameEntity>();

                foreach (Join join in map.Joins)
                {
                    GameEntity door = null;
                    if (join.bossDoor)
                    {
                        door = GameEntity.Get(join.bossEntityName, GamePlay);
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
                    bossDoors[screenOne][join],
                    bossDoors[screenTwo][join]);

                joins[screenTwo].Add(join, handlerTwo);
            }

            var screens = new Dictionary<string, ScreenHandler>();
            foreach (var screen in map.Screens.Values)
            {
                screens[screen.Name] = CreateScreen(screen, joins[screen].Values.ToList());
            }

            var handler = new MapHandler(map, pauseScreen, screens, GamePlay);
            handler.WinHandler = info.WinHandler;
            handler.LoseHandler = info.LoseHandler;

            return handler;
        }

        private ScreenHandler CreateScreen(Screen screen, IEnumerable<JoinHandler> joins)
        {
            var patterns = new List<BlocksPattern>(screen.BlockPatternInfo.Count);

            foreach (BlockPatternInfo info in screen.BlockPatternInfo)
            {
                BlocksPattern pattern = new BlocksPattern(info, GamePlay);
                patterns.Add(pattern);
            }

            MapSquare[][] tiles = new MapSquare[screen.Height][];
            for (int y = 0; y < screen.Height; y++)
            {
                tiles[y] = new MapSquare[screen.Width];
                for (int x = 0; x < screen.Width; x++)
                {
                    try
                    {
                        Tile tile = screen.TileAt(x, y);
                        tiles[y][x] = new MapSquare(screen, tile, x, y, x * screen.Tileset.TileSize, y * screen.Tileset.TileSize);
                    }
                    catch
                    {
                        throw new GameEntityException("There's an error in map " + screen.Map.Name + ", screen file " + screen.Name + ".scn,\nthere's a bad tile number somewhere.");
                    }
                }
            }

            Music music = null;
            string intropath = (screen.MusicIntroPath != null) ? screen.MusicIntroPath.Absolute : null;
            string looppath = (screen.MusicLoopPath != null) ? screen.MusicLoopPath.Absolute : null;
            if (intropath != null || looppath != null) music = Engine.Instance.SoundSystem.LoadMusic(intropath, looppath, 1);

            return new ScreenHandler(screen, tiles, joins, patterns, music, GamePlay);
        }
        
        private JoinHandler CreateJoin(Join join, Screen screen, GameEntity door, GameEntity otherDoor)
        {
            if (join.bossDoor)
            {
                return new BossDoorHandler(door, otherDoor, join, screen.Tileset.TileSize, screen.PixelHeight, screen.PixelWidth, screen.Name);
            }

            return new JoinHandler(join, screen.Tileset.TileSize, screen.PixelHeight, screen.PixelWidth, screen.Name);
        }
    }
}
