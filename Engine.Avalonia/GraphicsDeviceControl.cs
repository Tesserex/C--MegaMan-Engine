using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace WinFormsGraphicsDevice
{
    // System.Drawing and the XNA Framework both define Color and Rectangle
    // types. To avoid conflicts, we specify exactly which ones to use.
    /// <summary>
    /// Custom control uses the XNA Framework GraphicsDevice to render onto
    /// a Windows Form. Derived classes can override the Initialize and Draw
    /// methods to add their own drawing code.
    /// </summary>
    public abstract class GraphicsDeviceControl : Control
    {
        #region Fields


        // However many GraphicsDeviceControl instances you have, they all share
        // the same underlying GraphicsDevice, managed by this helper service.
        GraphicsDeviceService? graphicsDeviceService;


        #endregion

        #region Properties


        /// <summary>
        /// Gets a GraphicsDevice that can be used to draw onto this control.
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get { if (graphicsDeviceService == null) return null; return graphicsDeviceService.GraphicsDevice; }
        }


        /// <summary>
        /// Gets an IServiceProvider containing our IGraphicsDeviceService.
        /// This can be used with components such as the ContentManager,
        /// which use this service to look up the GraphicsDevice.
        /// </summary>
        public ServiceContainer Services
        {
            get { return services; }
        }

        ServiceContainer services = new ServiceContainer();


        #endregion

        #region Initialization


        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void OnLoaded(RoutedEventArgs e)
        {
            // Don't initialize the graphics device if we are running in the designer.
            if (!Design.IsDesignMode)
            {
                var handle = TopLevel.GetTopLevel(this)?.TryGetPlatformHandle()?.Handle;
                if (handle.HasValue)
                {
                    graphicsDeviceService = GraphicsDeviceService.AddRef(handle.Value,
                                                                         (int)Width,
                                                                         (int)Height);

                    // Register the service, so components like ContentManager can find it.
                    services.AddService<IGraphicsDeviceService>(graphicsDeviceService);

                    // Give derived classes a chance to initialize themselves.
                    Initialize();
                }
            }

            base.OnLoaded(e);
        }


        /// <summary>
        /// Disposes the control.
        /// </summary>
        protected override void OnUnloaded(RoutedEventArgs e)
        {
            if (graphicsDeviceService != null)
            {
                graphicsDeviceService.Release(true);
                graphicsDeviceService = null;
            }

            base.OnUnloaded(e);
        }


        #endregion

        #region Paint


        /// <summary>
        /// Redraws the control in response to a Render message.
        /// </summary>
        public override void Render(DrawingContext context)
        {
            base.Render(context);
        }


        /// <summary>
        /// Attempts to begin drawing the control. Returns an error message string
        /// if this was not possible, which can happen if the graphics device is
        /// lost, or if we are running inside the Form designer.
        /// </summary>
        protected string? BeginDraw()
        {
            // If we have no graphics device, we must be running in the designer.
            if (graphicsDeviceService == null)
            {
                return Name + "\n\n" + GetType();
            }

            // Make sure the graphics device is big enough, and is not lost.
            var deviceResetError = HandleDeviceReset();

            if (!string.IsNullOrEmpty(deviceResetError))
            {
                return deviceResetError;
            }

            if (Width == 0 || Height == 0) return null;

            // Many GraphicsDeviceControl instances can be sharing the same
            // GraphicsDevice. The device backbuffer will be resized to fit the
            // largest of these controls. But what if we are currently drawing
            // a smaller control? To avoid unwanted stretching, we set the
            // viewport to only use the top left portion of the full backbuffer.
            var viewport = new Viewport();

            viewport.X = 0;
            viewport.Y = 0;

            viewport.Width = (int)Width;
            viewport.Height = (int)Height;

            viewport.MinDepth = 0;
            viewport.MaxDepth = 1;

            GraphicsDevice.Viewport = viewport;

            return null;
        }


        /// <summary>
        /// Ends drawing the control. This is called after derived classes
        /// have finished their Draw method, and is responsible for presenting
        /// the finished image onto the screen, using the appropriate WinForms
        /// control handle to make sure it shows up in the right place.
        /// </summary>
        protected void EndDraw()
        {
            try
            {
                var sourceRectangle = new Rectangle(0, 0, (int)Width,
                                                                (int)Height);

                GraphicsDevice.Present();
            }
            catch
            {
                // Present might throw if the device became lost while we were
                // drawing. The lost device will be handled by the next BeginDraw,
                // so we just swallow the exception.
            }
        }


        /// <summary>
        /// Helper used by BeginDraw. This checks the graphics device status,
        /// making sure it is big enough for drawing the current control, and
        /// that the device is not lost. Returns an error string if the device
        /// could not be reset.
        /// </summary>
        string? HandleDeviceReset()
        {
            var deviceNeedsReset = false;

            switch (GraphicsDevice.GraphicsDeviceStatus)
            {
                case GraphicsDeviceStatus.Lost:
                    // If the graphics device is lost, we cannot use it at all.
                    return "Graphics device lost";

                case GraphicsDeviceStatus.NotReset:
                    // If device is in the not-reset state, we should try to reset it.
                    deviceNeedsReset = true;
                    break;

                default:
                    // If the device state is ok, check whether it is big enough.
                    var pp = GraphicsDevice.PresentationParameters;

                    deviceNeedsReset = ((int)Width != pp.BackBufferWidth) ||
                                       ((int)Height != pp.BackBufferHeight);
                    break;
            }

            // Do we need to reset the device?
            if (deviceNeedsReset)
            {
                try
                {
                    graphicsDeviceService?.ResetDevice((int)Width, (int)Height);
                }
                catch (Exception e)
                {
                    return "Graphics device reset failed\n\n" + e;
                }
            }

            return null;
        }

        static IBrush blueBrush = new SolidColorBrush(Avalonia.Media.Colors.CornflowerBlue);


        /// <summary>
        /// If we do not have a valid graphics device (for instance if the device
        /// is lost, or if we are running inside the Form designer), we must use
        /// regular Avalonia DrawingContext method to display a status message.
        /// </summary>
        protected virtual void PaintUsingDrawingContext(DrawingContext graphics, string text)
        {
            graphics.FillRectangle(blueBrush, new Avalonia.Rect(0, 0, Width, Height));

            Brush brush = new SolidColorBrush(Colors.Black);

            var format = new FormattedText(text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection, Typeface.Default, 5, brush);
            graphics.DrawText(format, new Avalonia.Point(Width / 2, Height / 2));
        }


        #endregion

        #region Abstract Methods


        /// <summary>
        /// Derived classes override this to initialize their drawing code.
        /// </summary>
        protected abstract void Initialize();


        /// <summary>
        /// Derived classes override this to draw themselves using the GraphicsDevice.
        /// </summary>
        protected abstract void Draw();


        #endregion
    }
}
