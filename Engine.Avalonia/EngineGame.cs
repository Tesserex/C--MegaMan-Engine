using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MegaMan.Engine.Avalonia
{
    /// <summary>
    /// This runs the engine in MonoGame and is then consumed by the MonoGameControl.
    /// We're pretty much bypassing all the usual MonoGame functions and just consuming the graphics device.
    /// Our own engine does the stepping and such itself.
    /// </summary>
    public class EngineGame : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Gets the graphics device manager. Even though it's not used, it's necessary.
        /// </summary>
        private GraphicsDeviceManager GraphicsDeviceManager { get; }

        private int _lastWidth, _lastHeight;

        public EngineGame()
        {
            // If we don't new() it, there won't be a graphics device at all.
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
