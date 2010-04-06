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
        Texture2D tex;
        SpriteBatch sprite;
        public void Init()
        {
            Engine.Instance.GetDevice += new EventHandler<Engine.DeviceEventArgs>(Instance_GetDevice);
            Engine.Instance.GameRenderEnd += new GameRenderEventHandler(Instance_GameRenderEnd);
            Engine.Instance.GameRenderBegin += new GameRenderEventHandler(Instance_GameRenderBegin);
            rand = new Random();
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
            string beginDrawError = BeginDraw();
            GraphicsDevice.Clear(Color.Black);
        }

        void Instance_GameRenderEnd(GameRenderEventArgs e)
        {
            EndDraw();
        }

        void Instance_GetDevice(object sender, Engine.DeviceEventArgs e)
        {
            e.Device = this.GraphicsDevice;
            tex = Texture2D.FromFile(GraphicsDevice, "D:\\junk\\programming\\C#\\Mega Man\\Mega Man\\bin\\Debug\\Demo Project\\images\\enemies\\mm2enemysheet.png");
            sprite = new SpriteBatch(GraphicsDevice);
        }

        protected override void Initialize()
        {
            
        }

        protected override void Draw()
        {
            //GraphicsDevice.Clear(Color.Black);
        }
    }
}
