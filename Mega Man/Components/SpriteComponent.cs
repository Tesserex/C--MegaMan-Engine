using System;
using System.Linq;
using System.Collections.Generic;
using MegaMan.Common;
using System.Xml.Linq;
using MegaMan.IO.Xml;
using MegaMan.Common.Geometry;
using MegaMan.Common.Rendering;
using MegaMan.Common.Entities;

namespace MegaMan.Engine
{
    public class SpriteComponent : Component
    {
        private readonly Dictionary<string, SpriteGroup> _sprites;
        private FilePath _sheetPath;

        private SpriteGroup currentSpriteGroup;

        private bool verticalFlip;

        private bool playing;
        public bool Playing
        {
            get { return playing; }
            set
            {
                playing = value;

                if (currentSpriteGroup != null)
                {
                    foreach (var sprite in currentSpriteGroup)
                    {
                        if (playing) sprite.Resume();
                        else sprite.Pause();
                    }
                }
            }
        }

        public bool Visible { get; set; }

        private PositionComponent PositionSrc { get; set; }

        public bool HorizontalFlip
        {
            set
            {
                if (currentSpriteGroup != null)
                {
                    foreach (var sprite in currentSpriteGroup)
                    {
                        sprite.HorizontalFlip = value;
                    }
                }
            }
        }

        public SpriteComponent()
        {
            _sprites = new Dictionary<string, SpriteGroup>();

            Playing = true;
            Visible = true;
        }

        public override Component Clone()
        {
            SpriteComponent copy = new SpriteComponent();

            foreach (var group in _sprites)
            {
                foreach (var spr in group.Value.NamedSprites)
                {
                    var copySprite = new Sprite(spr.Value);
                    copy.Add(group.Key, copySprite, spr.Key);
                }
            }

            copy.verticalFlip = verticalFlip;

            return copy;
        }

        public override void Start(IGameplayContainer container)
        {
            container.GameThink += Update;
            container.Draw += Instance_GameRender;
        }

        public override void Stop(IGameplayContainer container)
        {
            container.GameThink -= Update;
            container.Draw -= Instance_GameRender;
        }

        public override void Message(IGameMessage msg)
        {
            
        }

        internal void LoadInfo(SpriteComponentInfo componentInfo)
        {
            _sheetPath = componentInfo.SheetPath;

            foreach (var sprite in componentInfo.Sprites.Values)
                Add(sprite.Name ?? "Default", sprite, sprite.Part);
        }
        
        public override void LoadXml(XElement xmlNode)
        {
            throw new NotSupportedException("Should not call LoadXml for sprites anymore.");
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
                        bool play = prop.GetValue<bool>();
                        action += entity =>
                        {
                            SpriteComponent spritecomp = entity.GetComponent<SpriteComponent>();
                            spritecomp.Playing = play;
                        };
                        break;

                    case "Visible":
                        bool vis = prop.GetValue<bool>();
                        action += entity =>
                        {
                            SpriteComponent spritecomp = entity.GetComponent<SpriteComponent>();
                            spritecomp.Visible = vis;
                        };
                        break;

                    case "Palette":
                        string pal = prop.RequireAttribute("name").Value;
                        int index = prop.GetAttribute<int>("index");
                        action += entity =>
                        {
                            var palette = PaletteSystem.Get(pal);
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

        public void Add(string name, Sprite sprite, string partName = null)
        {
            SpriteGroup group;
            if (_sprites.ContainsKey(name))
            {
                group = _sprites[name];
            }
            else
            {
                group = new SpriteGroup(this);
                _sprites.Add(name, group);

                if (currentSpriteGroup == null)
                {
                    currentSpriteGroup = group;
                }
            }

            group.Add(sprite, partName);

            if (group == currentSpriteGroup)
            {
                sprite.Play();
            }
        }

        /// <summary>
        /// This should get deprecated. Palettes should be changed globally
        /// by the callers knowing their names.
        /// </summary>
        public void ChangePalette(int index)
        {
            currentSpriteGroup.ChangePalette(index);
        }

        private void ChangeSprite(string name)
        {
            if (!_sprites.ContainsKey(name) || _sprites[name] == null)
            {
                throw new GameRunException(String.Format("A sprite with name {0} was not found in the entity {1}.", name, Parent.Name));
            }

            foreach (var sprite in currentSpriteGroup)
            {
                sprite.Stop();
            }

            currentSpriteGroup = _sprites[name];
            
            if (Playing)
            {
                foreach (var sprite in currentSpriteGroup)
                {
                    sprite.Play();
                }
            }
        }

        protected override void Update()
        {
            if (!Parent.Paused)
            {
                foreach (var sprite in currentSpriteGroup)
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
            foreach (var sprite in currentSpriteGroup)
            {
                RenderSprite(sprite, e);
            }
        }

        private void RenderSprite(Sprite currentSprite, GameRenderEventArgs e)
        {
            if (evenframe)
            {
                foreach (var meter in HealthMeter.AllMeters)
                {
                    var bounds = new RectangleF(currentSprite.BoundBox.X, currentSprite.BoundBox.Y,
                        currentSprite.BoundBox.Width, currentSprite.BoundBox.Height);

                    bounds.Offset(-currentSprite.HotSpot.X, -currentSprite.HotSpot.Y);
                    bounds.Offset(PositionSrc.Position);
                    if (meter.Bounds.IntersectsWith(bounds))
                    {
                        Draw(currentSprite, e.RenderContext, 5);
                        return;
                    }
                }
            }
            Draw(currentSprite, e.RenderContext, currentSprite.Layer + 1);
        }

        private void Draw(Sprite sprite, IRenderingContext context, int layer)
        {
            if (PositionSrc == null) throw new InvalidOperationException("SpriteComponent has not been initialized with a position source.");
            float off_x = Parent.Screen.OffsetX;
            float off_y = Parent.Screen.OffsetY;
            if (sprite != null && Visible)
            {
                sprite.VerticalFlip = Parent.IsGravitySensitive ? Parent.Container.IsGravityFlipped : verticalFlip;
                sprite.Draw(context, layer, PositionSrc.Position.X - off_x, PositionSrc.Position.Y - off_y);
            }
        }

        private class SpriteGroup : IEnumerable<Sprite>
        {
            private SpriteComponent _parent;

            private string _defaultKey = Guid.NewGuid().ToString();
            private Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();

            public Dictionary<string, Sprite> NamedSprites
            {
                get
                {
                    return _sprites;
                }
            }

            public SpriteGroup(SpriteComponent parent)
            {
                _parent = parent;
            }

            public void Add(Sprite sprite, string partName = null)
            {
                if (partName == null)
                {
                    partName = _defaultKey;
                }

                if (_sprites.ContainsKey(partName))
                {
                    throw new GameRunException(String.Format("A sprite with the same part name already exists in the {0} entity.", _parent.Parent.Name));
                }

                _sprites.Add(partName, sprite);
            }

            public void ChangePalette(int index)
            {
                var paletteName = _sprites.Values.First().PaletteName;
                var palette = PaletteSystem.Get(paletteName);
                if (palette != null)
                {
                    palette.CurrentIndex = index;
                }
            }

            public IEnumerator<Sprite> GetEnumerator()
            {
                return _sprites.Values.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _sprites.Values.GetEnumerator();
            }
        }
    }
}
