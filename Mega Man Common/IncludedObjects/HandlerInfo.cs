using System.Collections.Generic;

namespace MegaMan.Common.IncludedObjects
{
    public interface IHandlerObjectInfo
    {
        string Name { get; }
    }

    public class HandlerSpriteInfo : IHandlerObjectInfo
    {
        public Sprite Sprite { get; set; }
        public string Name { get { return Sprite.Name; } }
    }

    public abstract class HandlerInfo : IncludedObject
    {
        public string Name { get; set; }
        public Dictionary<string, IHandlerObjectInfo> Objects { get; private set; }

        public HandlerInfo()
        {
            Objects = new Dictionary<string, IHandlerObjectInfo>();
        }
    }
}
