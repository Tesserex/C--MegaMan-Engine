using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;
using MegaMan.IO.Xml;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class ProjectViewModel : TreeViewItemViewModel
    {
        private ProjectDocument _project;

        public ProjectViewModel()
            : base(null)
        {
            ViewModelMediator.Current.GetEvent<ProjectChangedEventArgs>().Subscribe(ProjectChanged);
        }

        private void ProjectChanged(object sender, ProjectChangedEventArgs e)
        {
            Project = e.Project;
        }

        public ProjectDocument Project
        {
            get
            {
                return _project;
            }
            set
            {
                _project = value;

                _children.Clear();

                if (_project != null)
                {
                    _children.Add(new StagesRootViewModel(this, _project.Project.Stages));
                    _children.Add(new EntitiesRootViewModel(this, _project.Entities));
                }

                ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Raise(this, new StageChangedEventArgs(null));
            }
        }

        private StageDocument _stage;

        public StageDocument CurrentStage
        {
            get { return _stage; }
        }

        public void ChangeStage(string stageName)
        {
            try
            {
                var nextStage = _project.StageByName(stageName);

                _stage = nextStage;

                ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Raise(this, new StageChangedEventArgs(_stage));
            }
            catch (GameXmlException ex)
            {
                CustomMessageBox.ShowError(ex.Message, Project.Name);
            }
            catch (FileNotFoundException ex)
            {
                CustomMessageBox.ShowError(ex.Message, Project.Name);
            }
        }

        public void SelectEntity(string entityName)
        {
            var info = _project.EntityByName(entityName);
            ViewModelMediator.Current.GetEvent<EntitySelectedEventArgs>().Raise(this, new EntitySelectedEventArgs(info));
        }
    }

    public class StagesRootViewModel : TreeViewItemViewModel
    {
        public StagesRootViewModel(TreeViewItemViewModel parent, IEnumerable<StageLinkInfo> stages)
            : base(parent)
        {
            _children = new ObservableCollection<TreeViewItemViewModel>(stages.Select(s => new StageTreeItemViewModel(this, s)));
            ChildrenView = CollectionViewSource.GetDefaultView(_children);
            ChildrenView.SortDescriptions.Add(new SortDescription("StageName", ListSortDirection.Ascending));

            ViewModelMediator.Current.GetEvent<StageAddedEventArgs>().Subscribe(StageAdded);
        }

        private void StageAdded(object sender, StageAddedEventArgs e)
        {
            _children.Add(new StageTreeItemViewModel(this, e.Stage));
        }
    }

    public class StageTreeItemViewModel : TreeViewItemViewModel
    {
        private StageLinkInfo _stage;

        public string StageName
        {
            get { return _stage.Name; }
        }

        public StageTreeItemViewModel(TreeViewItemViewModel parent, StageLinkInfo stage)
            : base(parent)
        {
            _stage = stage;
        }
    }

    public class EntitiesRootViewModel : TreeViewItemViewModel
    {
        public EntitiesRootViewModel(TreeViewItemViewModel parent, IEnumerable<EntityInfo> entities)
            : base(parent)
        {
            _children = new ObservableCollection<TreeViewItemViewModel>(entities.OrderBy(e => e.Name).Select(e => new EntityTreeItemViewModel(this, e)));
            ViewModelMediator.Current.GetEvent<EntityAddedEventArgs>().Subscribe(EntityAdded);

            ChildrenView = CollectionViewSource.GetDefaultView(_children);
            ChildrenView.SortDescriptions.Add(new SortDescription("EntityName", ListSortDirection.Ascending));
        }

        private void EntityAdded(object sender, EntityAddedEventArgs e)
        {
            _children.Add(new EntityTreeItemViewModel(this, e.Entity));
        }
    }

    public class EntityTreeItemViewModel : TreeViewItemViewModel
    {
        private EntityInfo _entity;

        public string EntityName
        {
            get { return _entity.Name; }
        }

        public EntityTreeItemViewModel(TreeViewItemViewModel parent, EntityInfo entity)
            : base(parent)
        {
            _entity = entity;
        }
    }
}
