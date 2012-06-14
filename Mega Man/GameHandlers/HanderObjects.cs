using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MegaMan.Engine
{
    public interface IHandlerObject
    {
        void Start();
        void Stop();
        void Draw(GameGraphicsLayers layers, Color opacity);
    }

    public class HandlerSprite : IHandlerObject
    {
        private Sprite sprite;

        private float x, y;
        private float vx, vy, duration;
        private int stopX, stopY, moveFrame;

        public HandlerSprite(Sprite sprite, Point location)
        {
            this.sprite = new Sprite(sprite);
            this.sprite.SetTexture(Engine.Instance.GraphicsDevice, this.sprite.SheetPath.Absolute);
            this.x = location.X;
            this.y = location.Y;
            this.sprite.Play();
        }

        public void Start()
        {
            Engine.Instance.GameLogicTick += Update;
        }

        public void Stop()
        {
            Engine.Instance.GameLogicTick -= Update;
        }

        public void Reset()
        {
            sprite.Reset();
        }

        private void Update(GameTickEventArgs e)
        {
            sprite.Update();
        }

        public void Move(int nx, int ny, int duration)
        {
            this.stopX = nx;
            this.stopY = ny;
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

        public void Draw(GameGraphicsLayers layers, Color opacity)
        {
            sprite.DrawXna(layers.SpritesBatch[sprite.Layer], opacity, x, y);
        }
    }

    public class HandlerText : IHandlerObject
    {
        private IEntityContainer container;

        private string displayed = "";
        private int speed;
        private int frame;
        private Vector2 position;
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

        public HandlerText(SceneTextCommandInfo info, IEntityContainer container)
        {
            this.Content = info.Content ?? String.Empty;
            this.speed = info.Speed ?? 0;
            this.position = new Vector2(info.X, info.Y);
            this.container = container;
            this.font = info.Font ?? "Default";

            if (info.Binding != null)
            {
                this.binding = Binding.Create(info.Binding, this);
            }
        }

        public void Start()
        {
            if (this.binding != null)
            {
                this.binding.Start(container);
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
            if (this.binding != null)
            {
                this.binding.Stop();
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

        public void Draw(GameGraphicsLayers layers, Color opacity)
        {
            FontSystem.Draw(layers.ForegroundBatch, font, displayed, position);
        }
    }

    public class HandlerFill : IHandlerObject
    {
        private Texture2D texture;
        private float x, y, width, height;
        private float vx, vy, vw, vh, duration;
        private int stopX, stopY, stopWidth, stopHeight, moveFrame;
        private int layer;

        public HandlerFill(Color color, int x, int y, int width, int height, int layer)
        {
            this.texture = new Texture2D(Engine.Instance.GraphicsDevice, 1, 1);
            this.texture.SetData(new Color[] { color });
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.layer = layer;
        }

        public void Start() { }

        public void Stop() { }

        public void Draw(GameGraphicsLayers layers, Color opacity)
        {
            layers.SpritesBatch[layer].Draw(texture, new Rectangle((int)x, (int)y, (int)width, (int)height), opacity);
        }

        public void Move(int nx, int ny, int nwidth, int nheight, int duration)
        {
            this.stopX = nx;
            this.stopY = ny;
            this.stopWidth = nwidth;
            this.stopHeight = nheight;
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
            this.Meter = meter;
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

        public void Draw(GameGraphicsLayers layers, Color opacity)
        {
            Meter.Draw(layers.SpritesBatch[3]);
        }
    }
}
