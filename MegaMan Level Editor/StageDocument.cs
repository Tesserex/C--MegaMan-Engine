using System;
using System.Collections.Generic;
using System.Linq;
using MegaMan.Common;
using System.Windows.Forms;
using System.Drawing;

namespace MegaMan.LevelEditor
{
    // ========= What this IS, and IS NOT ===============
    // This class controls a single stage. NOT the whole damn project!
    // There should be no way to touch the stage, except through
    // one of these objects! All form updates should be event
    // driven, coming from this class!

    public class StageDocument
    {
        private readonly Map map;

        private StageForm stageForm;

        private readonly Dictionary<string, ScreenDocument> screens = new Dictionary<string,ScreenDocument>();

        public ProjectEditor Project { get; private set; }

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
        public event Action<Join> JoinChanged;
        public event Action<bool> DirtyChanged;

        public StageDocument(ProjectEditor project)
        {
            Project = project;
            map = new Map();
        }

        public StageDocument(ProjectEditor project, string basepath, string filepath)
        {
            Project = project;
            map = new Map(FilePath.FromAbsolute(filepath, basepath));

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
                RefreshInfo();
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
            map.Save();
            Dirty = false;
        }

        #endregion

        public void AddScreen(string name, int tile_width, int tile_height)
        {
            var screen = new MegaMan.Common.Screen(tile_width, tile_height, map) {Name = name};

            map.Screens.Add(name, screen);

            if (StartScreen == null) StartScreen = map.Screens.Keys.First();

            ScreenDocument doc = WrapScreen(screen);
            
            screen.Save(System.IO.Path.Combine(Path.Absolute, name + ".scn"));

            // now I can do things like fire an event... how useful!
            if (ScreenAdded != null) ScreenAdded(doc);

            Save();
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

        // this should probably be replaced by a join wrapper that has events
        public void RaiseJoinChange(Join join)
        {
            if (JoinChanged != null) JoinChanged(join);
        }

        private void RefreshInfo()
        {
            if (stageForm != null) stageForm.SetText();
        }

        public void ReFocus()
        {
            ShowStage();
        }

        private void ShowStage()
        {
            if (stageForm == null)
            {
                stageForm = new StageForm(this);
            }

            stageForm.GotFocus += StageForm_GotFocus;
            stageForm.FormClosing += StageForm_FormClosing;

            MainForm.Instance.ShowStageForm(stageForm);
            stageForm.Focus();
        }

        public bool Close()
        {
            if (!ConfirmSave()) return false;

            stageForm.GotFocus -= StageForm_GotFocus;
            stageForm.FormClosing -= StageForm_FormClosing;
            stageForm.Close();

            return true;
        }

        private bool ConfirmSave()
        {
            if (Dirty)
            {
                DialogResult result = MessageBox.Show("Do you want to save changes to " + map.Name + " before closing?", "Save Changes", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes) map.Save();
                else if (result == DialogResult.Cancel) return false;
            }
            return true;
        }

        public void Undo()
        {
            if (stageForm != null) stageForm.Undo();
        }

        public void Redo()
        {
            if (stageForm != null) stageForm.Redo();
        }

        public void Copy()
        {
            if (stageForm != null) stageForm.Copy();
        }

        public TileBrush Paste()
        {
            if (stageForm != null) return stageForm.Paste();
            return null;
        }

        void StageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (!ConfirmSave()) return;

            stageForm.Hide();
        }

        private void StageForm_GotFocus(object sender, EventArgs e)
        {
            MainForm.Instance.FocusScreen(this);
        }

        private ScreenDocument WrapScreen(MegaMan.Common.Screen screen)
        {
            ScreenDocument doc = new ScreenDocument(screen, this);
            screens.Add(screen.Name, doc);
            doc.Renamed += ScreenRenamed;
            doc.TileChanged += () => Dirty = true;
            return doc;
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
