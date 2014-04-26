using MegaMan.Common;
using MegaMan.IO.Xml.Includes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MegaMan.IO.Xml
{
    public class IncludeFileXmlReader
    {
        private Project _project;

        private Dictionary<string, IIncludeXmlReader> _readers = new Dictionary<string, IIncludeXmlReader>();

        public IncludeFileXmlReader()
        {
            _readers["Sounds"] = new SoundXmlReader();
            _readers["Scene"] = new SceneXmlReader();
            _readers["Scenes"] = new ScenesXmlReader(new SceneXmlReader());
            _readers["Menu"] = new MenuXmlReader();
            _readers["Menus"] = new MenusXmlReader(new MenuXmlReader());
            _readers["Font"] = new FontXmlReader();
            _readers["Fonts"] = new FontsXmlReader(new FontXmlReader());
        }

        public void LoadIncludedFile(Project project, string filePath)
        {
            _project = project;

            try
            {
                XDocument document = XDocument.Load(filePath, LoadOptions.SetLineInfo);
                foreach (XElement element in document.Elements())
                {
                    if (_readers.ContainsKey(element.Name.LocalName))
                    {
                        _readers[element.Name.LocalName].Load(project, element);
                    }

                    switch (element.Name.LocalName)
                    {
                        case "Palettes":
                            LoadPalettes(element);
                            break;
                    }
                }
            }
            catch (GameXmlException ex)
            {
                ex.File = filePath;
                throw;
            }
        }

        public static SoundInfo LoadSound(XElement soundNode, string basePath)
        {
            SoundInfo sound = new SoundInfo { Name = soundNode.RequireAttribute("name").Value };

            sound.Loop = soundNode.TryAttribute<bool>("loop");

            sound.Volume = soundNode.TryAttribute<float>("volume", 1);

            XAttribute pathattr = soundNode.Attribute("path");
            XAttribute trackAttr = soundNode.Attribute("track");
            if (pathattr != null)
            {
                sound.Type = AudioType.Wav;
                sound.Path = FilePath.FromRelative(pathattr.Value, basePath);
            }
            else if (trackAttr != null)
            {
                sound.Type = AudioType.NSF;

                int track;
                if (!trackAttr.Value.TryParse(out track) || track <= 0) throw new GameXmlException(trackAttr, "Sound track attribute must be an integer greater than zero.");
                sound.NsfTrack = track;

                sound.Priority = soundNode.TryAttribute<byte>("priority", 100);
            }
            else
            {
                sound.Type = AudioType.Unknown;
            }

            return sound;
        }

        private void LoadPalettes(XElement parentNode)
        {
            foreach (var node in parentNode.Elements("Palette"))
            {
                var palette = PaletteFromXml(node);

                _project.AddPalette(palette);
            }
        }

        private PaletteInfo PaletteFromXml(XElement node)
        {
            var palette = new PaletteInfo();

            var imagePathRelative = node.RequireAttribute("image").Value;
            palette.ImagePath = FilePath.FromRelative(imagePathRelative, _project.BaseDir);
            palette.Name = node.RequireAttribute("name").Value;

            return palette;
        }
    }
}
