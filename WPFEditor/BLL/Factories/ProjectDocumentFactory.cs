using MegaMan.Common;

namespace MegaMan.Editor.Bll.Factories
{
    public class ProjectDocumentFactory : IProjectDocumentFactory
    {
        private readonly FactoryCore _core;
        private readonly IStageDocumentFactory _stageFactory;

        public ProjectDocumentFactory(FactoryCore core, IStageDocumentFactory stageFactory)
        {
            _core = core;
            _stageFactory = stageFactory;
        }

        public ProjectDocument CreateNew(string directory)
        {
            var project = new Project()
            {
                GameFile = FilePath.FromRelative("game.xml", directory)
            };

            var p = new ProjectDocument(new ProjectFileStructure(project), project, _stageFactory);
            return p;
        }

        public ProjectDocument Load(string filePath)
        {
            _core.Load(filePath);
            var project = _core.Reader.GetProjectReader().Load();
            var structure = new ProjectFileStructure(project);
            var projectDocument = new ProjectDocument(structure, project, _stageFactory);
            return projectDocument;
        }
    }
}
