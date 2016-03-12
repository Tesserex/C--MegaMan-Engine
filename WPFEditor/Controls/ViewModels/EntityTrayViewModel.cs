using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MegaMan.Common.Entities;
using MegaMan.Editor.Bll.Tools;
using MegaMan.Editor.Mediator;
using MegaMan.Editor.Tools;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class EntityTrayViewModel : INotifyPropertyChanged, IToolProvider
    {
        public IEnumerable<EntityInfo> Entities
        {
            get;
            private set;
        }

        private IToolCursor _toolCursor;
        private IToolBehavior _toolBehavior;
        private EntityInfo _selectedEntity;

        public EntityInfo SelectedEntity
        {
            get { return _selectedEntity; }
            private set
            {
                _selectedEntity = value;

                _toolCursor = new SpriteCursor(_selectedEntity.DefaultSprite);
                _toolBehavior = new EntityToolBehavior(_selectedEntity);

                if (ToolChanged != null)
                {
                    ToolChanged(this, new ToolChangedEventArgs(_toolBehavior));
                }
            }
        }

        public bool SnapHorizontal { get; set; }
        public bool SnapVertical { get; set; }

        public EntityTrayViewModel()
        {
            ViewModelMediator.Current.GetEvent<ProjectOpenedEventArgs>().Subscribe(ProjectOpened);
        }

        private void ProjectOpened(object sender, ProjectOpenedEventArgs e)
        {
            Entities = e.Project.Entities.Where(x => x.EditorData == null || !x.EditorData.HideFromPlacement);
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
