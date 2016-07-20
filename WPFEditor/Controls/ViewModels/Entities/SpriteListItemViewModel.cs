using MegaMan.Common;

namespace MegaMan.Editor.Controls.ViewModels.Entities
{
    public class SpriteListItemViewModel
    {
        private readonly SpriteViewModel _sprite;

        public SpriteListItemViewModel(Sprite sprite)
        {
            _sprite = sprite != null ? new SpriteViewModel(sprite) : null;
        }
        
        public SpriteViewModel Sprite { get { return _sprite; } }

        public string ButtonText { get { return _sprite == null ? "Add" : "Edit"; } }
    }
}
