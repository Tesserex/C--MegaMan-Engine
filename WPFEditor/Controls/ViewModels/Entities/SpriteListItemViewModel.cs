using MegaMan.Common;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls.ViewModels.Entities
{
    public class SpriteListItemViewModel
    {
        private readonly SpriteModel _sprite;

        public SpriteListItemViewModel(Sprite sprite)
        {
            _sprite = sprite != null ? new SpriteModel(sprite) : null;
        }

        public string NameUpper { get { return Sprite == null || Sprite.Name == null ? string.Empty : Sprite.Name.ToUpper(); } }

        public SpriteModel Sprite => _sprite;

        public string ButtonText { get { return Sprite == null ? "ADD" : "EDIT"; } }
    }
}
