using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Controls.ViewModels.Entities
{
    public class EntityEditorViewModel : INotifyPropertyChanged
    {
        private ProjectDocument _project;

        public ICommand GoBackCommand { get; private set; }
        public ICommand EditSpriteCommand { get; private set; }

        public INotifyPropertyChanged ComponentViewModel { get; private set; }

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

                if (_currentEntity.SpriteComponent != null)
                {
                    foreach (var sprite in _currentEntity.SpriteComponent.Sprites.Values)
                    {
                        sprite.Play();
                        ((App)App.Current).AnimateSprite(sprite);
                    }
                }

                OnPropertyChanged("CurrentEntity");
                OnPropertyChanged("DefaultSpriteName");
                OnPropertyChanged("DefaultSprite");
                OnPropertyChanged("ShowPlacement");
                OnPropertyChanged("SpriteTabVisibility");
                OnPropertyChanged("Sprites");
            }
        }

        public IEnumerable<Sprite> Sprites { get { return (_currentEntity != null && _currentEntity.SpriteComponent != null) ? _currentEntity.SpriteComponent.Sprites.Values : null; } }

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

        public Visibility SpriteTabVisibility
        {
            get
            {
                return (_currentEntity != null && _currentEntity.SpriteComponent != null && _currentEntity.SpriteComponent.Sprites.Any()) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public EntityEditorViewModel()
        {
            ViewModelMediator.Current.GetEvent<ProjectOpenedEventArgs>().Subscribe(ProjectOpened);
            EditSpriteCommand = new RelayCommand(x => EditSprite((Sprite)x), arg => _currentEntity != null);
            GoBackCommand = new RelayCommand(x => GoBack(), null);
        }

        public void EditSprite(Sprite sprite)
        {
            ComponentViewModel = new SpriteEditorViewModel(sprite);
            OnPropertyChanged("ComponentViewModel");
        }

        public void GoBack()
        {
            ComponentViewModel = null;
            OnPropertyChanged("ComponentViewModel");
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
