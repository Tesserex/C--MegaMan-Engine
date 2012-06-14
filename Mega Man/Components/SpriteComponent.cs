using System;
using System.Collections.Generic;
using MegaMan.Common;
using SpriteGroup = System.Collections.Generic.Dictionary<string, MegaMan.Common.Sprite>;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;

namespace MegaMan.Engine
{
    public class SpriteComponent : Component
    {
        private readonly Dictionary<string, SpriteGroup> sprites;
        private readonly Dictionary<string, System.Drawing.Image> tilesheets = new Dictionary<string, System.Drawing.Image>();
        private readonly Dictionary<string, string> sheetpaths = new Dictionary<string, string>();
        private Sprite currentSprite;
        private string currentPalette = "Default";
        private string currentSpriteName;

        private bool verticalFlip;

        private bool playing;
        public bool Playing
        {
            get { return playing; }
            set
            {
                playing = value;
                if (currentSprite != null)
                {
                    if (playing) currentSprite.Resume();
                    else currentSprite.Pause();
                }
            }
        }

        public string Name
        {
            get { return currentSpriteName; }
        }

        public string Group
        {
            get { return currentPalette; }
        }

        public bool Visible { get; set; }

        private PositionComponent PositionSrc { get; set; }

        public bool HorizontalFlip
        {
            set { currentSprite.HorizontalFlip = value; }
        }

        public SpriteComponent()
        {
            sprites = new Dictionary<string, SpriteGroup> {{"Default", new SpriteGroup()}};

            Playing = true;
            Visible = true;
        }

        public override Component Clone()
        {
            SpriteComponent copy = new SpriteComponent();

            foreach (KeyValuePair<string, SpriteGroup> pair in sprites)
            {
                foreach (KeyValuePair<string, Sprite> spr in pair.Value)
                {
                    copy.Add(pair.Key, spr.Key, new Sprite(spr.Value));
                }
            }
            copy.verticalFlip = verticalFlip;

            return copy;
        }

        public override void Start()
        {
            Parent.Container.GameThink += Update;
            Parent.Container.Draw += Instance_GameRender;
        }

        public override void Stop()
        {
            Parent.Container.GameThink -= Update;
            Parent.Container.Draw -= Instance_GameRender;
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
                Sprite spr = Sprite.FromXml(xmlNode, Game.CurrentGame.BasePath);
                string sheetpath = System.IO.Path.Combine(Game.CurrentGame.BasePath, xmlNode.RequireAttribute("tilesheet").Value);
                spr.SetTexture(Engine.Instance.GraphicsDevice, sheetpath);
                Add(spritePallete, spriteName, spr);
            }
            else // load sprite for all palletes
            {
                foreach (KeyValuePair<string, System.Drawing.Image> pair in tilesheets)
                {
                    Sprite sprite = Sprite.FromXml(xmlNode, pair.Value);
                    sprite.SetTexture(Engine.Instance.GraphicsDevice, sheetpaths[pair.Key]);
                    Add(pair.Key, spriteName, sprite);
                }
            }
        }

        public static Effect ParseEffect(XElement node)
        {
            Effect action = entity => { };
            foreach (XElement prop in node.Elements())
            {
                switch (prop.Name.LocalName)
                {
                    case "Name":
                        string spritename = prop.Value;
                        action += entity =>
                        {
                            SpriteComponent spritecomp = entity.GetComponent<SpriteComponent>();
                            spritecomp.ChangeSprite(spritename);
                        };
                        break;

                    case "Playing":
                        bool play = prop.GetBool();
                        action += entity =>
                        {
                            SpriteComponent spritecomp = entity.GetComponent<SpriteComponent>();
                            spritecomp.Playing = play;
                        };
                        break;

                    case "Visible":
                        bool vis = prop.GetBool();
                        action += entity =>
                        {
                            SpriteComponent spritecomp = entity.GetComponent<SpriteComponent>();
                            spritecomp.Visible = vis;
                        };
                        break;

                    case "Group":
                        string group = prop.Value;
                        action += entity =>
                        {
                            SpriteComponent spritecomp = entity.GetComponent<SpriteComponent>();
                            spritecomp.ChangeGroup(group);
                        };
                        break;

                    case "Palette":
                        string pal = prop.RequireAttribute("name").Value;
                        int index = prop.GetInteger("index");
                        action += entity =>
                        {
                            var palette = Palette.Get(pal);
                            if (palette != null)
                            {
                                palette.CurrentIndex = index;
                            }
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
            System.Drawing.Bitmap sheet = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(path);
            sheet.SetResolution(Const.Resolution, Const.Resolution);
            if (!tilesheets.ContainsKey(pallete))
            {
                tilesheets.Add(pallete, sheet);
                sheetpaths.Add(pallete, path);
            }
        }

        private void Add(string palette, string name, Sprite sprite)
        {
            if (!sprites.ContainsKey(palette)) sprites.Add(palette, new SpriteGroup());

            sprites[palette].Add(name, sprite);
            if (sprites.Count == 1) currentPalette = palette;
            if (currentSprite == null)
            {
                currentSpriteName = name;
                currentSprite = sprite;
                currentSprite.Play();
            }
        }

        public void ChangePalette(int index)
        {
            var paletteName = currentSprite.PaletteName;
            var palette = Palette.Get(paletteName);
            if (palette != null)
            {
                palette.CurrentIndex = index;
            }
        }

        public void ChangeGroup(string group)
        {
            if (!sprites.ContainsKey(group) || sprites[group] == null) return;

            int frame = currentSprite.CurrentFrame;
            int time = currentSprite.FrameTime;

            currentPalette = group;

            ChangeSprite(currentSpriteName);
            currentSprite.CurrentFrame = frame;
            currentSprite.FrameTime = time;
        }

        private void ChangeSprite(string key)
        {
            if (!sprites[currentPalette].ContainsKey(key) || sprites[currentPalette][key] == null)
            {
                throw new GameRunException(String.Format("A sprite with name {0} was not found in the entity {1}.", key, Parent.Name));
            }

            if (currentSprite != null) currentSprite.Stop();

            currentSprite = sprites[currentPalette][key];
            currentSpriteName = key;
            if (Playing) currentSprite.Play();
        }

        protected override void Update()
        {
            if (!Parent.Paused) currentSprite.Update();
        }

        public override void RegisterDependencies(Component component)
        {
            if (component is PositionComponent) PositionSrc = component as PositionComponent;
        }

        private bool evenframe = true;
        private void Instance_GameRender(GameRenderEventArgs e)
        {
            evenframe = !evenframe;
            if (currentSprite.Layer < e.Layers.SpritesBatch.Length && (
                (currentSprite.Layer == 0 && Engine.Instance.SpritesOne) ||
                (currentSprite.Layer == 1 && Engine.Instance.SpritesTwo) ||
                (currentSprite.Layer == 2 && Engine.Instance.SpritesThree) ||
                (currentSprite.Layer == 3 && Engine.Instance.SpritesFour)
                ))
            {
                if (evenframe && Engine.Instance.Foreground)
                {
                    foreach (var meter in HealthMeter.AllMeters)
                    {
                        var bounds = currentSprite.BoundBox;
                        bounds.Offset(-currentSprite.HotSpot.X, -currentSprite.HotSpot.Y);
                        bounds.Offset(PositionSrc.Position);
                        if (meter.Bounds.IntersectsWith(bounds))
                        {
                            Draw(e.Layers.ForegroundBatch, e.OpacityColor);
                            return;
                        }
                    }
                }
                Draw(e.Layers.SpritesBatch[currentSprite.Layer], e.OpacityColor);
            }
        }

        private void Draw(SpriteBatch batch, Microsoft.Xna.Framework.Color color)
        {
            if (PositionSrc == null) throw new InvalidOperationException("SpriteComponent has not been initialized with a position source.");
            float off_x = Parent.Screen.OffsetX;
            float off_y = Parent.Screen.OffsetY;
            if (currentSprite != null && Visible)
            {
                currentSprite.VerticalFlip = Parent.GravityFlip ? Game.CurrentGame.GravityFlip : verticalFlip;
                currentSprite.DrawXna(batch, color, (float)Math.Round(PositionSrc.Position.X - off_x), (float)Math.Round(PositionSrc.Position.Y - off_y));
            }
        }
    }
}
