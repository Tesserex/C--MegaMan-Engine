using System;
using System.Linq;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using MegaMan.Common;
using System.Collections.Generic;

namespace MegaMan.Engine
{
    public class StageHandler : IGameplayContainer
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

        private ScreenHandler _currentScreen;

        private Dictionary<string, ScreenHandler> screens;

        private JoinHandler currentJoin;
        private ScreenHandler nextScreen;

        public StageInfo Info { get; private set; }

        public HandlerTransfer WinHandler { get; set; }

        public HandlerTransfer LoseHandler { get; set; }

        public PositionComponent PlayerPos;

        # region IGameplayContainer Members

        public GameEntity Player { get; private set; }

        public IEntityContainer Entities { get { return _currentScreen; } }

        /// <summary>
        /// This is the first phase of game logic, but comes after the GameLogicTick event.
        /// During this phase, entities should "think" - decide what they want to do this frame.
        /// </summary>
        public event Action GameThink;

        /// <summary>
        /// During this phase, which comes between GameThink and GameReact, entities should carry out
        /// the actions decided during the thinking phase. Mainly used for movement.
        /// </summary>
        public event Action GameAct;

        /// <summary>
        /// This is the last logic phase, in which entities should react to the actions of other
        /// entities on the screen. Primarily used for collision detection and response.
        /// </summary>
        public event Action GameReact;

        /// <summary>
        /// The final phase before rendering. Used to delete entities,
        /// so please do not enumerate through entity collections during this phase. If you must,
        /// then make a copy first.
        /// </summary>
        public event Action GameCleanup;

        public event GameRenderEventHandler Draw;

        public event Action<HandlerTransfer> End;

        #endregion

        public StageHandler(StageInfo stage)
        {
            Info = stage;
            startScreen = Info.StartScreen;

            if (string.IsNullOrEmpty(startScreen)) startScreen = Info.Screens.Keys.First();
            startX = Info.PlayerStartX;
            startY = Info.PlayerStartY;

            string intropath = (stage.MusicIntroPath != null) ? stage.MusicIntroPath.Absolute : null;
            string looppath = (stage.MusicLoopPath != null) ? stage.MusicLoopPath.Absolute : null;
            if (intropath != null || looppath != null) music = Engine.Instance.SoundSystem.LoadMusic(intropath, looppath, 1);

            String imagePath = Path.Combine(Game.CurrentGame.BasePath, @"images\ready.png");
            readyImage = Image.FromFile(imagePath);
            StreamReader sr = new StreamReader(imagePath);
            readyTexture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, sr.BaseStream);

            stage.Tileset.SetTextures(Engine.Instance.GraphicsDevice);

            Player = GameEntity.Get("Player", this);
            PlayerPos = Player.GetComponent<PositionComponent>();
        }

        public void InitScreens(Dictionary<string, ScreenHandler> screens)
        {
            this.screens = screens;
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
            if (_currentScreen.Music != null) _currentScreen.Music.Stop();
            
            playerDeadCount = 0;
            updateFunc = DeadUpdate;
            Game.CurrentGame.Player.Lives--;
        }

        private void BeginPlay()
        {
            Player.Start();
            Player.GetComponent<SpriteComponent>().Visible = true;

            StateMessage msg = new StateMessage(null, "Teleport");
            PlayerPos.SetPosition(new PointF(startX, 0));
            Player.SendMessage(msg);
            Action teleport = () => {};
            teleport += () =>
            {
                if (PlayerPos.Position.Y >= startY)
                {
                    PlayerPos.SetPosition(new PointF(startX, startY));
                    Player.SendMessage(new StateMessage(null, "TeleportEnd"));
                    GameThink -= teleport;
                    updateFunc = Update;
                }
            };
            GameThink += teleport;
        }

        private void DrawScreen(SpriteBatch batch)
        {
            _currentScreen.Draw(batch, PlayerPos.Position);
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
                foreach (var screen in this.screens.Values)
                {
                    screen.Reset();
                }

                StartHandler();
            }
        }

        // swaps nextscreen for currentscreen and makes necessary adjustments to player
        // does not necessary represent the "end" of a scroll operation (boss doors still have to close)
        private void ScrollDone(JoinHandler join)
        {
            Player.Paused = false;
            join.ScrollDone -= ScrollDone;
            ChangeScreen(nextScreen);

            updateFunc = Update;
            drawFunc = DrawScreen;

            // check for continue points
            if (Info.ContinuePoints.ContainsKey(nextScreen.Screen.Name))
            {
                startScreen = nextScreen.Screen.Name;
                startX = Info.ContinuePoints[nextScreen.Screen.Name].X;
                startY = Info.ContinuePoints[nextScreen.Screen.Name].Y;
            }
        }

        private void ChangeScreen(ScreenHandler nextScreen)
        {
            ScreenHandler oldscreen = _currentScreen;
            _currentScreen = nextScreen;

            oldscreen.Clean();
            StartScreen();

            if (nextScreen.Music != null || nextScreen.Screen.MusicNsfTrack != 0)
            {
                if (music != null) music.Stop();
                if (Info.MusicNsfTrack != 0) Engine.Instance.SoundSystem.StopMusicNsf();
                
            }

            if (nextScreen.Screen.MusicNsfTrack != 0) Engine.Instance.SoundSystem.PlayMusicNSF((uint)nextScreen.Screen.MusicNsfTrack);
            else if (nextScreen.Music != null) nextScreen.Music.Play();
        }

        private void Update()
        {
            _currentScreen.Update();
        }

        private void OnScrollTriggered(JoinHandler join)
        {
            currentJoin = join;

            Player.Paused = true;
            nextScreen = screens[join.NextScreenName];
            join.BeginScroll(nextScreen, PlayerPos.Position);

            updateFunc = () => join.Update(PlayerPos);
            join.ScrollDone += ScrollDone;

            drawFunc = DrawJoin;

            StopScreen();
        }

        private void DrawJoin(SpriteBatch batch)
        {
            _currentScreen.Draw(batch, PlayerPos.Position, 0, 0, currentJoin.OffsetX, currentJoin.OffsetY);
            nextScreen.Draw(batch, PlayerPos.Position, currentJoin.NextScreenX, currentJoin.NextScreenY, currentJoin.NextOffsetX, currentJoin.NextOffsetY);
        }

        private void StartScreen()
        {
            _currentScreen.JoinTriggered += OnScrollTriggered;
            _currentScreen.Teleport += OnTeleport;
            _currentScreen.BossDefeated += BossDefeated;
            _currentScreen.Start(this, Player);
        }

        private void BossDefeated()
        {
            if (End != null)
            {
                End(WinHandler);
            }
        }

        private void StopScreen()
        {
            _currentScreen.JoinTriggered -= OnScrollTriggered;
            _currentScreen.Teleport -= OnTeleport;
            _currentScreen.Stop();
        }

        private bool teleporting = false;
        private void OnTeleport(TeleportInfo info)
        {
            if (teleporting) return;
            teleporting = true;
            Action<string> setpos = (s) => { };
            if (info.TargetScreen == _currentScreen.Screen.Name)
            {
                setpos = (state) =>
                {
                    PlayerPos.SetPosition(info.To);
                    (Player.GetComponent<StateComponent>()).StateChanged -= setpos;
                    Player.SendMessage(new StateMessage(null, "TeleportEnd"));
                    teleporting = false;
                    (Player.GetComponent<MovementComponent>()).CanMove = true;
                };
            }
            else
            {
                setpos = state =>
                {
                    (Player.GetComponent<SpriteComponent>()).Visible = false;
                    (Player.GetComponent<StateComponent>()).StateChanged -= setpos;
                    Engine.Instance.FadeTransition(
                        () => 
                    { 
                        StopScreen();
                        ChangeScreen(screens[info.TargetScreen]);
                        PlayerPos.SetPosition(info.To); // do it here so drawing is correct for fade-in
                    }, () =>
                    {
                        (Player.GetComponent<SpriteComponent>()).Visible = true;
                        Player.SendMessage(new StateMessage(null, "TeleportEnd"));
                        (Player.GetComponent<MovementComponent>()).CanMove = true;
                        teleporting = false;
                    });
                };
            }
            (Player.GetComponent<MovementComponent>()).CanMove = false;
            Player.SendMessage(new StateMessage(null, "TeleportBlink"));
            (Player.GetComponent<StateComponent>()).StateChanged += setpos;
        }

        #region IHandleGameEvents Members

        public void StartHandler()
        {
            Player.Death += Player_Death;

            PlayerPos = Player.GetComponent<PositionComponent>();
            PlayerPos.SetPosition(new PointF(startX, 0));

            if (!Info.Screens.ContainsKey(startScreen)) throw new GameRunException("The start screen for \""+Info.Name+"\" is supposed to be \""+startScreen+"\", but it doesn't exist!");
            _currentScreen = screens[startScreen];
            StartScreen();

            Engine.Instance.SoundSystem.StopMusicNsf();

            if (music != null) music.Play();
            if (Info.MusicNsfTrack != 0) Engine.Instance.SoundSystem.PlayMusicNSF((uint)Info.MusicNsfTrack);

            // updateFunc isn't set until BeginPlay
            drawFunc = DrawScreen;

            ResumeHandler();
            StartDrawing();

            // ready flashing
            readyBlinkTime = 0;
            readyBlinks = 0;
            Engine.Instance.GameRender += BlinkReady;

            Player.GetComponent<SpriteComponent>().Visible = false;

            // make sure we can move
            (Player.GetComponent<InputComponent>()).Paused = false;
        }

        public void StopHandler()
        {
            Player.Death -= Player_Death;

            if (_currentScreen != null)
            {
                StopScreen();
                _currentScreen.Clean();
            }

            if (music != null) music.Stop();
            if (Info.MusicNsfTrack != 0) Engine.Instance.SoundSystem.StopMusicNsf();

            PauseHandler();
            StopDrawing();

            Engine.Instance.GameRender -= BlinkReady;
        }

        private int pauseCount = 1; // starts paused

        public void PauseHandler()
        {
            if (pauseCount == 0)
            {
                Player.Paused = true;

                Engine.Instance.GameLogicTick -= GameTick;
                Engine.Instance.GameInputReceived -= GameInputReceived;
            }

            pauseCount++;
        }

        public void ResumeHandler()
        {
            if (pauseCount == 0) return;

            pauseCount--;

            if (pauseCount == 0)
            {
                Engine.Instance.GameLogicTick += GameTick;
                Engine.Instance.GameInputReceived += GameInputReceived;

                Player.Paused = false;
            }
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
            if (updateFunc == null || (Player.GetComponent<InputComponent>()).Paused) return;

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

            foreach (Tile t in Info.Tileset)
            {
                t.Sprite.Update();
            }

            if (GameThink != null) GameThink();
            if (GameAct != null) GameAct();
            if (GameReact != null) GameReact();
            if (GameCleanup != null) GameCleanup();
        }

        public void GameRender(GameRenderEventArgs e)
        {
            if (drawFunc != null && Engine.Instance.Background) drawFunc(e.Layers.BackgroundBatch);

            if (Draw != null) Draw(e);
        }

        #endregion
    }
}
