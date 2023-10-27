using System.Diagnostics;
using MegaMan.Common.Rendering;
using MegaMan.Engine.Input;
using MegaMan.Engine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MegaMan.Engine
{
    public class GameInputEventArgs : EventArgs
    {
        public GameInputs Input { get; private set; }
        public bool Pressed { get; private set; }
        public GameInputEventArgs(GameInputs input, bool pressed)
        {
            Input = input;
            Pressed = pressed;
        }
    }
    public delegate void GameInputEventHandler(GameInputEventArgs e);

    /// <summary>
    /// So far, nothing uses this for the elapsed time, other than debug info.
    /// But it may be useful at some point. Maybe sound syncing?
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
        public IRenderingContext RenderContext { get; private set; }

        public GameRenderEventArgs(IRenderingContext context)
        {
            RenderContext = context;
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
        private static Engine? instance;
        public static Engine Instance
        {
            get { return instance ?? (instance = new Engine()); }
        }

        private static readonly int MIN_FPS = 10;
        private static readonly int MAX_FPS = 500;

        public static Random rand = new Random(0);

        private int fps;
        public int FPS
        {
            get { return fps; }
            set
            {
                fps = Math.Max(MIN_FPS, Math.Min(MAX_FPS, value));
                frameTicks = (long)(Stopwatch.Frequency / (float)fps);
            }
        }

        // this tracks how many pause requests have been made, a sort of stack for pausing.
        // the engine will only run when pauseCount is 0.
        private int pauseCount;

        // becomes true if Start has been called, false if Stop is called.
        // used to determine what to do when the engine is unpaused.
        private bool runIfUnpaused;

        // this timer is used to control framerate.
        private readonly Stopwatch timer;

        // how many cpu ticks should there be between frames?
        private long frameTicks;

        // this is just used as a pre-calculated number so the division isn't done every frame.
        // Premature optimization at its finest.
        private readonly float invFreq = 1 / (float)Stopwatch.Frequency;

        private List<bool> layerVisibility;

        private readonly SoundSystem soundsystem = new SoundSystem();

        // Opacity stuff is used for fade transitions.
        private float opacity = 1;
        private Color opacityColor = Color.White;
        public Color OpacityColor { get { return opacityColor; } }

        public SoundSystem SoundSystem { get { return soundsystem; } }

        // these are the flags for the debug menu stuff
        public bool DrawHitboxes { get; set; }
        public bool Invincible { get; set; }
        public bool NoDamage { get; set; }

        public SamplerState FilterState { get; set; }

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

        public GraphicsDevice GraphicsDevice { get; private set; }
        private IRenderingContext renderContext;

        // This event is used to query the graphics control and grab
        // its device, so we can send it out to things for drawing.
        // It's only fired when the engine is started.
        public class DeviceEventArgs : EventArgs
        {
            public GraphicsDevice Device;
        }
        public event EventHandler<DeviceEventArgs> GetDevice;

        public event Action<Exception> OnException;

        public float ThinkTime { get; private set; }

        private bool initialized;
        private bool running;
        public void Begin()
        {
            var args = new DeviceEventArgs();
            GetDevice?.Invoke(this, args);
            GraphicsDevice = args.Device;
            renderContext = new XnaRenderingContext(GraphicsDevice);
            initialized = true;
            Start();
        }

        public void Start()
        {
            runIfUnpaused = true;

            if (pauseCount == 0 && initialized && !running)
            {
                running = true;
                timer.Start();
                soundsystem.Start();
            }
        }

        public void Stop()
        {
            runIfUnpaused = false;

            if (running)
            {
                running = false;
                timer.Stop();
                soundsystem.Stop();
            }
        }

        public void Pause()
        {
            pauseCount++;
            Stop();
        }

        public void Unpause()
        {
            if (pauseCount > 0)
            {
                pauseCount--;

                if (pauseCount == 0 && runIfUnpaused)
                    Start();
            }
        }

        public bool IsRunning { get { return running; } }

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
            var count = 0;
            GameTickEventHandler handler = e => { count++; progress?.Invoke(count); };
            handler += e =>
            {
                if (delay == count)
                {
                    callback?.Invoke();
                    GameLogicTick -= handler;
                }
            };
            GameLogicTick += handler;
        }

        /// <summary>
        /// Fades the screen to black, calls an optional callback function, and then fades back in,
        /// and calls another callback function when done.
        /// </summary>
        /// <param name="callback">The function to call when the screen is black. Can be null.</param>
        /// <param name="finished">The function to call when the transition is finished. Can be null.</param>
        public void FadeTransition(Action callback, Action finished = null)
        {
            // if a fade in is in progress, finish that and immediately
            // switch to the new fade out
            if (fadeHandle != null)
            {
                GameLogicTick -= fadeHandle;
                fadeFinished?.Invoke();
            }

            fadeHandle = e => opacityDown(callback);
            GameLogicTick += fadeHandle;
            fadeFinished = finished;
        }

        private GameTickEventHandler? fadeHandle;
        private Action? fadeFinished;
        private void opacityDown(Action callback)
        {
            opacity -= 0.05f;
            opacityColor = new Color(opacity, opacity, opacity);
            if (opacity <= 0)
            {
                // call the callback, then switch to fading in
                callback?.Invoke();
                GameLogicTick -= fadeHandle;
                fadeHandle = e => opacityUp();
                GameLogicTick += fadeHandle;
            }
        }

        private void opacityUp()
        {
            opacity += 0.05f;
            opacityColor = new Color(opacity, opacity, opacity);
            if (opacity >= 1)   // done
            {
                GameLogicTick -= fadeHandle;
                fadeHandle = null;
                fadeFinished?.Invoke();
                fadeFinished = null;
            }
        }

        private Engine()
        {
            FPS = Const.FPS;

            timer = new Stopwatch();

            layerVisibility = Enumerable.Range(0, 6).Select(i => true).ToList();

            FilterState = SamplerState.PointClamp;
        }

        public bool GetLayerVisibility(int layer)
        {
            if (renderContext != null)
                return renderContext.IsLayerEnabled(layer);
            return true;
        }

        private void RenderContext(int layer)
        {
            if (renderContext != null)
            {
                if (layerVisibility[layer])
                    renderContext.EnableLayer(layer);
                else
                    renderContext.DisableLayer(layer);
            }
        }

        public void SetLayerVisibility(int layer, bool visibility)
        {
            layerVisibility[layer] = visibility;
            RenderContext(layer);
        }

        // This is run at the start of every step. It reads key states and checks for any changes.
        // If a key state changed, a GameInputReceived event is fired.
        private void CheckInput()
        {
            var changes = GameInput.GetChangedInputs();
            foreach (var key in changes.Keys)
            {
                GameInputReceived?.Invoke(new GameInputEventArgs(key, changes[key]));
            }
        }

        // Checks whether enough time has passed to fire the next frame, and then does it.
        // Runs both logic and rendering on the same thread.
        // Also keeps track of actual framerate and busy time.
        public void TryStepLogicAndRender()
        {
            TryStep((dt) =>
            {
                StepLogic(dt);
                StepRender();
            });
        }

        public void TryStepLogic()
        {
            TryStep(StepLogic);
        }

        private void TryStep(Action<float> action)
        {
            if (timer.ElapsedTicks < frameTicks || !running) return;
            var dt = timer.ElapsedTicks * invFreq;
            timer.Reset();
            timer.Start();

            try
            {
                action(dt);
            }
            catch (GameRunException ex)
            {
                OnException?.Invoke(ex);

                Stop();
            }

            ThinkTime = timer.ElapsedTicks * invFreq / dt;
        }

        // Executes one step (frame) of the game logic. The parameter is time
        // since last frame, but it isn't actually used except in the tick event. No one uses it
        // there either.
        private void StepLogic(float dt)
        {
            if (Game.CurrentGame is null) return;

            CheckInput();

            var e = new GameTickEventArgs(dt);
            GameLogicTick?.Invoke(e);

            SoundSystem.Tick();
        }

        public void StepRender()
        {
            if (Game.CurrentGame is null || GraphicsDevice is null) return;

            var r = new GameRenderEventArgs(renderContext);

            GraphicsDevice.Clear(Color.Green);

            GameRenderBegin?.Invoke(r);

            renderContext.SetOpacity(opacity);
            renderContext.Begin();

            GameRender?.Invoke(r);

            renderContext.End();

            GameRenderEnd?.Invoke(r);
        }
    }
}
