using System;
using System.ComponentModel;
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
        
        public ICommand ZoomOutViewSpriteCommand { get; private set; }
        public ICommand ZoomInViewSpriteCommand { get; private set; }

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
                        ViewingSprite = _currentEntity.DefaultSprite;
                    }
                }
                else
                {
                    ViewingSprite = null;
                }

                OnPropertyChanged(nameof(CurrentEntity));
                OnPropertyChanged(nameof(EntityNameUpper));
                OnPropertyChanged(nameof(DefaultSpriteName));
                OnPropertyChanged(nameof(DefaultSprite));
                OnPropertyChanged(nameof(ShowPlacement));
                OnPropertyChanged(nameof(IsProjectile));
                OnPropertyChanged(nameof(MessageVisibility));
                OnPropertyChanged(nameof(TabsVisibility));

                Sprite.Entity = value;
                Movement.Entity = value;
                Collision.Entity = value;
            }
        }

        public string EntityNameUpper
        {
            get { return CurrentEntity?.Name?.ToUpper(); }
        }

        public Visibility MessageVisibility { get { return CurrentEntity != null ? Visibility.Collapsed : Visibility.Visible; } }
        public Visibility TabsVisibility { get { return CurrentEntity != null ? Visibility.Visible : Visibility.Collapsed; } }

        public SpriteComponentEditorViewModel Sprite { get; private set; }
        public MovementComponentEditorViewModel Movement { get; private set; }
        public CollisionComponentEditorViewModel Collision { get; private set; }

        public Sprite DefaultSprite
        {
            get
            {
                if (_currentEntity != null)
                    return _currentEntity.DefaultSprite;
                return null;
            }
        }

        public string DefaultSpriteName
        {
            get
            {
                if (_currentEntity != null)
                    return _currentEntity.EditorData.DefaultSpriteName;
                return null;
            }
            set
            {
                if (_currentEntity != null && value != null)
                {
                    _currentEntity.EditorData.DefaultSpriteName = value;
                    _project.Dirty = true;
                    OnPropertyChanged("DefaultSpriteName");
                    OnPropertyChanged("DefaultSprite");
                }
            }
        }

        private Sprite _viewingSprite;
        public Sprite ViewingSprite
        {
            get { return _viewingSprite; }
            set
            {
                _viewingSprite = value;
                Collision.ChangeSprite(value);
                OnPropertyChanged("ViewingSprite");
            }
        }

        private int _viewSpriteZoom;
        public int ViewSpriteZoom
        {
            get { return _viewSpriteZoom; }
            set
            {
                _viewSpriteZoom = value;
                Collision.Zoom = value;
                OnPropertyChanged("ViewSpriteZoom");
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
                {
                    _currentEntity.EditorData.HideFromPlacement = !value;
                    _project.Dirty = true;
                }
            }
        }

        public bool IsProjectile
        {
            get
            {
                return _currentEntity != null ? _currentEntity.EditorData.IsProjectile : false;
            }
            set
            {
                if (_currentEntity != null)
                {
                    _currentEntity.EditorData.IsProjectile = value;
                    _project.Dirty = true;
                }
            }
        }

        public EntityEditorViewModel()
        {
            ZoomOutViewSpriteCommand = new RelayCommand(x => ZoomOutViewSprite(), arg => _currentEntity != null);
            ZoomInViewSpriteCommand = new RelayCommand(x => ZoomInViewSprite(), arg => _currentEntity != null);
            Sprite = new SpriteComponentEditorViewModel();
            Movement = new MovementComponentEditorViewModel();
            Collision = new CollisionComponentEditorViewModel();

            ViewSpriteZoom = 1;

            ViewModelMediator.Current.GetEvent<ProjectChangedEventArgs>().Subscribe(ProjectChanged);
            ViewModelMediator.Current.GetEvent<NewEntityEventArgs>().Subscribe(NewEntity);
            ViewModelMediator.Current.GetEvent<EntitySelectedEventArgs>().Subscribe(EntitySelected);
        }

        private void ZoomOutViewSprite()
        {
            ViewSpriteZoom = Math.Max(1, ViewSpriteZoom / 2);
        }

        private void ZoomInViewSprite()
        {
            ViewSpriteZoom = Math.Min(4, ViewSpriteZoom * 2);
        }

        private void EntitySelected(object sender, EntitySelectedEventArgs e)
        {
            CurrentEntity = e.Entity;
        }

        private void NewEntity(object sender, NewEntityEventArgs e)
        {
            if (_project == null)
                return;

            CurrentEntity = new EntityInfo {
                Name = e.Name
            };

            _project.AddEntity(CurrentEntity);
        }

        private void ProjectChanged(object sender, ProjectChangedEventArgs e)
        {
            _project = e.Project;
            CurrentEntity = null;
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
