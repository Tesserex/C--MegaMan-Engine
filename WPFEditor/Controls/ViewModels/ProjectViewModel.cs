using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MegaMan.Common;

namespace WPFEditor.Controls.ViewModels
{
    public class TreeViewItemViewModel : INotifyPropertyChanged
    {
        private bool _isSelected;
        private bool _isExpanded;

        protected TreeViewItemViewModel _parent;
        protected ObservableCollection<TreeViewItemViewModel> _children;

        public event PropertyChangedEventHandler PropertyChanged;

        protected TreeViewItemViewModel(TreeViewItemViewModel parent)
        {
            _parent = parent;

            _children = new ObservableCollection<TreeViewItemViewModel>();
        }

        public IEnumerable<TreeViewItemViewModel> Root
        {
            get { return new TreeViewItemViewModel[] { this }; }
        }

        public ObservableCollection<TreeViewItemViewModel> Children
        {
            get { return _children; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
            }
        }

        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }

    public class ProjectViewModel : TreeViewItemViewModel
    {
        public ProjectViewModel(Project project) : base(null)
        {
            _children.Add(new StagesRootViewModel(this, project.Stages));
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
