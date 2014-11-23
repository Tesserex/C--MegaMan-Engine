using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MegaMan.Common;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class ProjectViewModel : TreeViewItemViewModel
    {
        private ProjectDocument _project;

        public ProjectViewModel()
            : base(null)
        {
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
                    _children.Add(new StagesRootViewModel(this, _project.Project.Stages));

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
                CustomMessageBox.ShowError(ex.Message, this.Project.Name);
            }
        }
    }

    public class StagesRootViewModel : TreeViewItemViewModel
    {
        public StagesRootViewModel(TreeViewItemViewModel parent, IEnumerable<StageLinkInfo> stages)
            : base(parent)
        {
            _children = new ObservableCollection<TreeViewItemViewModel>(stages.Select(s => new StageTreeItemViewModel(this, s)));

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
}
