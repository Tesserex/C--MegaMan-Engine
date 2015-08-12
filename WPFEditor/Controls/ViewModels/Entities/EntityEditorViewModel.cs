using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Controls.ViewModels.Entities
{
    public class EntityEditorViewModel : INotifyPropertyChanged
    {
        private ProjectDocument _project;

        public IEnumerable<EntityInfo> EntityList { get; private set; }

        private EntityInfo _currentEntity;
        public EntityInfo CurrentEntity
        {
            get { return _currentEntity; }
            set
            {
                _currentEntity = value;

                if (_currentEntity.EditorData == null)
                    _currentEntity.EditorData = new EntityEditorData();

                OnPropertyChanged("CurrentEntity");
                OnPropertyChanged("DefaultSpriteName");
                OnPropertyChanged("DefaultSprite");
                OnPropertyChanged("ShowPlacement");
            }
        }

        public Sprite DefaultSprite
        {
            get
            {
                if (_currentEntity != null)
                    return _currentEntity.DefaultSprite;
                else
                    return null;
            }
        }

        public string DefaultSpriteName
        {
            get
            {
                if (_currentEntity != null)
                    return _currentEntity.EditorData.DefaultSpriteName;
                else
                    return null;
            }
            set
            {
                if (_currentEntity != null && value != null)
                {
                    _currentEntity.EditorData.DefaultSpriteName = value;
                    _currentEntity.DefaultSprite.Play();
                    ((App)App.Current).AnimateSprite(_currentEntity.DefaultSprite);
                    OnPropertyChanged("DefaultSpriteName");
                    OnPropertyChanged("DefaultSprite");
                }
            }
        }

        public bool ShowPlacement
        {
            get
            {
                return _currentEntity != null ? !_currentEntity.EditorData.HideFromPlacement : false;
            }
            set
            {
                if (_currentEntity != null)
                    _currentEntity.EditorData.HideFromPlacement = !value;
            }
        }

        public EntityEditorViewModel()
        {
            ViewModelMediator.Current.GetEvent<ProjectOpenedEventArgs>().Subscribe(ProjectOpened);
        }

        private void ProjectOpened(object sender, ProjectOpenedEventArgs e)
        {
            _project = e.Project;

            EntityList = e.Project.Entities
                .OrderBy(x => x.Name)
                .ToList();
            OnPropertyChanged("EntityList");
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
