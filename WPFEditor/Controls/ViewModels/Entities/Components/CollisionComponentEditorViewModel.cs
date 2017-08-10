using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MegaMan.Common.Entities;

namespace MegaMan.Editor.Controls.ViewModels.Entities.Components
{
    public class CollisionComponentEditorViewModel : ComponentEditorViewModel<CollisionComponentInfo>
    {
        public event Action<HitBoxInfo> HitBoxEdit;

        public ICommand AddHitBoxCommand { get; private set; }
        public ICommand EditHitBoxCommand { get; private set; }

        public CollisionComponentEditorViewModel()
        {
            AddHitBoxCommand = new RelayCommand(x => AddHitbox(), x => Entity != null);
            EditHitBoxCommand = new RelayCommand(x => EditHitbox(), x => Entity != null && SelectedHitBox != null);
        }

        private void AddHitbox()
        {
            if (Entity == null || Entity.CollisionComponent == null) return;

            var hitbox = new HitBoxInfo();
            Entity.CollisionComponent.HitBoxes.Add(hitbox);
            SelectedHitBox = hitbox;

            EditHitbox();
        }

        private void EditHitbox()
        {
            HitBoxEdit?.Invoke(SelectedHitBox);
        }

        protected override void UpdateProperties()
        {
            SelectedHitBox = null;
            OnPropertyChanged("HitBoxes");
            OnPropertyChanged("SelectedHitBox");
        }

        public IEnumerable<HitBoxInfo> HitBoxes
        {
            get
            {
                if (Entity == null || Entity.CollisionComponent == null)
                    return null;

                return Entity.CollisionComponent.HitBoxes;
            }
        }

        public HitBoxInfo SelectedHitBox { get; set; }
    }
}
