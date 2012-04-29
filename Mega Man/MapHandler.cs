using System;
using System.Linq;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using MegaMan.Common;
using System.Collections.Generic;

namespace MegaMan.Engine
{
    public class MapHandler : IHandleGameEvents
    {
        private int playerDeadCount;

        private Action updateFunc;
        private Action<SpriteBatch> drawFunc;

        private string startScreen;
        private int startX, startY;

        private int readyBlinkTime;
        private int readyBlinks;
        private readonly Image readyImage;
        private readonly Texture2D readyTexture;

        private readonly Music music;

        private Dictionary<string, ScreenHandler> screens;

        public Dictionary<string, bool[]> EntityRespawnable
        {
            get;
            private set;
        }

        private JoinHandler currentJoin;
        private ScreenHandler nextScreen;

        public GamePlay GamePlay { get; private set; }

        public Map Map { get; private set; }

        public HandlerTransfer WinHandler { get; set; }

        public HandlerTransfer LoseHandler { get; set; }

        public ScreenHandler CurrentScreen { get; private set; }

        public PositionComponent PlayerPos;

        public event Action<HandlerTransfer> End;

        public MapHandler(Map map, Dictionary<string, ScreenHandler> screens, GamePlay gamePlay)
        {
            Map = map;
            startScreen = Map.StartScreen;
            if (string.IsNullOrEmpty(startScreen)) startScreen = Map.Screens.Keys.First();
            startX = Map.PlayerStartX;
            startY = Map.PlayerStartY;

            string intropath = (map.MusicIntroPath != null) ? map.MusicIntroPath.Absolute : null;
            string looppath = (map.MusicLoopPath != null) ? map.MusicLoopPath.Absolute : null;
            if (intropath != null || looppath != null) music = Engine.Instance.SoundSystem.LoadMusic(intropath, looppath, 1);

            String imagePath = Path.Combine(Game.CurrentGame.BasePath, @"images\ready.png");
            readyImage = Image.FromFile(imagePath);
            StreamReader sr = new StreamReader(imagePath);
            readyTexture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, sr.BaseStream);

            map.Tileset.SetTextures(Engine.Instance.GraphicsDevice);

            this.screens = screens;

            this.EntityRespawnable = new Dictionary<string, bool[]>();

            this.GamePlay = gamePlay;
            PlayerPos = gamePlay.Player.GetComponent<PositionComponent>();
        }

        public void SetTestingStartPosition(string screen, Point startPosition)
        {
            startScreen = screen;
            startX = startPosition.X;
            startY = startPosition.Y;
        }

        void BlinkReady(GameRenderEventArgs e)
        {
            if (readyBlinkTime >= 0)
            {
                if (Engine.Instance.Foreground) 
                {
                    e.Layers.ForegroundBatch.Draw(
                        readyTexture,
                        new Microsoft.Xna.Framework.Vector2(
                            (Game.CurrentGame.PixelsAcross - readyImage.Width) / 2,
                            ((Game.CurrentGame.PixelsDown - readyImage.Height) / 2) - 24
                        ),
                        e.OpacityColor);
                }
            }
            readyBlinkTime++;
            if (readyBlinkTime > 8)
            {
                readyBlinkTime = -8;
                readyBlinks++;
                if (readyBlinks >= 8)
                {
                    Engine.Instance.GameRender -= BlinkReady;
                    BeginPlay();
                }
            }
        }

        private void Player_Death()
        {
            if (music != null) music.Stop();
            Engine.Instance.SoundSystem.StopMusicNsf();
            if (CurrentScreen.Music != null) CurrentScreen.Music.Stop();
            
            playerDeadCount = 0;
            updateFunc = DeadUpdate;
            Game.CurrentGame.Player.Lives--;
        }

        private void BeginPlay()
        {
            GamePlay.Player.Start(CurrentScreen);
            GamePlay.Player.GetComponent<SpriteComponent>().Visible = true;

            StateMessage msg = new StateMessage(null, "Teleport");
            PlayerPos.SetPosition(new PointF(startX, 0));
            GamePlay.Player.SendMessage(msg);
            Action teleport = () => {};
            teleport += () =>
            {
                if (PlayerPos.Position.Y >= startY)
                {
                    PlayerPos.SetPosition(new PointF(startX, startY));
                    GamePlay.Player.SendMessage(new StateMessage(null, "TeleportEnd"));
                    GamePlay.GameThink -= teleport;
                    updateFunc = Update;
                }
            };
            GamePlay.GameThink += teleport;
        }

        private void Draw(SpriteBatch batch)
        {
            CurrentScreen.Draw(batch, PlayerPos.Position);
        }

        private void DeadUpdate()
        {
            playerDeadCount++;
            if (playerDeadCount >= Const.MapDeadFrames)
            {
                updateFunc = null;
                Engine.Instance.FadeTransition(Reset);
            }
        }

        private void Reset()
        {
            StopHandler();
            GameEntity.StopAll();

            if (Game.CurrentGame.Player.Lives < 0) // game over!
            {
                if (End != null) End(LoseHandler);
            }
            else
            {
                // enable respawn for on-death-respawn entities
                foreach (var pair in this.screens)
                {
                    var screen = pair.Value;
                    var respawns = this.EntityRespawnable[pair.Key];
                    for (int i = 0; i < screen.Screen.EnemyInfo.Count; i++)
                    {
                        if (screen.Screen.EnemyInfo[i].respawn == RespawnBehavior.Death)
                        {
                            respawns[i] = true;
                        }
                    }
                }

                StartHandler();
            }
        }

        // swaps nextscreen for currentscreen and makes necessary adjustments to player
        // does not necessary represent the "end" of a scroll operation (boss doors still have to close)
        private void ScrollDone(JoinHandler join)
        {
            GamePlay.Player.Paused = false;
            join.ScrollDone -= ScrollDone;
            ChangeScreen(nextScreen);

            updateFunc = Update;
            drawFunc = Draw;

            // check for continue points
            if (Map.ContinuePoints.ContainsKey(nextScreen.Screen.Name))
            {
                startScreen = nextScreen.Screen.Name;
                startX = Map.ContinuePoints[nextScreen.Screen.Name].X;
                startY = Map.ContinuePoints[nextScreen.Screen.Name].Y;
            }
        }

        private void ChangeScreen(ScreenHandler nextScreen)
        {
            ScreenHandler oldscreen = CurrentScreen;
            CurrentScreen = nextScreen;
            GamePlay.Player.Screen = CurrentScreen;
            oldscreen.Clean();
            StartScreen();

            if (nextScreen.Music != null || nextScreen.Screen.MusicNsfTrack != 0)
            {
                if (music != null) music.Stop();
                if (Map.MusicNsfTrack != 0) Engine.Instance.SoundSystem.StopMusicNsf();
                
            }

            if (nextScreen.Screen.MusicNsfTrack != 0) Engine.Instance.SoundSystem.PlayMusicNSF((uint)nextScreen.Screen.MusicNsfTrack);
            else if (nextScreen.Music != null) nextScreen.Music.Play();
        }

        private void Update()
        {
            CurrentScreen.Update();
        }

        private void OnScrollTriggered(JoinHandler join)
        {
            currentJoin = join;

            GamePlay.Player.Paused = true;
            nextScreen = screens[join.NextScreenName];
            join.BeginScroll(nextScreen, PlayerPos.Position);

            updateFunc = () => join.Update(PlayerPos);
            join.ScrollDone += ScrollDone;

            drawFunc = DrawJoin;

            StopScreen();
        }

        private void DrawJoin(SpriteBatch batch)
        {
            CurrentScreen.Draw(batch, PlayerPos.Position, 0, 0, currentJoin.OffsetX, currentJoin.OffsetY);
            nextScreen.Draw(batch, PlayerPos.Position, currentJoin.NextScreenX, currentJoin.NextScreenY, currentJoin.NextOffsetX, currentJoin.NextOffsetY);
        }

        private void StartScreen()
        {
            CurrentScreen.JoinTriggered += OnScrollTriggered;
            CurrentScreen.Teleport += OnTeleport;
            CurrentScreen.BossDefeated += BossDefeated;
            CurrentScreen.Start(this);
        }

        private void BossDefeated()
        {
            GamePlay.EndPlay();
            if (End != null && WinHandler != null)
            {
                End(WinHandler);
            }
        }

        private void StopScreen()
        {
            CurrentScreen.JoinTriggered -= OnScrollTriggered;
            CurrentScreen.Teleport -= OnTeleport;
            CurrentScreen.Stop();
        }

        private bool teleporting = false;
        private void OnTeleport(TeleportInfo info)
        {
            if (teleporting) return;
            teleporting = true;
            Action<string> setpos = (s) => { };
            if (info.TargetScreen == CurrentScreen.Screen.Name)
            {
                setpos = (state) =>
                {
                    PlayerPos.SetPosition(info.To);
                    (GamePlay.Player.GetComponent<StateComponent>()).StateChanged -= setpos;
                    GamePlay.Player.SendMessage(new StateMessage(null, "TeleportEnd"));
                    teleporting = false;
                    (GamePlay.Player.GetComponent<MovementComponent>()).CanMove = true;
                };
            }
            else
            {
                setpos = state =>
                {
                    (GamePlay.Player.GetComponent<SpriteComponent>()).Visible = false;
                    (GamePlay.Player.GetComponent<StateComponent>()).StateChanged -= setpos;
                    Engine.Instance.FadeTransition(
                        () => 
                    { 
                        StopScreen();
                        ChangeScreen(screens[info.TargetScreen]);
                        PlayerPos.SetPosition(info.To); // do it here so drawing is correct for fade-in
                    }, () =>
                    {
                        (GamePlay.Player.GetComponent<SpriteComponent>()).Visible = true;
                        GamePlay.Player.SendMessage(new StateMessage(null, "TeleportEnd"));
                        (GamePlay.Player.GetComponent<MovementComponent>()).CanMove = true;
                        teleporting = false;
                    });
                };
            }
            (GamePlay.Player.GetComponent<MovementComponent>()).CanMove = false;
            GamePlay.Player.SendMessage(new StateMessage(null, "TeleportBlink"));
            (GamePlay.Player.GetComponent<StateComponent>()).StateChanged += setpos;
        }

        #region IHandleGameEvents Members

        public void StartHandler()
        {
            GamePlay.Player.Death += Player_Death;

            PlayerPos = GamePlay.Player.GetComponent<PositionComponent>();
            PlayerPos.SetPosition(new PointF(startX, 0));

            if (!Map.Screens.ContainsKey(startScreen)) throw new GameRunException("The start screen for \""+Map.Name+"\" is supposed to be \""+startScreen+"\", but it doesn't exist!");
            CurrentScreen = screens[startScreen];
            StartScreen();

            Engine.Instance.SoundSystem.StopMusicNsf();

            if (music != null) music.Play();
            if (Map.MusicNsfTrack != 0) Engine.Instance.SoundSystem.PlayMusicNSF((uint)Map.MusicNsfTrack);

            // updateFunc isn't set until BeginPlay
            drawFunc = Draw;

            ResumeHandler();
            StartDrawing();

            // ready flashing
            readyBlinkTime = 0;
            readyBlinks = 0;
            Engine.Instance.GameRender += BlinkReady;

            GamePlay.Player.GetComponent<SpriteComponent>().Visible = false;

            // make sure we can move
            (GamePlay.Player.GetComponent<InputComponent>()).Paused = false;
        }

        public void StopHandler()
        {
            GamePlay.Player.Death -= Player_Death;

            if (CurrentScreen != null)
            {
                StopScreen();
                CurrentScreen.Clean();
            }

            if (music != null) music.Stop();
            if (Map.MusicNsfTrack != 0) Engine.Instance.SoundSystem.StopMusicNsf();

            PauseHandler();
            StopDrawing();

            Engine.Instance.GameRender -= BlinkReady;
        }

        private bool running;

        public void PauseHandler()
        {
            if (!running) return;
            GamePlay.PauseHandler();
            GamePlay.Player.Paused = true;

            Engine.Instance.GameLogicTick -= GameTick;
            Engine.Instance.GameInputReceived -= GameInputReceived;
            
            running = false;
        }

        public void ResumeHandler()
        {
            if (running) return;
            Engine.Instance.GameLogicTick += GameTick;
            Engine.Instance.GameInputReceived += GameInputReceived;

            GamePlay.ResumeHandler();
            GamePlay.Player.Paused = false;
            running = true;
        }

        public void StopDrawing()
        {
            Engine.Instance.GameRender -= GameRender;
        }

        public void StartDrawing()
        {
            Engine.Instance.GameRender += GameRender;
        }

        private void GameInputReceived(GameInputEventArgs e)
        {
            if (updateFunc == null || (GamePlay.Player.GetComponent<InputComponent>()).Paused) return;

            /* This might be useful even though pause screens are replaced
            if (e.Input == GameInput.Start && e.Pressed)
            {
                if (Game.CurrentGame.Paused)
                {
                    Game.CurrentGame.Unpause();
                }
                else
                {
                    Game.CurrentGame.Pause();
                }
            }
            */
        }

        private void GameTick(GameTickEventArgs e)
        {
            if (updateFunc != null) updateFunc();

            foreach (Tile t in Map.Tileset)
            {
                t.Sprite.Update();
            }
        }

        public void GameRender(GameRenderEventArgs e)
        {
            if (drawFunc != null && Engine.Instance.Background) drawFunc(e.Layers.BackgroundBatch);
        }

        #endregion
    }
}
