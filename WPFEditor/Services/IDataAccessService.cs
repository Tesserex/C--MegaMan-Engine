using MegaMan.Common.Entities;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Services
{
    public interface IDataAccessService
    {
        void SaveProject(ProjectDocument project);
        void SaveStage(StageDocument stage);
        void SaveTileset(TilesetDocument tileset);
        void SaveEntity(EntityInfo entity, string path);
    }
}
