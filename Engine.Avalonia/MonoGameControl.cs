using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Microsoft.Xna.Framework.Graphics;

namespace MegaMan.Engine.Avalonia
{
    public sealed class MonoGameControl : Control
    {
        /// <summary>
        /// Avalonia property for <see cref="Game" />.
        /// </summary>
        public static readonly DirectProperty<MonoGameControl, EngineGame?> GameProperty =
            AvaloniaProperty.RegisterDirect<MonoGameControl, EngineGame?>(
                nameof(Game),
                o => o.Game,
                (o, v) => o.Game = v);

        private readonly PresentationParameters _presentationParameters = new() {
            BackBufferWidth = 1,
            BackBufferHeight = 1,
            BackBufferFormat = SurfaceFormat.Color,
            DepthStencilFormat = DepthFormat.Depth24,
            PresentationInterval = PresentInterval.Immediate,
            IsFullScreen = false
        };

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

        public override void Render(DrawingContext context)
        {
            if (Game is not { } game
                || Game?.GraphicsDevice is not { } device
                || _bitmap is null
                || Bounds is { Width: < 1, Height: < 1 }
                || !HandleDeviceReset(device))
            {
                context.DrawRectangle(FallbackBackground, null, new Rect(Bounds.Size));
                return;
            }

            // Execute a frame
            StepEngine(game);
            // Capture the executed frame into the bitmap
            CaptureFrame(device, _bitmap);

            // Flush the bitmap to context
            context.DrawImage(_bitmap, new Rect(_bitmap.Size), Bounds);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            finalSize = base.ArrangeOverride(finalSize);
            if (finalSize != _bitmap?.Size && Game?.GraphicsDevice is { } device)
            {
                ResetDevice(device, finalSize);
            }

            return finalSize;
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            Start();
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
                ResetDevice(device, Bounds.Size);

            Engine.Instance.GetDevice += Instance_GetDevice;
        }

        private void Instance_GetDevice(object? sender, Engine.DeviceEventArgs e)
        {
            if (Game is not { } game) return;
            e.Device = game.GraphicsDevice;
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
            var newWidth = Math.Max(1, (int)Math.Ceiling(newSize.Width));
            var newHeight = Math.Max(1, (int)Math.Ceiling(newSize.Height));

            device.Viewport = new Viewport(0, 0, newWidth, newHeight);
            _presentationParameters.BackBufferWidth = newWidth;
            _presentationParameters.BackBufferHeight = newHeight;
            device.Reset(_presentationParameters);

            _bitmap?.Dispose();
            _bitmap = new WriteableBitmap(
                new PixelSize(device.Viewport.Width, device.Viewport.Height),
                new Vector(96d, 96d),
                PixelFormat.Rgba8888,
                AlphaFormat.Opaque);
        }

        private void StepEngine(EngineGame game)
        {
            try
            {
                Engine.Instance.CheckNextFrame();
            }
            finally
            {
                Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
            }
        }

        private void CaptureFrame(GraphicsDevice device, WriteableBitmap bitmap)
        {
            using var bitmapLock = bitmap.Lock();
            var size = bitmapLock.RowBytes * bitmapLock.Size.Height;
            if (_bufferData.Length < size)
            {
                //_bufferData = new byte[size];
                Array.Resize(ref _bufferData, size);
            }

            device.GetBackBufferData(_bufferData, 0, size);
            Marshal.Copy(_bufferData, 0, bitmapLock.Address, size);
        }
    }
}
