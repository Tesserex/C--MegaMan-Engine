using System.Collections.Generic;
using System.IO;
using System.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Editor.Mediator;
using MegaMan.Editor.Services;

namespace MegaMan.Editor.Bll
{
    public class ProjectDocument
    {
        private readonly IDataAccessService _dataService;

        public IProjectFileStructure FileStructure { get; private set; }
        public Project Project { get; private set; }

        private bool _dirty;
        public bool Dirty
        {
            get
            {
                return _dirty || stageDocuments.Any(s => s.Value.Dirty);
            }
            set
            {
                _dirty = value;
            }
        }

        #region Game XML File Stuff

        private Dictionary<string, EntityInfo> entities;
        private List<EntityInfo> unloadedEntities = new List<EntityInfo>();

        public IEnumerable<EntityInfo> Entities
        {
            get { return Project.Entities; }
        }

        public IEnumerable<EntityInfo> UnloadedEntities
        {
            get { return unloadedEntities.AsReadOnly(); }
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
            get { return Project.MusicNsf != null ? Project.MusicNsf.Absolute : null; }
            set
            {
                if (Project.MusicNsf != null && Project.MusicNsf.Absolute == value)
                    return;

                Project.MusicNsf = FilePath.FromAbsolute(value, BaseDir);
                Dirty = true;
            }
        }

        public string EffectsNsf
        {
            get { return Project.EffectsNsf != null ? Project.EffectsNsf.Absolute : null; }
            set
            {
                if (Project.EffectsNsf != null && Project.EffectsNsf.Absolute == value)
                    return;

                Project.EffectsNsf = FilePath.FromAbsolute(value, BaseDir);
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
                    Project.StartHandler = new HandlerTransfer { Type = StartHandlerType };
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
                    Project.StartHandler = new HandlerTransfer { Type = StartHandlerType };
                }

                Project.StartHandler.Name = value;
            }
        }

        #endregion

        #region GUI Editor Stuff

        private readonly Dictionary<string, StageDocument> stageDocuments = new Dictionary<string, StageDocument>();

        public IEnumerable<string> StageNames => Project.Stages.Select(info => info.Name);

        public IEnumerable<string> SceneNames => Project.Scenes.Select(info => info.Name);

        public IEnumerable<string> MenuNames => Project.Menus.Select(info => info.Name);

        public IEnumerable<StageDocument> Stages => stageDocuments.Values;

        #endregion

        public ProjectDocument(IProjectFileStructure fileStructure, Project project, IDataAccessService dataService)
        {
            Project = project;
            FileStructure = fileStructure;
            _dataService = dataService;

            foreach (var info in Project.Stages)
            {
                var stage = _dataService.LoadStage(this, info);
                stageDocuments.Add(info.Name, stage);
            }
        }

        public StageDocument StageByName(string name)
        {
            if (stageDocuments.ContainsKey(name)) return stageDocuments[name];
            foreach (var info in Project.Stages)
            {
                if (info.Name == name)
                {
                    StageDocument stage = _dataService.LoadStage(this, info);
                    stageDocuments.Add(name, stage);
                    return stage;
                }
            }
            return null;
        }

        public EntityInfo EntityByName(string name)
        {
            if (entities == null || entities.Count != Project.Entities.Count())
                entities = Project.Entities.ToDictionary(e => e.Name, e => e);

            if (entities.ContainsKey(name))
                return entities[name];
            return null;
        }

        public StageDocument AddStage(string name)
        {
            var stagePath = FileStructure.CreateStagePath(name);

            var stage = new StageDocument(this) {
                Path = stagePath,
                Name = name
            };

            var info = new StageLinkInfo { Name = stage.Name, StagePath = stage.Path };

            AddStageToProject(stage, info);

            return stage;
        }

        public void LinkStage(string fileName)
        {
            var linkName = Path.GetFileNameWithoutExtension(fileName);
            var info = new StageLinkInfo { Name = linkName, StagePath = FilePath.FromAbsolute(fileName, BaseDir) };
            StageDocument stage = _dataService.LoadStage(this, info);

            var copyPath = FileStructure.CreateStagePath(stage.Name);
            stage.Path = copyPath;

            AddStageToProject(stage, info);
        }

        private void AddStageToProject(StageDocument stage, StageLinkInfo linkInfo)
        {
            stageDocuments.Add(stage.Name, stage);
            Project.AddStage(linkInfo);
            ViewModelMediator.Current.GetEvent<StageAddedEventArgs>().Raise(this, new StageAddedEventArgs { Stage = linkInfo });
            CheckStartHandler();
            Dirty = true;
        }

        public void AddEntity(EntityInfo entity)
        {
            Project.AddEntity(entity);
            ViewModelMediator.Current.GetEvent<EntityAddedEventArgs>().Raise(this, new EntityAddedEventArgs { Entity = entity });
            Dirty = true;
        }

        public void RemoveEntity(EntityInfo entity)
        {
            Project.RemoveEntity(entity);
            Dirty = true;
        }

        public void UnloadEntity(EntityInfo entity)
        {
            Project.RemoveEntity(entity);
            unloadedEntities.Add(entity);
            Dirty = true;
        }

        public void RenameEntityPlacements(string oldName, string currentName)
        {
            var placements = Stages
                .SelectMany(s => s.Screens)
                .SelectMany(s => s.Entities)
                .Where(p => p.Entity == oldName);

            foreach (var placement in placements)
            {
                placement.Entity = currentName;
            }

            Dirty = true;
        }

        private void CheckStartHandler()
        {
            if (Project.StartHandler == null)
            {
                if (Project.Stages.Any())
                {
                    Project.StartHandler = new HandlerTransfer { Type = HandlerType.Stage, Name = Project.Stages.First().Name };
                }
                else if (Project.Menus.Any())
                {
                    Project.StartHandler = new HandlerTransfer { Type = HandlerType.Menu, Name = Project.Menus.First().Name };
                }
                else if (Project.Scenes.Any())
                {
                    Project.StartHandler = new HandlerTransfer { Type = HandlerType.Scene, Name = Project.Scenes.First().Name };
                }
            }
        }
    }
}
