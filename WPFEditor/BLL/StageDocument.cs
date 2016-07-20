using System;
using System.Collections.Generic;
using System.Linq;
using MegaMan.Common;
using MegaMan.Common.Geometry;

namespace MegaMan.Editor.Bll
{
    public class StageDocument
    {
        private readonly StageInfo _map;
        public StageInfo Info { get { return _map; } }

        public History History { get; private set; }

        private readonly Dictionary<string, ScreenDocument> screens = new Dictionary<string, ScreenDocument>();

        public ProjectDocument Project { get; private set; }

        public Point StartPoint
        {
            get { return new Point(_map.PlayerStartX, _map.PlayerStartY); }
        }

        public event Action<ScreenDocument> ScreenAdded;
        public event Action<ScreenDocument> ScreenRemoved;
        public event Action<ScreenDocument, int, int> ScreenResized;
        public event Action<Join> JoinChanged;
        public event Action EntryPointsChanged;
        public event Action<bool> DirtyChanged;

        public StageDocument(ProjectDocument project)
        {
            Project = project;
            _map = new StageInfo();
            History = new History();
        }

        public StageDocument(ProjectDocument project, StageInfo info, StageLinkInfo linkInfo)
        {
            Project = project;
            History = new History();
            _map = info;
            Tileset = new TilesetDocument(_map.Tileset);
            LinkName = linkInfo.Name;

            // wrap all map screens in screendocuments
            // this should be the only time MegaMan.Screen's are touched directly
            foreach (var pair in _map.Screens)
            {
                WrapScreen(pair.Value);
            }
        }

        private bool dirty;
        public bool Dirty
        {
            get
            {
                return dirty;
            }
            set
            {
                if (dirty == value) return;
                dirty = value;
                if (DirtyChanged != null) DirtyChanged(dirty);
            }
        }

        #region Exposed Map Items

        public string Name
        {
            get { return _map.Name; }
            set
            {
                _map.Name = value;
                Dirty = true;
            }
        }

        public string LinkName { get; private set; }

        public FilePath Path
        {
            get { return _map.StagePath; }
            set { _map.StagePath = value; Dirty = true; }
        }

        public TilesetDocument Tileset { get; private set; }

        public void ChangeTileset(TilesetDocument tileset)
        {
            Tileset = tileset;
            _map.ChangeTileset(tileset.Tileset);
            Dirty = true;
        }

        public FilePath MusicIntro
        {
            get { return _map.MusicIntroPath; }
            set { _map.MusicIntroPath = value; Dirty = true; }
        }

        public FilePath MusicLoop
        {
            get { return _map.MusicLoopPath; }
            set { _map.MusicLoopPath = value; Dirty = true; }
        }

        public int MusicTrack
        {
            get { return _map.MusicNsfTrack; }
            set
            {
                _map.MusicNsfTrack = value;
                Dirty = true;
            }
        }

        public string StartScreen
        {
            get { return _map.StartScreen; }
        }

        public IDictionary<string, Point> ContinuePoints
        {
            get { return _map.ContinuePoints; }
        }

        public IEnumerable<ScreenDocument> Screens
        {
            get { return screens.Values; }
        }

        public IEnumerable<Join> Joins
        {
            get { return _map.Joins; }
        }

        #endregion

        public int FindNextScreenId()
        {
            int stageCount = Screens.Count();
            int nextScreenId = stageCount + 1;
            while (Screens.Any(s => s.Name == nextScreenId.ToString()))
            {
                nextScreenId++;
            }
            return nextScreenId;
        }

        public ScreenDocument AddScreen(string name, int tile_width, int tile_height)
        {
            var screen = new ScreenInfo(name, Tileset.Tileset);

            int[,] tiles = new int[tile_width, tile_height];

            screen.Layers.Add(new ScreenLayerInfo(name, new TileLayer(tiles, Tileset.Tileset, 0, 0), false, new List<ScreenLayerKeyframe>()));

            _map.Screens.Add(name, screen);

            if (StartScreen == null)
            {
                _map.StartScreen = _map.Screens.Keys.First();
                Dirty = true;
            }

            ScreenDocument doc = WrapScreen(screen);
            
            if (ScreenAdded != null) ScreenAdded(doc);

            return doc;
        }

        public void AddScreen(ScreenInfo screen)
        {
            var doc = WrapScreen(screen);
            _map.Screens.Add(screen.Name, screen);

            if (StartScreen == null)
            {
                _map.StartScreen = _map.Screens.Keys.First();
                Dirty = true;
            }

            if (ScreenAdded != null) ScreenAdded(doc);
        }

        public void RemoveScreen(ScreenDocument screen)
        {
            screen.Renamed -= ScreenRenamed;
            screen.TileChanged -= () => Dirty = true;
            screen.Resized -= (w, h) => OnScreenResized(screen, w, h);

            screens.Remove(screen.Name);

            if (ScreenRemoved != null) ScreenRemoved(screen);
        }

        public void AddJoin(Join join)
        {
            _map.Joins.Add(join);
            Dirty = true;
            if (JoinChanged != null) JoinChanged(join);
        }

        public void RemoveJoin(Join join)
        {
            _map.Joins.Remove(join);
            Dirty = true;
            if (JoinChanged != null) JoinChanged(join);
        }

        public void PushHistoryAction(IUndoableAction action)
        {
            History.Push(action);
        }

        public void Undo()
        {
            History.Undo();
        }

        public void Redo()
        {
            History.Redo();
        }

        private ScreenDocument WrapScreen(MegaMan.Common.ScreenInfo screen)
        {
            ScreenDocument doc = new ScreenDocument(screen, this);
            screens.Add(screen.Name, doc);
            doc.Renamed += ScreenRenamed;
            doc.TileChanged += () => Dirty = true;
            doc.Resized += (w, h) => OnScreenResized(doc, w, h);
            return doc;
        }

        void OnScreenResized(ScreenDocument screen, int width, int height)
        {
            if (ScreenResized != null)
            {
                ScreenResized(screen, width, height);
            }
        }

        private void ScreenRenamed(string oldName, string newName)
        {
            if (!screens.ContainsKey(oldName)) return;
            ScreenDocument doc = screens[oldName];
            screens.Remove(oldName);
            screens.Add(newName, doc);
            if (_map.StartScreen == oldName) _map.StartScreen = newName;
            foreach (var join in Joins)
            {
                if (join.screenOne == oldName) join.screenOne = newName;
                if (join.screenTwo == oldName) join.screenTwo = newName;
            }
            Dirty = true;
        }

        public void SetStartPoint(ScreenDocument screenDocument, Point location)
        {
            _map.StartScreen = screenDocument.Name;
            _map.PlayerStartX = location.X;
            _map.PlayerStartY = location.Y;
            Dirty = true;
            if (EntryPointsChanged != null)
                EntryPointsChanged();
        }

        public void AddContinuePoint(ScreenDocument screenDocument, Point location)
        {
            if (_map.ContinuePoints.ContainsKey(screenDocument.Name))
            {
                _map.ContinuePoints[screenDocument.Name] = location;
            }
            else
            {
                _map.AddContinuePoint(screenDocument.Name, location);
            }

            Dirty = true;
            if (EntryPointsChanged != null)
                EntryPointsChanged();
        }
    }
}
