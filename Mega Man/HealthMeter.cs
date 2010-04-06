using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Mega_Man
{
    public class HealthMeter : IHandleGameEvents
    {
        private float value;
        private float maxvalue;
        private float tickSize;
        private Bitmap meterImage;
        private Texture2D meterTexture;
        private Image tick;
        private Texture2D tickTexture;
        private int sound;
        private int tickframes;
        private int stopvalue;

        private bool running;

        private float positionX;
        private float positionY;

        private bool horizontal;

        /// <summary>
        /// Do not rely on this as the actual health. It can differ while it animates. It only reflects the value present in the bar itself.
        /// </summary>
        public float Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (running && value > this.value) // tick up slowly
                {
                    tickframes = 0;
                    stopvalue = (int)value;
                    Engine.Instance.GameLogicTick -= new GameTickEventHandler(GameTick);
                    Engine.Instance.GameLogicTick += new GameTickEventHandler(GameTick);
                }
                else
                {
                    this.value = value;
                    if (this.value < 0) this.value = 0;
                    ResetImage();
                }
            }
        }

        void UpTick()
        {
            tickframes++;
            if (tickframes >= 3)
            {
                tickframes = 0;
                this.value += this.tickSize;
                Engine.Instance.PlaySound(sound);
                ResetImage();
            }
        }

        public float MaxValue
        {
            get { return this.maxvalue; }
            set
            {
                this.maxvalue = value;
                this.tickSize = this.maxvalue / 28;
                ResetImage();
            }
        }

        public HealthMeter()
        {
            this.value = this.maxvalue;
            sound = Engine.Instance.LoadSoundEffect(System.IO.Path.Combine(Game.CurrentGame.BasePath, "sounds\\health.wav"), false);
            running = false;
        }

        public void LoadXml(XElement node)
        {
            this.positionX = float.Parse(node.Attribute("x").Value);
            this.positionY = float.Parse(node.Attribute("y").Value);
            XAttribute imageAttr = node.Attribute("image");
            if (imageAttr == null) throw new EntityXmlException(node, "HealthMeters must have an image attribute to specify the tick image.");
            
            if (this.tick != null) this.tick.Dispose();
            if (this.tickTexture != null) this.tickTexture.Dispose();
            this.tick = Image.FromFile(System.IO.Path.Combine(Game.CurrentGame.BasePath, imageAttr.Value));
            this.tickTexture = Texture2D.FromFile(Engine.Instance.GraphicsDevice, System.IO.Path.Combine(Game.CurrentGame.BasePath, imageAttr.Value));

            bool horiz = false;
            XAttribute dirAttr = node.Attribute("orientation");
            if (dirAttr != null)
            {
                horiz = (dirAttr.Value == "horizontal");
            }
            this.horizontal = horiz;

            if (horizontal) this.meterImage = new Bitmap(56, 8);
            else this.meterImage = new Bitmap(8, 56);

            this.meterImage.SetResolution(Const.Resolution, Const.Resolution);
            ResetImage();
        }

        public void Reset()
        {
            this.value = this.maxvalue;
            ResetImage();
        }

        private void ResetImage()
        {
            if (meterImage == null) return;
            using (Graphics g = Graphics.FromImage(meterImage))
            {
                g.Clear(System.Drawing.Color.Black);
                int i = 0;
                int ticks = (int)Math.Round(this.value / this.tickSize);

                if (this.tick != null && this.tickTexture != null)
                {
                    if (this.horizontal)
                    {
                        for (int y = 0; i < ticks; i++, y += tick.Width)
                        {
                            g.DrawImage(tick, y, 0);
                        }
                    }
                    else
                    {
                        for (int y = (int)this.Height - tick.Height; i < ticks; i++, y -= tick.Height)
                        {
                            g.DrawImage(tick, 0, y);
                        }
                    }
                }
            }
        }

        public float Width
        {
            get { return meterImage.Width; }
        }

        public float Height
        {
            get { return meterImage.Height; }
        }

        public void Draw(Graphics g)
        {
            Draw(g, positionX, positionY);
        }

        public void Draw(Graphics g, float positionX, float positionY)
        {
            if (meterImage != null) g.DrawImage(meterImage, positionX, positionY);
        }

        public void Draw(SpriteBatch batch)
        {
            Draw(batch, positionX, positionY);
        }

        public void Draw(SpriteBatch batch, float positionX, float positionY)
        {
            if (this.tickTexture != null)
            {
                int i = 0;
                int ticks = (int)Math.Round(this.value / this.tickSize);

                if (this.horizontal)
                {
                    for (int y = (int)positionX; i < ticks; i++, y += tick.Width)
                    {
                        batch.Draw(tickTexture, new Microsoft.Xna.Framework.Vector2(y, positionY), Engine.Instance.OpacityColor);
                    }
                }
                else
                {
                    for (int y = (int)this.Height - tick.Height + (int)positionY; i < ticks; i++, y -= tick.Height)
                    {
                        batch.Draw(tickTexture, new Microsoft.Xna.Framework.Vector2(positionX, y), Engine.Instance.OpacityColor);
                    }
                }
            }
        }

        #region IHandleGameEvents Members

        public void StartHandler()
        {
            Engine.Instance.GameRender += new GameRenderEventHandler(GameRender);
            Game.CurrentGame.AddGameHandler(this);
            running = true;
        }

        public void StopHandler()
        {
            Engine.Instance.GameLogicTick -= new GameTickEventHandler(GameTick);
            Engine.Instance.GameRender -= new GameRenderEventHandler(GameRender);
            Game.CurrentGame.RemoveGameHandler(this);
            running = false;
        }

        public void GameInputReceived(GameInputEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void GameTick(GameTickEventArgs e)
        {
            UpTick();
            if (this.value >= stopvalue || this.value >= maxvalue) Engine.Instance.GameLogicTick -= new GameTickEventHandler(GameTick);
        }

        public void GameRender(GameRenderEventArgs e)
        {
            using (Graphics g = Graphics.FromImage(e.Layers.Sprites[1]))
            {
                this.Draw(g, positionX, positionY);
            }
            this.Draw(e.Layers.ForegroundBatch, positionX, positionY);
        }

        #endregion
    }
}
