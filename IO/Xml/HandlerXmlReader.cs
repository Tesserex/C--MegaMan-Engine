using MegaMan.Common;
using MegaMan.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MegaMan.IO.Xml
{
    public class HandlerXmlReader
    {
        protected static void LoadHandlerBase(HandlerInfo handler, XElement node, string basePath)
        {
            handler.Name = node.RequireAttribute("name").Value;

            foreach (var spriteNode in node.Elements("Sprite"))
            {
                var info = new HandlerSpriteInfo();
                info.Sprite = Sprite.FromXml(spriteNode, basePath);
                handler.Objects.Add(info.Name, info);
            }

            foreach (var meterNode in node.Elements("Meter"))
            {
                var meter = LoadMeter(meterNode, basePath);
                handler.Objects.Add(meter.Name, meter);
            }
        }

        public static List<SceneCommandInfo> LoadCommands(XElement node, string basePath)
        {
            var list = new List<SceneCommandInfo>();

            foreach (var cmdNode in node.Elements())
            {
                switch (cmdNode.Name.LocalName)
                {
                    case "PlayMusic":
                    case "Music":
                        list.Add(LoadPlayCommand(cmdNode, basePath));
                        break;

                    case "StopMusic":
                        list.Add(LoadStopCommand(cmdNode));
                        break;

                    case "Sprite":
                    case "Meter":
                    case "Add":
                        list.Add(LoadAddCommand(cmdNode));
                        break;

                    case "SpriteMove":
                        list.Add(LoadMoveCommand(cmdNode));
                        break;

                    case "Remove":
                        list.Add(LoadRemoveCommand(cmdNode));
                        break;

                    case "Entity":
                        list.Add(LoadEntityCommand(cmdNode));
                        break;

                    case "Text":
                        list.Add(LoadTextCommand(cmdNode));
                        break;

                    case "Fill":
                        list.Add(LoadFillCommand(cmdNode));
                        break;

                    case "FillMove":
                        list.Add(LoadFillMoveCommand(cmdNode));
                        break;

                    case "Option":
                        list.Add(LoadOptionCommand(cmdNode, basePath));
                        break;

                    case "Sound":
                        list.Add(LoadSoundCommand(cmdNode, basePath));
                        break;

                    case "Next":
                        list.Add(LoadNextCommand(cmdNode));
                        break;

                    case "Call":
                        list.Add(LoadCallCommand(cmdNode));
                        break;

                    case "Effect":
                        list.Add(LoadEffectCommand(cmdNode));
                        break;

                    case "Condition":
                        list.Add(LoadConditionCommand(cmdNode, basePath));
                        break;

                    case "WaitForInput":
                        list.Add(new SceneWaitCommandInfo());
                        break;
                }
            }

            return list;
        }

        private static ScenePlayCommandInfo LoadPlayCommand(XElement node, string basePath)
        {
            var info = new ScenePlayCommandInfo();

            info.Track = node.TryAttribute<int>("nsftrack", node.TryAttribute<int>("track"));

            XElement intro = node.Element("Intro");
            XElement loop = node.Element("Loop");
            info.IntroPath = (intro != null) ? FilePath.FromRelative(intro.Value, basePath) : null;
            info.LoopPath = (loop != null) ? FilePath.FromRelative(loop.Value, basePath) : null;

            return info;
        }

        private static SceneStopMusicCommandInfo LoadStopCommand(XElement node)
        {
            return new SceneStopMusicCommandInfo();
        }

        private static SceneAddCommandInfo LoadAddCommand(XElement node)
        {
            var info = new SceneAddCommandInfo();
            var nameAttr = node.Attribute("name");
            if (nameAttr != null) info.Name = nameAttr.Value;
            info.Object = node.RequireAttribute("object").Value;
            info.X = node.GetAttribute<int>("x");
            info.Y = node.GetAttribute<int>("y");
            return info;
        }

        public static SceneRemoveCommandInfo LoadRemoveCommand(XElement node)
        {
            var info = new SceneRemoveCommandInfo();
            info.Name = node.RequireAttribute("name").Value;
            return info;
        }

        public static SceneEntityCommandInfo LoadEntityCommand(XElement node)
        {
            var info = new SceneEntityCommandInfo();
            info.Placement = EntityPlacement.FromXml(node);
            return info;
        }

        public static SceneTextCommandInfo LoadTextCommand(XElement node)
        {
            var info = new SceneTextCommandInfo();
            info.Content = node.TryAttribute<string>("content");
            info.Name = node.TryAttribute<string>("name");
            info.Speed = node.TryAttribute<int>("speed");
            info.X = node.GetAttribute<int>("x");
            info.Y = node.GetAttribute<int>("y");

            var bindingNode = node.Element("Binding");
            if (bindingNode != null) info.Binding = SceneBindingInfo.FromXml(bindingNode);

            info.Font = node.TryAttribute<string>("font");

            return info;
        }

        public static SceneFillCommandInfo LoadFillCommand(XElement node)
        {
            var info = new SceneFillCommandInfo();
            var nameAttr = node.Attribute("name");
            if (nameAttr != null) info.Name = nameAttr.Value;
            var colorAttr = node.RequireAttribute("color");
            var color = colorAttr.Value;
            var split = color.Split(',');
            info.Red = byte.Parse(split[0]);
            info.Green = byte.Parse(split[1]);
            info.Blue = byte.Parse(split[2]);
            info.X = node.GetAttribute<int>("x");
            info.Y = node.GetAttribute<int>("y");
            info.Width = node.GetAttribute<int>("width");
            info.Height = node.GetAttribute<int>("height");
            info.Layer = node.TryAttribute<int>("layer");
            return info;
        }

        public static SceneFillMoveCommandInfo LoadFillMoveCommand(XElement node)
        {
            var info = new SceneFillMoveCommandInfo();
            info.Name = node.RequireAttribute("name").Value;
            info.Duration = node.GetAttribute<int>("duration");
            info.X = node.GetAttribute<int>("x");
            info.Y = node.GetAttribute<int>("y");
            info.Width = node.GetAttribute<int>("width");
            info.Height = node.GetAttribute<int>("height");
            return info;
        }

        public static SceneMoveCommandInfo LoadMoveCommand(XElement node)
        {
            var info = new SceneMoveCommandInfo();
            info.Name = node.RequireAttribute("name").Value;

            info.Duration = node.TryAttribute<int>("duration");

            info.X = node.GetAttribute<int>("x");
            info.Y = node.GetAttribute<int>("y");
            return info;
        }

        public static MenuOptionCommandInfo LoadOptionCommand(XElement node, string basePath)
        {
            var info = new MenuOptionCommandInfo();

            var nameAttr = node.Attribute("name");
            if (nameAttr != null)
            {
                info.Name = nameAttr.Value;
            }

            info.X = node.GetAttribute<int>("x");
            info.Y = node.GetAttribute<int>("y");

            var onNode = node.Element("On");
            if (onNode != null)
            {
                info.OnEvent = LoadCommands(onNode, basePath);
            }

            var offNode = node.Element("Off");
            if (offNode != null)
            {
                info.OffEvent = LoadCommands(offNode, basePath);
            }

            var selectNode = node.Element("Select");
            if (selectNode != null)
            {
                info.SelectEvent = LoadCommands(selectNode, basePath);
            }

            return info;
        }

        public static SceneSoundCommandInfo LoadSoundCommand(XElement node, string basePath)
        {
            var info = new SceneSoundCommandInfo();

            info.SoundInfo = IncludeFileXmlReader.LoadSound(node, basePath);

            return info;
        }

        public static SceneNextCommandInfo LoadNextCommand(XElement node)
        {
            var info = new SceneNextCommandInfo();

            info.NextHandler = HandlerTransfer.FromXml(node);

            return info;
        }

        public static SceneCallCommandInfo LoadCallCommand(XElement node)
        {
            var info = new SceneCallCommandInfo();

            info.Name = node.Value;

            return info;
        }

        public static SceneEffectCommandInfo LoadEffectCommand(XElement node)
        {
            var info = new SceneEffectCommandInfo();

            info.GeneratedName = Guid.NewGuid().ToString();

            var attr = node.Attribute("entity");
            if (attr != null)
            {
                info.EntityId = attr.Value;
            }
            info.EffectNode = node;

            return info;
        }

        public static SceneConditionCommandInfo LoadConditionCommand(XElement node, string basePath)
        {
            var info = new SceneConditionCommandInfo();

            info.ConditionExpression = node.RequireAttribute("condition").Value;

            var attr = node.Attribute("entity");
            if (attr != null)
            {
                info.ConditionEntity = attr.Value;
            }

            info.Commands = LoadCommands(node, basePath);

            return info;
        }

        private static MeterInfo LoadMeter(XElement meterNode, string basePath)
        {
            MeterInfo meter = new MeterInfo();

            meter.Name = meterNode.RequireAttribute("name").Value;

            meter.Position = new PointF(meterNode.GetAttribute<float>("x"), meterNode.GetAttribute<float>("y"));

            XAttribute imageAttr = meterNode.RequireAttribute("image");
            meter.TickImage = FilePath.FromRelative(imageAttr.Value, basePath);

            XAttribute backAttr = meterNode.Attribute("background");
            if (backAttr != null)
            {
                meter.Background = FilePath.FromRelative(backAttr.Value, basePath);
            }

            bool horiz = false;
            XAttribute dirAttr = meterNode.Attribute("orientation");
            if (dirAttr != null)
            {
                horiz = (dirAttr.Value == "horizontal");
            }
            meter.Orient = horiz ? MegaMan.Common.MeterInfo.Orientation.Horizontal : MegaMan.Common.MeterInfo.Orientation.Vertical;

            int x = meterNode.TryAttribute<int>("tickX");
            int y = meterNode.TryAttribute<int>("tickY");

            meter.TickOffset = new Point(x, y);

            XElement soundNode = meterNode.Element("Sound");
            if (soundNode != null) meter.Sound = IncludeFileXmlReader.LoadSound(soundNode, basePath);

            XElement bindingNode = meterNode.Element("Binding");
            if (bindingNode != null) meter.Binding = SceneBindingInfo.FromXml(bindingNode);

            return meter;
        }
    }
}
