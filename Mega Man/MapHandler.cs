using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;

namespace Mega_Man
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
        private Image readyImage;
        private Texture2D readyTexture;

        public Music music;

        public MegaMan.Map Map { get; private set; }
        public float OffsetX { get; private set; }
        public float OffsetY { get; private set; }
        public MegaMan.Tileset Tileset { get { return Map.Tileset; } }

        public ScreenHandler CurrentScreen { get; private set; }
        public ScreenHandler NextScreen { get; private set; }

        public GameEntity Player { get; private set; }

        public PositionComponent PlayerPos;

        public event Action Paused;
        public event Action End;

        public MapHandler(MegaMan.Map map)
        {
            Map = map;
            this.startScreen = Map.StartScreen;
            if (string.IsNullOrEmpty(this.startScreen)) this.startScreen = Map.Screens.Keys.First();
            this.startX = Map.PlayerStartX;
            this.startY = Map.PlayerStartY;

            string intropath = (map.MusicIntroPath != null) ? map.MusicIntroPath.Absolute : null;
            string looppath = (map.MusicLoopPath != null) ? map.MusicLoopPath.Absolute : null;
            if (intropath != null || looppath != null || map.MusicNsfTrack > 0) music = Engine.Instance.SoundSystem.LoadMusic(intropath, looppath, 1, map.MusicNsfTrack);

            readyImage = Image.FromFile(System.IO.Path.Combine(Game.CurrentGame.BasePath, @"images\ready.png"));
            readyTexture = Texture2D.FromFile(Engine.Instance.GraphicsDevice, System.IO.Path.Combine(Game.CurrentGame.BasePath, @"images\ready.png"));

            map.Tileset.SetTextures(Engine.Instance.GraphicsDevice);
        }

        public bool IsOnScreen(float x, float y)
        {
            x -= CurrentScreen.OffsetX;
            y -= CurrentScreen.OffsetY;
            return x >= OffsetX && y >= OffsetY &&
                x <= OffsetX + Game.CurrentGame.PixelsAcross &&
                y <= OffsetY + Game.CurrentGame.PixelsDown;
        }

        void BlinkReady(GameRenderEventArgs e)
        {
            if (readyBlinkTime >= 0)
            {
                e.Layers.ForegroundBatch.Draw(readyTexture, new Microsoft.Xna.Framework.Vector2((Game.CurrentGame.PixelsAcross - readyImage.Width) / 2, ((Game.CurrentGame.PixelsDown - readyImage.Height) / 2) - 24), e.OpacityColor);
            }
            readyBlinkTime++;
            if (readyBlinkTime > 8)
            {
                readyBlinkTime = -8;
                readyBlinks++;
                if (readyBlinks >= 8)
                {
                    Engine.Instance.GameRender -= new GameRenderEventHandler(BlinkReady);
                    BeginPlay();
                }
            }
        }

        private void Player_Death()
        {
            if (music != null) music.FadeOut(60);
            if (CurrentScreen.music != null) CurrentScreen.music.FadeOut(60);
            
            playerDeadCount = 0;
            updateFunc = DeadUpdate;
        }

        private void BeginPlay()
        {
            Player.Start();
            StateMessage msg = new StateMessage(null, "Teleport");
            PlayerPos.SetPosition(new PointF(this.startX, 0));
            Player.SendMessage(msg);
            Action teleport = () => {};
            teleport += () =>
            {
                if (PlayerPos.Position.Y >= this.startY)
                {
                    PlayerPos.SetPosition(new PointF(this.startX, this.startY));
                    Player.SendMessage(new StateMessage(null, "TeleportEnd"));
                    Engine.Instance.GameThink -= teleport;
                    updateFunc = Update;
                }
            };
            Engine.Instance.GameThink += teleport;
        }

        private void Draw(SpriteBatch batch)
        {
            CurrentScreen.Draw(batch);
        }

        private void DeadUpdate()
        {
            playerDeadCount++;
            if (playerDeadCount >= Const.MapDeadFrames)
            {
                updateFunc = null;
                Engine.Instance.FadeTransition(Game.CurrentGame.ResetMap);
            }
        }

        // swaps nextscreen for currentscreen and makes necessary adjustments to player
        // does not necessary represent the "end" of a scroll operation (boss doors still have to close)
        private void ScrollDone(JoinHandler join, ScreenHandler nextScreen)
        {
            Player.Paused = false;
            join.ScrollDone -= ScrollDone;
            ChangeScreen(nextScreen);

            updateFunc = Update;
            drawFunc = Draw;

            // check for continue points
            if (Map.ContinuePoints.ContainsKey(nextScreen.Screen.Name))
            {
                this.startScreen = nextScreen.Screen.Name;
                this.startX = Map.ContinuePoints[nextScreen.Screen.Name].X;
                this.startY = Map.ContinuePoints[nextScreen.Screen.Name].Y;
            }
        }

        private void ChangeScreen(ScreenHandler nextScreen)
        {
            ScreenHandler oldscreen = CurrentScreen;
            CurrentScreen = nextScreen;
            Player.Screen = CurrentScreen;
            oldscreen.Clean();
            StartScreen();

            if (nextScreen.music != null)
            {
                if (music != null) music.Stop();
                nextScreen.music.Play();
            }
        }

        private void Update()
        {
            CurrentScreen.Update();
        }

        private void OnScrollTriggered(JoinHandler join)
        {
            Player.Paused = true;
            join.BeginScroll(new ScreenHandler(Map.Screens[join.NextScreenName], PlayerPos, Map.Joins), PlayerPos.Position);
            updateFunc = () => join.Update(PlayerPos);
            join.ScrollDone += ScrollDone;

            drawFunc = (b) => { join.Draw(b); };

            StopScreen();
        }

        private void StartScreen()
        {
            CurrentScreen.JoinTriggered += OnScrollTriggered;
            CurrentScreen.Teleport += OnTeleport;
            CurrentScreen.BossDefeated += () => { if (End != null) End(); };
            CurrentScreen.Start();
        }

        private void StopScreen()
        {
            CurrentScreen.JoinTriggered -= OnScrollTriggered;
            CurrentScreen.Teleport -= OnTeleport;
            CurrentScreen.Stop();
        }

        private bool teleporting = false;
        private void OnTeleport(MegaMan.TeleportInfo info)
        {
            if (teleporting) return;
            teleporting = true;
            Action<string> setpos = (s) => { };
            if (info.TargetScreen == CurrentScreen.Screen.Name)
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
                setpos = (state) =>
                {
                    (Player.GetComponent<SpriteComponent>()).Visible = false;
                    (Player.GetComponent<StateComponent>()).StateChanged -= setpos;
                    Engine.Instance.FadeTransition(
                        () => 
                    { 
                        StopScreen();
                        ChangeScreen(new ScreenHandler(Map.Screens[info.TargetScreen], PlayerPos, Map.Joins));
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
            Game.CurrentGame.AddGameHandler(this);

            Player = GameEntity.Get("Player");

            Player.Stopped += Player_Death;
            PlayerPos = Player.GetComponent<PositionComponent>();

            if (!Map.Screens.ContainsKey(this.startScreen)) throw new GameEntityException("The start screen for \""+Map.Name+"\" is supposed to be \""+this.startScreen+"\", but it doesn't exist!");
            CurrentScreen = new ScreenHandler(Map.Screens[this.startScreen], PlayerPos, Map.Joins);
            StartScreen();

            if (music != null) music.Play();

            // updateFunc isn't set until BeginPlay
            drawFunc = Draw;

            Unpause();

            // ready flashing
            readyBlinkTime = 0;
            readyBlinks = 0;
            Engine.Instance.GameRender += new GameRenderEventHandler(BlinkReady);

            // make sure we can move
            (Player.GetComponent<InputComponent>()).Paused = false;
        }

        public void StopHandler()
        {
            Game.CurrentGame.RemoveGameHandler(this);

            if (Player != null)
            {
                Player.Stopped -= Player_Death;
                Player.Stop();
                Player = null;
            }

            if (CurrentScreen != null)
            {
                StopScreen();
                CurrentScreen.Clean();
            }

            if (music != null) music.Stop();

            Pause();
            Engine.Instance.GameRender -= new GameRenderEventHandler(BlinkReady);
        }

        public void Pause()
        {
            Engine.Instance.GameThink -= GameTick;
            Engine.Instance.GameRender -= new GameRenderEventHandler(GameRender);
            
            Engine.Instance.GameInputReceived -= new GameInputEventHandler(GameInputReceived);

            if (music != null) music.Volume = 0.7f;
        }

        public void Unpause()
        {
            Engine.Instance.GameThink += GameTick;
            Engine.Instance.GameRender += new GameRenderEventHandler(GameRender);
            
            Engine.Instance.GameInputReceived += new GameInputEventHandler(GameInputReceived);

            if (music != null) music.Volume = 1;
        }

        public void GameInputReceived(GameInputEventArgs e)
        {
            if (updateFunc == null || (Player.GetComponent<InputComponent>()).Paused) return;
            if (e.Input == GameInput.Start && e.Pressed)
            {
                // has to handle both pause and unpause, in case a pause screen isn't defined
                if (Game.CurrentGame.Paused)
                {
                    Engine.Instance.FadeTransition(null, () => { Game.CurrentGame.Unpause(); });
                }
                else
                {
                    if (Paused != null) Paused();
                }
            }
        }

        public void GameTick()
        {
            if (updateFunc != null) updateFunc();

            foreach (MegaMan.Tile t in Map.Tileset)
            {
                t.Sprite.Update();
            }
        }

        public void GameRender(GameRenderEventArgs e)
        {
            if (drawFunc != null) drawFunc(e.Layers.BackgroundBatch);
        }

        #endregion
    }
}
