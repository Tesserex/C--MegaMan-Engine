
namespace MegaMan.Editor.Bll.Factories
{
    public interface IProjectDocumentFactory
    {
        ProjectDocument CreateNew(string directory);
        ProjectDocument Load(string filePath);
    }
}
