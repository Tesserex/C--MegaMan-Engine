using System.Linq;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.IO.DataSources;
using MegaMan.IO.Xml.Handlers;

namespace MegaMan.IO.Xml
{
    internal class ProjectXmlReader : IProjectReader
    {
        private Project project;
        private IDataSource dataSource;
        private readonly HandlerTransferXmlReader transferReader;

        public ProjectXmlReader(HandlerTransferXmlReader transferReader)
        {
            this.transferReader = transferReader;
        }

        public string Extension { get { return ".xml"; } }

        public void Init(IDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        public Project Load()
        {
            project = new Project();

            var gameFilePath = dataSource.GetGameFile();
            project.GameFile = gameFilePath;

            var stream = dataSource.GetData(gameFilePath);
            var reader = XElement.Load(stream);
            stream.Dispose();

            var nameAttr = reader.Attribute("name");
            if (nameAttr != null) project.Name = nameAttr.Value;

            var authAttr = reader.Attribute("author");
            if (authAttr != null) project.Author = authAttr.Value;

            var sizeNode = reader.Element("Size");
            if (sizeNode != null)
            {
                project.ScreenWidth = sizeNode.TryAttribute<int>("x");
                project.ScreenHeight = sizeNode.TryAttribute<int>("y");
            }

            var nsfNode = reader.Element("NSF");
            if (nsfNode != null)
            {
                LoadNsfInfo(nsfNode);
            }

            var stagesNode = reader.Element("Stages");
            if (stagesNode != null)
            {
                foreach (var stageNode in stagesNode.Elements("Stage"))
                {
                    var info = new StageLinkInfo();
                    info.Name = stageNode.RequireAttribute("name").Value;
                    info.StagePath = FilePath.FromRelative(stageNode.RequireAttribute("path").Value, project.BaseDir);

                    var winNode = stageNode.Element("Win");
                    if (winNode != null)
                    {
                        var winHandlerNode = winNode.Element("Next");
                        if (winHandlerNode != null)
                        {
                            info.WinHandler = transferReader.Load(winHandlerNode);
                        }
                    }

                    var loseNode = stageNode.Element("Lose");
                    if (loseNode != null)
                    {
                        var loseHandlerNode = loseNode.Element("Next");
                        if (loseHandlerNode != null)
                        {
                            info.LoseHandler = transferReader.Load(loseHandlerNode);
                        }
                    }

                    project.AddStage(info);
                }
            }

            var startNode = reader.Element("Next");
            if (startNode != null)
            {
                project.StartHandler = transferReader.Load(startNode);
            }

            project.AddIncludeFiles(reader.Elements("Include")
                .Select(e => e.Value)
                .Where(v => !string.IsNullOrEmpty(v.Trim())));

            project.AddIncludeFolders(reader.Elements("IncludeFolder")
                .Select(e => e.Value)
                .Where(v => !string.IsNullOrEmpty(v.Trim())));

            var includeReader = new IncludeFileXmlReader();

            var includedFilesFromFolders = project.IncludeFolders.SelectMany(dataSource.GetFilesInFolder);
            var allIncludedFiles = project.IncludeFiles.ToList()
                .Concat(includedFilesFromFolders)
                .Distinct().ToList();
            foreach (var includePath in allIncludedFiles)
            {
                var includeStream = dataSource.GetData(includePath);
                includeReader.LoadIncludedFile(project, includePath, includeStream, dataSource);
                includeStream.Dispose();
            }

            stream.Close();

            return project;
        }

        private void LoadNsfInfo(XElement nsfNode)
        {
            var musicNode = nsfNode.Element("Music");
            if (musicNode != null)
            {
                project.MusicNsf = FilePath.FromRelative(musicNode.Value, project.BaseDir);
            }

            var sfxNode = nsfNode.Element("SFX");
            if (sfxNode != null)
            {
                project.EffectsNsf = FilePath.FromRelative(sfxNode.Value, project.BaseDir);
            }
        }
    }
}
