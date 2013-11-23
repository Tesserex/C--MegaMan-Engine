using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MegaMan.IO.Xml
{
    public class ProjectXmlReader : GameXmlReader
    {
        private Project _project;

        public Project FromXml(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("The project file does not exist: " + path);

            _project = new Project();

            _project.GameFile = FilePath.FromAbsolute(path, Path.GetDirectoryName(path));

            XElement reader = XElement.Load(path);

            XAttribute nameAttr = reader.Attribute("name");
            if (nameAttr != null) _project.Name = nameAttr.Value;

            XAttribute authAttr = reader.Attribute("author");
            if (authAttr != null) _project.Author = authAttr.Value;

            XElement sizeNode = reader.Element("Size");
            if (sizeNode != null)
            {
                int across, down;
                if (int.TryParse(sizeNode.Attribute("x").Value, out across))
                {
                    _project.ScreenWidth = across;
                }
                else
                {
                    _project.ScreenWidth = 0;
                }

                if (int.TryParse(sizeNode.Attribute("y").Value, out down))
                {
                    _project.ScreenHeight = down;
                }
                else
                {
                    _project.ScreenHeight = 0;
                }
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
                            info.WinHandler = LoadHandlerTransfer(winHandlerNode);
                        }
                    }

                    var loseNode = stageNode.Element("Lose");
                    if (loseNode != null)
                    {
                        var loseHandlerNode = loseNode.Element("Next");
                        if (loseHandlerNode != null)
                        {
                            info.LoseHandler = LoadHandlerTransfer(loseHandlerNode);
                        }
                    }

                    _project.AddStage(info);
                }
            }

            XElement startNode = reader.Element("Next");
            if (startNode != null)
            {
                _project.StartHandler = LoadHandlerTransfer(startNode);
            }

            _project.AddIncludeFiles(reader.Elements("Include")
                .Select(e => e.Value)
                .Where(v => !string.IsNullOrEmpty(v.Trim())));

            _project.AddIncludeFolders(reader.Elements("IncludeFolder")
                .Select(e => e.Value)
                .Where(v => !string.IsNullOrEmpty(v.Trim())));

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
