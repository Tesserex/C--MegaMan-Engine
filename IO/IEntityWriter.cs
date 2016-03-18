using MegaMan.Common.Entities;

namespace MegaMan.IO
{
    public interface IEntityWriter
    {
        void Write(EntityInfo entity, string filepath);
    }
}
