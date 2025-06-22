using System;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

// The IGraphicsDeviceService interface requires a DeviceCreated event, but we
// always just create the device inside our constructor, so we have no place to
// raise that event. The C# compiler warns us that the event is never used, but
// we don't care so we just disable this warning.
#pragma warning disable 67

namespace WinFormsGraphicsDevice {
	/// <summary>
	/// Helper class responsible for creating and managing the GraphicsDevice.
	/// All GraphicsDeviceControl instances share the same GraphicsDeviceService,
	/// so even though there can be many controls, there will only ever be a single
	/// underlying GraphicsDevice. This implements the standard IGraphicsDeviceService
	/// interface, which provides notification events for when the device is reset
	/// or disposed.
	/// </summary>
	public class GraphicsDeviceService : IGraphicsDeviceService {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		// Singleton device service instance.
		private static GraphicsDeviceService singletonInstance;

		// Keep track of how many controls are sharing the singletonInstance.
		private static int referenceCount;

		// Store the current device settings.
		private PresentationParameters parameters;

		// IGraphicsDeviceService events.
		public event EventHandler<EventArgs> DeviceCreated;
		public event EventHandler<EventArgs> DeviceDisposing;
		public event EventHandler<EventArgs> DeviceReset;
		public event EventHandler<EventArgs> DeviceResetting;

		/// <summary>
		/// Gets the current graphics device.
		/// </summary>
		public GraphicsDevice GraphicsDevice {
			get { return graphicsDevice; }
		}
		private GraphicsDevice graphicsDevice;


		/// <summary>
		/// Gets an IServiceProvider containing our IGraphicsDeviceService.
		/// This can be used with components such as the ContentManager,
		/// which use this service to look up the GraphicsDevice.
		/// </summary>
		public ServiceContainer Services {
			get { return services; }
		}
		private ServiceContainer services = new ServiceContainer();
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Constructor is private, because this is a singleton class:
		/// client controls should use the public AddRef method instead.
		/// </summary>
		private GraphicsDeviceService(IntPtr windowHandle, int width, int height) {
			parameters = new PresentationParameters();

			parameters.BackBufferWidth = Math.Max(width, 1);
			parameters.BackBufferHeight = Math.Max(height, 1);
			parameters.BackBufferFormat = SurfaceFormat.Color;
			parameters.DepthStencilFormat = DepthFormat.Depth24;
			parameters.DeviceWindowHandle = windowHandle;
			parameters.PresentationInterval = PresentInterval.Immediate;
			parameters.IsFullScreen = false;

			graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach, parameters);
		}


		/// <summary>
		/// Gets a reference to the singleton instance.
		/// </summary>
		public static GraphicsDeviceService AddRef(IntPtr windowHandle, int width, int height) {
			// Increment the "how many controls sharing the device" reference count.
			if (Interlocked.Increment(ref referenceCount) == 1) {
				// If this is the first control to start using the
				// device, we must create the singleton instance.
				singletonInstance = new GraphicsDeviceService(windowHandle, width, height);
				singletonInstance.services.AddService<IGraphicsDeviceService>(singletonInstance);
			}

			return singletonInstance;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Public Methods
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Releases a reference to the singleton instance.
		/// </summary>
		public void Release(bool disposing) {
			// Decrement the "how many controls sharing the device" reference count.
			if (Interlocked.Decrement(ref referenceCount) == 0) {
				// If this is the last control to finish using the
				// device, we should dispose the singleton instance.
				if (disposing) {
                    DeviceDisposing?.Invoke(this, EventArgs.Empty);

                    graphicsDevice.Dispose();
				}

				graphicsDevice = null;
			}
		}


		/// <summary>
		/// Resets the graphics device to the specified resolution.
		/// </summary>
		public void ResetDevice(int width, int height) {
            DeviceResetting?.Invoke(this, EventArgs.Empty);

            parameters.BackBufferWidth = width;
			parameters.BackBufferHeight = height;

			graphicsDevice.Reset(parameters);

            DeviceReset?.Invoke(this, EventArgs.Empty);
        }
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
