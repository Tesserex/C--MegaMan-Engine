using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml;

namespace MegaMan.Common
{
    public class StageInfo
    {
        public string Name { get; set; }
        public FilePath StagePath { get; set; }
        public HandlerTransfer WinHandler { get; set; }
        public HandlerTransfer LoseHandler { get; set; }
    }

    public class Project
    {
        #region Game XML File Stuff

        private List<StageSelect> stageSelects = new List<StageSelect>();

        private List<StageInfo> stages = new List<StageInfo>();
        
        private List<string> includeFiles = new List<string>();

        public IEnumerable<StageInfo> Stages
        {
            get { return stages.AsReadOnly(); }
        }

        public IEnumerable<StageSelect> StageSelects
        {
            get { return stageSelects.AsReadOnly(); }
        }

        public void AddStage(StageInfo stage)
        {
            this.stages.Add(stage);
        }

        public void AddStageSelect(StageSelect select)
        {
            stageSelects.Add(select);
        }

        public IEnumerable<string> Includes
        {
            get { return includeFiles; }
        }

        public string Name
        {
            get;
            set;
        }

        public string Author
        {
            get;
            set;
        }

        public string GameFile { get; private set; }
        public string BaseDir { get; private set; }

        public int ScreenWidth
        {
            get;
            set;
        }
        public int ScreenHeight
        {
            get;
            set;
        }

        public FilePath MusicNSF { get; set; }
        public FilePath EffectsNSF { get; set; }

        public PauseScreen PauseScreen { get; set; }

        public HandlerTransfer StartHandler { get; set; }

        #endregion

        public Project()
        {
            // sensible defaults where possible
            ScreenWidth = 256;
            ScreenHeight = 224;
        }

        public void Load(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("The project file does not exist: " + path);

            GameFile = path;
            BaseDir = Path.GetDirectoryName(path);
            XElement reader = XElement.Load(path);

            XAttribute nameAttr = reader.Attribute("name");
            if (nameAttr != null) this.Name = nameAttr.Value;

            XAttribute authAttr = reader.Attribute("author");
            if (authAttr != null) this.Author = authAttr.Value;

            XElement sizeNode = reader.Element("Size");
            if (sizeNode != null)
            {
                int across, down;
                if (int.TryParse(sizeNode.Attribute("x").Value, out across))
                {
                    ScreenWidth = across;
                }
                else ScreenWidth = 0;

                if (int.TryParse(sizeNode.Attribute("y").Value, out down))
                {
                    ScreenHeight = down;
                }
                else ScreenHeight = 0;
            }

            XElement nsfNode = reader.Element("NSF");
            if (nsfNode != null) LoadNSFInfo(nsfNode);

            XElement stagesNode = reader.Element("Stages");
            if (stagesNode != null)
            {
                foreach (XElement stageNode in stagesNode.Elements("Stage"))
                {
                    var info = new StageInfo();
                    info.Name = stageNode.RequireAttribute("name").Value;
                    info.StagePath = FilePath.FromRelative(stageNode.RequireAttribute("path").Value, this.BaseDir);

                    var winNode = stageNode.Element("Win");
                    if (winNode != null)
                    {
                        var winHandlerNode = winNode.Element("Next");
                        if (winHandlerNode != null)
                        {
                            info.WinHandler = HandlerTransfer.FromXml(winHandlerNode);
                        }
                    }

                    var loseNode = stageNode.Element("Lose");
                    if (loseNode != null)
                    {
                        var loseHandlerNode = loseNode.Element("Next");
                        if (loseHandlerNode != null)
                        {
                            info.LoseHandler = HandlerTransfer.FromXml(loseHandlerNode);
                        }
                    }

                    stages.Add(info);
                }
            }

            XElement startNode = reader.Element("Next");
            if (startNode != null)
            {
                StartHandler = HandlerTransfer.FromXml(startNode);
            }

            foreach (var stageSelectNode in reader.Elements("StageSelect"))
            {
                stageSelects.Add(new StageSelect(stageSelectNode, this.BaseDir));
            }

            XElement pauseNode = reader.Element("PauseScreen");
            if (pauseNode != null) PauseScreen = new PauseScreen(pauseNode, this.BaseDir);

            foreach (XElement entityNode in reader.Elements("Entities"))
            {
                if (!string.IsNullOrEmpty(entityNode.Value.Trim())) includeFiles.Add(entityNode.Value);
            }
            foreach (XElement entityNode in reader.Elements("Include"))
            {
                if (!string.IsNullOrEmpty(entityNode.Value.Trim())) includeFiles.Add(entityNode.Value);
            }
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(this.GameFile)) return;

            XmlTextWriter writer = new XmlTextWriter(this.GameFile, Encoding.Default);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';

            writer.WriteStartElement("Game");
            if (!string.IsNullOrEmpty(this.Name)) writer.WriteAttributeString("name", this.Name);
            if (!string.IsNullOrEmpty(this.Author)) writer.WriteAttributeString("author", this.Author);
            writer.WriteAttributeString("version", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

            writer.WriteStartElement("Size");
            writer.WriteAttributeString("x", this.ScreenWidth.ToString());
            writer.WriteAttributeString("y", this.ScreenHeight.ToString());
            writer.WriteEndElement();

            if (MusicNSF != null || EffectsNSF != null)
            {
                writer.WriteStartElement("NSF");
                if (MusicNSF != null) writer.WriteElementString("Music", MusicNSF.Relative);
                if (EffectsNSF != null) writer.WriteElementString("SFX", EffectsNSF.Relative);
                writer.WriteEndElement();
            }

            writer.WriteStartElement("Stages");
            foreach (var info in stages)
            {
                writer.WriteStartElement("Stage");
                writer.WriteAttributeString("name", info.Name);
                writer.WriteAttributeString("path", info.StagePath.Relative);

                if (info.WinHandler != null)
                {
                    writer.WriteStartElement("Win");
                    info.WinHandler.Save(writer);
                    writer.WriteEndElement();
                }

                if (info.LoseHandler != null)
                {
                    writer.WriteStartElement("Lose");
                    info.LoseHandler.Save(writer);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // Stages

            if (StartHandler != null) StartHandler.Save(writer);

            foreach (var select in stageSelects)
            {
                select.Save(writer);
            }

            if (this.PauseScreen != null) this.PauseScreen.Save(writer);

            foreach (string entityFile in this.includeFiles)
            {
                writer.WriteElementString("Include", entityFile);
            }

            writer.WriteEndElement(); // Game

            writer.Close();
        }

        private void LoadNSFInfo(XElement nsfNode)
        {
            XElement musicNode = nsfNode.Element("Music");
            if (musicNode != null)
            {
                MusicNSF = FilePath.FromRelative(musicNode.Value, this.BaseDir);
            }

            XElement sfxNode = nsfNode.Element("SFX");
            if (sfxNode != null)
            {
                EffectsNSF = FilePath.FromRelative(sfxNode.Value, this.BaseDir);
            }
        }
    }
}
