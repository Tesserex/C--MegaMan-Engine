using System.Collections.Generic;
using System.Linq;

namespace MegaMan.Common.Entities
{
    public class EntityInfo
    {
        public string Name { get; set; }
        public int MaxAlive { get; set; }
        public EntityEditorData EditorData { get; set; }

        public SpriteComponentInfo SpriteComponent { get; set; }

        public Sprite DefaultSprite
        {
            get
            {
                if (SpriteComponent == null || !SpriteComponent.Sprites.Any())
                    return null;

                if (EditorData != null && EditorData.DefaultSpriteName != null)
                    return SpriteComponent.Sprites[EditorData.DefaultSpriteName];
                else
                    return SpriteComponent.Sprites.Values.First();
            }
        }

        public PositionComponentInfo PositionComponent { get; set; }
        public InputComponentInfo InputComponent { get; set; }
        public CollisionComponentInfo CollisionComponent { get; set; }
    }
}
