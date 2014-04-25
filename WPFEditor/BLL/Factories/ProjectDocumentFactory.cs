using MegaMan.Common;
using MegaMan.IO;

namespace MegaMan.Editor.Bll.Factories
{
    public class ProjectDocumentFactory : IProjectDocumentFactory
    {
        private readonly IProjectReader _projectReader;

        public ProjectDocumentFactory(IProjectReader projectReader)
        {
            _projectReader = projectReader;
        }

        public ProjectDocument CreateNew(string directory)
        {
            var project = new Project()
            {
                GameFile = FilePath.FromRelative("game.xml", directory)
            };

            var p = new ProjectDocument(new ProjectFileStructure(project), project);
            return p;
        }

        public ProjectDocument Load(string filePath)
        {
            var project = _projectReader.Load(filePath);
            var structure = new ProjectFileStructure(project);
            var projectDocument = new ProjectDocument(structure, project);
            LoadIncludes(projectDocument, project);
            return projectDocument;
        }

        private void LoadIncludes(ProjectDocument projectDocument, Project project)
        {

        }
    }
}
