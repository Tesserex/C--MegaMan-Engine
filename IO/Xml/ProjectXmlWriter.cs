using System.IO;
using System.Text;
using System.Xml;
using MegaMan.Common;
using MegaMan.IO.Xml.Handlers;

namespace MegaMan.IO.Xml
{
    internal class ProjectXmlWriter : IProjectWriter
    {
        private XmlTextWriter _writer;
        private readonly HandlerTransferXmlWriter _transferWriter;

        public ProjectXmlWriter(HandlerTransferXmlWriter transferWriter)
        {
            _transferWriter = transferWriter;
        }

        public void Save(Project project)
        {
            Directory.CreateDirectory(project.GameFile.BasePath);
            _writer = new XmlTextWriter(project.GameFile.Absolute, Encoding.Default);
            _writer.Formatting = Formatting.Indented;
            _writer.Indentation = 1;
            _writer.IndentChar = '\t';

            _writer.WriteStartElement("Game");
            if (!string.IsNullOrEmpty(project.Name)) _writer.WriteAttributeString("name", project.Name);
            if (!string.IsNullOrEmpty(project.Author)) _writer.WriteAttributeString("author", project.Author);
            _writer.WriteAttributeString("version", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

            _writer.WriteStartElement("Size");
            _writer.WriteAttributeString("x", project.ScreenWidth.ToString());
            _writer.WriteAttributeString("y", project.ScreenHeight.ToString());
            _writer.WriteEndElement();

            if (project.MusicNSF != null || project.EffectsNSF != null)
            {
                _writer.WriteStartElement("NSF");
                if (project.MusicNSF != null) _writer.WriteElementString("Music", project.MusicNSF.Relative);
                if (project.EffectsNSF != null) _writer.WriteElementString("SFX", project.EffectsNSF.Relative);
                _writer.WriteEndElement();
            }

            _writer.WriteStartElement("Stages");
            foreach (var info in project.Stages)
            {
                _writer.WriteStartElement("Stage");
                _writer.WriteAttributeString("name", info.Name);
                _writer.WriteAttributeString("path", info.StagePath.Relative);

                if (info.WinHandler != null)
                {
                    _writer.WriteStartElement("Win");
                    _transferWriter.Write(info.WinHandler, _writer);
                    _writer.WriteEndElement();
                }

                if (info.LoseHandler != null)
                {
                    _writer.WriteStartElement("Lose");
                    _transferWriter.Write(info.LoseHandler, _writer);
                    _writer.WriteEndElement();
                }

                _writer.WriteEndElement();
            }
            _writer.WriteEndElement(); // Stages

            if (project.StartHandler != null)
            {
                _transferWriter.Write(project.StartHandler, _writer);
            }

            foreach (var folder in project.IncludeFolders)
            {
                _writer.WriteElementString("IncludeFolder", folder.Relative);
            }

            foreach (var file in project.IncludeFiles)
            {
                _writer.WriteElementString("Include", file.Relative);
            }

            _writer.WriteEndElement(); // Game

            _writer.Close();
        }
    }
}
