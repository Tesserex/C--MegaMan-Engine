using System.Collections.Generic;
using System.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Common.Entities
{
    public class EntityInfo
    {
        public string Name { get; set; }
        public int MaxAlive { get; set; }
        public bool GravityFlip { get; set; }
        public EntityEditorData EditorData { get; set; }

        public EffectInfo Death { get; set; }

        public SpriteComponentInfo SpriteComponent
        {
            get
            {
                return Components.OfType<SpriteComponentInfo>().SingleOrDefault();
            }
        }

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

        public List<IComponentInfo> Components { get; set; }

        public PositionComponentInfo PositionComponent { get; set; }
        public InputComponentInfo InputComponent { get; set; }
        public CollisionComponentInfo CollisionComponent { get; set; }
        public StateComponentInfo StateComponent { get; set; }
        public MovementComponentInfo MovementComponent { get; set; }
        public HealthComponentInfo HealthComponent { get; set; }
        public LadderComponentInfo LadderComponent { get; set; }
        public WeaponComponentInfo WeaponComponent { get; set; }
    }
}
