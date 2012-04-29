using System;
using System.Collections.Generic;
using System.Linq;
using MegaMan.Common;
using System.IO;
using System.Xml.Linq;
using System.Windows.Forms;

namespace MegaMan.LevelEditor
{
    public class ProjectEditor
    {
        public Project Project { get; private set; }

        private bool dirty;
        private bool Dirty
        {
            get { return dirty; }
            set
            {
                dirty = value;
            }
        }

        #region Game XML File Stuff

        private readonly Dictionary<string, Entity> entities = new Dictionary<string,Entity>();

        public IEnumerable<Entity> Entities
        {
            get { return entities.Values; }
        }

        private string BaseDir
        {
            get { return Project.BaseDir; }
        }

        public string Name
        {
            get { return Project.Name; }
            set
            {
                if (Project.Name == value) return;
                Project.Name = value;
                Dirty = true;
            }
        }

        public string Author
        {
            get { return Project.Author; }
            set
            {
                if (Project.Author == value) return;
                Project.Author = value;
                Dirty = true;
            }
        }

        public int ScreenWidth
        {
            get { return Project.ScreenWidth; }
            set
            {
                if (Project.ScreenWidth == value) return;
                Project.ScreenWidth = value;
                Dirty = true;
            }
        }
        public int ScreenHeight
        {
            get { return Project.ScreenHeight; }
            set
            {
                if (Project.ScreenHeight == value) return;
                Project.ScreenHeight = value;
                Dirty = true;
            }
        }

        #endregion

        #region GUI Editor Stuff

        private readonly Dictionary<string, StageDocument> openStages = new Dictionary<string,StageDocument>();

        public IEnumerable<string> StageNames
        {
            get
            {
                return Project.Stages.Select(info => info.Name);
            }
        }

        #endregion

        public event Action<StageDocument> StageAdded;

        public static ProjectEditor CreateNew()
        {
            var p = new ProjectEditor();
            return p;
        }

        public static ProjectEditor FromFile(string path)
        {
            var p = new ProjectEditor();
            p.Project.Load(path);
            p.LoadIncludes();
            return p;
        }

        public StageDocument StageByName(string name)
        {
            if (openStages.ContainsKey(name)) return openStages[name];
            foreach (var info in Project.Stages)
            {
                if (info.Name == name)
                {
                    try
                    {
                        StageDocument stage = new StageDocument(this, BaseDir, info.StagePath.Absolute);
                        openStages.Add(name, stage);
                        return stage;
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show("A required file or directory for the stage was not found:\n\n" + ex.Message, "CME Level Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                }
            }
            return null;
        }

        public Entity EntityByName(string name)
        {
            return entities[name];
        }

        private ProjectEditor()
        {
            Project = new Project();
        }

        private void LoadIncludes()
        {
            foreach (string path in Project.Includes)
            {
                string fullpath = Path.Combine(BaseDir, path);
                XDocument document = XDocument.Load(fullpath, LoadOptions.SetLineInfo);
                foreach (XElement element in document.Elements())
                {
                    switch (element.Name.LocalName)
                    {
                        case "Entities":
                            LoadEntities(element);
                            break;
                    }
                }
            }
        }

        private void LoadEntities(XElement entitiesNode)
        {
            foreach (XElement entityNode in entitiesNode.Elements("Entity"))
            {
                var entity = new Entity(entityNode, BaseDir);
                entities.Add(entity.Name, entity);
            }
        }

        public StageDocument AddStage(string name, string tilesetPath)
        {
            string stageDir = Path.Combine(BaseDir, "stages");
            if (!Directory.Exists(stageDir))
            {
                Directory.CreateDirectory(stageDir);
            }
            string stagePath = Path.Combine(stageDir, name);
            if (!Directory.Exists(stagePath))
            {
                Directory.CreateDirectory(stagePath);
            }

            var stage = new StageDocument(this)
            {
                Path = FilePath.FromAbsolute(stagePath, this.BaseDir),
                Name = name
            };
            stage.ChangeTileset(tilesetPath);

            stage.Save();
            
            openStages.Add(name, stage);

            var info = new StageInfo {Name = name, StagePath = FilePath.FromAbsolute(stagePath, BaseDir)};
            Project.AddStage(info);

            Save(); // need to save the reference to the new stage

            if (StageAdded != null) StageAdded(stage);

            return stage;
        }

        public void Save()
        {
            Project.Save();
            Dirty = false;
        }

        public bool Close()
        {
            if (openStages.Values.Any(stage => !stage.Close()))
            {
                return false;
            }

            if (!ConfirmSave()) return false;

            return true;
        }

        private bool ConfirmSave()
        {
            if (Dirty)
            {
                DialogResult result = MessageBox.Show("Do you want to save changes to " + Name + " before closing?", "Save Changes", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes) Save();
                else if (result == DialogResult.Cancel) return false;
            }
            return true;
        }
    }
}
