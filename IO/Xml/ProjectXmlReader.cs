﻿using System.IO;
using System.Linq;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.IO.DataSources;
using MegaMan.IO.Xml.Handlers;

namespace MegaMan.IO.Xml
{
    internal class ProjectXmlReader : IProjectReader
    {
        private Project _project;
        private IDataSource _dataSource;
        private readonly HandlerTransferXmlReader _transferReader;

        public ProjectXmlReader(HandlerTransferXmlReader transferReader)
        {
            _transferReader = transferReader;
        }

        public string Extension { get { return ".xml"; } }

        public void Init(IDataSource dataSource)
        {
            this._dataSource = dataSource;
        }

        public Project Load()
        {
            _project = new Project();

            var gameFilePath = _dataSource.GetGameFile();
            _project.GameFile = gameFilePath;

            var stream = _dataSource.GetData(gameFilePath);
            XElement reader = XElement.Load(stream);
            stream.Dispose();

            XAttribute nameAttr = reader.Attribute("name");
            if (nameAttr != null) _project.Name = nameAttr.Value;

            XAttribute authAttr = reader.Attribute("author");
            if (authAttr != null) _project.Author = authAttr.Value;

            XElement sizeNode = reader.Element("Size");
            if (sizeNode != null)
            {
                _project.ScreenWidth = sizeNode.TryAttribute<int>("x");
                _project.ScreenHeight = sizeNode.TryAttribute<int>("y");
            }

            XElement nsfNode = reader.Element("NSF");
            if (nsfNode != null)
            {
                LoadNSFInfo(nsfNode);
            }

            XElement stagesNode = reader.Element("Stages");
            if (stagesNode != null)
            {
                foreach (XElement stageNode in stagesNode.Elements("Stage"))
                {
                    var info = new StageLinkInfo();
                    info.Name = stageNode.RequireAttribute("name").Value;
                    info.StagePath = FilePath.FromRelative(stageNode.RequireAttribute("path").Value, _project.BaseDir);

                    var winNode = stageNode.Element("Win");
                    if (winNode != null)
                    {
                        var winHandlerNode = winNode.Element("Next");
                        if (winHandlerNode != null)
                        {
                            info.WinHandler = _transferReader.Load(winHandlerNode);
                        }
                    }

                    var loseNode = stageNode.Element("Lose");
                    if (loseNode != null)
                    {
                        var loseHandlerNode = loseNode.Element("Next");
                        if (loseHandlerNode != null)
                        {
                            info.LoseHandler = _transferReader.Load(loseHandlerNode);
                        }
                    }

                    _project.AddStage(info);
                }
            }

            XElement startNode = reader.Element("Next");
            if (startNode != null)
            {
                _project.StartHandler = _transferReader.Load(startNode);
            }

            _project.AddIncludeFiles(reader.Elements("Include")
                .Select(e => e.Value)
                .Where(v => !string.IsNullOrEmpty(v.Trim())));

            _project.AddIncludeFolders(reader.Elements("IncludeFolder")
                .Select(e => e.Value)
                .Where(v => !string.IsNullOrEmpty(v.Trim())));

            var includeReader = new IncludeFileXmlReader();

            var includedFilesFromFolders = _project.IncludeFolders.SelectMany(_dataSource.GetFilesInFolder);
            var allIncludedFiles = _project.IncludeFiles.ToList()
                .Concat(includedFilesFromFolders)
                .Distinct().ToList();
            foreach (var includePath in allIncludedFiles)
            {
                var includeStream = _dataSource.GetData(includePath);
                includeReader.LoadIncludedFile(_project, includePath, includeStream, _dataSource);
                includeStream.Dispose();
            }

            stream.Close();

            return _project;
        }

        private void LoadNSFInfo(XElement nsfNode)
        {
            XElement musicNode = nsfNode.Element("Music");
            if (musicNode != null)
            {
                _project.MusicNSF = FilePath.FromRelative(musicNode.Value, _project.BaseDir);
            }

            XElement sfxNode = nsfNode.Element("SFX");
            if (sfxNode != null)
            {
                _project.EffectsNSF = FilePath.FromRelative(sfxNode.Value, _project.BaseDir);
            }
        }
    }
}
