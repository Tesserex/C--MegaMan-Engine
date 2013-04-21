using MegaMan.Common;
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

        public void LoadIncludedFile(Project project, string path)
        {
            _project = project;

            try
            {
                XDocument document = XDocument.Load(path, LoadOptions.SetLineInfo);
                foreach (XElement element in document.Elements())
                {
                    switch (element.Name.LocalName)
                    {
                        case "Sounds":
                            AddSounds(element);
                            break;

                        case "Scenes":
                            AddScenes(element);
                            break;

                        case "Scene":
                            AddScene(element);
                            break;

                        case "Menus":
                            AddMenus(element);
                            break;

                        case "Menu":
                            AddMenu(element);
                            break;
                    }
                }
            }
            catch (GameXmlException ex)
            {
                ex.File = path;
                throw;
            }
        }

        private void AddSounds(XElement node)
        {
            foreach (XElement soundNode in node.Elements("Sound"))
            {
                _project.AddSound(LoadSound(soundNode, _project.BaseDir));
            }
        }

        private void AddScenes(XElement node)
        {
            foreach (var sceneNode in node.Elements("Scene"))
            {
                AddScene(sceneNode);
            }
        }

        private void AddScene(XElement node)
        {
            var scene = SceneXmlReader.LoadScene(node, _project.BaseDir);

            _project.AddScene(scene);
        }

        private void AddMenus(XElement node)
        {
            foreach (var menuNode in node.Elements("Menu"))
            {
                AddMenu(menuNode);
            }
        }

        private void AddMenu(XElement node)
        {
            var info = MenuXmlReader.LoadMenu(node, _project.BaseDir);

            _project.AddMenu(info);
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

        public static FontInfo LoadFont(XElement node, string basePath)
        {
            var info = new FontInfo();

            info.CharWidth = node.GetAttribute<int>("charwidth");
            info.CaseSensitive = node.GetAttribute<bool>("cased");

            foreach (var lineNode in node.Elements("Line"))
            {
                var x = lineNode.GetAttribute<int>("x");
                var y = lineNode.GetAttribute<int>("y");

                var lineText = lineNode.Value;

                info.AddLine(x, y, lineText);
            }

            info.ImagePath = FilePath.FromRelative(node.RequireAttribute("image").Value, basePath);

            return info;
        }
    }
}
