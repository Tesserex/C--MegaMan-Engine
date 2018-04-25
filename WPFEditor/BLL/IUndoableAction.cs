using System;
using System.Collections.Generic;
using System.Linq;
using MegaMan.Common;
using MegaMan.Common.Geometry;

namespace MegaMan.Editor.Bll
{
    public interface IUndoableAction
    {
        string Name { get; }

        void Execute();
        IUndoableAction Reverse();
    }

    public class TileChange
    {
        private ScreenDocument screen;
        private readonly int tileX;
        private readonly int tileY;
        private readonly int oldTileId;
        private readonly int newTileId;

        public ScreenDocument Screen { get { return screen; } }

        public TileChange(ScreenDocument screen, int tx, int ty, int oldId, int newId)
        {
            this.screen = screen;
            this.tileX = tx;
            this.tileY = ty;
            this.oldTileId = oldId;
            this.newTileId = newId;
        }

        public TileChange Reverse()
        {
            return new TileChange(screen, tileX, tileY, newTileId, oldTileId);
        }

        public void Apply()
        {
            screen.ChangeTile(tileX, tileY, newTileId);
        }
    }

    public class DrawAction : IUndoableAction
    {
        private readonly List<TileChange> changes;
        private readonly List<ScreenDocument> screens;

        public string Name
        {
            get;
            private set;
        }

        public DrawAction(string name, IEnumerable<TileChange> changes)
        {
            this.Name = name;
            this.changes = new List<TileChange>(changes);
            this.screens = changes.Select(c => c.Screen).Distinct().ToList();
        }

        public override string ToString()
        {
            return Name;
        }

        public void Execute()
        {
            foreach (var screen in screens)
                screen.BeginDrawBatch();

            foreach (TileChange change in changes)
                change.Apply();

            foreach (var screen in screens)
                screen.EndDrawBatch();
        }

        public IUndoableAction Reverse()
        {
            List<TileChange> ch = new List<TileChange>(changes.Count);
            ch.AddRange(changes.Select(change => change.Reverse()));
            return new DrawAction(Name, ch);
        }
    }

    public class AddEntityAction : IUndoableAction
    {
        private readonly EntityPlacement entity;
        private readonly ScreenDocument screen;

        public string Name { get { return "Add Entity"; } }

        public AddEntityAction(EntityPlacement entity, ScreenDocument screen)
        {
            this.entity = entity;
            this.screen = screen;
        }

        public void Execute()
        {
            screen.AddEntity(entity);
        }

        public IUndoableAction Reverse()
        {
            return new RemoveEntityAction(entity, screen);
        }
    }

    public class MoveEntityAction : IUndoableAction
    {
        private readonly EntityPlacement entity;
        private readonly ScreenDocument screen;
        private readonly Point start;
        private readonly Point end;

        public string Name { get { return "Move Entity"; } }

        public MoveEntityAction(EntityPlacement entity, ScreenDocument screen, Point start, Point end)
        {
            this.entity = entity;
            this.screen = screen;
            this.start = start;
            this.end = end;
        }

        public void Execute()
        {
            this.screen.MoveEntity(this.entity, end);
        }

        public IUndoableAction Reverse()
        {
            return new MoveEntityAction(entity, screen, end, start);
        }
    }

    public class RemoveEntityAction : IUndoableAction
    {
        private readonly EntityPlacement entity;
        private readonly ScreenDocument screen;

        public string Name { get { return "Remove Entity"; } }

        public RemoveEntityAction(EntityPlacement entity, ScreenDocument screen)
        {
            this.entity = entity;
            this.screen = screen;
        }

        public void Execute()
        {
            screen.RemoveEntity(entity);
        }

        public IUndoableAction Reverse()
        {
            return new AddEntityAction(entity, screen);
        }
    }

    public class AddScreenAction : IUndoableAction
    {
        private readonly ScreenDocument screen;

        public AddScreenAction(ScreenDocument screen)
        {
            this.screen = screen;
        }

        public string Name { get { return "Add Screen"; } }

        public void Execute()
        {
            this.screen.Stage.AddScreenDocumentWithoutHistory(this.screen);
        }

        public IUndoableAction Reverse()
        {
            return new RemoveScreenAction(this.screen);
        }
    }

    public class RemoveScreenAction : IUndoableAction
    {
        private readonly ScreenDocument screen;

        public RemoveScreenAction(ScreenDocument screen)
        {
            this.screen = screen;
        }

        public string Name { get { return "Remove Screen"; } }

        public void Execute()
        {
            this.screen.Stage.RemoveScreenWithoutHistory(this.screen);
        }

        public IUndoableAction Reverse()
        {
            return new AddScreenAction(this.screen);
        }
    }
}
