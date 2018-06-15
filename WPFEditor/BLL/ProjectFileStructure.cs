using System.IO;
using MegaMan.Common;

namespace MegaMan.Editor.Bll
{
    public class ProjectFileStructure : IProjectFileStructure
    {
        private string _basePath;

        public ProjectFileStructure(Project project)
        {
            _basePath = project.BaseDir;
        }

        public FilePath CreateStagePath(string stageName)
        {
            string stagePath = EnsureDirectory("stages", stageName);

            return FilePath.FromAbsolute(stagePath, _basePath);
        }

        public FilePath CreateTilesetPath(string tilesetName)
        {
            var tilesetPath = EnsureDirectory("tilesets", tilesetName);

            var tilesetFile = Path.Combine(tilesetPath, "tiles.xml");
            return FilePath.FromAbsolute(tilesetFile, _basePath);
        }

        public FilePath CreateEntityPath(string entityName)
        {
            var entityDir = EnsureDirectory("entities");
            var entityFile = Path.Combine(entityDir, entityName + ".xml");
            return FilePath.FromAbsolute(entityFile, _basePath);
        }

        private string EnsureDirectory(params string[] dirs)
        {
            var root = _basePath;

            foreach (var dir in dirs)
            {
                root = Path.Combine(root, dir);
                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }
            }
            
            return root;
        }
    }
}
