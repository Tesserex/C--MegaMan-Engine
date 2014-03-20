using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            string stageDir = Path.Combine(_basePath, "stages");
            if (!Directory.Exists(stageDir))
            {
                Directory.CreateDirectory(stageDir);
            }
            string stagePath = Path.Combine(stageDir, stageName);
            if (!Directory.Exists(stagePath))
            {
                Directory.CreateDirectory(stagePath);
            }

            return FilePath.FromAbsolute(stagePath, this._basePath);
        }
    }
}
