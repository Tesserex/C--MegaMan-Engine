using System;
using System.Linq;
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

        public int PixelsAcross { get; private set; }
        public int PixelsDown { get; private set; }
        public float Gravity { get; private set; }
        public bool GravityFlip { get; set; }

        public bool Paused { get; private set; }

        public string BasePath { get; private set; }

        public Player Player { get; private set; }

        public static event EventHandler<ScreenSizeChangedEventArgs> ScreenSizeChanged;

        public static void Load(string path)
        {
            Engine.Instance.Begin();
            if (CurrentGame != null)
            {
                CurrentGame.Unload();
            }
            CurrentGame = new Game();
            CurrentGame.LoadFile(path);
            // TODO: load fonts from xml
            FontSystem.LoadFont("Big", Path.Combine(Game.CurrentGame.BasePath, @"images\font.png"), 8, 0);
            FontSystem.LoadFont("Boss", Path.Combine(Game.CurrentGame.BasePath, @"images\font_boss.png"), 8, 0);
        }

        public void Unload()
        {
            Engine.Instance.Stop();
            currentHandler.StopHandler();
            GameEntity.UnloadAll();
            Engine.Instance.UnloadAudio();
            FontSystem.Unload();
            HealthMeter.Unload();
            Scene.Unload();
            Menu.Unload();
            CurrentGame = null;
        }

        public void Reset()
        {
            Unload();
            Load(currentPath);
        }

        private Game()
        {
            Gravity = 0.25f;
            GravityFlip = false;
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

            foreach (string includePath in project.Includes)
            {
                string includefile = Path.Combine(BasePath, includePath);
                IncludeXmlFile(includefile);
            }

            currentPath = path;

            if (project.StartHandler != null)
            {
                StartHandler(project.StartHandler);
            }
            else
            {
                throw new GameEntityException("The game file loaded correctly, but it failed to specify a starting point!");
            }

            Player = new Player();
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

                        case "Menu":
                            Menu.Load(element);
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

            if (nextHandler.Fade)
            {
                Engine.Instance.FadeTransition(() =>
                {
                    currentHandler.StopHandler();
                    StartHandler(nextHandler);
                });
            }
            else
            {
                currentHandler.StopHandler();
                StartHandler(nextHandler);
            }
        }

        private void StartHandler(HandlerTransfer handler)
        {
            if (handler != null)
            {
                switch (handler.Type)
                {
                    case HandlerType.Scene:
                        StartScene(handler.Name);
                        break;

                    case HandlerType.Stage:
                        StartMap(handler.Name);
                        break;

                    case HandlerType.StageSelect:
                        StageSelect(handler.Name);
                        break;

                    case HandlerType.Menu:
                        StartMenu(handler.Name);
                        break;
                }
            }
        }

        private void StartMenu(string name)
        {
            var menu = Menu.Get(name);
            menu.End += EndHandler;
            menu.StartHandler();
            currentHandler = menu;
        }

        private void StartScene(string name)
        {
            var scene = Scene.Get(name);
            scene.End += EndHandler;
            scene.StartHandler();
            currentHandler = scene;
        }

        private void StartMap(string name)
        {
            var stage = project.Stages.FirstOrDefault(s => s.Name == name);
            if (stage == null)
            {
                throw new GameEntityException(String.Format("I couldn't find a stage called {0}. Sorry.", name));
            }

            var factory = new MapFactory();

            try
            {
                var map = factory.CreateMap(stage, project.PauseScreen);
                currentHandler = map;
                currentHandler.StartHandler();
                currentHandler.End += EndHandler;
            }
            catch (XmlException e)
            {
                throw new GameEntityException(String.Format("The map file for stage {0} has badly formatted XML:\n\n{1}", name, e.Message));
            }
        }

        private void StageSelect(string name)
        {
            var selectInfo = project.StageSelects.FirstOrDefault(s => s.Name == name);
            if (selectInfo == null)
            {
                throw new GameEntityException(String.Format("I couldn't find a stage select called {0}. Sorry.", name));
            }

            var select = new StageSelect(selectInfo);
            currentHandler = select;
            select.End += EndHandler;
            select.StartHandler();
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
                map.GamePlay.Player.SendMessage(new DamageMessage(null, float.PositiveInfinity));
            }
        }

        public void DebugFillHealth()
        {
            var map = currentHandler as MapHandler;
            if (map != null)
            {
                map.GamePlay.Player.SendMessage(new HealMessage(null, float.PositiveInfinity));
            }
        }

        public void DebugEmptyWeapon()
        {
            var map = currentHandler as MapHandler;
            if (map != null)
            {
                var weaponComponent = map.GamePlay.Player.GetComponent<WeaponComponent>();
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
                var weaponComponent = map.GamePlay.Player.GetComponent<WeaponComponent>();
                if (weaponComponent != null)
                {
                    weaponComponent.AddAmmo(weaponComponent.MaxAmmo(weaponComponent.CurrentWeapon));
                }
            }
        }

        #endregion
    }
}
