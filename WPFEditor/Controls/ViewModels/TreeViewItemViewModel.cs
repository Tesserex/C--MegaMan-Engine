using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MegaMan.Editor.Controls.ViewModels
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

            _isExpanded = true;
        }

        public IEnumerable<TreeViewItemViewModel> Root
        {
            get { return new[] { this }; }
        }

        public ObservableCollection<TreeViewItemViewModel> Children
        {
            get { return _children; }
        }

        public ICollectionView ChildrenView { get; protected set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
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
                    OnPropertyChanged("IsExpanded");
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
}
