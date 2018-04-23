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
        public ICommand ZoomOutViewSpriteCommand { get; private set; }
        public ICommand ZoomInViewSpriteCommand { get; private set; }
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
                        ViewingSprite = _currentEntity.DefaultSprite;
                    }
                }
                else
                {
                    ViewingSprite = null;
                }

                OnPropertyChanged("CurrentEntity");
                OnPropertyChanged("DefaultSpriteName");
                OnPropertyChanged("DefaultSprite");
                OnPropertyChanged("ShowPlacement");
                OnPropertyChanged("IsProjectile");
                OnPropertyChanged("SpriteTabVisibility");
                OnPropertyChanged("Sprites");

                Sprite.Entity = value;
                Movement.Entity = value;
                Collision.Entity = value;

                HitBoxEditor.ChangeHitbox(null);
                showHitboxEditor = false;
                UpdatePreview();
            }
        }

        public SpriteComponentEditorViewModel Sprite { get; private set; }
        public MovementComponentEditorViewModel Movement { get; private set; }
        public CollisionComponentEditorViewModel Collision { get; private set; }
        public HitboxEditorViewModel HitBoxEditor { get; private set; }

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
                HitBoxEditor.ChangeSprite(value);
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
                HitBoxEditor.Zoom = value;
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

        public Visibility PreviewVisibility
        {
            get { return (HitboxEditorVisibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible; }
        }

        private bool showHitboxEditor;
        public Visibility HitboxEditorVisibility
        {
            get
            {
                return showHitboxEditor ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public EntityEditorViewModel()
        {
            EditSpriteCommand = new RelayCommand(x => EditSprite((SpriteListItemViewModel)x), arg => _currentEntity != null);
            ZoomOutViewSpriteCommand = new RelayCommand(x => ZoomOutViewSprite(), arg => _currentEntity != null);
            ZoomInViewSpriteCommand = new RelayCommand(x => ZoomInViewSprite(), arg => _currentEntity != null);
            Sprite = new SpriteComponentEditorViewModel();
            Movement = new MovementComponentEditorViewModel();
            Collision = new CollisionComponentEditorViewModel();
            HitBoxEditor = new HitboxEditorViewModel();

            ViewSpriteZoom = 1;

            ViewModelMediator.Current.GetEvent<ProjectChangedEventArgs>().Subscribe(ProjectChanged);
            ViewModelMediator.Current.GetEvent<NewEntityEventArgs>().Subscribe(NewEntity);
            ViewModelMediator.Current.GetEvent<EntitySelectedEventArgs>().Subscribe(EntitySelected);

            GoBackCommand = new RelayCommand(x => GoBack(), null);
            Collision.HitBoxEdit += Collision_HitBoxEdit;
        }

        private void Collision_HitBoxEdit(HitBoxInfo hitbox)
        {
            HitBoxEditor.ChangeHitbox(hitbox);
            showHitboxEditor = true;
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            OnPropertyChanged("HitboxEditorVisibility");
            OnPropertyChanged("PreviewVisibility");
        }

        private void EditSprite(SpriteListItemViewModel sprite)
        {
            var model = sprite.Sprite;
            if (model == null)
            {
                model = Sprite.AddSprite();
            }

            ComponentViewModel = new SpriteEditorViewModel(new SpriteViewModel(model), _project);
            OnPropertyChanged("ComponentViewModel");
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

        private void ProjectChanged(object sender, ProjectChangedEventArgs e)
        {
            _project = e.Project;
            CurrentEntity = null;
            HitBoxEditor.ChangeProject(_project);
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
