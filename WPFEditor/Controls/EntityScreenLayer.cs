﻿using System.Windows;
using System.Windows.Media;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls
{
    public class EntityScreenLayer : ScreenLayer
    {
        protected override void UnbindScreen(ScreenDocument oldScreen)
        {
            oldScreen.EntitiesChanged -= Update;
        }

        protected override void BindScreen(ScreenDocument newScreen)
        {
            newScreen.EntitiesChanged += Update;
        }

        protected override void Update()
        {
            InvalidateVisual();
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);

            foreach (var placement in Screen.Entities)
            {
                var entity = Screen.Stage.Project.EntityByName(placement.entity);

                if (entity != null)
                {
                    var sprite = entity.DefaultSprite;
                    if (sprite != null)
                    {
                        var frame = SpriteBitmapCache.GetOrLoadFrame(sprite.SheetPath.Absolute, sprite.CurrentFrame.SheetLocation);

                        var flip = (placement.direction == Common.Direction.Left) ^ sprite.Reversed;
                        int hx = flip ? sprite.Width - sprite.HotSpot.X : sprite.HotSpot.X;

                        dc.DrawImage(frame, new Rect(placement.screenX - hx, placement.screenY - sprite.HotSpot.Y, frame.PixelWidth, frame.PixelHeight));

                        continue;
                    }
                }

                dc.DrawEllipse(Brushes.Orange, null, new System.Windows.Point(placement.screenX, placement.screenY), 10, 10);
            }
        }
    }
}