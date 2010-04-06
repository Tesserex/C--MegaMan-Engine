using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan;
using System.Drawing;

using SpriteGroup = System.Collections.Generic.Dictionary<string, MegaMan.Sprite>;
using Microsoft.Xna.Framework.Graphics;

namespace Mega_Man
{
    public class SpriteComponent : Component
    {
        private Dictionary<string, SpriteGroup> sprites;
        private Sprite sprite = null;
        private string group = "Default";
        private string name = null;

        public string Name { get { return this.name; } }
        public string Group { get { return this.group; } }

        private bool verticalFlip;

        private bool playing;
        public bool Playing
        {
            get { return playing; }
            set
            {
                playing = value;
                if (sprite != null)
                {
                    if (playing) sprite.Resume();
                    else sprite.Pause();
                }
            }
        }

        public bool Visible { get; set; }

        public IPositioned PositionSrc { get; private set; }
        public IMovement MovementSrc { get; private set; }

        public bool HorizontalFlip
        {
            get { return sprite.HorizontalFlip; }
            set { sprite.HorizontalFlip = value; }
        }

        public SpriteComponent()
        {
            sprites = new Dictionary<string, SpriteGroup>();
            sprites.Add("Default", new SpriteGroup());

            Playing = true;
            Visible = true;
        }

        public override Component Clone()
        {
            SpriteComponent copy = new SpriteComponent();

            foreach (KeyValuePair<string, SpriteGroup> pair in this.sprites)
            {
                foreach (KeyValuePair<string, Sprite> spr in pair.Value)
                {
                    copy.Add(pair.Key, spr.Key, new Sprite(spr.Value));
                }
            }
            copy.verticalFlip = this.verticalFlip;

            return copy;
        }

        public override void  Start()
        {
            Engine.Instance.GameThink += Update;
            Engine.Instance.GameRender += new GameRenderEventHandler(Instance_GameRender);
        }

        public override void Stop()
        {
            Engine.Instance.GameThink -= Update;
            Engine.Instance.GameRender -= new GameRenderEventHandler(Instance_GameRender);
        }

        public override void Message(IGameMessage msg)
        {
            
        }

        public void Add(string group, string name, MegaMan.Sprite sprite)
        {
            if (!sprites.ContainsKey(group)) sprites.Add(group, new SpriteGroup());

            sprites[group].Add(name, sprite);
            if (sprites.Count == 1) this.group = group;
            if (this.sprite == null)
            {
                this.name = name;
                this.sprite = sprite;
                this.sprite.Play();
            }
        }

        public void ChangeGroup(string group)
        {
            if (!sprites.ContainsKey(group) || sprites[group] == null) return;

            int frame = sprite.CurrentFrame;
            int time = sprite.FrameTime;

            this.group = group;

            ChangeSprite(this.name);
            sprite.CurrentFrame = frame;
            sprite.FrameTime = time;
        }

        public void ChangeSprite(string key)
        {
            if (!sprites[this.group].ContainsKey(key) || sprites[this.group][key] == null) throw new KeyNotFoundException("A sprite with key \""+key+"\" was not found in the collection.");
            if (sprite != null) sprite.Stop();

            sprite = this.sprites[this.group][key];
            this.name = key;
            if (this.Playing) sprite.Play();
        }

        protected override void Update()
        {
            sprite.Update();
        }

        public override void RegisterDependencies(Component component)
        {
            if (component is IPositioned) this.PositionSrc = component as IPositioned;
            else if (component is IMovement) this.MovementSrc = component as IMovement;
        }

        private void Instance_GameRender(GameRenderEventArgs e)
        {
            if (sprite.Layer < e.Layers.SpritesBatch.Length)
            {
                Draw(e.Device, e.Layers.SpritesBatch[sprite.Layer], e.OpacityColor);
            }
        }

        private void Draw(GraphicsDevice device, SpriteBatch batch, Microsoft.Xna.Framework.Graphics.Color color)
        {
            if (PositionSrc == null) throw new InvalidOperationException("SpriteComponent has not been initialized with a position source.");
            float off_x = Parent.Screen.OffsetX;
            float off_y = Parent.Screen.OffsetY;
            if (sprite != null && Visible)
            {
                sprite.VerticalFlip = Parent.GravityFlip ? Game.CurrentGame.GravityFlip : this.verticalFlip;
                sprite.DrawXna(batch, color, PositionSrc.Position.X - off_x, PositionSrc.Position.Y - off_y);
            }
        }
    }
}
