using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;
using System.Linq;
using System;
using System.Windows.Media.Imaging;
using System.Collections.Specialized;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class HistoryControlViewModel : INotifyPropertyChanged
    {
        private History _history;
        private Dictionary<Type, ImageSource> _icons;

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<UndoableActionViewModel> Items
        {
            get
            {
                if (_history == null)
                    return null;

                return _history.Actions.Select((a, i) => new UndoableActionViewModel() {
                    Name = a.Name,
                    Icon = PickIcon(a.GetType()),
                    Background = (i > _history.CurrentIndex) ? Brushes.LightGray : Brushes.Transparent
                }).ToList();
            }
        }

        public void MoveHistory(int index)
        {
            _history.MoveTo(index);
        }

        private ImageSource PickIcon(System.Type type)
        {
            if (_icons.ContainsKey(type))
            {
                return _icons[type];
            }

            return null;
        }

        public HistoryControlViewModel()
        {
            ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Subscribe(StageChanged);

            _icons = new Dictionary<Type, ImageSource>();
            _icons[typeof(DrawAction)] = new BitmapImage(new Uri("pack://application:,,,/Resources/brush.png"));
            _icons[typeof(AddEntityAction)] = new BitmapImage(new Uri("pack://application:,,,/Resources/metool.png"));
            _icons[typeof(RemoveEntityAction)] = new BitmapImage(new Uri("pack://application:,,,/Resources/metool_x.png"));
        }

        private void StageChanged(object sender, StageChangedEventArgs e)
        {
            if (e.Stage != null)
            {
                if (_history != null)
                {
                    _history.Updated -= UpdateActions;
                }

                _history = e.Stage.History;
                OnPropertyChanged("Items");

                _history.Updated += UpdateActions;
            }
        }

        private void UpdateActions(object sender, EventArgs e)
        {
            OnPropertyChanged("Items");
        }

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class UndoableActionViewModel
    {
        public ImageSource Icon { get; set; }
        public string Name { get; set; }
        public Brush Background { get; set; }
    }
}
