using System.Collections.Generic;
using System.Linq;
using MegaMan.Common;

namespace MegaMan.LevelEditor
{
    public interface HistoryAction
    {
        void Run();
        HistoryAction Reverse();
    }

    public class TileChange
    {
        private readonly int tileX;
        private readonly int tileY;
        private readonly int oldTileId;
        private readonly int newTileId;

        public TileChange(int tx, int ty, int oldId, int newId)
        {
            tileX = tx;
            tileY = ty;
            oldTileId = oldId;
            newTileId = newId;
        }

        public TileChange Reverse()
        {
            return new TileChange(tileX, tileY, newTileId, oldTileId);
        }

        public void ApplyToSurface(IScreenSurface surface)
        {
            surface.Screen.ChangeTile(tileX, tileY, newTileId);
        }
    }

    public class DrawAction : HistoryAction
    {
        private readonly List<TileChange> changes;
        private readonly IScreenSurface surface;
        private readonly string name;

        public DrawAction(string name, IEnumerable<TileChange> changes, IScreenSurface surface)
        {
            this.name = name;
            this.changes = new List<TileChange>(changes);
            this.surface = surface;
        }

        public override string ToString()
        {
            return name;
        }

        public void Run()
        {
            foreach (TileChange change in changes)
            {
                change.ApplyToSurface(surface);
            }
        }

        public HistoryAction Reverse()
        {
            List<TileChange> ch = new List<TileChange>(changes.Count);
            ch.AddRange(changes.Select(change => change.Reverse()));
            return new DrawAction(name, ch, surface);
        }
    }

    public class AddEntityAction : HistoryAction
    {
        private readonly EntityPlacement entity;
        private readonly IScreenSurface surface;

        public AddEntityAction(EntityPlacement entity, IScreenSurface surface)
        {
            this.entity = entity;
            this.surface = surface;
        }

        public void Run()
        {
            surface.Screen.AddEntity(entity);
        }

        public HistoryAction Reverse()
        {
            return new RemoveEntityAction(entity, surface);
        }
    }

    public class RemoveEntityAction : HistoryAction
    {
        private readonly EntityPlacement entity;
        private readonly IScreenSurface surface;

        public RemoveEntityAction(EntityPlacement entity, IScreenSurface surface)
        {
            this.entity = entity;
            this.surface = surface;
        }

        public void Run()
        {
            surface.Screen.RemoveEntity(entity);
        }

        public HistoryAction Reverse()
        {
            return new AddEntityAction(entity, surface);
        }
    }
}
