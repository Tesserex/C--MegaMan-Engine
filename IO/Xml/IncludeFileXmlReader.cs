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
            var scene = LoadScene(node, _project.BaseDir);

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
            var info = LoadMenu(node, _project.BaseDir);

            _project.AddMenu(info);
        }

        private void LoadHandlerBase(HandlerInfo handler, XElement node, string basePath)
        {
            handler.Name = node.RequireAttribute("name").Value;

            foreach (var spriteNode in node.Elements("Sprite"))
            {
                var sprite = HandlerSpriteInfo.FromXml(spriteNode, basePath);
                handler.Objects.Add(sprite.Name, sprite);
            }

            foreach (var meterNode in node.Elements("Meter"))
            {
                var meter = MeterInfo.FromXml(meterNode, basePath);
                handler.Objects.Add(meter.Name, meter);
            }
        }

        public SceneInfo LoadScene(XElement node, string basePath)
        {
            var scene = new SceneInfo();

            LoadHandlerBase(scene, node, basePath);

            scene.Duration = node.GetAttribute<int>("duration");

            scene.CanSkip = node.TryAttribute<bool>("canskip");

            foreach (var keyNode in node.Elements("Keyframe"))
            {
                scene.KeyFrames.Add(KeyFrameInfo.FromXml(keyNode, basePath));
            }

            var transferNode = node.Element("Next");
            if (transferNode != null)
            {
                scene.NextHandler = HandlerTransfer.FromXml(transferNode);
            }

            return scene;
        }

        public MenuInfo LoadMenu(XElement node, string basePath)
        {
            var menu = new MenuInfo();

            LoadHandlerBase(menu, node, basePath);

            foreach (var keyNode in node.Elements("State"))
            {
                menu.States.Add(MenuStateInfo.FromXml(keyNode, basePath));
            }

            return menu;
        }

        public SoundInfo LoadSound(XElement soundNode, string basePath)
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
    }
}
