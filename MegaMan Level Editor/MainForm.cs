using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using MegaMan.Common;
using WeifenLuo.WinFormsUI.Docking;
using System.Diagnostics;

namespace MegaMan.LevelEditor
{
    public partial class MainForm : Form
    {
        #region Private Members

        private readonly TilesetStrip tilestrip;
        private ProjectEditor activeProject;
        private StageDocument activeStage;

        private BrushForm brushForm;
        public ProjectForm projectForm;
        public HistoryForm historyForm;
        private EntityForm entityForm;

        private bool drawGrid;
        private bool drawTiles;
        private bool drawBlock;
        private bool drawJoins;
        private bool drawEntities;

        private readonly string recentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mega Man", "Editor", "recent.ini");
        private readonly List<string> recentFiles = new List<string>(10);

        private readonly string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");

        private readonly string engineInfoPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mega Man", "Editor", "enginepath.ini");
        private string enginePath = null;

        public ITileBrush CurrentBrush { get; private set; }
        private ToolType currentToolType;
        private Entity currentEntity;

        #endregion Private Members

        #region Properties

        private ProjectEditor ActiveProject
        {
            get { return activeProject; }
            set
            {
                activeProject = value;
                saveToolStripMenuItem.Enabled =
                    closeToolStripMenuItem.Enabled = propertiesToolStripMenuItem.Enabled =
                    newScreenMenuItem.Enabled =
                    mergeScreenToolStripMenuItem.Enabled =
                    splitScreenToolStripMenuItem.Enabled = (value != null);
            }
        }

        private StageDocument ActiveStage
        {
            get { return activeStage; }
            set
            {
                activeStage = value;
            }
        }

        public TilesetStrip TileStrip { get { return tilestrip; } }

        public bool DrawGrid
        {
            get { return drawGrid; }
            private set
            {
                drawGrid = value;
                showGridToolStripMenuItem.Checked = value;
                if (DrawOptionToggled != null) DrawOptionToggled();
            }
        }

        public bool DrawTiles
        {
            get { return drawTiles; }
            private set
            {
                drawTiles = value;
                showBackgroundsToolStripMenuItem.Checked = value;
                if (DrawOptionToggled != null) DrawOptionToggled();
            }
        }

        public bool DrawBlock
        {
            get { return drawBlock; }
            private set
            {
                drawBlock = value;
                showBlockingToolStripMenuItem.Checked = value;
                if (DrawOptionToggled != null) DrawOptionToggled();
            }
        }

        public bool DrawJoins
        {
            get { return drawJoins; }
            private set
            {
                drawJoins = value;
                joinsToolStripMenuItem.Checked = value;
                if (DrawOptionToggled != null) DrawOptionToggled();
            }
        }

        public bool DrawEntities
        {
            get { return drawEntities; }
            private set
            {
                drawEntities = value;
                showEnemiesToolStripMenuItem.Checked = value;
                if (DrawOptionToggled != null) DrawOptionToggled();
            }
        }

        public ITool CurrentTool { get; private set; }
        #endregion

        public event Action DrawOptionToggled;

        public static MainForm Instance { get; private set; }

        public MainForm()
        {
            InitializeComponent();
            
            tilestrip = new TilesetStrip();
            Controls.Add(tilestrip);
            tilestrip.BringToFront();
            dockPanel1.BringToFront();
            tilestrip.TileChanged += TileChanged;

            Instance = this;

            CreateBrushForm();
            CreateProjectForm();
            CreateHistoryForm();
            CreateEntityForm();

            if (File.Exists(configFile))
            {
                dockPanel1.LoadFromXml(configFile, GetContentFromPersistString);
            }

            if (!projectForm.Visible) projectForm.Show(dockPanel1, DockState.DockRight);
            if (!historyForm.Visible) historyForm.Show(dockPanel1, DockState.DockRight);
            if (!brushForm.Visible) brushForm.Show(dockPanel1, DockState.DockLeft);
            if (!entityForm.Visible) entityForm.Show(dockPanel1, DockState.DockLeft);

            DrawGrid = false;
            DrawTiles = true;
            DrawBlock = false;
            DrawEntities = true;

            LoadRecentFiles();

            ActiveStage = null;

            currentToolType = ToolType.Cursor;
            AssembleTool();
        }

        void CreateBrushForm()
        {
            brushForm = new BrushForm();
            brushForm.BrushChanged += brushForm_BrushChanged;
            brushForm.Shown += (s, e) => brushesToolStripMenuItem.Checked = true;
            brushForm.FormClosing += (s, e) =>
            {
                e.Cancel = true;
                brushForm.Hide();
                brushesToolStripMenuItem.Checked = false;
            };
        }

        void CreateProjectForm()
        {
            projectForm = new ProjectForm();
            projectForm.Shown += (s, e) => projectToolStripMenuItem.Checked = true;
            projectForm.FormClosing += (s, e) =>
            {
                e.Cancel = true;
                projectForm.Hide();
                projectToolStripMenuItem.Checked = false;
            };   
        }

        void CreateHistoryForm()
        {
            historyForm = new HistoryForm();
            historyForm.Shown += (s, e) => historyToolStripMenuItem.Checked = true;
            historyForm.FormClosing += (s, e) =>
            {
                e.Cancel = true;
                historyForm.Hide();
                historyToolStripMenuItem.Checked = false;
            };
        }

        private void CreateEntityForm()
        {
            entityForm = new EntityForm();
            entityForm.Shown += (s, e) => entitiesToolStripMenuItem.Checked = true;
            entityForm.FormClosing += (s, e) =>
            {
                e.Cancel = true;
                entityForm.Hide();
                entitiesToolStripMenuItem.Checked = false;
            };
            entityForm.EntityChanged += entityForm_EntityChanged;
        }

        private void entityForm_EntityChanged(Entity entity)
        {
            currentEntity = entity;
            currentToolType = ToolType.Entity;
            AssembleTool();
            foreach (ToolStripButton item in toolBar.Items) { item.Checked = false; }
            DrawJoins = false;
        }

        public void ShowStageForm(StageForm form)
        {
            form.DockAreas = DockAreas.Document;
            form.Show(dockPanel1);
        }

        public void FocusScreen(StageDocument stage)
        {
            if (stage != activeStage)
            {
                ActiveStage = stage;
                ChangeTileset(stage.Tileset);
                brushForm.ChangeTileset(stage.Tileset);

                if (stage.Project != activeProject)
                {
                    activeProject = stage.Project;
                    entityForm.LoadEntities(stage.Project);
                }
            }
        }

        private void ChangeTileset(Tileset tileset)
        {
            if (brushForm != null) brushForm.ChangeTileset(activeStage.Tileset);

            tilestrip.ChangeTileset(tileset);
        }

        private void brushForm_BrushChanged(BrushChangedEventArgs e)
        {
            CurrentBrush = e.Brush;
            AssembleTool();
        }

        private void TileChanged(Tile tile)
        {
            this.CurrentBrush = tile == null ? null : new SingleTileBrush(tile);
            AssembleTool();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            File.WriteAllLines(recentPath, recentFiles.ToArray());

            dockPanel1.SaveAsXml(configFile);

            e.Cancel = false;
            base.OnClosing(e);
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(ProjectForm).ToString())
                return projectForm;
            else if (persistString == typeof(HistoryForm).ToString())
                return historyForm;
            else if (persistString == typeof(BrushForm).ToString())
                return brushForm;
            else if (persistString == typeof(EntityForm).ToString())
                return entityForm;
            else return null;
        }

        #region Private Methods
        private void AddRecentFile(string path)
        {
            if (recentFiles.Contains(path)) // move to top
            {
                recentFiles.Remove(path);
            }
            recentFiles.Add(path);
            if (recentFiles.Count > 9) recentFiles.RemoveRange(0, recentFiles.Count - 9);
        }

        private void LoadRecentFiles()
        {
            if (File.Exists(recentPath))
            {
                string[] recent = File.ReadAllLines(recentPath);
                recentFiles.Clear();
                recentFiles.AddRange(recent.Reverse().Take(9).ToList());

                int i = 1;
                foreach (string path in recentFiles)
                {
                    ToolStripMenuItem r = new ToolStripMenuItem(path);
                    r.Click += RecentMenu_Click;
                    Keys key = (Keys)Enum.Parse(typeof(Keys), ("D" + i));
                    r.ShortcutKeys = Keys.Control | key;
                    recentMenuItem.DropDownItems.Add(r);
                    i++;
                }
                if (recentFiles.Count > 0)
                {
                    string path = recentFiles[0].Remove(recentFiles[0].LastIndexOf('\\'));
                    folderDialog.SelectedPath = path;
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(recentPath));
                File.Create(recentPath);
            }
        }

        #endregion Private Methods

        private void OpenProject(string gamefile)
        {
            ProjectEditor project;
            try
            {
                project = ProjectEditor.FromFile(gamefile);
            }
            catch
            {
                MessageBox.Show("The selected file could not be loaded. Perhaps it was created with a different version of this editor.",
                    "CME Project Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            projectForm.AddProject(project);
            entityForm.LoadEntities(project);
            AddRecentFile(gamefile);
            ActiveProject = project;
        }

        private void AssembleTool()
        {
            if (CurrentTool != null && CurrentTool is IDisposable)
            {
                ((IDisposable)CurrentTool).Dispose();
            }

            switch (currentToolType)
            {
                case ToolType.Cursor:
                    CurrentTool = new CursorTool();
                    break;

                case ToolType.Brush:
                    CurrentTool = null;
                    if (CurrentBrush != null) CurrentTool = new BrushTool(CurrentBrush);
                    break;

                case ToolType.Bucket:
                    CurrentTool = null;
                    if (CurrentBrush != null) CurrentTool = new Bucket(CurrentBrush);
                    break;

                case ToolType.Join:
                    CurrentTool = new JoinTool();
                    break;

                case ToolType.Start:
                    CurrentTool = new StartPositionTool();
                    break;

                case ToolType.Entity:
                    CurrentTool = new EntityTool(currentEntity);
                    break;

                case ToolType.Zoom:
                    CurrentTool = new Zoom();
                    break;

                case ToolType.Rectangle:
                    CurrentTool = null;
                    if (CurrentBrush != null) CurrentTool = new RectangleTool(CurrentBrush);
                    break;

                case ToolType.Selection:
                    CurrentTool = new SelectionTool();
                    break;
            }

            if (currentToolType != ToolType.Entity) entityForm.Deselect();
        }

        //***********************
        // Click Event Handlers *
        //***********************

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select a CME Game Project File",
                Filter = "Game Project (XML)|*.xml"
            };
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string file = dialog.FileName;
                OpenProject(file);
            }
        }


        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawGrid = !DrawGrid;
        }

        private void RecentMenu_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = sender as ToolStripMenuItem;
            OpenProject(t.Text);
        }

        private void showBackgroundsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawTiles = !DrawTiles;
        }

        private void showBlockingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawBlock = !DrawBlock;
        }

        private void joinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawJoins = !DrawJoins;
        }

        private void showEnemiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawEntities = !DrawEntities;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveProject != null)
            {
                ActiveProject.Save();
                if (ActiveStage != null) ActiveStage.Save();
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveProject == null) return;

            if (ActiveProject.Close())
            {
                projectForm.CloseProject();
                tilestrip.ChangeTileset(null);
                entityForm.Unload();
                brushForm.Clear();
                ActiveProject = null;
                ActiveStage = null;
            }
        }

        //*****************
        // Undo/Redo Menu *
        //*****************

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activeStage.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activeStage.Redo();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var projectProperties = new ProjectProperties {Owner = this};
            projectProperties.Location = new Point((Width - projectProperties.Width) / 2, (Height - projectProperties.Height) / 2);
            projectProperties.Show();
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveStage == null) return;

            StageProp.EditStage(ActiveStage);
        }

        //***************
        // Tool Windows *
        //***************

        private void newScreenStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveStage != null) ScreenProp.CreateScreen(ActiveStage);
        }

        private void brushesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (brushForm.Visible)
                brushForm.Hide();
            else
                brushForm.Show();
            brushesToolStripMenuItem.Checked = brushForm.Visible;
        }

        private void animateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Animated = !Program.Animated;
            animateToolStripMenuItem.Checked = Program.Animated;
        }

        private void projectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projectForm.Visible)
                projectForm.Hide();
            else
                projectForm.Show();

            projectToolStripMenuItem.Checked = projectForm.Visible;
        }

        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (historyForm.Visible)
                historyForm.Hide();
            else
                historyForm.Show();

            historyToolStripMenuItem.Checked = historyForm.Visible;
        }

        private void entitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (entityForm.Visible)
                entityForm.Hide();
            else
                entityForm.Show();

            entitiesToolStripMenuItem.Checked = entityForm.Visible;
        }

        private void brushToolButton_Click(object sender, EventArgs e)
        {
            currentToolType = ToolType.Brush;
            AssembleTool();
            foreach (ToolStripButton item in toolBar.Items) { item.Checked = false; }
            brushToolButton.Checked = true;
            DrawJoins = false;
            DrawTiles = true;
        }

        private void bucketToolButton_Click(object sender, EventArgs e)
        {
            currentToolType = ToolType.Bucket;
            AssembleTool();
            foreach (ToolStripButton item in toolBar.Items) { item.Checked = false; }
            bucketToolButton.Checked = true;
            DrawJoins = false;
            DrawTiles = true;
        }

        private void joinToolButton_Click(object sender, EventArgs e)
        {
            currentToolType = ToolType.Join;
            AssembleTool();
            foreach (ToolStripButton item in toolBar.Items) { item.Checked = false; }
            joinToolButton.Checked = true;
            DrawJoins = true;
        }

        private void startPosToolButton_Click(object sender, EventArgs e)
        {
            currentToolType = ToolType.Start;
            AssembleTool();
            foreach (ToolStripButton item in toolBar.Items) { item.Checked = false; }
            startPosToolButton.Checked = true;
            DrawJoins = false;
        }

        private void zoomToolButton_Click(object sender, EventArgs e)
        {
            currentToolType = ToolType.Zoom;
            AssembleTool();
            foreach (ToolStripButton item in toolBar.Items) { item.Checked = false; }
            zoomToolButton.Checked = true;
            DrawJoins = false;
        }

        private void rectToolButton_Click(object sender, EventArgs e)
        {
            currentToolType = ToolType.Rectangle;
            AssembleTool();
            foreach (ToolStripButton item in toolBar.Items) { item.Checked = false; }
            rectToolButton.Checked = true;
            DrawJoins = false;
        }

        private void cursorToolButton_Click(object sender, EventArgs e)
        {
            currentToolType = ToolType.Cursor;
            AssembleTool();
            foreach (ToolStripButton item in toolBar.Items) { item.Checked = false; }
            cursorToolButton.Checked = true;
            DrawJoins = false;
        }

        private void selectToolButton_Click(object sender, EventArgs e)
        {
            currentToolType = ToolType.Selection;
            AssembleTool();
            foreach (ToolStripButton item in toolBar.Items) { item.Checked = false; }
            selectToolButton.Checked = true;
            DrawJoins = false;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveStage != null)
            {
                ActiveStage.Copy();
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveStage != null)
            {
                var brush = ActiveStage.Paste();
                if (brush != null)
                {
                    CurrentBrush = brush;
                    currentToolType = ToolType.Brush;
                    AssembleTool();
                    foreach (ToolStripButton item in toolBar.Items) { item.Checked = false; }
                    brushToolButton.Checked = true;
                    DrawJoins = false;
                    DrawTiles = true;

                    // this is a bad hack to get it to draw immediately
                    Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y + 1);
                    Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y - 1);
                }
            }
        }

        private void pasteBrushToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveStage != null)
            {
                var brush = ActiveStage.Paste();
                if (brushForm != null && brush != null)
                {
                    brushForm.AddBrush(brush);
                }
            }
        }

        private void setEnginePathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeEnginePath();
        }

        private void loadEnginePath()
        {
            if (!File.Exists(engineInfoPath))
            {
                changeEnginePath();
            }
            else
            {
                enginePath = File.ReadLines(engineInfoPath).First();
            }
        }

        private void changeEnginePath()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "executable files (*.exe)|*.exe";
            dialog.Title = "Find Mega Man Engine";
            
            var result = dialog.ShowDialog(this);

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                enginePath = dialog.FileName;
                File.WriteAllLines(engineInfoPath, new[] { enginePath });
            }
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activeProject == null) return;

            if (enginePath == null || !File.Exists(enginePath))
            {
                loadEnginePath();
            }

            string args = String.Format("\"{0}\"", activeProject.Project.GameFile);

            Process.Start(enginePath, args);
        }

        private void runStageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activeProject == null || activeStage == null) return;

            if (enginePath == null || !File.Exists(enginePath))
            {
                loadEnginePath();
            }

            string args = String.Format("\"{0}\" \"Stage\\{1}\"",
                activeProject.Project.GameFile,
                activeStage.Name);

            Process.Start(enginePath, args);
        }

        public void RunTestFromStage(ScreenDocument screen, Point location)
        {
            if (enginePath == null || !File.Exists(enginePath))
            {
                loadEnginePath();
            }

            string args = String.Format("\"{0}\" \"Stage\\{1}\" \"{2}\" \"{3},{4}\"",
                activeProject.Project.GameFile,
                screen.Stage.Name,
                screen.Name,
                location.X.ToString(),
                location.Y.ToString());

            Process.Start(enginePath, args);
        }
    }
}

