using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MegaMan.Editor.Bll;
using MegaMan.Common;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class ProjectViewModel : TreeViewItemViewModel, IStageProvider
    {
        private ProjectDocument _project;
        public ProjectViewModel(ProjectDocument project) : base(null)
        {
            _project = project;

            _children.Add(new StagesRootViewModel(this, _project.Project.Stages));
        }

        private StageDocument _stage;

        public StageDocument CurrentStage
        {
            get { return _stage; }
        }

        public void ChangeStage(string stageName)
        {
            var nextStage = _project.StageByName(stageName);
            if (nextStage != _stage)
            {
                _stage = nextStage;

                if (StageChanged != null)
                {
                    StageChanged(this, new StageChangedEventArgs(_stage));
                }
            }
        }

        public event EventHandler<StageChangedEventArgs> StageChanged;
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
