using MegaMan.Common;

namespace MegaMan.Editor.Controls.ViewModels.Entities
{
    public class SpriteListItemViewModel
    {
        private readonly Sprite _sprite;

        public SpriteListItemViewModel(Sprite sprite)
        {
            _sprite = sprite;
        }

        public string Name { get { return _sprite == null ? string.Empty : _sprite.Name; } }
        public Sprite Sprite { get { return _sprite; } }

        public string ButtonText { get { return _sprite == null ? "Add" : "Edit"; } }
    }
}
