using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace Mega_Man
{
    public class GameGraphicsLayers
    {
        public Image Background { get; private set; }
        public Image[] Sprites { get; private set; }
        public Image Foreground { get; private set; }

        public SpriteBatch BackgroundBatch { get; private set; }
        public SpriteBatch[] SpritesBatch { get; private set; }
        public SpriteBatch ForegroundBatch { get; private set; }

        public GameGraphicsLayers(int across, int down, GraphicsDevice device)
        {
            Bitmap backimage = new Bitmap(across, down);
            backimage.SetResolution(Const.Resolution, Const.Resolution);
            Background = backimage;

            Rectangle rect = new Rectangle(0, 0, across, down);
            Foreground = backimage.Clone(rect, System.Drawing.Imaging.PixelFormat.DontCare);

            Sprites = new Image[4];
            for (int i = 0; i < 4; i++) Sprites[i] = backimage.Clone(rect, System.Drawing.Imaging.PixelFormat.DontCare);

            BackgroundBatch = new SpriteBatch(device);
            ForegroundBatch = new SpriteBatch(device);
            SpritesBatch = new SpriteBatch[4];
            for (int i = 0; i < 4; i++) SpritesBatch[i] = new SpriteBatch(device);
        }
    }

    public class GameInputEventArgs : EventArgs
    {
        public GameInput Input { get; private set; }
        public bool Pressed { get; private set; }
        public GameInputEventArgs(GameInput input, bool pressed)
        {
            Input = input;
            Pressed = pressed;
        }
    }
    public delegate void GameInputEventHandler(GameInputEventArgs e);

    public class GameTickEventArgs : EventArgs
    {
        public float TimeElapsed { get; private set; }

        public GameTickEventArgs(float dt)
        {
            TimeElapsed = dt;
        }
    }
    public delegate void GameTickEventHandler(GameTickEventArgs e);

    public class GameRenderEventArgs : EventArgs
    {
        public GameGraphicsLayers Layers { get; private set; }
        public float Opacity { get; set; }
        public GraphicsDevice Device { get; private set; }

        public GameRenderEventArgs(GameGraphicsLayers layers, GraphicsDevice device)
        {
            Layers = layers;
            Device = device;
        }
    }
    public delegate void GameRenderEventHandler(GameRenderEventArgs e);

    public class Engine
    {
        private static Engine instance = null;
        public static Engine Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Engine();
                }
                return instance;
            }
        }

        private Stopwatch timer;
        private Stopwatch diagnostic;
        private long frameTicks = (long)(Stopwatch.Frequency / Const.FPS);
        private float invFreq = 1 / (float)Stopwatch.Frequency;
        private Dictionary<Keys, bool> inputFlags = new Dictionary<Keys, bool>();
        private GameGraphicsLayers graphics;
        private SoundSystem soundsystem = new SoundSystem();
        private float opacity = 1;

        public bool DrawHitboxes { get; set; }
        public bool Invincible { get; set; }

        public event GameInputEventHandler GameInputReceived;
        public event GameTickEventHandler GameLogicTick;
        public event GameRenderEventHandler GameRenderBegin;
        public event GameRenderEventHandler GameRender;
        public event GameRenderEventHandler GameRenderEnd;

        public event Action GameThink;
        public event Action GameAct;
        public event Action GameReact;

        public GraphicsDevice GraphicsDevice { get; private set; }

        public class DeviceEventArgs : EventArgs
        {
            public GraphicsDevice Device;
        }

        public event EventHandler<DeviceEventArgs> GetDevice;

        /// <summary>
        /// The final logic phase before rendering. Used to delete entities,
        /// so please do not enumerate through entity collections during this phase.
        /// </summary>
        public event Action GameCleanup;

        public float ThinkTime { get; private set; }

        public void Start()
        {
            timer.Start();
            DeviceEventArgs args = new DeviceEventArgs();
            if (GetDevice != null) GetDevice(this, args);
            this.GraphicsDevice = args.Device;
            //Resize(Const.PixelsAcross, Const.PixelsDown);
        }

        public void Stop()
        {
            timer.Stop();
        }

        public int LoadMusic(string intro, string loop)
        {
            if (intro == null && loop == null) throw new ArgumentNullException("LoadMusic was passed null for both arguments.");
            return soundsystem.LoadMusic(intro, loop);
        }

        public int LoadSoundEffect(string path, bool loop)
        {
            return soundsystem.LoadSoundEffect(path, loop);
        }

        public void UnloadAudio()
        {
            soundsystem.Unload();
        }

        public void PlayMusic(int soundHandle)
        {
            soundsystem.PlayMusic(soundHandle);
        }

        public void PlaySound(int soundHandle)
        {
            soundsystem.PlayEffect(soundHandle);
        }

        public void StopMusic(int soundHandle)
        {
            soundsystem.StopMusic(soundHandle);
        }

        public void StopSound(int soundHandle)
        {
            soundsystem.StopEffect(soundHandle);
        }

        public void StopSoundIfLoop(int soundHandle)
        {
            soundsystem.StopIfLooping(soundHandle);
        }

        public void SetVolume(int soundHandle, float volume)
        {
            soundsystem.SetVolume(soundHandle, volume);
        }

        public void FadeTransition(Action callback) { FadeTransition(callback, null); }

        public void FadeTransition(Action callback, Action finished)
        {
            if (fadeHandle != null) return; // can't do more than one at a time
            fadeHandle = new GameTickEventHandler((e) => opacityDown(callback));
            GameLogicTick += fadeHandle;
            fadeFinished = finished;
        }

        private GameTickEventHandler fadeHandle;
        private Action fadeFinished;
        private void opacityDown(Action callback)
        {
            opacity -= 0.05f;
            if (opacity <= 0)
            {
                if (callback != null) callback();
                this.GameLogicTick -= fadeHandle;
                fadeHandle = new GameTickEventHandler((e) => opacityUp());
                this.GameLogicTick += fadeHandle;
            }
        }

        private void opacityUp()
        {
            opacity += 0.05f;
            if (opacity >= 1)
            
            {
                this.GameLogicTick -= fadeHandle;
                fadeHandle = null;
                if (fadeFinished != null) fadeFinished();
                fadeFinished = null;
            }
        }

        private Engine()
        {
            foreach (Keys key in GameInputKeys.Instance)
            {
                inputFlags[key] = false;
            }

            Game.ScreenSizeChanged += new EventHandler<ScreenSizeChangedEventArgs>(Game_ScreenSizeChanged);

            Application.Idle += (s, e) => { while (Program.AppIdle) Application_Idle(); };

            diagnostic = new Stopwatch();

            timer = new Stopwatch();
        }

        void Game_ScreenSizeChanged(object sender, ScreenSizeChangedEventArgs e)
        {
            Resize(e.PixelsAcross, e.PixelsDown);
        }

        private void Resize(int across, int down)
        {
            graphics = new GameGraphicsLayers(across, down, this.GraphicsDevice);
        }

        private void CheckInput()
        {
            foreach (Keys key in GameInputKeys.Instance)
            {
                if (Program.KeyDown(key))
                {
                    if (!inputFlags.ContainsKey(key) || inputFlags[key]==false)
                    {
                        inputFlags[key] = true;
                        if (GameInputReceived != null) GameInputReceived(new GameInputEventArgs(KeyToInput(key), true));
                    }
                }
                else if (inputFlags.ContainsKey(key) && inputFlags[key] == true)
                {
                    inputFlags[key] = false;
                    if (GameInputReceived != null) GameInputReceived(new GameInputEventArgs(KeyToInput(key), false));
                }
            }
        }

        private void Application_Idle()
        {
            if (timer.ElapsedTicks < frameTicks) return;
            float dt = timer.ElapsedTicks * invFreq;
            timer.Reset();
            timer.Start();
            if (Step(dt)) Application.Exit();
            ThinkTime = timer.ElapsedTicks * invFreq / dt;
        }

        private bool Step(float dt)
        {
            //diagnostic.Start();
            CheckInput();

            GameTickEventArgs e = new GameTickEventArgs(dt);

            if (GameLogicTick != null) GameLogicTick(e);    // this one is for more basic operations
            
            // these ones are for entities
            if (Game.CurrentGame == null || Game.CurrentGame.CurrentMap == null || !Game.CurrentGame.Paused)
            {
                if (GameThink != null) GameThink();
                if (GameAct != null) GameAct();
                if (GameReact != null) GameReact();
            }
            if (GameCleanup != null) GameCleanup();

            // render phase
            using (Graphics g = Graphics.FromImage(graphics.Background)) g.Clear(System.Drawing.Color.Transparent);

            for (int i = 0; i < graphics.Sprites.Length; i++)
            {
                using (Graphics g = Graphics.FromImage(graphics.Sprites[i])) g.Clear(System.Drawing.Color.Transparent);
            }
            using (Graphics g = Graphics.FromImage(graphics.Foreground)) g.Clear(System.Drawing.Color.Transparent);

            GameRenderEventArgs r = new GameRenderEventArgs(graphics, this.GraphicsDevice);
            r.Opacity = opacity;

            this.GraphicsDevice.Clear(Microsoft.Xna.Framework.Graphics.Color.Green);
            
            if (GameRenderBegin != null) GameRenderBegin(r);

            r.Layers.BackgroundBatch.Begin();
            foreach (SpriteBatch batch in r.Layers.SpritesBatch) batch.Begin();
            r.Layers.ForegroundBatch.Begin();
            if (GameRender != null) GameRender(r);
            r.Layers.BackgroundBatch.End();
            foreach (SpriteBatch batch in r.Layers.SpritesBatch) batch.End();
            r.Layers.ForegroundBatch.End();

            if (GameRenderEnd != null) GameRenderEnd(r);
            
            //diagnostic.Stop();
            //Console.WriteLine(diagnostic.ElapsedMilliseconds);
            //diagnostic.Reset();
            return false;
        }

        private GameInput KeyToInput(Keys key)
        {
            // does not work with switch - "A constant value is expected"
            if (key == GameInputKeys.Down) return GameInput.Down;
            if (key == GameInputKeys.Jump) return GameInput.Jump;
            if (key == GameInputKeys.Left) return GameInput.Left;
            if (key == GameInputKeys.Right) return GameInput.Right;
            if (key == GameInputKeys.Shoot) return GameInput.Shoot;
            if (key == GameInputKeys.Up) return GameInput.Up;
            if (key == GameInputKeys.Start) return GameInput.Start;
            if (key == GameInputKeys.Select) return GameInput.Select;
        
            return GameInput.None;
        }
    }
}
