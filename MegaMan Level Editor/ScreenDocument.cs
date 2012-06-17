using System;
using System.Linq;
using MegaMan.Common;
using System.Drawing;

namespace MegaMan.LevelEditor
{
    public class ScreenDocument
    {
        private readonly ScreenInfo screen;

        private int? selectedEntityIndex;

        public StageDocument Stage { get; private set; }

        public event Action<int, int> Resized;
        public event Action TileChanged;

        private bool Dirty
        {
            set
            {
                Stage.Dirty = value;
            }
        }

        public string Name
        {
            get { return screen.Name; }
            set
            {
                if (value == screen.Name) return;

                string old = screen.Name;
                screen.Name = value;
                Dirty = true;
                if (Renamed != null) Renamed(old, value);
            }
        }

        public int Width
        {
            get { return screen.Width; }
        }

        public int Height
        {
            get { return screen.Height; }
        }

        public Tileset Tileset { get { return screen.Tileset; } }
        public int PixelWidth { get { return screen.PixelWidth; } }
        public int PixelHeight { get { return screen.PixelHeight; } }

        public event Action<string, string> Renamed;

        public ScreenDocument(ScreenInfo screen, StageDocument stage)
        {
            Stage = stage;
            this.screen = screen;
        }

        public void Resize(int width, int height)
        {
            screen.Layers[0].Tiles.Resize(width, height);
            Dirty = true;
            if (Resized != null) Resized(width, height);
        }

        public Tile TileAt(int x, int y)
        {
            return screen.Layers[0].Tiles.TileAt(x, y);
        }

        public void ChangeTile(int tile_x, int tile_y, int tile_id)
        {
            screen.Layers[0].Tiles.ChangeTile(tile_x, tile_y, tile_id);
            Dirty = true;
            if (TileChanged != null) TileChanged();
        }

        public EntityPlacement AddEntity(Entity entity, Point location)
        {
            var info = new EntityPlacement
                {
                    entity = entity.Name,
                    screenX = location.X,
                    screenY = location.Y,
                };

            screen.Layers[0].Entities.Add(info);

            Dirty = true;

            return info;
        }

        public void AddEntity(EntityPlacement info)
        {
            screen.Layers[0].Entities.Add(info);
            Dirty = true;
        }

        public void RemoveEntity(Entity entity, Point location)
        {
            screen.Layers[0].Entities.RemoveAll(i =>
                i.entity == entity.Name && i.screenX == location.X && i.screenY == location.Y
            );
            Dirty = true;
        }

        public void RemoveEntity(EntityPlacement info)
        {
            screen.Layers[0].Entities.Remove(info);
        }

        public int FindEntityAt(Point location)
        {
            return screen.Layers[0].Entities.FindIndex(e => EntityBounded(e, location));
        }

        public EntityPlacement GetEntity(int index)
        {
            if (index >= 0 && index < screen.Layers[0].Entities.Count)
            {
                return screen.Layers[0].Entities[index];
            }
            return null;
        }

        private bool EntityBounded(EntityPlacement entityInfo, Point location)
        {
            Entity entity = Stage.Project.EntityByName(entityInfo.entity);
            RectangleF bounds;

            if (entity.MainSprite == null)
            {
                bounds = new RectangleF(-8, -8, 16, 16);
            }
            else
            {
                bounds = entity.MainSprite.BoundBox;
                bounds.Offset(-entity.MainSprite.HotSpot.X, -entity.MainSprite.HotSpot.Y);
            }

            bounds.Offset(entityInfo.screenX, entityInfo.screenY);
            return bounds.Contains(location);
        }

        public void DrawOn(Graphics graphics)
        {
            screen.Draw(graphics, 0, 0, screen.PixelWidth, screen.PixelHeight);
        }

        public void DrawEntities(Graphics graphics)
        {
            for (var i = 0; i < screen.Layers[0].Entities.Count; i++)
            {
                var info = screen.Layers[0].Entities[i];

                var sprite = Stage.Project.EntityByName(info.entity).MainSprite;

                if (sprite != null)
                {
                    sprite.Draw(graphics, info.screenX, info.screenY);

                    if (selectedEntityIndex == i)
                    {
                        graphics.DrawRectangle(Pens.LimeGreen, info.screenX - sprite.HotSpot.X, info.screenY - sprite.HotSpot.Y, sprite.Width, sprite.Height);
                    }
                }
                else
                {
                    graphics.DrawImage(Properties.Resources.nosprite, info.screenX - 8, info.screenY - 8);

                    if (selectedEntityIndex == i)
                    {
                        graphics.DrawRectangle(Pens.LimeGreen, info.screenX - 8, info.screenY - 8, 16, 16);
                    }
                }
            }
        }

        public void SelectEntity(int index)
        {
            if (index >= 0 && index < screen.Layers[0].Entities.Count)
            {
                selectedEntityIndex = index;
            }
            else
            {
                selectedEntityIndex = null;
            }
        }
    }
}
