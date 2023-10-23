using System;
using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.Common.Rendering;
using MegaMan.Engine.Entities;

namespace MegaMan.Engine
{
    public interface IHandlerObject
    {
        void Start();
        void Stop();
        void Draw(IRenderingContext renderContext);
    }

    public class HandlerSprite : IHandlerObject
    {
        private readonly Sprite sprite;
        private readonly SpriteAnimator animator;

        private float x, y;
        private float vx, vy, duration;
        private int stopX, stopY, moveFrame;

        public HandlerSprite(Sprite sprite, Point location)
        {
            this.sprite = new Sprite(sprite);
            animator = new SpriteAnimator(this.sprite);
            x = location.X;
            y = location.Y;
            animator.Play();
        }

        public void Start()
        {
            Engine.Instance.GameLogicTick += Update;
        }

        public void Stop()
        {
            Engine.Instance.GameLogicTick -= Update;
        }

        private void Update(GameTickEventArgs e)
        {
            animator.Update();
        }

        public void Move(int nx, int ny, int duration)
        {
            stopX = nx;
            stopY = ny;
            this.duration = duration;
            vx = (nx - x) / duration;
            vy = (ny - y) / duration;
            moveFrame = 0;

            Engine.Instance.GameLogicTick += MoveUpdate;
        }

        private void MoveUpdate(GameTickEventArgs e)
        {
            x += vx;
            y += vy;
            moveFrame++;

            if (moveFrame >= duration)
            {
                x = stopX;
                y = stopY;
                Engine.Instance.GameLogicTick -= MoveUpdate;
            }
        }

        public void Draw(IRenderingContext renderContext)
        {
            sprite.Draw(renderContext, sprite.Layer, x, y, animator.CurrentIndex);
        }
    }

    public class HandlerText : IHandlerObject
    {
        private IEntityPool entityPool;

        private string displayed = "";
        private int speed;
        private int frame;
        private Point position;
        private Binding binding;
        private string font;

        private string _content;
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                if (speed == 0)
                {
                    displayed = value;
                }
                else
                {
                    displayed = "";
                    frame = 0;
                }
            }
        }

        public HandlerText(SceneTextCommandInfo info, IEntityPool entityPool)
        {
            Content = info.Content ?? String.Empty;
            speed = info.Speed ?? 0;
            position = new Point(info.X, info.Y);
            this.entityPool = entityPool;
            font = info.Font ?? "Default";

            if (info.Binding != null)
            {
                binding = Binding.Create(info.Binding, this);
            }
        }

        public void Start()
        {
            if (binding != null)
            {
                binding.Start(entityPool);
            }

            if (speed != 0)
            {
                displayed = "";
                Engine.Instance.GameLogicTick += Update;
                frame = 0;
            }
        }

        public void Stop()
        {
            if (binding != null)
            {
                binding.Stop();
            }

            if (speed != 0)
            {
                Engine.Instance.GameLogicTick -= Update;
            }
        }

        private void Update(GameTickEventArgs e)
        {
            frame++;
            if (frame >= speed && displayed.Length < Content.Length)
            {
                // add a character to the displayed text
                displayed += Content.Substring(displayed.Length, 1);
                frame = 0;
            }
        }

        public void Draw(IRenderingContext renderContext)
        {
            FontSystem.Draw(renderContext, 5, font, displayed, position);
        }
    }

    public class HandlerFill : IHandlerObject
    {
        private IResourceImage texture;
        private Color color;
        private float x, y, width, height;
        private float vx, vy, vw, vh, duration;
        private int stopX, stopY, stopWidth, stopHeight, moveFrame;
        private int layer;

        public HandlerFill(Color color, int x, int y, int width, int height, int layer)
        {
            this.color = color;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.layer = layer;
        }

        public void Start() { }

        public void Stop() { }

        public void Draw(IRenderingContext renderContext)
        {
            if (texture == null)
                texture = renderContext.CreateColorResource(color);

            renderContext.Draw(texture, layer, new Point((int)x, (int)y),
                new Rectangle((int)x, (int)y, (int)width, (int)height));
        }

        public void Move(int nx, int ny, int nwidth, int nheight, int duration)
        {
            stopX = nx;
            stopY = ny;
            stopWidth = nwidth;
            stopHeight = nheight;
            this.duration = duration;
            vx = (nx - x) / duration;
            vy = (ny - y) / duration;
            vw = (nwidth - width) / duration;
            vh = (nheight - height) / duration;
            moveFrame = 0;

            Engine.Instance.GameLogicTick += Update;
        }

        private void Update(GameTickEventArgs e)
        {
            x += vx;
            y += vy;
            width += vw;
            height += vh;
            moveFrame++;

            if (moveFrame >= duration)
            {
                x = stopX;
                y = stopY;
                width = stopWidth;
                height = stopHeight;
                Engine.Instance.GameLogicTick -= Update;
            }
        }
    }

    public class HandlerMeter : IHandlerObject
    {
        private IGameplayContainer container;
        public HealthMeter Meter { get; set; }

        public HandlerMeter(HealthMeter meter, IGameplayContainer container)
        {
            Meter = meter;
            this.container = container;
        }

        public void Start()
        {
            Meter.Start(container);
        }

        public void Stop()
        {
            Meter.Stop();
        }

        public void Draw(IRenderingContext renderContext)
        {
            Meter.Draw(renderContext);
        }
    }
}
