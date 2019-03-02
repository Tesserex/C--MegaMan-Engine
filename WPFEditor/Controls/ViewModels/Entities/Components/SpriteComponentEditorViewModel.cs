using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Geometry;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls.ViewModels.Entities.Components
{
    public class SpriteComponentEditorViewModel : ComponentEditorViewModel<SpriteComponentInfo>
    {
        public ICommand EditSpriteCommand { get; private set; }

        public IEnumerable<SpriteListItemViewModel> Sprites
        {
            get
            {
                if (!Enabled)
                {
                    return new List<SpriteListItemViewModel> {
                        new SpriteListItemViewModel(null)
                    };
                }

                var sprites = Entity.SpriteComponent.Sprites.Values
                    .Select(s => new SpriteListItemViewModel(s))
                    .ToList();

                sprites.Add(new SpriteListItemViewModel(null));

                return sprites;
            }
        }

        public SpriteEditorViewModel SpriteEditorViewModel { get; private set; }

        public SpriteComponentEditorViewModel()
        {
            EditSpriteCommand = new RelayCommand(x => EditSprite((SpriteListItemViewModel)x), arg => Entity != null);
        }

        public SpriteModel AddSprite()
        {
            if (!Enabled)
            {
                Enabled = true;
            }

            Sprite sprite = CreateEmptySprite();
            Entity.SpriteComponent.Sprites.Add(sprite.Name, sprite);
            OnPropertyChanged("Sprites");

            return new SpriteModel(sprite);
        }

        private Sprite CreateEmptySprite()
        {
            var size = Entity.SpriteComponent.Sprites.Any() ?
                ModeOf(Entity.SpriteComponent.Sprites
                    .Select(s => new Point(s.Value.Width, s.Value.Height))) :
                new Point(16, 16);

            var sprite = new Sprite(size.X, size.Y);
            sprite.Name = GetNewSpriteName();

            sprite.SheetPath = Entity.SpriteComponent.SheetPath;
            sprite.AddFrame();
            return sprite;
        }

        private string GetNewSpriteName()
        {
            var i = 0;
            var name = string.Empty;
            do
            {
                i++;
                name = "NewSprite" + i;
            } while (Entity.SpriteComponent.Sprites.ContainsKey(name));

            return name;
        }

        private void EditSprite(SpriteListItemViewModel sprite)
        {
            var model = sprite.Sprite;
            if (model == null)
            {
                model = AddSprite();
            }

            SpriteEditorViewModel = new SpriteEditorViewModel(new SpriteViewModel(model), Project);
            OnPropertyChanged(nameof(SpriteEditorViewModel));
        }

        protected override void UpdateProperties()
        {
            OnPropertyChanged("Sprites");
        }

        private T ModeOf<T>(IEnumerable<T> sequence)
        {
            return sequence
                .GroupBy(x => x)
                .OrderByDescending(g => g.Count())
                .First()
                .Key;
        }
    }
}
