using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MegaMan.Common;
using MegaMan.Common.Entities;

namespace MegaMan.Editor.Controls.ViewModels.Entities.Components
{
    public class SpriteComponentEditorViewModel : ComponentEditorViewModel<SpriteComponentInfo>
    {
        public IEnumerable<SpriteListItemViewModel> Sprites
        {
            get
            {
                if (!HasComponent())
                    return null;

                var sprites = Entity.SpriteComponent.Sprites.Values
                    .Select(s => new SpriteListItemViewModel(s))
                    .ToList();

                sprites.Add(new SpriteListItemViewModel(null));

                return sprites;
            }
        }

        public SpriteViewModel AddSprite()
        {
            Sprite sprite = CreateEmptySprite();
            var vm = new SpriteViewModel(sprite);

            Entity.SpriteComponent.Sprites.Add(sprite.Name, sprite);

            OnPropertyChanged("Sprites");

            return vm;
        }

        private Sprite CreateEmptySprite()
        {
            var size = ModeOf(Entity.SpriteComponent.Sprites
                .Select(s => new Common.Geometry.Point(s.Value.Width, s.Value.Height)));

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
                name = "NewSprite" + i.ToString();
            } while (Entity.SpriteComponent.Sprites.ContainsKey(name));

            return name;
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
