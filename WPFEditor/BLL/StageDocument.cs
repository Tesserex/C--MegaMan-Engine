﻿using System;
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
                var doc = WrapScreen(pair.Value);
                screens.Add(doc.Name, doc);
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
            get { return _map.StoragePath; }
            set { _map.StoragePath = value; Dirty = true; }
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

        public ScreenDocument CreateScreen(int tileWidth, int tileHeight)
        {
            var name = FindNextScreenId().ToString();
            var screen = new ScreenInfo(name, Tileset.Tileset);

            int[,] tiles = new int[tileWidth, tileHeight];
            screen.Layers.Add(new ScreenLayerInfo(name, new TileLayer(tiles, Tileset.Tileset, 0, 0), false, new List<ScreenLayerKeyframe>()));

            return AddScreen(screen);
        }

        public ScreenDocument AddScreen(ScreenInfo screen)
        {
            var doc = WrapScreen(screen);
            AddScreen(doc);
            return doc;
        }

        public void AddScreen(ScreenDocument doc)
        {
            _map.Screens.Add(doc.Name, doc.Info);
            screens.Add(doc.Name, doc);

            if (StartScreen == null)
            {
                _map.StartScreen = _map.Screens.Keys.First();
                Dirty = true;
            }

            ScreenAdded?.Invoke(doc);
        }

        public void RemoveScreen(ScreenDocument screen)
        {
            foreach (var join in screen.Joins.ToList())
            {
                RemoveJoin(join);
            }

            screen.Renamed -= ScreenRenamed;
            screen.TileChanged -= () => Dirty = true;
            screen.Resized -= (w, h) => OnScreenResized(screen, w, h);

            screens.Remove(screen.Name);
            _map.Screens.Remove(screen.Name);

            if (StartScreen == screen.Name)
            {
                _map.StartScreen = _map.Screens.Keys.FirstOrDefault();
                Dirty = true;
            }

            ScreenRemoved?.Invoke(screen);
        }

        public void AddJoin(Join join)
        {
            _map.Joins.Add(join);
            Dirty = true;
            JoinChanged?.Invoke(join);
        }

        public void RemoveJoin(Join join)
        {
            _map.Joins.Remove(join);
            Dirty = true;
            JoinChanged?.Invoke(join);
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

        private ScreenDocument WrapScreen(ScreenInfo screen)
        {
            ScreenDocument doc = new ScreenDocument(screen, this);
            doc.Renamed += ScreenRenamed;
            doc.TileChanged += () => Dirty = true;
            doc.Resized += (w, h) => OnScreenResized(doc, w, h);
            return doc;
        }

        void OnScreenResized(ScreenDocument screen, int width, int height)
        {
            ScreenResized?.Invoke(screen, width, height);
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
                if (join.ScreenOne == oldName) join.ScreenOne = newName;
                if (join.ScreenTwo == oldName) join.ScreenTwo = newName;
            }
            Dirty = true;
        }

        public void SetStartPoint(ScreenDocument screenDocument, Point location)
        {
            _map.StartScreen = screenDocument.Name;
            _map.PlayerStartX = location.X;
            _map.PlayerStartY = location.Y;
            Dirty = true;
            EntryPointsChanged?.Invoke();
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
            EntryPointsChanged?.Invoke();
        }
    }
}
