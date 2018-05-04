using System;
using System.Collections.Generic;
using MegaMan.Common.Geometry;
using MegaMan.Common;
using MegaMan.Common.Rendering;

namespace MegaMan.Engine
{
    public class HealthMeter
    {
        private MeterInfo info;
        private Binding binding;
        private float value;
        private float maxvalue = 28;
        private float tickSize;
        private IResourceImage meterTexture;
        private IResourceImage tickTexture;
        private string sound;
        private int tickframes;
        private int stopvalue;

        private IGameplayContainer container;

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

        private static readonly List<HealthMeter> allMeters = new List<HealthMeter>();

        public static IEnumerable<HealthMeter> AllMeters
        {
            get { return allMeters.AsReadOnly(); }
        }

        public static void Unload()
        {
            foreach (var meter in allMeters)
            {
                meter.Stop();
            }

            allMeters.Clear();
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
                return value;
            }
            set
            {
                if (running && value > this.value) // tick up slowly
                {
                    stopvalue = (int)value;
                    if (!animating)
                    {
                        tickframes = 0;
                        Engine.Instance.GameLogicTick += GameTick;
                        if (IsPlayer) container.PauseHandler();
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
            
            if (tickframes >= 3)
            {
                tickframes = 0;
                value += tickSize;
            }
        }

        public float MaxValue
        {
            get { return maxvalue; }
            set
            {
                maxvalue = value;
                tickSize = maxvalue / 28;
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

        private HealthMeter()
        {
            value = maxvalue;
            running = false;
        }

        private int filldelay;
        public void DelayedFill(int frames)
        {
            Value = 0;
            filldelay = frames;
            container.GameThink += DelayFill;
        }

        void DelayFill()
        {
            filldelay--;
            if (filldelay == 0)
            {
                container.GameThink -= DelayFill;
                Value = MaxValue;
            }
        }

        private void LoadInfo(MeterInfo info)
        {
            this.info = info;
            positionX = info.Position.X;
            positionY = info.Position.Y;

            if (info.Binding != null)
            {
                this.binding = Binding.Create(info.Binding, this);
                MaxValue = 1; // use 0 - 1 range for values
            }

            horizontal = (info.Orient == MeterInfo.Orientation.Horizontal);
            tickOffset = new MegaMan.Common.Geometry.Point(info.TickOffset.X, info.TickOffset.Y);

            if (info.Sound != null) sound = Engine.Instance.SoundSystem.EffectFromInfo(info.Sound);
        }

        public void Reset()
        {
            value = maxvalue;
        }

        public void Draw(IRenderingContext context)
        {
            Draw(context, positionX, positionY);
        }

        private void Draw(IRenderingContext context, float positionX, float positionY)
        {
            if (meterTexture == null)
            {
                meterTexture = context.LoadResource(this.info.Background, this.info.BackgroundData);
                bounds = new RectangleF(positionX, positionY, meterTexture.Width, meterTexture.Height);
            }

            if (tickTexture == null)
                tickTexture = context.LoadResource(this.info.TickImage, this.info.TickImageData);

            if (tickTexture != null)
            {
                int i = 0;
                int ticks = (int)Math.Ceiling(value / tickSize);
                // prevent float errors
                if (ticks > 28) ticks = 28;

                if (meterTexture != null)
                    context.Draw(meterTexture, 4, new Common.Geometry.Point((int)positionX, (int)positionY));

                if (horizontal)
                {
                    for (int y = (int)positionX; i < ticks; i++, y += tickTexture.Width)
                    {
                        context.Draw(tickTexture, 4, new Common.Geometry.Point(y, (int)positionY));
                    }
                }
                else
                {
                    for (int y = 54 + (int)positionY; i < ticks; i++, y -= tickTexture.Height)
                    {
                        context.Draw(tickTexture, 4, new Common.Geometry.Point((int)(positionX + tickOffset.X), (int)(y + tickOffset.Y)));
                    }
                }
            }
        }

        public void Start(IGameplayContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            this.container = container;

            if (this.binding != null)
            {
                this.binding.Start(container.Entities);
            }

            container.Draw += GameRender;
            running = true;
        }

        public void Stop()
        {
            if (this.binding != null)
            {
                this.binding.Stop();
            }

            Engine.Instance.GameLogicTick -= GameTick;
            if (container != null)
            {
                container.Draw -= GameRender;
            }
            running = false;
        }

        private void GameTick(GameTickEventArgs e)
        {
            UpTick();
            if (value >= stopvalue || value >= maxvalue)
            {
                Engine.Instance.GameLogicTick -= GameTick;
                animating = false;
                if (IsPlayer) container.ResumeHandler();
                if (sound != null) Engine.Instance.SoundSystem.StopSfx(sound);
            }
        }

        public void GameRender(GameRenderEventArgs e)
        {
            if (inGamePlay) Draw(e.RenderContext, positionX, positionY);
        }
    }
}
