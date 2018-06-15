using System.Collections.Generic;

namespace MegaMan.Common.Entities
{
    public class SpriteComponentInfo : IComponentInfo
    {
        public FilePath SheetPath { get; set; }
        public Dictionary<string, Sprite> Sprites { get; private set; }

        public SpriteComponentInfo()
        {
            Sprites = new Dictionary<string, Sprite>();
        }
    }
}
