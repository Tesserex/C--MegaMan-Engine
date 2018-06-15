using System.IO;
using System.Text;
using System.Xml;
using MegaMan.Common;
using MegaMan.IO.Xml.Handlers;

namespace MegaMan.IO.Xml
{
    internal class ProjectXmlWriter : IProjectWriter
    {
        private XmlTextWriter writer;
        private readonly HandlerTransferXmlWriter transferWriter;

        public ProjectXmlWriter(HandlerTransferXmlWriter transferWriter)
        {
            this.transferWriter = transferWriter;
        }

        public void Save(Project project)
        {
            Directory.CreateDirectory(project.GameFile.BasePath);
            writer = new XmlTextWriter(project.GameFile.Absolute, Encoding.Default);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';

            writer.WriteStartElement("Game");
            if (!string.IsNullOrEmpty(project.Name)) writer.WriteAttributeString("name", project.Name);
            if (!string.IsNullOrEmpty(project.Author)) writer.WriteAttributeString("author", project.Author);
            writer.WriteAttributeString("version", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

            writer.WriteStartElement("Size");
            writer.WriteAttributeString("x", project.ScreenWidth.ToString());
            writer.WriteAttributeString("y", project.ScreenHeight.ToString());
            writer.WriteEndElement();

            if (project.MusicNSF != null || project.EffectsNSF != null)
            {
                writer.WriteStartElement("NSF");
                if (project.MusicNSF != null) writer.WriteElementString("Music", project.MusicNSF.Relative);
                if (project.EffectsNSF != null) writer.WriteElementString("SFX", project.EffectsNSF.Relative);
                writer.WriteEndElement();
            }

            writer.WriteStartElement("Stages");
            foreach (var info in project.Stages)
            {
                writer.WriteStartElement("Stage");
                writer.WriteAttributeString("name", info.Name);
                writer.WriteAttributeString("path", info.StagePath.Relative);

                if (info.WinHandler != null)
                {
                    writer.WriteStartElement("Win");
                    transferWriter.Write(info.WinHandler, writer);
                    writer.WriteEndElement();
                }

                if (info.LoseHandler != null)
                {
                    writer.WriteStartElement("Lose");
                    transferWriter.Write(info.LoseHandler, writer);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // Stages

            if (project.StartHandler != null)
            {
                transferWriter.Write(project.StartHandler, writer);
            }

            foreach (var folder in project.IncludeFolders)
            {
                writer.WriteElementString("IncludeFolder", folder.Relative);
            }

            foreach (var file in project.IncludeFiles)
            {
                writer.WriteElementString("Include", file.Relative);
            }

            writer.WriteEndElement(); // Game

            writer.Close();
        }
    }
}
