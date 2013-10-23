using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.IO.Xml;
using MegaMan.Engine.Entities;
using MegaMan.Engine.StateMachine;

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
            _stateMachine.Unload();
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
            var projectReader = new ProjectXmlReader();

            project = projectReader.FromXml(path);

            BasePath = project.BaseDir;

            PixelsDown = project.ScreenHeight;
            PixelsAcross = project.ScreenWidth;

            if (ScreenSizeChanged != null)
            {
                ScreenSizeChangedEventArgs args = new ScreenSizeChangedEventArgs(PixelsAcross, PixelsDown);
                ScreenSizeChanged(this, args);
            }

            if (project.MusicNSF != null) Engine.Instance.SoundSystem.LoadMusicNSF(project.MusicNSF.Absolute);
            if (project.EffectsNSF != null) Engine.Instance.SoundSystem.LoadSfxNSF(project.EffectsNSF.Absolute);

            foreach (var stageInfo in project.Stages)
            {
                stageFactory.Load(stageInfo);
            }

            var includeReader = new IncludeFileXmlReader();

            foreach (string includePath in project.Includes)
            {
                string includefile = Path.Combine(BasePath, includePath);
                IncludeXmlFile(includefile);
                includeReader.LoadIncludedFile(project, includefile);
            }

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

        private void IncludeXmlFile(string path)
        {
            try
            {
                XDocument document = XDocument.Load(path, LoadOptions.SetLineInfo);
                foreach (XElement element in document.Elements())
                {
                    switch (element.Name.LocalName)
                    {
                        case "Entities":
                            _entitySource.LoadEntities(element);
                            _tileProperties.LoadProperties(element);
                            break;

                        case "Functions":
                            EffectParser.LoadEffectsList(element);
                            break;

                        case "Sounds":
                        case "Scenes":
                        case "Scene":
                        case "Menus":
                        case "Menu":
                        case "Fonts":
                        case "Palettes":
                            break;

                        default:
                            throw new GameXmlException(element, string.Format("Unrecognized XML type: \"{0}\"", element.Name.LocalName));
                    }
                }
            }
            catch (GameXmlException ex)
            {
                ex.File = path;
                throw;
            }
        }

        public void ProcessHandler(HandlerTransfer handler)
        {
            _stateMachine.ProcessHandler(handler);
        }

        #region Debug Menu

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

        public static int DebugEntitiesAlive()
        {
            if (CurrentGame != null)
                return CurrentGame._entityPool.GetTotalAlive();
            else
                return 0;
        }

        public bool DebugFlipGravity()
        {
            return _stateMachine.DebugFlipGravity();
        }

        public static int XmlNumAlive(string name)
        {
            if (CurrentGame != null)
                return CurrentGame._entityPool.GetNumberAlive(name);
            else
                return 0;
        }
        #endregion
    }
}
