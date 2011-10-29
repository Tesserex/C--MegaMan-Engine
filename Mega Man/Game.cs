using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using MegaMan.Common;

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

        private string currentPath;
        private IHandleGameEvents currentHandler;
        private StageSelect select;
        private readonly Dictionary<string, FilePath> stages;

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

        public void Unload()
        {
            Engine.Instance.Stop();
            currentHandler.StopHandler();
            GameEntity.UnloadAll();
            select = null;
            Engine.Instance.UnloadAudio();
            FontSystem.Unload();
            HealthMeter.Unload();
            Scene.Unload();
            CurrentGame = null;
        }

        public void Reset()
        {
            Unload();
            Load(currentPath);
        }

        private Game()
        {
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

            foreach (string includePath in project.Includes)
            {
                string includefile = Path.Combine(BasePath, includePath);
                IncludeXmlFile(includefile);
            }

            currentPath = path;

            if (project.TitleScene != null)
            {
                currentHandler = Scene.Get(project.TitleScene);
                currentHandler.End += EndHandler;
                currentHandler.StartHandler();
            }
            else
            {
                StageSelect();
            }
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

                        case "Functions":
                            EffectParser.LoadEffects(element);
                            break;

                        case "Sounds":
                            Engine.Instance.SoundSystem.LoadEffectsFromXml(element);
                            break;

                        case "Scene":
                            Scene.LoadScene(element);
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

        private void EndHandler(HandlerTransfer nextHandler)
        {
            currentHandler.End -= EndHandler;
            currentHandler.StopHandler();

            if (nextHandler != null)
            {
                switch (nextHandler.Type)
                {
                    case HandlerType.Scene:
                        StartScene(nextHandler.Name);
                        break;

                    case HandlerType.Stage:
                        StartMap(nextHandler.Name);
                        break;

                    case HandlerType.StageSelect:
                        StageSelect();
                        break;
                }
            }
        }

        private void StartScene(string name)
        {
            Engine.Instance.FadeTransition(() =>
            {
                var scene = Scene.Get(name);
                scene.End += EndHandler;
                scene.StartHandler();
                currentHandler = scene;
            });
        }

        private void StartMap(string name)
        {
            if (!stages.ContainsKey(name)) return;

            var factory = new MapFactory();

            try
            {
                var map = factory.CreateMap(new Map(stages[name]), project.PauseScreen);
                map.Player.Death += PlayerDied;
                currentHandler = map;
                currentHandler.StartHandler();
                currentHandler.End += EndHandler;
            }
            catch (XmlException e)
            {
                throw new GameEntityException(String.Format("The map file for stage {0} has badly formatted XML:\n\n{1}", name, e.Message));
            }
        }

        private void StageSelect()
        {
            currentHandler = select;
            select.End += EndHandler;
            select.StartHandler();
        }

        private void PlayerDied()
        {
            PlayerLives--;
        }

        public void ResetMap()
        {
            if (currentHandler == null) return;
            currentHandler.StopHandler();
            GameEntity.StopAll();

            if (PlayerLives < 0) // game over!
            {
                var next = new HandlerTransfer();
                next.Type = HandlerType.StageSelect;
                EndHandler(next);

                PlayerLives = 2;
            }
            else
            {
                currentHandler.StartHandler();
            }
        }

        public void Pause()
        {
            Paused = true;
        }

        public void Unpause()
        {
            Paused = false;
        }

        #region Debug Menu

        public void DebugEmptyHealth()
        {
            var map = currentHandler as MapHandler;
            if (map != null)
            {
                map.Player.SendMessage(new DamageMessage(null, float.PositiveInfinity));
            }
        }

        public void DebugFillHealth()
        {
            var map = currentHandler as MapHandler;
            if (map != null)
            {
                map.Player.SendMessage(new HealMessage(null, float.PositiveInfinity));
            }
        }

        public void DebugEmptyWeapon()
        {
            var map = currentHandler as MapHandler;
            if (map != null)
            {
                var weaponComponent = map.Player.GetComponent<WeaponComponent>();
                if (weaponComponent != null)
                {
                    weaponComponent.AddAmmo(-1 * weaponComponent.Ammo(weaponComponent.CurrentWeapon));
                }
            }
        }

        public void DebugFillWeapon()
        {
            var map = currentHandler as MapHandler;
            if (map != null)
            {
                var weaponComponent = map.Player.GetComponent<WeaponComponent>();
                if (weaponComponent != null)
                {
                    weaponComponent.AddAmmo(weaponComponent.MaxAmmo(weaponComponent.CurrentWeapon));
                }
            }
        }

        #endregion
    }
}
