using MegaMan.Common.Entities;

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
