using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;
using MegaMan.Engine.Avalonia.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MegaMan.Engine.Avalonia
{
    public sealed class MonoGameControl : Control
    {
        public static readonly DirectProperty<MonoGameControl, EngineGame?> GameProperty =
            AvaloniaProperty.RegisterDirect<MonoGameControl, EngineGame?>(
                nameof(Game),
                o => o.Game,
                (o, v) => o.Game = v);

        public static readonly DirectProperty<MonoGameControl, Size> GameSizeProperty =
            AvaloniaProperty.RegisterDirect<MonoGameControl, Size>(
                nameof(GameSize),
                o => o.GameSize,
                (o, v) => o.GameSize = v);

        public static readonly DirectProperty<MonoGameControl, bool> NTSCProperty =
            AvaloniaProperty.RegisterDirect<MonoGameControl, bool>(
                nameof(NTSC),
                o => o.NTSC,
                (o, v) => o.NTSC = v);

        public static readonly DirectProperty<MonoGameControl, snes_ntsc_setup_t> NTSCSetupProperty =
            AvaloniaProperty.RegisterDirect<MonoGameControl, snes_ntsc_setup_t>(
                nameof(NTSCSetup),
                o => o.NTSCSetup,
                (o, v) => o.NTSCSetup = v);

        private readonly PresentationParameters _presentationParameters = new() {
            BackBufferWidth = 1,
            BackBufferHeight = 1,
            BackBufferFormat = SurfaceFormat.Color,
            DepthStencilFormat = DepthFormat.Depth24,
            PresentationInterval = PresentInterval.Immediate,
            IsFullScreen = false
        };

        RenderTarget2D? masterRenderingTarget;
        SpriteBatch? masterSpriteBatch;
        IntPtr ntsc;
        Texture2D? ntscTexture;
        readonly ushort[] pixels = new ushort[256 * 224];
        readonly ushort[] filtered = new ushort[602 * 448];

        private bool isNtsc;
        public bool NTSC {
            get => isNtsc;
            set
            {
                if (value == isNtsc) return;
                isNtsc = value;
                if (Game?.GraphicsDevice is not null)
                {
                    ResetDevice(Game.GraphicsDevice, gameSize);
                }
            }
        }

        private snes_ntsc_setup_t ntscSetup;
        public snes_ntsc_setup_t NTSCSetup
        {
            get => ntscSetup;
            private set
            {
                ntscSetup = value;
                ntscInit(value);
            }
        }

        [DllImport("ntsc.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr snes_ntsc_alloc();

        [DllImport("ntsc.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void snes_ntsc_init(IntPtr ntsc, snes_ntsc_setup_t setup);

        [DllImport("ntsc.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void snes_ntsc_free(IntPtr ntsc);

        [DllImport("ntsc.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void snes_ntsc_blit(IntPtr ntsc, ushort[] input,
            int in_row_width, int burst_phase, int in_width, int in_height,
            [In, Out] ushort[] rgb_out, int out_pitch);

        private static ushort[]? ntscPixelsDimmed;

        private byte[] _bufferData = Array.Empty<byte>();
        private WriteableBitmap? _bitmap;
        private bool _isInitialized;
        private EngineGame? _game;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoGameControl" /> class.
        /// </summary>
        public MonoGameControl()
        {
            Focusable = true;

            RunEngine();
        }

        private void RunEngine()
        {
            Task.Run(() => {
                try
                {
                    while (true)
                    {
                        Engine.Instance.TryStepLogic();
                    }
                }
                catch (OperationCanceledException)
                {

                }
            });
        }

        /// <summary>
        /// Gets or sets the fallback background brush.
        /// </summary>
        public IBrush FallbackBackground { get; set; } = Brushes.Purple;

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        public EngineGame? Game
        {
            get => _game;
            set
            {
                if (_game == value) return;
                _game = value;

                if (_isInitialized)
                {
                    Initialize();
                }
            }
        }

        private Size gameSize;
        public Size GameSize
        {
            get => gameSize;
            set
            {
                gameSize = value;
                SetSize();
            }
        }

        public override void Render(DrawingContext context)
        {
            if (Game is not { } game
                || Game?.GraphicsDevice is not { } device
                || _bitmap is null
                || Bounds is { Width: < 1, Height: < 1 }
                || !HandleDeviceReset(device))
            {
                context.DrawRectangle(FallbackBackground, null, new Rect(0, 0, Width, Height));
                return;
            }

            Engine.Instance.StepRender();

            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
            // Capture the last executed frame into the bitmap
            CaptureFrame(device, _bitmap);


            using (context.PushRenderOptions(new RenderOptions() { BitmapInterpolationMode = BitmapInterpolationMode.None }))
                // Flush the bitmap to context
                context.DrawImage(_bitmap, new Rect(_bitmap.Size), new Rect(0, 0, Bounds.Width, Bounds.Height));
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            Start();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            Engine.Instance.Stop();
        }

        protected override void OnGotFocus(GotFocusEventArgs e)
        {
            base.OnGotFocus(e);
            Engine.Instance.Start();
        }

        private bool HandleDeviceReset(GraphicsDevice device)
        {
            if (device.GraphicsDeviceStatus == GraphicsDeviceStatus.NotReset)
            {
                ResetDevice(device, Bounds.Size);
            }

            return device.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal;
        }

        private void Initialize()
        {
            if (this.GetVisualRoot() is Window { PlatformImpl: { } } window && window.TryGetPlatformHandle()?.Handle is { } handle)
                _presentationParameters.DeviceWindowHandle = handle;

            if (Game is not { } game) return;

            // this initializes the Game so it creates a GraphicsDevice
            game.RunOneFrame();

            if (game.GraphicsDevice is { } device)
            {
                masterSpriteBatch = new SpriteBatch(device);
                ResetDevice(device, Bounds.Size);
            }

            Engine.Instance.GetDevice += Instance_GetDevice;
            Engine.Instance.GameRenderEnd += Instance_GameRenderEnd;
            Engine.Instance.GameRenderBegin += Instance_GameRenderBegin;
            Margin = new Thickness(0);

            ntsc = snes_ntsc_alloc();
            ntscInit(snes_ntsc_setup_t.snes_ntsc_composite);

            ntscTexture = new Texture2D(game.GraphicsDevice, 602, 448, false, SurfaceFormat.Bgr565);

            ntscPixelsDimmed = new ushort[ushort.MaxValue + 1];
            for (var i = 0; i <= ushort.MaxValue; i++)
            {
                var red = (i & 0xf800);
                var green = (i & 0x7e0);
                var blue = (i & 0x1f);
                red = ((red - (red >> 3)) & 0xf800);
                green = ((green - (green >> 3)) & 0x7e0);
                blue = ((blue - (blue >> 3)) & 0x1f);
                ntscPixelsDimmed[i] = (ushort)(red | green | blue);
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            if (ntsc != IntPtr.Zero) snes_ntsc_free(ntsc);
            base.OnDetachedFromVisualTree(e);
        }

        public void ntscInit(snes_ntsc_setup_t setup)
        {
            if (ntsc == IntPtr.Zero) return;
            
            snes_ntsc_init(ntsc, setup);
            ForceRedraw();
        }

        private void SetSize()
        {
            if (Game is null || Game.GraphicsDevice is null) return;
            
            if (gameSize.Width != 256 || gameSize.Height != 224)
            {
                isNtsc = false;
            }

            masterRenderingTarget = new RenderTarget2D(
                Game.GraphicsDevice,
                (int)Math.Ceiling(gameSize.Width),
                (int)Math.Ceiling(gameSize.Height),
                false,
                SurfaceFormat.Bgr565,
                DepthFormat.None);

            ResetDevice(Game.GraphicsDevice, GameSize);
        }

        public void SaveCap(Stream stream)
        {
            masterRenderingTarget?.SaveAsPng(stream, masterRenderingTarget.Width, masterRenderingTarget.Height);
        }

        private void Instance_GetDevice(object? sender, Engine.DeviceEventArgs e)
        {
            if (Game is null) return;
            if (Game.GraphicsDevice is null) return;
            e.Device = Game.GraphicsDevice;
        }

        private void Instance_GameRenderBegin(GameRenderEventArgs e)
        {
            if (Game is null) return;
            if (Game.GraphicsDevice is null) return;

            Game.GraphicsDevice.SetRenderTarget(masterRenderingTarget);
            Game.GraphicsDevice.Clear(Color.Black);
        }

        private void Instance_GameRenderEnd(GameRenderEventArgs e)
        {
            if (Game is null) return;
            if (Game.GraphicsDevice is null) return;
            DrawMasterTargetToBatch(Game.GraphicsDevice);
            Game.GraphicsDevice.Present();
        }

        private void Start()
        {
            if (_isInitialized)
            {
                return;
            }

            Initialize();
            _isInitialized = true;
        }

        private void ResetDevice(GraphicsDevice device, Size newSize)
        {
            var newWidth = NTSC ? 602 : Math.Max(1, (int)Math.Ceiling(newSize.Width));
            var newHeight = NTSC ? 448 : Math.Max(1, (int)Math.Ceiling(newSize.Height));

            device.Viewport = new Viewport(0, 0, newWidth, newHeight);
            _presentationParameters.BackBufferWidth = newWidth;
            _presentationParameters.BackBufferHeight = newHeight;
            device.Reset(_presentationParameters);

            _bitmap?.Dispose();
            _bitmap = new WriteableBitmap(
                new PixelSize(newWidth, newHeight),
                new Vector(96d, 96d),
                PixelFormat.Rgba8888,
                AlphaFormat.Opaque);
        }

        private void ForceRedraw()
        {
            if (Game is null) return;
            if (Game.GraphicsDevice is null) return;
            if (MegaMan.Engine.Game.CurrentGame != null && !Engine.Instance.IsRunning)
            {
                Game.GraphicsDevice.Textures[0] = null;
                DrawMasterTargetToBatch(Game.GraphicsDevice);
            }
        }

        private void DrawMasterTargetToBatch(GraphicsDevice device)
        {
            if (masterRenderingTarget is null || masterSpriteBatch is null) return;

            Texture2D drawTexture = masterRenderingTarget;

            if (NTSC && ntscTexture is not null)
            {
                masterRenderingTarget.GetData(pixels);

                snes_ntsc_blit(ntsc, pixels, 256, 0, 256, 224, filtered, 1204);

                ntscTexture.SetData(filtered);

                drawTexture = ntscTexture;
            }

            device.SetRenderTarget(null);

            masterSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Engine.Instance.FilterState, null, null);
            masterSpriteBatch.Draw(drawTexture, new Rectangle(0, 0, device.Viewport.Width, device.Viewport.Height), Color.White);
            masterSpriteBatch.End();
        }

        private void CaptureFrame(GraphicsDevice device, WriteableBitmap bitmap)
        {
            using var bitmapLock = bitmap.Lock();
            var size = bitmapLock.RowBytes * bitmapLock.Size.Height;
            if (_bufferData.Length < size)
            {
                Array.Resize(ref _bufferData, size);
            }

            device.GetBackBufferData(_bufferData, 0, size);
            Marshal.Copy(_bufferData, 0, bitmapLock.Address, size);
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    public class snes_ntsc_setup_t
    {
        public static readonly snes_ntsc_setup_t snes_ntsc_composite = new snes_ntsc_setup_t(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, true);
        public static readonly snes_ntsc_setup_t snes_ntsc_svideo = new snes_ntsc_setup_t(0, 0, 0, 0, .2, 0, .2, -1, -1, 0, true);
        public static readonly snes_ntsc_setup_t snes_ntsc_rgb = new snes_ntsc_setup_t(0, 0, 0, 0, .2, 0, .7, -1, -1, -1, true);

        /* Basic parameters */
        public double hue;        /* -1 = -180 degrees     +1 = +180 degrees */
        public double saturation; /* -1 = grayscale (0.0)  +1 = oversaturated colors (2.0) */
        public double contrast;   /* -1 = dark (0.5)       +1 = light (1.5) */
        public double brightness; /* -1 = dark (0.5)       +1 = light (1.5) */
        public double sharpness;  /* edge contrast enhancement/blurring */

        /* Advanced parameters */
        public double gamma;      /* -1 = dark (1.5)       +1 = light (0.5) */
        public double resolution; /* image resolution */
        public double artifacts;  /* artifacts caused by color changes */
        public double fringing;   /* color artifacts caused by brightness changes */
        public double bleed;      /* color bleed (color resolution reduction) */
        public bool merge_fields;  /* if 1, merges even and odd fields together to reduce flicker */

        public float[] decoder_matrix; /* optional RGB decoder matrix, 6 elements */

        uint[] bsnes_colortbl; /* undocumented; set to 0 */

        public snes_ntsc_setup_t(double hue, double saturation, double contrast, double brightness,
            double sharpness, double gamma, double resolution, double artifacts,
            double fringing, double bleed, bool merge_fields)
        {
            this.hue = hue;
            this.saturation = saturation;
            this.contrast = contrast;
            this.brightness = brightness;
            this.sharpness = sharpness;
            this.gamma = gamma;
            this.resolution = resolution;
            this.artifacts = artifacts;
            this.fringing = fringing;
            this.bleed = bleed;
            this.merge_fields = merge_fields ? true : false;

            // default decoder matrix
            decoder_matrix = null;
        }

        public snes_ntsc_setup_t(NTSC_CustomOptions options)
            : this(options.Hue, options.Saturation, options.Contrast, options.Brightness,
                  options.Sharpness, options.Gamma, options.Resolution, options.Artifacts,
                  options.Fringing, options.Bleed, options.Merge_Fields)
        { }

        public snes_ntsc_setup_t snes_ntsc_custom()
        {
            return new snes_ntsc_setup_t(hue, saturation, contrast, brightness, sharpness, gamma, resolution, artifacts, fringing, bleed, merge_fields);
        }
    }
}
