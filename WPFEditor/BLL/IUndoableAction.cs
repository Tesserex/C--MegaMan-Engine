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
            tileX = tx;
            tileY = ty;
            oldTileId = oldId;
            newTileId = newId;
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
            Name = name;
            this.changes = new List<TileChange>(changes);
            screens = changes.Select(c => c.Screen).Distinct().ToList();
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
            screen.MoveEntity(entity, end);
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
            screen.Stage.AddScreen(screen);
        }

        public IUndoableAction Reverse()
        {
            return new RemoveScreenAction(screen);
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
            screen.Stage.RemoveScreen(screen);
        }

        public IUndoableAction Reverse()
        {
            return new AddScreenAction(screen);
        }
    }

    public class SplitScreenAction : IUndoableAction
    {
        private readonly ScreenDocument screen;
        private readonly List<Join> originalJoins;
        private readonly int left;
        private Tuple<ScreenDocument, ScreenDocument> split;

        public SplitScreenAction(ScreenDocument screen, int left)
        {
            this.screen = screen;
            this.originalJoins = screen.Joins.ToList();
            this.left = left;
        }

        public string Name => "Split Screen";

        public void Execute()
        {
            this.split = screen.CleaveVertically(left);
        }

        public IUndoableAction Reverse()
        {
            return new SplitScreenReverseAction(split, screen, originalJoins);
        }
    }

    public class SplitScreenReverseAction : IUndoableAction
    {
        private readonly Tuple<ScreenDocument, ScreenDocument> pair;
        private readonly ScreenDocument original;
        private readonly List<Join> joins;

        public string Name => "Undo Split Screen";

        public SplitScreenReverseAction(Tuple<ScreenDocument, ScreenDocument> pair, ScreenDocument original, List<Join> originalJoins)
        {
            this.pair = pair;
            this.original = original;
            this.joins = originalJoins;
        }

        public void Execute()
        {
            pair.Item1.Stage.RemoveScreen(pair.Item1);
            pair.Item2.Stage.RemoveScreen(pair.Item2);
            original.Stage.AddScreen(original);
            foreach (var join in joins)
            {
                original.Stage.AddJoin(join);
            }
        }

        public IUndoableAction Reverse()
        {
            // this action should only exist as an undo of split screen.
            // it should never be added to the stack alone, so it should never be reversed.
            throw new NotImplementedException();
        }
    }

    public class MergeScreensAction : IUndoableAction
    {
        private readonly Tuple<ScreenDocument, ScreenDocument> pair;
        private readonly int left;
        private ScreenDocument merged;

        public MergeScreensAction(Tuple<ScreenDocument, ScreenDocument> pair)
        {
            this.pair = pair;
            this.left = pair.Item1.Width;
        }

        public string Name => "Merge Screens";

        public void Execute()
        {
            merged = pair.Item1.MergeWith(pair.Item2);
        }

        public IUndoableAction Reverse()
        {
            return new SplitScreenAction(merged, left);
        }
    }
}
