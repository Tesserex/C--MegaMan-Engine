using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Common.Entities;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Controls.ViewModels.Entities.Components
{
    public abstract class ComponentEditorViewModel<TComponent> : INotifyPropertyChanged where TComponent : IComponentInfo, new()
    {
        private EntityInfo _currentEntity;

        public ComponentEditorViewModel()
        {
            ViewModelMediator.Current.GetEvent<ProjectOpenedEventArgs>().Subscribe(ProjectOpened);
        }

        private void ProjectOpened(object sender, ProjectOpenedEventArgs e)
        {
            this.Project = e.Project;
        }

        protected ProjectDocument Project { get; private set; }

        public EntityInfo Entity
        {
            get { return _currentEntity; }
            set
            {
                if (value != _currentEntity)
                {
                    _currentEntity = value;
                    OnPropertyChanged("Enabled");
                    UpdateProperties();
                }
            }
        }

        public bool Enabled
        {
            get
            {
                return HasComponent();
            }
            set
            {
                if (_currentEntity == null)
                    return;

                if (value && !HasComponent())
                {
                    _currentEntity.Components.Add(new TComponent());
                }
                else if (!value && HasComponent())
                {
                    _currentEntity.Components.Remove(_currentEntity.Components.OfType<TComponent>().Single());
                }

                OnPropertyChanged("Enabled");
            }
        }

        protected bool HasComponent()
        {
            return _currentEntity != null && _currentEntity.Components.OfType<TComponent>().Any();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected abstract void UpdateProperties();

        protected void OnPropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
