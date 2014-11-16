using System;
using System.Collections.Generic;
using System.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Geometry;

namespace MegaMan.Editor.Bll
{
    public class ScreenDocument
    {
        private readonly ScreenInfo screen;

        private int? selectedEntityIndex;

        public StageDocument Stage { get; private set; }

        public event Action<int, int> Resized;
        public event Action TileChanged;
        public event Action EntitiesChanged;

        public Rectangle? Selection { get; private set; }
        public event Action<Rectangle?> SelectionChanged;

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

        public IEnumerable<Join> Joins
        {
            get
            {
                return Stage.Joins.Where(j => j.screenOne == Name || j.screenTwo == Name);
            }
        }

        public IEnumerable<EntityPlacement> Entities
        {
            get
            {
                return this.screen.Layers[0].Entities.AsReadOnly();
            }
        }

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

        public void ResizeTopLeft(int width, int height)
        {
            screen.Layers[0].Tiles.ResizeTopLeft(width, height);
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

        public void CleaveVertically(int leftHalfTileWidth)
        {
            if (leftHalfTileWidth > 0 && leftHalfTileWidth < screen.Width)
            {
                var leftSide = screen.Layers[0].Tiles.GetTiles(Point.Empty, leftHalfTileWidth, Height);
                var rightSide = screen.Layers[0].Tiles.GetTiles(new Point(leftHalfTileWidth, 0), Width - leftHalfTileWidth, Height);

                var leftScreen = Stage.AddScreen(Stage.FindNextScreenId().ToString(), leftHalfTileWidth, Height);
                var rightScreen = Stage.AddScreen(Stage.FindNextScreenId().ToString(), Width - leftHalfTileWidth, Height);

                leftScreen.screen.Layers[0].Tiles.ChangeTiles(Point.Empty, leftSide);
                rightScreen.screen.Layers[0].Tiles.ChangeTiles(Point.Empty, rightSide);

                foreach (var join in this.Joins.ToArray())
                {
                    Stage.RemoveJoin(join);
                }

                Stage.RemoveScreen(this);

                Stage.AddJoin(new Join() {
                    direction = JoinDirection.Both,
                    type = JoinType.Vertical,
                    screenOne = leftScreen.Name,
                    screenTwo = rightScreen.Name,
                    offsetOne = 0,
                    offsetTwo = 0,
                    Size = Height
                });
            }
        }

        public void SeverAllJoins()
        {
            var myJoins = Joins.ToList();

            foreach (var join in myJoins)
            {
                Stage.RemoveJoin(join);
            }
        }

        public EntityPlacement AddEntity(EntityInfo entity, Point location)
        {
            var info = new EntityPlacement {
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
            if (EntitiesChanged != null) EntitiesChanged();
        }

        public void RemoveEntity(EntityInfo entity, Point location)
        {
            screen.Layers[0].Entities.RemoveAll(i =>
                i.entity == entity.Name && i.screenX == location.X && i.screenY == location.Y
            );
            Dirty = true;
            if (EntitiesChanged != null) EntitiesChanged();
        }

        public void RemoveEntity(EntityPlacement info)
        {
            screen.Layers[0].Entities.Remove(info);
            Dirty = true;
            if (EntitiesChanged != null) EntitiesChanged();
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
            var entity = Stage.Project.EntityByName(entityInfo.entity);
            RectangleF bounds;

            if (entity.DefaultSprite == null)
            {
                bounds = new RectangleF(-8, -8, 16, 16);
            }
            else
            {
                var sprite = entity.DefaultSprite;
                bounds = sprite.BoundBox;
                bounds.Offset(-sprite.HotSpot.X, -sprite.HotSpot.Y);
            }

            bounds.Offset(entityInfo.screenX, entityInfo.screenY);
            return bounds.Contains(location);
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

        public void SetSelection(int tx, int ty, int width, int height)
        {
            if (width != 0 && height != 0)
            {
                if (tx < 0)
                {
                    width += tx;
                    tx = 0;
                }
                if (ty < 0)
                {
                    height += ty;
                    ty = 0;
                }

                if (width + tx > Width) width = Width - tx;
                if (height + ty > Height) height = Height - ty;

                // all in tile sizes
                Selection = new Rectangle(tx, ty, width, height);
            }
            else
            {
                Selection = null;
            }

            if (SelectionChanged != null)
            {
                SelectionChanged(Selection);
            }
        }
    }
}
