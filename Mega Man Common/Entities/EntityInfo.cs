using System.Collections.Generic;
using System.Linq;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.Common.Entities
{
    public class EntityInfo : IncludedObject
    {
        public EntityInfo()
        {
            Components = new List<IComponentInfo>();
        }

        public string Name { get; set; }
        public int MaxAlive { get; set; }
        public bool GravityFlip { get; set; }
        public EntityEditorData EditorData { get; set; }

        public EffectInfo Death { get; set; }

        public List<IComponentInfo> Components { get; set; }

        public Sprite DefaultSprite
        {
            get
            {
                if (SpriteComponent == null || !SpriteComponent.Sprites.Any())
                    return null;

                if (EditorData != null && EditorData.DefaultSpriteName != null)
                    return SpriteComponent.Sprites[EditorData.DefaultSpriteName];
                return SpriteComponent.Sprites.Values.First();
            }
        }

        public SpriteComponentInfo SpriteComponent { get { return Components.OfType<SpriteComponentInfo>().SingleOrDefault(); } }
        public PositionComponentInfo PositionComponent { get { return Components.OfType<PositionComponentInfo>().SingleOrDefault(); } }
        public InputComponentInfo InputComponent { get { return Components.OfType<InputComponentInfo>().SingleOrDefault(); } }
        public CollisionComponentInfo CollisionComponent { get { return Components.OfType<CollisionComponentInfo>().SingleOrDefault(); } }
        public StateComponentInfo StateComponent { get { return Components.OfType<StateComponentInfo>().SingleOrDefault(); } }
        public MovementComponentInfo MovementComponent { get { return Components.OfType<MovementComponentInfo>().SingleOrDefault(); } }
        public HealthComponentInfo HealthComponent { get { return Components.OfType<HealthComponentInfo>().SingleOrDefault(); } }
        public LadderComponentInfo LadderComponent { get { return Components.OfType<LadderComponentInfo>().SingleOrDefault(); } }
        public WeaponComponentInfo WeaponComponent { get { return Components.OfType<WeaponComponentInfo>().SingleOrDefault(); } }
    }
}
