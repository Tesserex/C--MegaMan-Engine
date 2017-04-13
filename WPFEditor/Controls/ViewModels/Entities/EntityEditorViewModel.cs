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
            }
        }

        public IEnumerable<SpriteListItemViewModel> Sprites
        {
            get
            {
                if (_currentEntity == null || _currentEntity.SpriteComponent == null)
                    return null;

                var sprites = _currentEntity.SpriteComponent.Sprites.Values
                    .Select(s => new SpriteListItemViewModel(s))
                    .ToList();

                sprites.Add(new SpriteListItemViewModel(null));

                return sprites;
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
            ViewModelMediator.Current.GetEvent<NewEntityEventArgs>().Subscribe(NewEntity);
            ViewModelMediator.Current.GetEvent<EntitySelectedEventArgs>().Subscribe(EntitySelected);
            EditSpriteCommand = new RelayCommand(x => EditSprite((SpriteListItemViewModel)x), arg => _currentEntity != null);
            GoBackCommand = new RelayCommand(x => GoBack(), null);
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

        private void AddSprite()
        {
            Sprite sprite = CreateEmptySprite();

            _currentEntity.SpriteComponent.Sprites.Add(sprite.Name, sprite);
            ComponentViewModel = new SpriteEditorViewModel(new SpriteViewModel(sprite), _project);
            OnPropertyChanged("ComponentViewModel");
            OnPropertyChanged("Sprites");
        }

        private Sprite CreateEmptySprite()
        {
            var size = ModeOf(_currentEntity.SpriteComponent.Sprites
                .Select(s => new Common.Geometry.Point(s.Value.Width, s.Value.Height)));

            var sprite = new Sprite(size.X, size.Y);
            sprite.Name = GetNewSpriteName();

            sprite.SheetPath = _currentEntity.SpriteComponent.SheetPath;
            sprite.AddFrame();
            return sprite;
        }

        private string GetNewSpriteName()
        {
            var i = 0;
            var name = string.Empty;
            do
            {
                i++;
                name = "NewSprite" + i.ToString();
            } while (_currentEntity.SpriteComponent.Sprites.ContainsKey(name));

            return name;
        }

        public void EditSprite(SpriteListItemViewModel sprite)
        {
            if (sprite.Sprite == null)
            {
                AddSprite();
            }
            else
            {
                ComponentViewModel = new SpriteEditorViewModel(sprite.Sprite, _project);
                OnPropertyChanged("ComponentViewModel");
            }
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

            CurrentEntity = EntityList.FirstOrDefault();
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

        private T ModeOf<T>(IEnumerable<T> sequence)
        {
            return sequence
                .GroupBy(x => x)
                .OrderByDescending(g => g.Count())
                .First()
                .Key;
        }
    }
}
