using System.Collections.Generic;
using System.Linq;

namespace MegaMan.Common.Entities
{
    public class EntityInfo
    {
        public string Name { get; set; }
        public int MaxAlive { get; set; }
        public Dictionary<string, Sprite> Sprites { get; private set; }
        public EntityEditorData EditorData { get; set; }

        public EntityInfo()
        {
            Sprites = new Dictionary<string, Sprite>();
        }

        public Sprite DefaultSprite
        {
            get
            {
                if (!Sprites.Any())
                    return null;

                if (EditorData != null && EditorData.DefaultSpriteName != null)
                    return Sprites[EditorData.DefaultSpriteName];
                else
                    return Sprites.Values.First();
            }
        }
    }
}
