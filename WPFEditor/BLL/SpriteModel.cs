using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common.Geometry;

namespace MegaMan.Editor.Bll
{
    public class SpriteModel
    {
        private Sprite _sprite;

        public SpriteModel(Sprite sprite)
        {
            if (sprite == null)
                throw new ArgumentNullException("sprite");

            _sprite = sprite;
        }

        public int Width { get { return _sprite.Width; } }
        public int Height { get { return _sprite.Height; } }
        public bool Reversed { get { return _sprite.Reversed; } }
        public Point HotSpot { get { return _sprite.HotSpot; } }

        public virtual WriteableBitmap GetImageSource(double zoom, int frameIndex)
        {
            var location = _sprite[frameIndex].SheetLocation;

            var image = SpriteBitmapCache.GetOrLoadFrame(_sprite.SheetPath.Absolute, location);
            if (zoom != 1)
                image = SpriteBitmapCache.Scale(image, zoom);

            return image;
        }

        public static SpriteModel ForEntity(EntityInfo entity, ProjectDocument project)
        {
            var hasSprites = entity.SpriteComponent != null && entity.SpriteComponent.Sprites.Any();

            if (!hasSprites)
            {
                var allEffectParts = entity.StateComponent.States.SelectMany(s => s.Initializer.Parts.Concat(s.Logic.Parts).Concat(s.Triggers.SelectMany(t => t.Effect.Parts)));
                var spawn = allEffectParts.OfType<SpawnEffectPartInfo>().Select(s => s.Name).FirstOrDefault();
                if (spawn != null)
                {
                    var spawnEntity = project.EntityByName(spawn);
                    return new OverlaySpriteModel(spawnEntity.DefaultSprite, "spawn.png");
                }
            }

            return new SpriteModel(entity.DefaultSprite);
        }
    }

    public class OverlaySpriteModel : SpriteModel
    {
        private WriteableBitmap _overlay;

        public OverlaySpriteModel(Sprite sprite, string overlayPath) : base(sprite)
        {
            _overlay = BitmapFactory.ConvertToPbgra32Format(SpriteBitmapCache.GetResource(overlayPath));
        }

        public override WriteableBitmap GetImageSource(double zoom, int frameIndex)
        {
            var scaledOverlay = SpriteBitmapCache.Scale(_overlay, zoom);
            var spriteImg = BitmapFactory.ConvertToPbgra32Format(base.GetImageSource(zoom, frameIndex));
            var centerX = (spriteImg.Width - scaledOverlay.Width) / 2;
            var centerY = (spriteImg.Height - scaledOverlay.Height) / 2;
            var destRect = new System.Windows.Rect(centerX, centerY, scaledOverlay.Width, scaledOverlay.Height);
            spriteImg.Blit(destRect, scaledOverlay, new System.Windows.Rect(0, 0, scaledOverlay.Width, scaledOverlay.Height));
            
            return spriteImg;
        }
    }
}
