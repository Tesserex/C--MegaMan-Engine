using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Common.Entities;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Editor.Controls.ViewModels.Entities.Components
{
    public class MovementComponentEditorViewModel : ComponentEditorViewModel<MovementComponentInfo>
    {
        public bool Floating
        {
            get { return HasComponent() ? Entity.MovementComponent.EffectInfo.Floating == true : false; }
            set
            {
                if (HasComponent())
                {
                    Entity.MovementComponent.EffectInfo.Floating = value;
                    OnPropertyChanged("Floating");
                }
            }
        }

        public bool FlipSprite
        {
            get { return HasComponent() ? Entity.MovementComponent.EffectInfo.FlipSprite == true : false; }
            set
            {
                if (HasComponent())
                {
                    Entity.MovementComponent.EffectInfo.FlipSprite = value;
                    OnPropertyChanged("FlipSprite");
                }
            }
        }

        protected override void UpdateProperties()
        {
            OnPropertyChanged("Floating");
            OnPropertyChanged("FlipSprite");
        }
    }
}
