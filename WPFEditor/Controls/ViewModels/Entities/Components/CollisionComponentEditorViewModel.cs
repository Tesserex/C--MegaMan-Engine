using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Common.Entities;

namespace MegaMan.Editor.Controls.ViewModels.Entities.Components
{
    public class CollisionComponentEditorViewModel : ComponentEditorViewModel<CollisionComponentInfo>
    {
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
