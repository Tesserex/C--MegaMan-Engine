using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Mega_Man
{
    public class EngineGraphicsControl : WinFormsGraphicsDevice.GraphicsDeviceControl
    {
        Random rand;
        RenderTarget2D backing;
        SpriteBatch sprite;

        protected override void Initialize()
        {
            Engine.Instance.GetDevice += new EventHandler<Engine.DeviceEventArgs>(Instance_GetDevice);
            Engine.Instance.GameRenderEnd += new GameRenderEventHandler(Instance_GameRenderEnd);
            Engine.Instance.GameRenderBegin += new GameRenderEventHandler(Instance_GameRenderBegin);
            rand = new Random();
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Padding = new System.Windows.Forms.Padding(0);

            sprite = new SpriteBatch(GraphicsDevice);
            backing = new RenderTarget2D(GraphicsDevice, this.Width, this.Height, 0, SurfaceFormat.Color);
        }

        public void SetSize()
        {
            if (GraphicsDevice == null) return;
            backing = new RenderTarget2D(GraphicsDevice, this.Width, this.Height, 0, SurfaceFormat.Color, RenderTargetUsage.DiscardContents);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (!DesignMode)
            {
                BeginDraw();
                GraphicsDevice.Clear(Color.Black);
                EndDraw();
            }
            base.OnPaint(e);
        }

        void Instance_GameRenderBegin(GameRenderEventArgs e)
        {
            BeginDraw();
            GraphicsDevice.SetRenderTarget(0, this.backing);
            GraphicsDevice.Clear(Color.Black);
        }

        void Instance_GameRenderEnd(GameRenderEventArgs e)
        {
            GraphicsDevice.SetRenderTarget(0, null);
            sprite.Begin();
            GraphicsDevice.Clear(Color.Black);
            sprite.Draw(backing.GetTexture(), new Rectangle(0, 0, this.Width, this.Height), Color.White);
            sprite.End();
            EndDraw();
        }

        void Instance_GetDevice(object sender, Engine.DeviceEventArgs e)
        {
            e.Device = this.GraphicsDevice;
        }

        protected override void Draw()
        {
            //GraphicsDevice.Clear(Color.Black);
        }
    }
}
