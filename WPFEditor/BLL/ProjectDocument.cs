using System.Collections.Generic;
using System.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Editor.Mediator;
using MegaMan.IO.Xml;

namespace MegaMan.Editor.Bll
{
    public class ProjectDocument
    {
        public IProjectFileStructure FileStructure { get; private set; }
        public Project Project { get; private set; }

        private bool _dirty;
        public bool Dirty
        {
            get
            {
                return _dirty || openStages.Any(s => s.Value.Dirty);
            }
            private set
            {
                _dirty = value;
            }
        }

        #region Game XML File Stuff

        private readonly Dictionary<string, EntityInfo> entities = new Dictionary<string, EntityInfo>();

        public IEnumerable<EntityInfo> Entities
        {
            get { return entities.Values; }
        }

        private string BaseDir
        {
            get
            {
                return Project.BaseDir;
            }
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

        public string MusicNsf
        {
            get { return Project.MusicNSF.Absolute; }
            set
            {
                if (Project.MusicNSF.Absolute == value) return;
                Project.MusicNSF = FilePath.FromAbsolute(value, BaseDir);
                Dirty = true;
            }
        }

        public string EffectsNsf
        {
            get { return Project.EffectsNSF.Absolute; }
            set
            {
                if (Project.EffectsNSF.Absolute == value) return;
                Project.EffectsNSF = FilePath.FromAbsolute(value, BaseDir);
                Dirty = true;
            }
        }

        public HandlerType StartHandlerType
        {
            get
            {
                if (Project.StartHandler == null)
                    return HandlerType.Scene;

                return Project.StartHandler.Type;
            }
            set
            {
                if (Project.StartHandler == null)
                {
                    Project.StartHandler = new HandlerTransfer() { Type = StartHandlerType };
                }

                Project.StartHandler.Type = value;
            }
        }

        public string StartHandlerName
        {
            get
            {
                if (Project.StartHandler == null)
                    return null;

                return Project.StartHandler.Name;
            }
            set
            {
                if (Project.StartHandler == null)
                {
                    Project.StartHandler = new HandlerTransfer() { Type = StartHandlerType };
                }

                Project.StartHandler.Name = value;
            }
        }

        #endregion

        #region GUI Editor Stuff

        private readonly Dictionary<string, StageDocument> openStages = new Dictionary<string, StageDocument>();

        public IEnumerable<string> StageNames
        {
            get
            {
                return Project.Stages.Select(info => info.Name);
            }
        }

        public IEnumerable<string> SceneNames
        {
            get
            {
                return Project.Scenes.Select(info => info.Name);
            }
        }

        public IEnumerable<string> MenuNames
        {
            get
            {
                return Project.Menus.Select(info => info.Name);
            }
        }

        #endregion

        public StageDocument StageByName(string name)
        {
            if (openStages.ContainsKey(name)) return openStages[name];
            foreach (var info in Project.Stages)
            {
                if (info.Name == name)
                {
                    StageDocument stage = new StageDocument(this, info.StagePath);
                    openStages.Add(name, stage);
                    return stage;
                }
            }
            return null;
        }

        public EntityInfo EntityByName(string name)
        {
            if (entities.ContainsKey(name))
                return entities[name];
            else
                return null;
        }

        public ProjectDocument(IProjectFileStructure fileStructure, Project project)
        {
            Project = project;
            FileStructure = fileStructure;

            entities = project.Entities.ToDictionary(e => e.Name, e => e);
        }

        public StageDocument AddStage(string name)
        {
            var stagePath = FileStructure.CreateStagePath(name);

            var stage = new StageDocument(this) {
                Path = stagePath,
                Name = name
            };

            openStages.Add(name, stage);

            var info = new StageLinkInfo { Name = name, StagePath = stagePath };
            Project.AddStage(info);

            ViewModelMediator.Current.GetEvent<StageAddedEventArgs>().Raise(this, new StageAddedEventArgs() { Stage = info });

            Dirty = true;

            return stage;
        }

        public void Save()
        {
            var writer = new ProjectXmlWriter(Project);
            writer.Write();

            foreach (var stage in openStages.Values)
                stage.Save();

            Dirty = false;
        }
    }
}
