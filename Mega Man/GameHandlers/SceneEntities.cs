using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common;

namespace MegaMan.Engine
{
    /// <summary>
    /// This is used for scenes and menus that don't get their entity container from gameplay
    /// </summary>
    public class SceneEntities : IEntityContainer
    {
        private List<GameEntity> entities;

        public SceneEntities()
        {
            entities = new List<GameEntity>();
        }

        public int TileSize
        {
            get
            {
                return 32;
            }
        }

        public float OffsetX
        {
            get { return 0; }
        }

        public float OffsetY
        {
            get { return 0; }
        }

        public MapSquare SquareAt(int x, int y)
        {
            return null;
        }

        public Tile TileAt(int tx, int ty)
        {
            return null;
        }

        public IEnumerable<MapSquare> Tiles
        {
            get { return null; }
        }

        public void AddEntity(GameEntity entity)
        {
            entities.Add(entity);
        }

        public IEnumerable<GameEntity> GetEntities(string name)
        {
            return entities.Where(e => e.Name == name);
        }

        public void ClearEntities()
        {
            foreach (var entity in entities)
            {
                entity.Stop();
            }
            entities.Clear();
        }

        public bool IsOnScreen(float x, float y)
        {
            return true;
        }
    }
}
