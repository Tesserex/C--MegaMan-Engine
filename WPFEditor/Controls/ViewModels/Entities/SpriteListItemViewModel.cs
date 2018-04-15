using MegaMan.Common;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls.ViewModels.Entities
{
    public class SpriteListItemViewModel
    {
        private readonly SpriteModel _model;

        public SpriteListItemViewModel(Sprite sprite)
        {
            if (sprite != null)
            {
                _model = new SpriteModel(sprite);
                _model.Play();
            }
        }

        public string NameUpper { get { return Sprite == null || Sprite.Name == null ? string.Empty : Sprite.Name.ToUpper(); } }

        public SpriteModel Sprite => _model;

        public string ButtonText { get { return Sprite == null ? "ADD" : "EDIT"; } }
    }
}
