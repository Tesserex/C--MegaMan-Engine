using System;
using System.Collections.Generic;
using System.Linq;
using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.Engine.Entities;
using MegaMan.Engine.StateMachine;
using MegaMan.IO;

namespace MegaMan.Engine
{
    // These args, and the event, are used to forcibly resize
    // the window and screen when the game is loaded, since
    // the game can specify its size in pixels.
    public class ScreenSizeChangedEventArgs : EventArgs
    {
        public int PixelsAcross { get; private set; }
        public int PixelsDown { get; private set; }

        public ScreenSizeChangedEventArgs(int across, int down)
        {
            PixelsAcross = across;
            PixelsDown = down;
        }
    }

    public class Game
    {
        public static Game CurrentGame { get; private set; }

        public IReaderProvider FileReaderProvider { get; private set; }

        private Project project;
        private StageFactory stageFactory;

        private string currentPath;

        private GameEntitySource _entitySource;
        private GameEntityPool _entityPool;
        private GameTilePropertiesSource _tileProperties;
        private IEntityRespawnTracker _respawnTracker;

        private readonly GameStateMachine _stateMachine;

        public int PixelsAcross { get; private set; }
        public int PixelsDown { get; private set; }

        public ITilePropertiesSource TileProperties { get { return _tileProperties; } }

        public string Name
        {
            get
            {
                if (project == null) return "Mega Man";
                if (string.IsNullOrWhiteSpace(project.Name)) return "Untitled Fan Game";
                return project.Name;
            }
        }

        public string BasePath { get; private set; }

        public Player Player { get; private set; }

        public static event EventHandler<ScreenSizeChangedEventArgs> ScreenSizeChanged;

        public static void Load(string path, List<string> pathArgs = null)
        {
            Engine.Instance.Begin();
            if (CurrentGame != null)
            {
                CurrentGame.Unload();
            }
            CurrentGame = new Game();
            CurrentGame.LoadFile(path, pathArgs);
        }

        public void Unload()
        {
            Engine.Instance.Stop();
            _stateMachine.StopAllHandlers();
            _entitySource.Unload();
            Engine.Instance.UnloadAudio();
            FontSystem.Unload();
            HealthMeter.Unload();
            Scene.Unload();
            Menu.Unload();
            PaletteSystem.Unload();
            EffectParser.Unload();
            CurrentGame = null;
        }

        public void Reset()
        {
            Unload();
            Load(currentPath);
        }

        private Game()
        {
            _entitySource = new GameEntitySource();
            _entityPool = new GameEntityPool(_entitySource);
            _tileProperties = new GameTilePropertiesSource();
            _respawnTracker = new GameEntityRespawnTracker();
            stageFactory = new StageFactory(_entityPool, _respawnTracker);
            _stateMachine = new GameStateMachine(_entityPool, stageFactory);
        }

        private void LoadFile(string path, List<string> pathArgs = null)
        {
            var projectLoader = new GameLoader();
            this.FileReaderProvider = projectLoader.Load(path);
            project = FileReaderProvider.GetProjectReader().Load();

            BasePath = project.BaseDir;

            PixelsDown = project.ScreenHeight;
            PixelsAcross = project.ScreenWidth;

            if (ScreenSizeChanged != null)
            {
                ScreenSizeChangedEventArgs args = new ScreenSizeChangedEventArgs(PixelsAcross, PixelsDown);
                ScreenSizeChanged(this, args);
            }

            var nsfReader = FileReaderProvider.GetRawReader();
            if (project.MusicNSF != null)
            {
                var musicData = nsfReader.GetRawData(project.MusicNSF);
                Engine.Instance.SoundSystem.LoadMusicNSF(musicData);
            }

            if (project.EffectsNSF != null)
            {
                var sfxData = nsfReader.GetRawData(project.EffectsNSF);
                Engine.Instance.SoundSystem.LoadSfxNSF(sfxData);
            }

            foreach (var stageInfo in project.Stages)
            {
                stageFactory.Load(stageInfo);
            }

            _tileProperties.LoadProperties(project.EntityProperties);
            _entitySource.LoadEntities(project.Entities);
            EffectParser.LoadEffectsList(project.Functions);
            Engine.Instance.SoundSystem.LoadEffectsFromInfo(project.Sounds);
            Scene.LoadScenes(project.Scenes);
            Menu.LoadMenus(project.Menus);
            FontSystem.Load(project.Fonts);
            PaletteSystem.LoadPalettes(project.Palettes);

            currentPath = path;

            if (pathArgs != null && pathArgs.Any())
            {
                ProcessCommandLineArgs(pathArgs);
            }
            else if (project.StartHandler != null)
            {
                _stateMachine.ProcessHandler(project.StartHandler);
            }
            else
            {
                throw new GameRunException("The game file loaded correctly, but it failed to specify a starting point!");
            }

            Player = new Player();
        }

        private void ProcessCommandLineArgs(List<string> pathArgs)
        {
            var start = pathArgs[0];

            var parts = start.Split('\\');
            if (parts.Length != 2)
            {
                throw new GameRunException("The starting point given by command line argument was invalid.");
            }
            var name = parts[1];
            switch (parts[0].ToUpper())
            {
                case "SCENE":
                    _stateMachine.StartScene(new HandlerTransfer() { Name = name, Mode = HandlerMode.Next });
                    break;

                case "STAGE":
                    var screen = (pathArgs.Count > 1) ? pathArgs[1] : null;
                    Point? startPos = null;
                    if (pathArgs.Count > 2)
                    {
                        var point = pathArgs[2];
                        var coords = point.Split(',');
                        startPos = new Point(int.Parse(coords[0]), int.Parse(coords[1]));
                    }
                    _stateMachine.StartStage(name, screen, startPos);
                    break;

                case "MENU":
                    _stateMachine.StartMenu(new HandlerTransfer() { Name = name, Mode = HandlerMode.Next });
                    break;

                default:
                    throw new GameRunException("The starting point given by command line argument was invalid.");
            }
        }

        public void ProcessHandler(HandlerTransfer handler)
        {
            _stateMachine.ProcessHandler(handler);
        }

        public static int DebugEntitiesAlive()
        {
            if (CurrentGame != null)
                return CurrentGame._entityPool.GetTotalAlive();
            else
                return 0;
        }

        public static int XmlNumAlive(string name)
        {
            if (CurrentGame != null)
                return CurrentGame._entityPool.GetNumberAlive(name);
            else
                return 0;
        }

#if DEBUG
        public void DebugEmptyHealth()
        {
            _stateMachine.DebugEmptyHealth();
        }

        public void DebugFillHealth()
        {
            _stateMachine.DebugFillHealth();
        }

        public void DebugEmptyWeapon()
        {
            _stateMachine.DebugEmptyWeapon();
        }

        public void DebugFillWeapon()
        {
            _stateMachine.DebugFillWeapon();
        }

        public bool DebugFlipGravity()
        {
            return _stateMachine.DebugFlipGravity();
        }

        public bool GetFlipGravity()
        {
            return _stateMachine.GetFlipGravity();
        }
#endif
    }
}
