﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Bll.Tools;
using MegaMan.Editor.Controls.ViewModels.Entities;
using MegaMan.Editor.Mediator;
using MegaMan.Editor.Tools;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class EntityTrayViewModel : INotifyPropertyChanged, IToolProvider
    {
        public IEnumerable<EntityViewModel> Entities
        {
            get;
            private set;
        }

        private IToolCursor _toolCursor;
        private IEntityToolBehavior _toolBehavior;
        private EntityViewModel _selectedEntity;
        private ProjectDocument _currentProject;

        public ICommand ChangeToolCommand { get; set; }

        public EntityViewModel SelectedEntity
        {
            get { return _selectedEntity; }
            private set
            {
                _selectedEntity = value;

                UpdateTool("Entity");
            }
        }

        public string CursorIcon { get { return IconFor("cursor"); } }

        private string _activeIcon;
        private string ActiveIcon
        {
            get { return _activeIcon; }
            set
            {
                _activeIcon = value;
                OnPropertyChanged("CursorIcon");
            }
        }

        private string IconFor(string icon)
        {
            return String.Format("/Resources/{0}_{1}.png", icon, (_activeIcon == icon) ? "on" : "off");
        }

        public void UpdateTool(object toolParam = null)
        {
            if (toolParam == null)
            {
                _toolBehavior.SnapX = SnapHorizontal ? HorizSnapAmount : 1;
                _toolBehavior.SnapY = SnapVertical ? VertSnapAmount : 1;
                if (_toolCursor is SpriteCursor)
                {
                    ((SpriteCursor)_toolCursor).UpdateSnap(SnapHorizontal ? HorizSnapAmount : 1, SnapVertical ? VertSnapAmount : 1);
                }
            }
            else
            {
                switch (toolParam.ToString())
                {
                    case "Hand":
                        _toolCursor = new StandardToolCursor("hand.cur");
                        _toolBehavior = new EntityHandToolBehavior(SnapHorizontal ? HorizSnapAmount : 1, SnapVertical ? VertSnapAmount : 1);
                        ActiveIcon = "cursor";
                        break;

                    case "Entity":
                        _toolCursor = new SpriteCursor(SelectedEntity.DefaultSprite, SnapHorizontal ? HorizSnapAmount : 1, SnapVertical ? VertSnapAmount : 1);
                        _toolBehavior = new EntityToolBehavior(SelectedEntity.Entity, SnapHorizontal ? HorizSnapAmount : 1, SnapVertical ? VertSnapAmount : 1);
                        ActiveIcon = "";
                        break;
                }
            }

            ToolChanged?.Invoke(this, new ToolChangedEventArgs(_toolBehavior));
        }

        private bool _snapHoriz;
        private bool _snapVert;
        private int _horizSnapAmount;
        private int _vertSnapAmount;

        public bool SnapHorizontal
        {
            get { return _snapHoriz; }
            set
            {
                _snapHoriz = value;
                UpdateTool();
                OnPropertyChanged("SnapHorizontal");
            }
        }
        
        public bool SnapVertical
        {
            get { return _snapVert; }
            set
            {
                _snapVert = value;
                UpdateTool();
                OnPropertyChanged("SnapVertical");
            }
        }

        public int HorizSnapAmount
        {
            get { return _horizSnapAmount; }
            set
            {
                _horizSnapAmount = value;
                UpdateTool();
                OnPropertyChanged("HorizSnapAmount");
            }
        }

        public int VertSnapAmount
        {
            get { return _vertSnapAmount; }
            set
            {
                _vertSnapAmount = value;
                UpdateTool();
                OnPropertyChanged("VertSnapAmount");
            }
        }

        public EntityTrayViewModel()
        {
            ViewModelMediator.Current.GetEvent<ProjectChangedEventArgs>().Subscribe(ProjectChanged);
            ChangeToolCommand = new RelayCommand(UpdateTool);
            _horizSnapAmount = 8;
            _vertSnapAmount = 8;
        }

        private void ProjectChanged(object sender, ProjectChangedEventArgs e)
        {
            _currentProject = e.Project;

            if (_currentProject != null)
            {
                Entities = e.Project.Entities
                    .Where(x => x.EditorData == null || !x.EditorData.HideFromPlacement)
                    .OrderBy(x => x.Name)
                    .Select(x => new EntityViewModel(x, e.Project));
            }
            else
            {
                Entities = Enumerable.Empty<EntityViewModel>();
            }

            OnPropertyChanged("Entities");
        }

        private void OnPropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IToolBehavior Tool
        {
            get { return _toolBehavior; }
        }

        public IToolCursor ToolCursor
        {
            get { return _toolCursor; }
        }

        public event EventHandler<ToolChangedEventArgs> ToolChanged;
    }
}
