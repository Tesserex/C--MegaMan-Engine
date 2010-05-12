using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan;
using System.Drawing;

using SpriteGroup = System.Collections.Generic.Dictionary<string, MegaMan.Sprite>;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;

namespace Mega_Man
{
    public class SpriteComponent : Component
    {
        private Dictionary<string, SpriteGroup> sprites;
        private Dictionary<string, System.Drawing.Image> tilesheets = new Dictionary<string, System.Drawing.Image>();
        private Dictionary<string, string> sheetpaths = new Dictionary<string, string>();
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

        public override void LoadXml(XElement xmlNode)
        {
            string spriteName = "Default";
            string spritePallete = "Default";
            XAttribute spriteNameAttr = xmlNode.Attribute("name");
            if (spriteNameAttr != null) spriteName = spriteNameAttr.Value;
            XAttribute palleteAttr = xmlNode.Attribute("pallete");
            if (palleteAttr != null) spritePallete = palleteAttr.Value;

            if (xmlNode.Attribute("tilesheet") != null) // explicitly specified pallete for this sprite
            {
                MegaMan.Sprite spr = MegaMan.Sprite.FromXml(xmlNode, Game.CurrentGame.BasePath);
                string sheetpath = System.IO.Path.Combine(Game.CurrentGame.BasePath, xmlNode.Attribute("tilesheet").Value);
                spr.SetTexture(Engine.Instance.GraphicsDevice, sheetpath);
                Add(spritePallete, spriteName, spr);
            }
            else // load sprite for all palletes
            {
                foreach (KeyValuePair<string, System.Drawing.Image> pair in tilesheets)
                {
                    MegaMan.Sprite sprite = MegaMan.Sprite.FromXml(xmlNode, pair.Value);
                    sprite.SetTexture(Engine.Instance.GraphicsDevice, sheetpaths[pair.Key]);
                    Add(pair.Key, spriteName, sprite);
                }
            }
        }

        public override Effect ParseEffect(XElement node)
        {
            Effect action = new Effect((entity) => { });
            foreach (XElement prop in node.Elements())
            {
                switch (prop.Name.LocalName)
                {
                    case "Name":
                        string spritename = prop.Value;
                        action += (entity) =>
                        {
                            SpriteComponent spritecomp = entity.GetComponent<SpriteComponent>();
                            spritecomp.ChangeSprite(spritename);
                        };
                        break;

                    case "Playing":
                        bool play;
                        if (!bool.TryParse(prop.Value, out play)) throw new EntityXmlException(prop, "Playing tag must be a valid bool (true or false).");
                        action += (entity) =>
                        {
                            SpriteComponent spritecomp = entity.GetComponent<SpriteComponent>();
                            spritecomp.Playing = play;
                        };
                        break;

                    case "Visible":
                        bool vis;
                        if (!bool.TryParse(prop.Value, out vis)) throw new EntityXmlException(prop, "Visible tag must be a valid bool (true or false).");
                        action += (entity) =>
                        {
                            SpriteComponent spritecomp = entity.GetComponent<SpriteComponent>();
                            spritecomp.Visible = vis;
                        };
                        break;

                    case "Group":
                        string group = prop.Value;
                        action += (entity) =>
                        {
                            SpriteComponent spritecomp = entity.GetComponent<SpriteComponent>();
                            spritecomp.ChangeGroup(group);
                        };
                        break;
                }
            }
            return action;
        }

        public void LoadTilesheet(XElement xmlComp)
        {
            XAttribute palAttr = xmlComp.Attribute("pallete");
            string pallete = "Default";
            if (palAttr != null) pallete = palAttr.Value;
            string path = System.IO.Path.Combine(Game.CurrentGame.BasePath, xmlComp.Value);
            System.Drawing.Bitmap sheet = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(path);
            sheet.SetResolution(Const.Resolution, Const.Resolution);
            if (!tilesheets.ContainsKey(pallete))
            {
                tilesheets.Add(pallete, sheet);
                sheetpaths.Add(pallete, path);
            }
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
