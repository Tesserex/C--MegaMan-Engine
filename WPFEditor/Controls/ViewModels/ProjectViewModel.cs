using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MegaMan.Editor.Bll;
using MegaMan.Common;
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
            var nextStage = _project.StageByName(stageName);

            _stage = nextStage;

            ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Raise(this, new StageChangedEventArgs(_stage));
        }
    }

    public class StagesRootViewModel : TreeViewItemViewModel
    {
        public StagesRootViewModel(TreeViewItemViewModel parent, IEnumerable<StageLinkInfo> stages)
            : base(parent)
        {
            _children = new ObservableCollection<TreeViewItemViewModel>(stages.Select(s => new StageTreeItemViewModel(this, s)));
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
