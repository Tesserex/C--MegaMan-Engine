using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;
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
        private ProjectDocument _project;

        public string Name
        {
            get { return _project.Name; }
            set
            {
                _project.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Author
        {
            get { return _project.Author; }
            set
            {
                _project.Author = value;
                OnPropertyChanged("Author");
            }
        }

        public string MusicNsf
        {
            get { return _project.MusicNsf; }
            set
            {
                _project.MusicNsf = value;
                OnPropertyChanged("MusicNsf");
            }
        }

        public string EffectsNsf
        {
            get { return _project.EffectsNsf; }
            set
            {
                _project.EffectsNsf = value;
                OnPropertyChanged("EffectsNsf");
            }
        }

        public IEnumerable<Common.HandlerType> HandlerTypes
        {
            get
            {
                return Enum.GetValues(typeof(Common.HandlerType)).Cast<Common.HandlerType>();
            }
        }

        public Common.HandlerType StartType
        {
            get { return _project.StartHandlerType; }
            set
            {
                _project.StartHandlerType = value;
                OnPropertyChanged("StartType");
                OnPropertyChanged("StartHandlers");
            }
        }

        public IEnumerable<string> StartHandlers
        {
            get
            {
                if (StartType == Common.HandlerType.Stage)
                    return _project.StageNames;

                return Enumerable.Empty<string>();
            }
        }

        public string StartName
        {
            get { return _project.StartHandlerName; }
            set
            {
                _project.StartHandlerName = value;
                OnPropertyChanged("StartName");
            }
        }

        public ProjectSettingsViewModel(ProjectDocument project)
        {
            SetProject(project);
            ViewModelMediator.Current.GetEvent<ProjectOpenedEventArgs>().Subscribe((s, e) => SetProject(e.Project));
        }

        public void SetProject(ProjectDocument projectDocument)
        {
            _project = projectDocument;
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
