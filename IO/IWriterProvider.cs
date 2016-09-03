
namespace MegaMan.IO
{
    public interface IWriterProvider
    {
        IProjectWriter GetProjectWriter();
        IStageWriter GetStageWriter();
        ITilesetWriter GetTilesetWriter();
        IEntityWriter GetEntityWriter();
        IIncludedObjectGroupWriter GetEntityGroupWriter();
    }
}
