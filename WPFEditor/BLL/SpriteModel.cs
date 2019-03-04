using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Entities.Effects;
using MegaMan.Editor.Services;
using Point = MegaMan.Common.Geometry.Point;

namespace MegaMan.Editor.Bll
{
    public class SpriteModel : IEntityImage
    {
        private Sprite _sprite;
        private SpriteAnimator _animator;
        
        public Sprite Sprite => _sprite;

        public SpriteModel(Sprite sprite)
        {
            if (sprite == null)
                throw new ArgumentNullException("sprite");

            _sprite = sprite;
            _animator = new SpriteAnimator(sprite);
            TickWeakEventManager.AddHandler(Tick);
        }

        private void Tick(object sender, EventArgs e)
        {
            _animator.Update();
        }

        public string Name { get { return _sprite.Name; } }
        public int Width { get { return _sprite.Width; } }
        public int Height { get { return _sprite.Height; } }
        public bool Reversed { get { return _sprite.Reversed; } }
        public Point HotSpot { get { return _sprite.HotSpot; } }

        public void Play()
        {
            _animator.Play();
        }

        public void Pause()
        {
            _animator.Pause();
        }

        public int CurrentIndex
        {
            get
            {
                return _animator.CurrentIndex;
            }
            set
            {
                _animator.CurrentIndex = value;
            }
        }
        
        public SpriteFrame CurrentFrame { get { return _animator.CurrentFrame; } }

        public bool Playing { get { return _animator.Playing; } }

        public virtual WriteableBitmap GetImageSource(double zoom)
        {
            if (_sprite.SheetPath == null)
                return null;

            var location = _animator.CurrentFrame.SheetLocation;

            var image = SpriteBitmapCache.GetOrLoadFrame(_sprite.SheetPath.Absolute, location);
            if (zoom != 1)
                image = SpriteBitmapCache.Scale(image, zoom);

            return image;
        }

        public static IEntityImage ForEntity(EntityInfo entity, ProjectDocument project)
        {
            if (entity.DefaultSprite == null)
            {
                var allEffectParts = entity.StateComponent.States.SelectMany(s => s.Initializer.Parts.Concat(s.Logic.Parts).Concat(s.Triggers.SelectMany(t => t.Effect.Parts)));
                var spawn = allEffectParts.OfType<SpawnEffectPartInfo>().Select(s => s.Name).FirstOrDefault();
                if (spawn != null)
                {
                    var spawnEntity = project.EntityByName(spawn);
                    var model = new OverlaySpriteModel(spawnEntity.DefaultSprite, "spawn.png");
                    model.Play();
                    return model;
                }

                return new EmptySpriteModel("nosprite.png");
            }

            {
                var model = new SpriteModel(entity.DefaultSprite);
                model.Play();
                return model;
            }
        }
    }

    public class OverlaySpriteModel : SpriteModel
    {
        private WriteableBitmap _overlay;

        public OverlaySpriteModel(Sprite sprite, string overlayPath) : base(sprite)
        {
            _overlay = BitmapFactory.ConvertToPbgra32Format(SpriteBitmapCache.GetResource(overlayPath));
        }

        public override WriteableBitmap GetImageSource(double zoom)
        {
            var scaledOverlay = _overlay.Resize((int)(_overlay.PixelWidth * zoom), (int)(_overlay.PixelHeight * zoom), WriteableBitmapExtensions.Interpolation.NearestNeighbor);
            var spriteImg = BitmapFactory.ConvertToPbgra32Format(base.GetImageSource(zoom));
            var centerX = (spriteImg.Width - scaledOverlay.Width) / 2;
            var centerY = (spriteImg.Height - scaledOverlay.Height) / 2;
            var destRect = new Rect(centerX, centerY, scaledOverlay.Width, scaledOverlay.Height);
            spriteImg.Blit(destRect, scaledOverlay, new Rect(0, 0, scaledOverlay.Width, scaledOverlay.Height));
            
            return spriteImg;
        }
    }
}
