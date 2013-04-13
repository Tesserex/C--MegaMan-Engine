using System;
using System.Collections.Generic;
using System.Linq;
using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.IO.Xml;

namespace MegaMan.Editor.Bll
{
    // ========= What this IS, and IS NOT ===============
    // This class controls a single stage. NOT the whole damn project!
    // There should be no way to touch the stage, except through
    // one of these objects! All form updates should be event
    // driven, coming from this class!

    public class StageDocument
    {
        private readonly StageInfo map;

        private History history;

        private readonly Dictionary<string, ScreenDocument> screens = new Dictionary<string,ScreenDocument>();

        public ProjectDocument Project { get; private set; }

        public Point StartPoint
        {
            get { return new Point(map.PlayerStartX, map.PlayerStartY); }
            set
            {
                map.PlayerStartX = value.X;
                map.PlayerStartY = value.Y;
            }
        }

        public event Action<ScreenDocument> ScreenAdded;
        public event Action<ScreenDocument, int, int> ScreenResized;
        public event Action<Join> JoinChanged;
        public event Action<bool> DirtyChanged;

        public StageDocument(ProjectDocument project)
        {
            Project = project;
            map = new StageInfo();
            history = new History();
        }

        public StageDocument(ProjectDocument project, string basepath, string filepath)
        {
            Project = project;
            var stageReader = new StageXmlReader();
            map = stageReader.LoadStageXml(FilePath.FromAbsolute(filepath, basepath));

            // wrap all map screens in screendocuments
            // this should be the only time MegaMan.Screen's are touched directly
            foreach (var pair in map.Screens)
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
            get { return map.Name; }
            set
            {
                map.Name = value;
                Dirty = true;
            }
        }

        public FilePath Path
        {
            get { return map.StagePath; }
            set { map.StagePath = value; Dirty = true; }
        }

        public Tileset Tileset
        {
            get { return map.Tileset; }
        }

        public void ChangeTileset(string path)
        {
            map.ChangeTileset(path);
            Dirty = true;
        }

        public FilePath MusicIntro
        {
            get { return map.MusicIntroPath; }
            set { map.MusicIntroPath = value; Dirty = true; }
        }

        public FilePath MusicLoop
        {
            get { return map.MusicLoopPath; }
            set { map.MusicLoopPath = value; Dirty = true; }
        }

        public string StartScreen
        {
            get { return map.StartScreen; }
            set { map.StartScreen = value; Dirty = true; }
        }

        public IEnumerable<ScreenDocument> Screens
        {
            get { return screens.Values; }
        }

        public IEnumerable<Join> Joins
        {
            get { return map.Joins; }
        }

        public void Save()
        {
            var stageWriter = new StageXmlWriter(map);
            stageWriter.Write();
            Dirty = false;
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
            var screen = new MegaMan.Common.ScreenInfo(name, Tileset);

            int[,] tiles = new int[tile_width, tile_height];

            screen.Layers.Add(new ScreenLayerInfo(name, new TileLayer(tiles, Tileset, 0, 0), false, new List<EntityPlacement>(), new List<ScreenLayerKeyframe>()));

            map.Screens.Add(name, screen);

            if (StartScreen == null) StartScreen = map.Screens.Keys.First();

            ScreenDocument doc = WrapScreen(screen);

            // now I can do things like fire an event... how useful!
            if (ScreenAdded != null) ScreenAdded(doc);

            return doc;
        }

        public void RemoveScreen(ScreenDocument screen)
        {
            screen.Renamed -= ScreenRenamed;
            screen.TileChanged -= () => Dirty = true;
            screen.Resized -= (w, h) => OnScreenResized(screen, w, h);

            screens.Remove(screen.Name);
        }

        public void AddJoin(Join join)
        {
            map.Joins.Add(join);
            Dirty = true;
            if (JoinChanged != null) JoinChanged(join);
        }

        public void RemoveJoin(Join join)
        {
            map.Joins.Remove(join);
            Dirty = true;
            if (JoinChanged != null) JoinChanged(join);
        }

        public void Undo()
        {
            history.Undo();
        }

        public void Redo()
        {
            history.Redo();
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
            if (map.StartScreen == oldName) map.StartScreen = newName;
            foreach (var join in Joins)
            {
                if (join.screenOne == oldName) join.screenOne = newName;
                if (join.screenTwo == oldName) join.screenTwo = newName;
            }
            Dirty = true;
        }
    }
}
