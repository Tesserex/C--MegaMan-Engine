using MegaMan.Common;

namespace MegaMan.Editor.Bll
{
    public interface IProjectFileStructure
    {
        FilePath CreateStagePath(string stageName);
        FilePath CreateTilesetPath(string tilesetName);
        FilePath CreateEntityPath(string entityName);
    }
}
