using System;
using System.Linq;
using System.Collections.Generic;
using MegaMan.Common;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;

namespace MegaMan.Engine
{
    public class SpriteComponent : Component
    {
        private readonly Dictionary<string, Sprite> sprites;
        private System.Drawing.Image _spriteSheet;
        private string _sheetPath;

        private string _defaultKey = Guid.NewGuid().ToString();
        private Dictionary<string, Sprite> currentSprites = new Dictionary<string, Sprite>();

        private bool verticalFlip;

        private bool playing;
        public bool Playing
        {
            get { return playing; }
            set
            {
                playing = value;
                foreach (var sprite in currentSprites.Values)
                {
                    if (playing) sprite.Resume();
                    else sprite.Pause();
                }
            }
        }

        public bool Visible { get; set; }

        private PositionComponent PositionSrc { get; set; }

        public bool HorizontalFlip
        {
            set
            {
                foreach (var sprite in currentSprites.Values)
                {
                    sprite.HorizontalFlip = value;
                }
            }
        }

        public SpriteComponent()
        {
            sprites = new Dictionary<string, Sprite>();

            Playing = true;
            Visible = true;
        }

        public override Component Clone()
        {
            SpriteComponent copy = new SpriteComponent();

            foreach (var spr in sprites)
            {
                copy.Add(spr.Key, new Sprite(spr.Value));
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

            if (xmlNode.Attribute("tilesheet") != null) // explicitly specified sheet for this sprite
            {
                _sheetPath = System.IO.Path.Combine(Game.CurrentGame.BasePath, xmlNode.RequireAttribute("tilesheet").Value);
            }

            Sprite sprite = Sprite.FromXml(xmlNode, _spriteSheet);
            sprite.SetTexture(Engine.Instance.GraphicsDevice, _sheetPath);
            Add(spriteName, sprite);
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

                        string key = null;
                        var keyAttr = prop.Attribute("key");
                        if (keyAttr != null)
                        {
                            key = keyAttr.Value;
                        }

                        action += entity =>
                        {
                            SpriteComponent spritecomp = entity.GetComponent<SpriteComponent>();
                            spritecomp.ChangeSprite(spritename, key);
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
            _sheetPath = System.IO.Path.Combine(Game.CurrentGame.BasePath, xmlComp.Value);
            var sheet = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(_sheetPath);
            sheet.SetResolution(Const.Resolution, Const.Resolution);
            _spriteSheet = sheet;
        }

        private void Add(string name, Sprite sprite)
        {
            sprites.Add(name, sprite);

            if (currentSprites.Count == 0)
            {
                currentSprites.Add(_defaultKey, sprite);
                sprite.Play();
            }
        }

        public void ChangePalette(int index)
        {
            var paletteName = currentSprites.Values.First().PaletteName;
            var palette = Palette.Get(paletteName);
            if (palette != null)
            {
                palette.CurrentIndex = index;
            }
        }

        private void ChangeSprite(string name, string key = null)
        {
            if (!sprites.ContainsKey(name) || sprites[name] == null)
            {
                throw new GameRunException(String.Format("A sprite with name {0} was not found in the entity {1}.", name, Parent.Name));
            }

            if (key == null) key = _defaultKey;

            if (currentSprites.ContainsKey(key)) currentSprites[key].Stop();

            currentSprites[key] = sprites[name];
            if (Playing) currentSprites[key].Play();
        }

        protected override void Update()
        {
            if (!Parent.Paused)
            {
                foreach (var sprite in currentSprites.Values)
                {
                    sprite.Update();
                }
            }
        }

        public override void RegisterDependencies(Component component)
        {
            if (component is PositionComponent) PositionSrc = component as PositionComponent;
        }

        private bool evenframe = true;
        private void Instance_GameRender(GameRenderEventArgs e)
        {
            evenframe = !evenframe;
            foreach (var sprite in currentSprites.Values)
            {
                RenderSprite(sprite, e);
            }
        }

        private void RenderSprite(Sprite currentSprite, GameRenderEventArgs e)
        {
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
                            Draw(currentSprite, e.Layers.ForegroundBatch, e.OpacityColor);
                            return;
                        }
                    }
                }
                Draw(currentSprite, e.Layers.SpritesBatch[currentSprite.Layer], e.OpacityColor);
            }
        }

        private void Draw(Sprite sprite, SpriteBatch batch, Microsoft.Xna.Framework.Color color)
        {
            if (PositionSrc == null) throw new InvalidOperationException("SpriteComponent has not been initialized with a position source.");
            float off_x = Parent.Screen.OffsetX;
            float off_y = Parent.Screen.OffsetY;
            if (sprite != null && Visible)
            {
                sprite.VerticalFlip = Parent.GravityFlip ? Game.CurrentGame.GravityFlip : verticalFlip;
                sprite.DrawXna(batch, color, (float)Math.Round(PositionSrc.Position.X - off_x), (float)Math.Round(PositionSrc.Position.Y - off_y));
            }
        }
    }
}
