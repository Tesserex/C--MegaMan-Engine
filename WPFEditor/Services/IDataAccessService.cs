using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Services
{
    public interface IDataAccessService
    {
        ProjectDocument CreateProject(string directory);
        ProjectDocument LoadProject(string filePath);
        void SaveProject(ProjectDocument project);
        void ExportProject(ProjectDocument project);
        StageDocument LoadStage(ProjectDocument project, StageLinkInfo linkInfo);
        void SaveStage(StageDocument stage);
        TilesetDocument CreateTileset(FilePath filePath);
        TilesetDocument LoadTileset(FilePath filePath);
        void SaveTileset(TilesetDocument tileset);
        void SaveEntity(EntityInfo entity, string path);
    }
}
