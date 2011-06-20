using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using MegaMan;

namespace Mega_Man
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

        private string currentPath;
        private IHandleGameEvents currentHandler;
        private StageSelect select;
        private PauseScreen pauseScreen;
        private readonly List<IHandleGameEvents> gameObjects;
        private readonly Dictionary<string, FilePath> stages;

        public MapHandler CurrentMap { get; private set; }
        public int PixelsAcross { get; private set; }
        public int PixelsDown { get; private set; }
        public float Gravity { get; private set; }
        public bool GravityFlip { get; set; }

        public bool Paused { get; private set; }

        public string BasePath { get; private set; }

        public static event EventHandler<ScreenSizeChangedEventArgs> ScreenSizeChanged;

        public int PlayerLives { get; set; }

        public static void Load(string path)
        {
            Engine.Instance.Begin();
            if (CurrentGame != null)
            {
                CurrentGame.Unload();
            }
            CurrentGame = new Game();
            CurrentGame.LoadFile(path);
        }

        private void StopHandlers()
        {
            List<IHandleGameEvents> temp = new List<IHandleGameEvents>(gameObjects);
            foreach (IHandleGameEvents handler in temp) handler.StopHandler();
        }

        public void Unload()
        {
            Engine.Instance.Stop();
            StopHandlers();
            GameEntity.UnloadAll();
            select = null;
            Engine.Instance.UnloadAudio();
            FontSystem.Unload();
            HealthMeter.Unload();
            CurrentGame = null;
        }

        public void Reset()
        {
            Unload();
            Load(currentPath);
        }

        private Game()
        {
            gameObjects = new List<IHandleGameEvents>();
            stages = new Dictionary<string, FilePath>();

            Gravity = 0.25f;
            GravityFlip = false;

            PlayerLives = 2;
        }

        private void LoadFile(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("The project file does not exist: " + path);

            project = new Project();
            project.Load(path);

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

            foreach (var stage in project.Stages)
            {
                stages.Add(stage.Name, stage.StagePath);
            }

            if (project.StageSelect != null) select = new StageSelect(project.StageSelect);

            if (project.PauseScreen != null) pauseScreen = new PauseScreen(project.PauseScreen);

            if (pauseScreen != null) pauseScreen.Unpaused += pauseScreen_Unpaused;

            foreach (string includePath in project.Includes)
            {
                string includefile = Path.Combine(BasePath, includePath);
                IncludeXmlFile(includefile);
            }

            currentPath = path;

            StageSelect();
        }

        private static void IncludeXmlFile(string path)
        {
            try
            {
                XDocument document = XDocument.Load(path, LoadOptions.SetLineInfo);
                foreach (XElement element in document.Elements())
                {
                    switch (element.Name.LocalName)
                    {
                        case "Entities":
                            GameEntity.LoadEntities(element);
                            break;

                        case "Sounds":
                            Engine.Instance.SoundSystem.LoadEffectsFromXml(element);
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

        private void select_MapSelected(string name)
        {
            if (!stages.ContainsKey(name)) return;

            select.MapSelected -= select_MapSelected;
            currentHandler.StopHandler();

            try
            {
                CurrentMap = new MapHandler(new Map(stages[name]));
            }
            catch (XmlException e)
            {
                throw new GameEntityException(String.Format("The map file for stage {0} has badly formatted XML:\n\n{1}", name, e.Message));
            }

            currentHandler = CurrentMap;
            CurrentMap.StartHandler();
            CurrentMap.Paused += CurrentMap_Paused;
            CurrentMap.End += CurrentMap_End;
            CurrentMap.Player.Death += () => { PlayerLives--; };
        }

        // do this when a map is won - should change to get weapon screen
        void CurrentMap_End()
        {
            EndMap();
            StageSelect();
        }

        void CurrentMap_Paused()
        {
            CurrentGame.Pause();
            if (pauseScreen != null) pauseScreen.Sound();
            Engine.Instance.FadeTransition(PauseScreen);
        }

        private void EndMap()
        {
            // includes the map
            StopHandlers();
            GameEntity.StopAll();
            CurrentMap.Paused -= CurrentMap_Paused;
            CurrentMap.End -= CurrentMap_End;
            CurrentMap = null;
        }

        private void StageSelect()
        {
            currentHandler = select;
            select.MapSelected += select_MapSelected;
            select.StartHandler();
        }

        private void PauseScreen()
        {
            if (pauseScreen != null)
            {
                CurrentMap.Pause();
                pauseScreen.StartHandler();
            }
        }

        void pauseScreen_Unpaused()
        {
            if (pauseScreen != null) pauseScreen.Sound();
            Engine.Instance.FadeTransition(UnPause, CurrentGame.Unpause);
        }

        private void UnPause()
        {
            CurrentMap.Unpause();
            if (pauseScreen != null)
            {
                pauseScreen.ApplyWeapon();
                pauseScreen.StopHandler();
            }
        }

        public void ResetMap()
        {
            if (CurrentMap == null) return;
            StopHandlers();
            GameEntity.StopAll();
            CurrentMap.StopHandler();

            if (PlayerLives < 0) // game over!
            {
                EndMap();
                StageSelect();
                PlayerLives = 2;
            }
            else
            {
                CurrentMap.StartHandler();
                CurrentMap.Player.Death += () => { PlayerLives--; };
            }
        }

        public void AddGameHandler(IHandleGameEvents handler)
        {
            gameObjects.Add(handler);
        }

        public void RemoveGameHandler(IHandleGameEvents handler)
        {
            gameObjects.Remove(handler);
        }

        public void Pause()
        {
            Paused = true;
        }

        public void Unpause()
        {
            Paused = false;
        }
    }
}
