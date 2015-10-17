using System;
using System.IO;
using System.Text;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml
{
    internal class ProjectXmlWriter : IProjectWriter
    {
        private XmlTextWriter _writer;

        public ProjectXmlWriter()
        {
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
                    WriteHandlerTransfer(info.WinHandler);
                    _writer.WriteEndElement();
                }

                if (info.LoseHandler != null)
                {
                    _writer.WriteStartElement("Lose");
                    WriteHandlerTransfer(info.LoseHandler);
                    _writer.WriteEndElement();
                }

                _writer.WriteEndElement();
            }
            _writer.WriteEndElement(); // Stages

            if (project.StartHandler != null)
            {
                WriteHandlerTransfer(project.StartHandler);
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

        private void WriteHandlerTransfer(HandlerTransfer handlerTransfer)
        {
            _writer.WriteStartElement("Next");

            if (handlerTransfer.Mode != HandlerMode.Next)
            {
                _writer.WriteAttributeString("mode", handlerTransfer.Mode.ToString());
            }

            if (handlerTransfer.Mode == HandlerMode.Push)
            {
                _writer.WriteAttributeString("pause", handlerTransfer.Pause.ToString());
            }

            if (handlerTransfer.Mode != HandlerMode.Pop)
            {
                _writer.WriteAttributeString("type", Enum.GetName(typeof(HandlerType), handlerTransfer.Type));
                _writer.WriteAttributeString("name", handlerTransfer.Name);
            }

            _writer.WriteAttributeString("fade", handlerTransfer.Fade.ToString());

            _writer.WriteEndElement();
        }
    }
}
