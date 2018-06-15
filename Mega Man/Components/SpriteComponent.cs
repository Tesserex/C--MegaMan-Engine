﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Geometry;
using MegaMan.Common.Rendering;

namespace MegaMan.Engine
{
    public class SpriteComponent : Component
    {
        private readonly Dictionary<string, SpriteGroup> _sprites;
        private readonly Dictionary<Guid, SpriteAnimator> _animators;
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
                        var animator = _animators[sprite.Id];
                        if (playing) animator.Resume();
                        else animator.Pause();
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
            _animators = new Dictionary<Guid, SpriteAnimator>();

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
            foreach (var animator in _animators.Values)
            {
                animator.Play();
            }

            container.GameThink += Update;
            container.Draw += Instance_GameRender;
        }

        public override void Stop(IGameplayContainer container)
        {
            foreach (var animator in _animators.Values)
            {
                animator.Stop();
            }

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
            var animator = new SpriteAnimator(sprite);
            _animators.Add(sprite.Id, animator);

            if (group == currentSpriteGroup)
            {
                animator.Play();
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

        public void ChangeSprite(string name)
        {
            if (!_sprites.ContainsKey(name) || _sprites[name] == null)
            {
                throw new GameRunException(String.Format("A sprite with name {0} was not found in the entity {1}.", name, Parent.Name));
            }

            foreach (var sprite in currentSpriteGroup)
            {
                _animators[sprite.Id].Stop();
            }

            currentSpriteGroup = _sprites[name];
            
            if (Playing)
            {
                foreach (var sprite in currentSpriteGroup)
                {
                    _animators[sprite.Id].Play();
                }
            }
        }

        protected override void Update()
        {
            if (!Parent.Paused)
            {
                foreach (var sprite in currentSpriteGroup)
                {
                    _animators[sprite.Id].Update();
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
                sprite.Draw(context, layer, PositionSrc.Position.X - off_x, PositionSrc.Position.Y - off_y, _animators[sprite.Id].CurrentIndex);
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

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _sprites.Values.GetEnumerator();
            }
        }
    }
}
