using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMan.Editor.Controls.ViewModels
{
    class ProjectSettingsViewModel : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _author;
        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                OnPropertyChanged("Author");
            }
        }

        private string _musicNsf;
        public string MusicNsf
        {
            get { return _musicNsf; }
            set
            {
                _musicNsf = value;
                OnPropertyChanged("MusicNsf");
            }
        }

        private string _effectsNsf;
        public string EffectsNsf
        {
            get { return _effectsNsf; }
            set
            {
                _effectsNsf = value;
                OnPropertyChanged("EffectsNsf");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
