using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Controls.ViewModels.Entities.Components;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Controls.ViewModels.Entities
{
    public class EntityEditorViewModel : INotifyPropertyChanged
    {
        private ProjectDocument _project;

        public ICommand GoBackCommand { get; private set; }

        public ICommand EditSpriteCommand { get; private set; }
        public INotifyPropertyChanged ComponentViewModel { get; private set; }

        private EntityInfo _currentEntity;
        public EntityInfo CurrentEntity
        {
            get { return _currentEntity; }
            set
            {
                _currentEntity = value;

                if (_currentEntity != null)
                {
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
                }

                OnPropertyChanged("CurrentEntity");
                OnPropertyChanged("DefaultSpriteName");
                OnPropertyChanged("DefaultSprite");
                OnPropertyChanged("ShowPlacement");
                OnPropertyChanged("SpriteTabVisibility");
                OnPropertyChanged("Sprites");

                Sprite.Entity = value;
                Movement.Entity = value;
                Collision.Entity = value;
            }
        }

        public SpriteComponentEditorViewModel Sprite { get; private set; }
        public MovementComponentEditorViewModel Movement { get; private set; }
        public CollisionComponentEditorViewModel Collision { get; private set; }

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

        public EntityEditorViewModel()
        {
            EditSpriteCommand = new RelayCommand(x => EditSprite((SpriteListItemViewModel)x), arg => _currentEntity != null);
            Sprite = new SpriteComponentEditorViewModel();
            Movement = new MovementComponentEditorViewModel();
            Collision = new CollisionComponentEditorViewModel();

            ViewModelMediator.Current.GetEvent<ProjectOpenedEventArgs>().Subscribe(ProjectOpened);
            ViewModelMediator.Current.GetEvent<NewEntityEventArgs>().Subscribe(NewEntity);
            ViewModelMediator.Current.GetEvent<EntitySelectedEventArgs>().Subscribe(EntitySelected);
            
            GoBackCommand = new RelayCommand(x => GoBack(), null);
        }

        private void EditSprite(SpriteListItemViewModel sprite)
        {
            var vm = sprite.Sprite;
            if (vm == null)
            {
                vm = Sprite.AddSprite();
            }

            ComponentViewModel = new SpriteEditorViewModel(vm, _project);
            OnPropertyChanged("ComponentViewModel");
        }

        private void EntitySelected(object sender, EntitySelectedEventArgs e)
        {
            CurrentEntity = e.Entity;
        }

        private void NewEntity(object sender, NewEntityEventArgs e)
        {
            CurrentEntity = new EntityInfo() {
                Name = e.Name
            };

            _project.AddEntity(CurrentEntity);
        }

        public void GoBack()
        {
            ComponentViewModel = null;
            OnPropertyChanged("ComponentViewModel");
        }

        private void ProjectOpened(object sender, ProjectOpenedEventArgs e)
        {
            _project = e.Project;
        }

        private void Save()
        {
            if (CurrentEntity == null)
                return;
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
