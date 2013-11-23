using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace MegaMan.IO.Xml
{
    public class ProjectXmlWriter
    {
        private Project _project;
        private XmlTextWriter _writer;

        public ProjectXmlWriter(Project project)
        {
            this._project = project;
        }

        public void Write()
        {
            Directory.CreateDirectory(_project.GameFile.BasePath);
            _writer = new XmlTextWriter(_project.GameFile.Absolute, Encoding.Default);
            _writer.Formatting = Formatting.Indented;
            _writer.Indentation = 1;
            _writer.IndentChar = '\t';

            _writer.WriteStartElement("Game");
            if (!string.IsNullOrEmpty(_project.Name)) _writer.WriteAttributeString("name", _project.Name);
            if (!string.IsNullOrEmpty(_project.Author)) _writer.WriteAttributeString("author", _project.Author);
            _writer.WriteAttributeString("version", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

            _writer.WriteStartElement("Size");
            _writer.WriteAttributeString("x", _project.ScreenWidth.ToString());
            _writer.WriteAttributeString("y", _project.ScreenHeight.ToString());
            _writer.WriteEndElement();

            if (_project.MusicNSF != null || _project.EffectsNSF != null)
            {
                _writer.WriteStartElement("NSF");
                if (_project.MusicNSF != null) _writer.WriteElementString("Music", _project.MusicNSF.Relative);
                if (_project.EffectsNSF != null) _writer.WriteElementString("SFX", _project.EffectsNSF.Relative);
                _writer.WriteEndElement();
            }

            _writer.WriteStartElement("Stages");
            foreach (var info in _project.Stages)
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

            if (_project.StartHandler != null)
            {
                WriteHandlerTransfer(_project.StartHandler);
            }

            foreach (string folder in _project.IncludeFolders)
            {
                _writer.WriteElementString("IncludeFolder", folder);
            }

            foreach (string file in _project.IncludeFiles)
            {
                _writer.WriteElementString("Include", file);
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
