using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class HistoryControlViewModel : INotifyPropertyChanged
    {
        private ReadOnlyObservableCollection<IUndoableAction> _actions;

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<IUndoableAction> Items
        {
            get
            {
                return _actions;
            }
        }

        public HistoryControlViewModel()
        {
            ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Subscribe(StageChanged);
        }

        private void StageChanged(object sender, StageChangedEventArgs e)
        {
            if (e.Stage != null)
            {
                _actions = e.Stage.History.Actions;
                OnPropertyChanged("Items");
            }
        }

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
