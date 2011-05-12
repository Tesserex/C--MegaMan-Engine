using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using MegaMan;

namespace Mega_Man
{
    public class HealthMeter : IHandleGameEvents
    {
        private float value;
        private float maxvalue;
        private float tickSize;
        private Texture2D meterTexture;
        private Texture2D tickTexture;
        private string sound;
        private int tickframes;
        private int stopvalue;

        private Point tickOffset;

        private bool running;
        private bool animating;

        private float positionX;
        private float positionY;

        private bool horizontal;

        // true if the meter is shown overlaid on gameplay, not in a pause screen
        private bool inGamePlay;

        private RectangleF bounds;
        public RectangleF Bounds { get { return bounds; } }

        private static List<HealthMeter> allMeters = new List<HealthMeter>();

        public static IEnumerable<HealthMeter> AllMeters
        {
            get { return allMeters.AsReadOnly(); }
        }

        // this is just needed so we can pause the game when filling
        public bool IsPlayer { get; set; }

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
                    if (!animating)
                    {
                        Engine.Instance.GameLogicTick += new GameTickEventHandler(GameTick);
                        animating = true;
                        if (sound != null) Engine.Instance.SoundSystem.PlaySfx(sound);
                    }
                }
                else
                {
                    this.value = value;
                    if (this.value < 0) this.value = 0;
                }
            }
        }

        void UpTick()
        {
            tickframes++;
            if (tickframes > 1 && IsPlayer) Game.CurrentGame.Pause();
            if (tickframes >= 3)
            {
                tickframes = 0;
                this.value += this.tickSize;
            }
        }

        public float MaxValue
        {
            get { return this.maxvalue; }
            set
            {
                this.maxvalue = value;
                this.tickSize = this.maxvalue / 28;
            }
        }

        public static HealthMeter Create(MeterInfo info, bool inGamePlay)
        {
            var meter = new HealthMeter();
            meter.LoadInfo(info);
            meter.inGamePlay = inGamePlay;
            if (inGamePlay) allMeters.Add(meter);
            return meter;
        }

        public static HealthMeter Create(XElement node, bool inGamePlay)
        {
            var meter = new HealthMeter();
            meter.LoadXml(node);
            meter.inGamePlay = inGamePlay;
            if (inGamePlay) allMeters.Add(meter);
            return meter;
        }

        private HealthMeter()
        {
            this.value = this.maxvalue;
            running = false;
        }

        private int filldelay = 0;
        public void DelayedFill(int frames)
        {
            this.Value = 0;
            filldelay = frames;
            Engine.Instance.GameThink += DelayFill;
        }

        void DelayFill()
        {
            filldelay--;
            if (filldelay == 0)
            {
                Engine.Instance.GameThink -= DelayFill;
                this.Value = this.MaxValue;
            }
        }

        private void LoadInfo(MeterInfo info)
        {
            this.positionX = info.Position.X;
            this.positionY = info.Position.Y;

            if (this.tickTexture != null) this.tickTexture.Dispose();
            StreamReader srTick = new StreamReader(info.TickImage.Absolute);
            this.tickTexture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, srTick.BaseStream);

            if (info.Background != null)
            {
                StreamReader srMeter = new StreamReader(info.Background.Absolute);
                this.meterTexture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, srMeter.BaseStream);
                this.bounds = new RectangleF(this.positionX, this.positionY, this.meterTexture.Width, this.meterTexture.Height);
            }

            this.horizontal = (info.Orient == MeterInfo.Orientation.Horizontal);
            this.tickOffset = info.TickOffset;

            if (info.Sound != null) this.sound = Engine.Instance.SoundSystem.EffectFromInfo(info.Sound);
        }

        private void LoadXml(XElement node)
        {
            this.positionX = node.GetFloat("x");
            this.positionY = node.GetFloat("y");
            XAttribute imageAttr = node.RequireAttribute("image");
            
            if (this.tickTexture != null) this.tickTexture.Dispose();
			StreamReader srTick = new StreamReader(System.IO.Path.Combine(Game.CurrentGame.BasePath, System.IO.Path.Combine(Game.CurrentGame.BasePath, imageAttr.Value)));
			this.tickTexture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, srTick.BaseStream);

            XAttribute backAttr = node.Attribute("background");
			if (backAttr != null)
            {
                StreamReader srMeter = new StreamReader(System.IO.Path.Combine(Game.CurrentGame.BasePath, System.IO.Path.Combine(Game.CurrentGame.BasePath, backAttr.Value)));
                this.meterTexture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, srMeter.BaseStream);
                this.bounds = new RectangleF(this.positionX, this.positionY, this.meterTexture.Width, this.meterTexture.Height);
            }

            bool horiz = false;
            XAttribute dirAttr = node.Attribute("orientation");
            if (dirAttr != null)
            {
                horiz = (dirAttr.Value == "horizontal");
            }
            this.horizontal = horiz;

            int x = 0; int y = 0;
            node.TryInteger("tickX", out x);
            node.TryInteger("tickY", out y);

            this.tickOffset = new Point(x, y);

            XElement soundNode = node.Element("Sound");
            if (soundNode != null) sound = Engine.Instance.SoundSystem.EffectFromXml(soundNode);
        }

        public void Reset()
        {
            this.value = this.maxvalue;
        }

        public void Draw(SpriteBatch batch)
        {
            Draw(batch, positionX, positionY);
        }

        private void Draw(SpriteBatch batch, float positionX, float positionY)
        {
            if (this.tickTexture != null)
            {
                int i = 0;
                int ticks = (int)Math.Ceiling(this.value / this.tickSize);
                // prevent float errors
                if (ticks > 28) ticks = 28;

                if (this.meterTexture != null) batch.Draw(this.meterTexture, new Microsoft.Xna.Framework.Vector2(positionX, positionY), Engine.Instance.OpacityColor);
                if (this.horizontal)
                {
                    for (int y = (int)positionX; i < ticks; i++, y += tickTexture.Width)
                    {
                        batch.Draw(tickTexture, new Microsoft.Xna.Framework.Vector2(y, positionY), Engine.Instance.OpacityColor);
                    }
                }
                else
                {
                    for (int y = 54 + (int)positionY; i < ticks; i++, y -= tickTexture.Height)
                    {
                        batch.Draw(tickTexture, new Microsoft.Xna.Framework.Vector2(positionX + tickOffset.X, y + tickOffset.Y), Engine.Instance.OpacityColor);
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
            if (this.value >= stopvalue || this.value >= maxvalue)
            {
                Engine.Instance.GameLogicTick -= new GameTickEventHandler(GameTick);
                animating = false;
                if (IsPlayer) Game.CurrentGame.Unpause();
                if (sound != null) Engine.Instance.SoundSystem.StopSfx(sound);
            }
        }

        public void GameRender(GameRenderEventArgs e)
        {
            if (inGamePlay && Engine.Instance.SpritesFour) this.Draw(e.Layers.SpritesBatch[3], positionX, positionY);
        }

        #endregion
    }
}
