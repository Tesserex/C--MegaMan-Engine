using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MegaMan.Common.Entities;

namespace MegaMan.Editor.Controls.ViewModels.Entities.Components
{
    public class CollisionComponentEditorViewModel : ComponentEditorViewModel<CollisionComponentInfo>
    {
        public event Action<HitBoxInfo> HitBoxEdit;

        public ICommand AddHitBoxCommand { get; private set; }
        public ICommand EditHitBoxCommand { get; private set; }
        public ICommand DeleteHitBoxCommand { get; private set; }

        public CollisionComponentEditorViewModel()
        {
            AddHitBoxCommand = new RelayCommand(x => AddHitbox(), x => Entity != null);
            EditHitBoxCommand = new RelayCommand(x => EditHitbox(), x => Entity != null && SelectedHitBox != null);
            DeleteHitBoxCommand = new RelayCommand(x => DeleteHitbox(), x => Entity != null && SelectedHitBox != null);
        }

        private void DeleteHitbox()
        {
            Entity.CollisionComponent.HitBoxes.Remove(SelectedHitBox);
            Project.Dirty = true;
            UpdateProperties();
        }

        private void AddHitbox()
        {
            if (Entity == null || Entity.CollisionComponent == null) return;

            var hitbox = new HitBoxInfo();
            Entity.CollisionComponent.HitBoxes.Add(hitbox);
            SelectedHitBox = hitbox;
            Project.Dirty = true;

            EditHitbox();
        }

        private void EditHitbox()
        {
            HitBoxEdit?.Invoke(SelectedHitBox);
        }

        protected override void UpdateProperties()
        {
            SelectedHitBox = null;
            OnPropertyChanged("SelectedHitBox");
            OnPropertyChanged("HitBoxes");
        }

        public IEnumerable<HitBoxInfo> HitBoxes
        {
            get
            {
                if (Entity == null || Entity.CollisionComponent == null)
                    return Enumerable.Empty<HitBoxInfo>();

                // for some reason this copying is necessary for clearing the list when deleting a box
                return new List<HitBoxInfo>(Entity.CollisionComponent.HitBoxes);
            }
        }

        public HitBoxInfo SelectedHitBox { get; set; }
    }
}
