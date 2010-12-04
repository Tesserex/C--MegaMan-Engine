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
    /// <summary>
    /// The graphics layers are a set of XNA sprite batches, used as
    /// surfaces so that things can be drawn in a layered order.
    /// </summary>
    public class GameGraphicsLayers
    {
        public SpriteBatch BackgroundBatch { get; private set; }
        public SpriteBatch[] SpritesBatch { get; private set; }
        public SpriteBatch ForegroundBatch { get; private set; }

        public GameGraphicsLayers(int across, int down, GraphicsDevice device)
        {
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

    /// <summary>
    /// So far, nothing uses this for the elapsed time. But it may be
    /// useful at some point. Maybe sound syncing?
    /// </summary>
    public class GameTickEventArgs : EventArgs
    {
        public float TimeElapsed { get; private set; }

        public GameTickEventArgs(float dt)
        {
            TimeElapsed = dt;
        }
    }
    public delegate void GameTickEventHandler(GameTickEventArgs e);

    /// <summary>
    /// These args are sent with drawing events so that things can
    /// draw on them. Pretty straightforward.
    /// </summary>
    public class GameRenderEventArgs : EventArgs
    {
        public GameGraphicsLayers Layers { get; private set; }
        public GraphicsDevice Device { get; private set; }
        public Microsoft.Xna.Framework.Graphics.Color OpacityColor;

        public GameRenderEventArgs(GameGraphicsLayers layers, GraphicsDevice device)
        {
            Layers = layers;
            Device = device;
        }
    }
    public delegate void GameRenderEventHandler(GameRenderEventArgs e);

    /// <summary>
    /// This engine class controls the low level behaviors. Its main job
    /// is keeping a "heartbeat" at a constant framerate. Basically, it's
    /// the main loop for the game - logic, then draw, repeat. It also holds
    /// the device context that things will draw with, and has some basic
    /// drawing control functions.
    /// </summary>
    public class Engine
    {
        // Yes, it's a singleton
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

        // this timer is used to control framerate. Because the Idle event is used,
        // the inherent limit on speed is about 70 fps. The events just wont fire
        // more often than that.
        private Stopwatch timer;

        // how many cpu ticks should there be between frames?
        private long frameTicks = (long)(Stopwatch.Frequency / Const.FPS);

        // this is just used as a pre-calculated number so the division isn't done every frame.
        // Premature optimization at its finest.
        private float invFreq = 1 / (float)Stopwatch.Frequency;

        // this holds the key pressed state of all input keys, so that when they change,
        // they can be translated into a GameInput event.
        private Dictionary<Keys, bool> inputFlags = new Dictionary<Keys, bool>();

        private GameGraphicsLayers graphics;

        public bool Background { get; set; }
        public bool SpritesOne { get; set; }
        public bool SpritesTwo { get; set; }
        public bool SpritesThree { get; set; }
        public bool SpritesFour { get; set; }
        public bool Foreground { get; set; }

        private SoundSystem soundsystem = new SoundSystem();

        // Opacity stuff is used for fade transitions.
        private float opacity = 1;
        private Microsoft.Xna.Framework.Graphics.Color opacityColor = Microsoft.Xna.Framework.Graphics.Color.White;
        public Microsoft.Xna.Framework.Graphics.Color OpacityColor { get { return opacityColor; } }

        public SoundSystem SoundSystem { get { return soundsystem; } }

        // these are the flags for the debug menu stuff
        public bool DrawHitboxes { get; set; }
        public bool Invincible { get; set; }

        public TextureFilter MagFilter { get; set; }

        // --- These events, and the order in which they fire, are very important.

        /// <summary>
        /// Fires any time a GameInput key is pressed or released.
        /// Will fire regardless of game state, as long as the engine
        /// is running.
        /// </summary>
        public event GameInputEventHandler GameInputReceived;

        /// <summary>
        /// Fires every frame when the engine is running. Use this for
        /// things that don't care about the game state.
        /// </summary>
        public event GameTickEventHandler GameLogicTick;

        // This one is used by the graphics control to clear everything for this frame.
        public event GameRenderEventHandler GameRenderBegin;

        /// <summary>
        /// This event does all the actual rendering - everything that wants to draw
        /// must respond to it.
        /// </summary>
        public event GameRenderEventHandler GameRender;

        // This is used by the graphics control to merge all drawing layers
        // and display them on the screen.
        public event GameRenderEventHandler GameRenderEnd;


        // -- These events only fire when the game is in play, and not paused (yes, that means the pause screen).

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

        public GraphicsDevice GraphicsDevice { get; private set; }

        // This event is used to query the graphics control and grab
        // its device, so we can send it out to things for drawing.
        // It's only fired when the engine is started.
        public class DeviceEventArgs : EventArgs
        {
            public GraphicsDevice Device;
        }
        public event EventHandler<DeviceEventArgs> GetDevice;

        public float ThinkTime { get; private set; }

        public void Start()
        {
            timer.Start();
            DeviceEventArgs args = new DeviceEventArgs();
            if (GetDevice != null) GetDevice(this, args);
            this.GraphicsDevice = args.Device;
        }

        public void Stop()
        {
            timer.Stop();
        }

        /// <summary>
        /// Disposes of all audio objects. If you try to play audio after
        /// this is called, you will get an error.
        /// </summary>
        public void UnloadAudio()
        {
            soundsystem.Unload();
        }

        /// <summary>
        /// Calls a function after a given number of frames.
        /// </summary>
        /// <param name="callback">The callback function to call.</param>
        /// <param name="progress">A function called each frame to report the progress of the delay.</param>
        /// <param name="delay">The number of frames to wait before calling.</param>
        public void DelayedCall(Action callback, Action<int> progress, int delay)
        {
            int count = 0;
            GameTickEventHandler handler = (e) => { count++; if (progress != null) progress(count); };
            handler += (e) =>
            {
                if (delay == count)
                {
                    if (callback != null) callback();
                    GameLogicTick -= handler;
                }
            };
            GameLogicTick += handler;
        }

        /// <summary>
        /// Fades the screen to black, calls an optional callback function, and then fades back in.
        /// Only one transition can be in progress at a time. If it is called during a transition,
        /// it will not do anything.
        /// </summary>
        /// <param name="callback">The function to call when the screen is black. Can be null.</param>
        public void FadeTransition(Action callback) { FadeTransition(callback, null); }

        /// <summary>
        /// Fades the screen to black, calls an optional callback function, and then fades back in,
        /// and calls another callback function when done.
        /// Only one transition can be in progress at a time. If it is called during a transition,
        /// it will not do anything.
        /// </summary>
        /// <param name="callback">The function to call when the screen is black. Can be null.</param>
        /// <param name="finished">The function to call when the transition is finished. Can be null.</param>
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
            opacityColor = new Microsoft.Xna.Framework.Graphics.Color(opacity, opacity, opacity);
            if (opacity <= 0)
            {
                // call the callback, then switch to fading in
                if (callback != null) callback();
                this.GameLogicTick -= fadeHandle;
                fadeHandle = new GameTickEventHandler((e) => opacityUp());
                this.GameLogicTick += fadeHandle;
            }
        }

        private void opacityUp()
        {
            opacity += 0.05f;
            opacityColor = new Microsoft.Xna.Framework.Graphics.Color(opacity, opacity, opacity);
            if (opacity >= 1)   // done
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

            timer = new Stopwatch();

            Foreground = Background = SpritesOne = SpritesTwo = SpritesThree = SpritesFour = true;

            this.MagFilter = TextureFilter.Point;
        }

        void Game_ScreenSizeChanged(object sender, ScreenSizeChangedEventArgs e)
        {
            Resize(e.PixelsAcross, e.PixelsDown);
        }

        // resizing the form requires us to resize the drawing layers to match.
        // This isn't for when the user resizes the form manually, it's when a game is loaded
        // and tells everyone how big it is in pixels.
        private void Resize(int across, int down)
        {
            graphics = new GameGraphicsLayers(across, down, this.GraphicsDevice);
        }

        // This is run at the start of every step. It reads key states and checks for any changes.
        // If a key state changed, a GameInputReceived event is fired.
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

        // Checks whether enough time has passed to fire the next frame, and then does it.
        // Also keeps track of actual framerate and busy time.
        private void Application_Idle()
        {
            if (timer.ElapsedTicks < frameTicks) return;
            float dt = timer.ElapsedTicks * invFreq;
            timer.Reset();
            timer.Start();
            if (Step(dt)) Application.Exit();
            ThinkTime = timer.ElapsedTicks * invFreq / dt;
        }

        // Executes one step (frame) of the game, both logic and drawing. The parameter is time
        // since last frame, but it isn't actually used except in the tick event. No one uses it
        // there either.
        private bool Step(float dt)
        {
            CheckInput();

            GameTickEventArgs e = new GameTickEventArgs(dt);

            if (GameLogicTick != null) GameLogicTick(e);    // this one is for more basic operations
            
            // these ones are for entities, so they only fire if the game is in play.
            if (Game.CurrentGame == null || Game.CurrentGame.CurrentMap == null || !Game.CurrentGame.Paused)
            {
                if (GameThink != null) GameThink();
                if (GameAct != null) GameAct();
                if (GameReact != null) GameReact();
            }
            if (GameCleanup != null) GameCleanup();

            // render phase
            GameRenderEventArgs r = new GameRenderEventArgs(graphics, this.GraphicsDevice);
            r.OpacityColor = opacityColor;

            this.GraphicsDevice.Clear(Microsoft.Xna.Framework.Graphics.Color.Green);
            
            if (GameRenderBegin != null) GameRenderBegin(r);

            r.Layers.BackgroundBatch.Begin();
            foreach (SpriteBatch batch in r.Layers.SpritesBatch) batch.Begin();
            r.Layers.ForegroundBatch.Begin();

            if (GameRender != null) GameRender(r);

            // only draw the layers if they're enabled
            if (Background) r.Layers.BackgroundBatch.End();
            if (SpritesOne) r.Layers.SpritesBatch[0].End();
            if (SpritesTwo) r.Layers.SpritesBatch[1].End();
            if (SpritesThree) r.Layers.SpritesBatch[2].End();
            if (SpritesFour) r.Layers.SpritesBatch[3].End();
            if (Foreground) r.Layers.ForegroundBatch.End();

            if (GameRenderEnd != null) GameRenderEnd(r);

            // now after rendering, if the layers are disabled,
            // just dump them in a ditch somewhere
            if (!Background) r.Layers.BackgroundBatch.End();
            if (!SpritesOne) r.Layers.SpritesBatch[0].End();
            if (!SpritesTwo) r.Layers.SpritesBatch[1].End();
            if (!SpritesThree) r.Layers.SpritesBatch[2].End();
            if (!SpritesFour) r.Layers.SpritesBatch[3].End();
            if (!Foreground) r.Layers.ForegroundBatch.End();
            
            return false;
        }

        // Looking at this now, it seems like an unnecessary extra level of abstraction
        // from the keyboard keys. But maybe not. This translates the Keys enum value
        // stored in the GameInputKeys class to a GameInput enum value.
        private GameInput KeyToInput(Keys key)
        {
            // does not work with switch statement - "A constant value is expected"
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
