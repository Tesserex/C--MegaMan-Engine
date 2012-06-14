using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace MegaMan.Common
{
    public enum SceneCommands
    {
        PlayMusic,
        Add,
        Move,
        Remove,
        Entity,
        Text,
        Fill,
        FillMove,
        Option,
        Sound,
        Next,
        Call,
        Effect
    }

    public abstract class SceneCommandInfo
    {
        public abstract SceneCommands Type { get; }
        public abstract void Save(XmlTextWriter writer);

        public static List<SceneCommandInfo> Load(XElement node, string basePath)
        {
            var list = new List<SceneCommandInfo>();

            foreach (var cmdNode in node.Elements())
            {
                switch (cmdNode.Name.LocalName)
                {
                    case "PlayMusic":
                        list.Add(ScenePlayCommandInfo.FromXml(cmdNode));
                        break;

                    case "Sprite":
                    case "Meter":
                    case "Add":
                        list.Add(SceneAddCommandInfo.FromXml(cmdNode));
                        break;

                    case "SpriteMove":
                        list.Add(SceneMoveCommandInfo.FromXml(cmdNode));
                        break;

                    case "Remove":
                        list.Add(SceneRemoveCommandInfo.FromXml(cmdNode));
                        break;

                    case "Entity":
                        list.Add(SceneEntityCommandInfo.FromXml(cmdNode));
                        break;

                    case "Text":
                        list.Add(SceneTextCommandInfo.FromXml(cmdNode));
                        break;

                    case "Fill":
                        list.Add(SceneFillCommandInfo.FromXml(cmdNode));
                        break;

                    case "FillMove":
                        list.Add(SceneFillMoveCommandInfo.FromXml(cmdNode));
                        break;

                    case "Option":
                        list.Add(MenuOptionCommandInfo.FromXml(cmdNode, basePath));
                        break;

                    case "Sound":
                        list.Add(SceneSoundCommandInfo.FromXml(cmdNode, basePath));
                        break;

                    case "Next":
                        list.Add(SceneNextCommandInfo.FromXml(cmdNode));
                        break;

                    case "Call":
                        list.Add(SceneCallCommandInfo.FromXml(cmdNode));
                        break;

                    case "Effect":
                        list.Add(SceneEffectCommandInfo.FromXml(cmdNode));
                        break;
                }
            }

            return list;
        }
    }

    public class ScenePlayCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.PlayMusic; } }
        public int Track { get; set; }

        public static ScenePlayCommandInfo FromXml(XElement node)
        {
            var info = new ScenePlayCommandInfo();
            info.Track = node.GetInteger("track");
            return info;
        }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("PlayMusic");
            writer.WriteAttributeString("track", Track.ToString());
            writer.WriteEndElement();
        }
    }

    public class SceneAddCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Add; } }
        public string Name { get; set; }
        public string Object { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public static SceneAddCommandInfo FromXml(XElement node)
        {
            var info = new SceneAddCommandInfo();
            var nameAttr = node.Attribute("name");
            if (nameAttr != null) info.Name = nameAttr.Value;
            info.Object = node.RequireAttribute("object").Value;
            info.X = node.GetInteger("x");
            info.Y = node.GetInteger("y");
            return info;
        }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Add");
            if (!string.IsNullOrEmpty(Name)) writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("object", Object);
            writer.WriteAttributeString("x", X.ToString());
            writer.WriteAttributeString("y", Y.ToString());
            writer.WriteEndElement();
        }
    }

    public class SceneRemoveCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Remove; } }
        public string Name { get; set; }

        public static SceneRemoveCommandInfo FromXml(XElement node)
        {
            var info = new SceneRemoveCommandInfo();
            info.Name = node.RequireAttribute("name").Value;
            return info;
        }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Remove");
            writer.WriteAttributeString("name", Name);
            writer.WriteEndElement();
        }
    }

    public class SceneEntityCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Entity; } }
        public string Name { get; set; }
        public string Entity { get; set; }
        public string State { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public static SceneEntityCommandInfo FromXml(XElement node)
        {
            var info = new SceneEntityCommandInfo();
            info.Entity = node.RequireAttribute("entity").Value;
            var nameAttr = node.Attribute("name");
            if (nameAttr != null) info.Name = nameAttr.Value;
            var stateAttr = node.Attribute("state");
            if (stateAttr != null) info.State = stateAttr.Value;
            info.X = node.GetInteger("x");
            info.Y = node.GetInteger("y");
            return info;
        }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Entity");
            if (!string.IsNullOrEmpty(Name)) writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("entity", Entity);
            if (!string.IsNullOrEmpty(State)) writer.WriteAttributeString("state", State);
            writer.WriteAttributeString("x", X.ToString());
            writer.WriteAttributeString("y", Y.ToString());
            writer.WriteEndElement();
        }
    }

    public class SceneTextCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Text; } }
        public string Name { get; set; }
        public string Content { get; set; }
        public SceneBindingInfo Binding { get; set; }
        public int? Speed { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Font { get; set; }

        public static SceneTextCommandInfo FromXml(XElement node)
        {
            var info = new SceneTextCommandInfo();
            var contentAttr = node.Attribute("content");
            if (contentAttr != null)
            {
                info.Content = contentAttr.Value;
            }
            var nameAttr = node.Attribute("name");
            if (nameAttr != null) info.Name = nameAttr.Value;
            int speed;
            if (node.TryInteger("speed", out speed)) info.Speed = speed;
            info.X = node.GetInteger("x");
            info.Y = node.GetInteger("y");
            var bindingNode = node.Element("Binding");
            if (bindingNode != null) info.Binding = SceneBindingInfo.FromXml(bindingNode);

            var fontAttr = node.Attribute("font");
            if (fontAttr != null)
            {
                info.Font = fontAttr.Value;
            }
            return info;
        }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Text");
            if (!string.IsNullOrEmpty("Font")) writer.WriteAttributeString("font", Font);
            if (!string.IsNullOrEmpty(Name)) writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("content", Content);
            if (Speed != null) writer.WriteAttributeString("speed", Speed.Value.ToString());
            writer.WriteAttributeString("x", X.ToString());
            writer.WriteAttributeString("y", Y.ToString());
            if (Binding != null) Binding.Save(writer);
            writer.WriteEndElement();
        }
    }

    public class SceneFillCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Fill; } }
        public string Name { get; set; }
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Layer { get; set; }

        public static SceneFillCommandInfo FromXml(XElement node)
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
            info.X = node.GetInteger("x");
            info.Y = node.GetInteger("y");
            info.Width = node.GetInteger("width");
            info.Height = node.GetInteger("height");
            int layer = 1;
            if (node.TryInteger("layer", out layer)) info.Layer = layer;
            return info;
        }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Fill");
            if (!string.IsNullOrEmpty(Name)) writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("color", Red.ToString() + "," + Green.ToString() + "," + Blue.ToString());
            writer.WriteAttributeString("x", X.ToString());
            writer.WriteAttributeString("y", Y.ToString());
            writer.WriteAttributeString("width", Width.ToString());
            writer.WriteAttributeString("height", Height.ToString());
            writer.WriteAttributeString("layer", Layer.ToString());
            writer.WriteEndElement();
        }
    }

    public class SceneFillMoveCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.FillMove; } }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Duration { get; set; }

        public static SceneFillMoveCommandInfo FromXml(XElement node)
        {
            var info = new SceneFillMoveCommandInfo();
            info.Name = node.RequireAttribute("name").Value;
            info.Duration = node.GetInteger("duration");
            info.X = node.GetInteger("x");
            info.Y = node.GetInteger("y");
            info.Width = node.GetInteger("width");
            info.Height = node.GetInteger("height");
            return info;
        }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("FillMove");
            if (!string.IsNullOrEmpty(Name)) writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("x", X.ToString());
            writer.WriteAttributeString("y", Y.ToString());
            writer.WriteAttributeString("width", Width.ToString());
            writer.WriteAttributeString("height", Height.ToString());
            writer.WriteAttributeString("duration", Duration.ToString());
            writer.WriteEndElement();
        }
    }

    public class SceneMoveCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Move; } }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Duration { get; set; }

        public static SceneMoveCommandInfo FromXml(XElement node)
        {
            var info = new SceneMoveCommandInfo();
            info.Name = node.RequireAttribute("name").Value;

            int d = 0;
            node.TryInteger("duration", out d);
            info.Duration = d;

            info.X = node.GetInteger("x");
            info.Y = node.GetInteger("y");
            return info;
        }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Move");
            if (!string.IsNullOrEmpty(Name)) writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("x", X.ToString());
            writer.WriteAttributeString("y", Y.ToString());
            if (Duration > 0) writer.WriteAttributeString("duration", Duration.ToString());
            writer.WriteEndElement();
        }
    }

    public class MenuOptionCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Option; } }

        public int X { get; set; }
        public int Y { get; set; }

        public List<SceneCommandInfo> OnEvent { get; private set; }
        public List<SceneCommandInfo> OffEvent { get; private set; }
        public List<SceneCommandInfo> SelectEvent { get; private set; }

        public static MenuOptionCommandInfo FromXml(XElement node, string basePath)
        {
            var info = new MenuOptionCommandInfo();
            info.X = node.GetInteger("x");
            info.Y = node.GetInteger("y");

            var onNode = node.Element("On");
            if (onNode != null)
            {
                info.OnEvent = SceneCommandInfo.Load(onNode, basePath);
            }

            var offNode = node.Element("Off");
            if (offNode != null)
            {
                info.OffEvent = SceneCommandInfo.Load(offNode, basePath);
            }

            var selectNode = node.Element("Select");
            if (selectNode != null)
            {
                info.SelectEvent = SceneCommandInfo.Load(selectNode, basePath);
            }

            return info;
        }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Option");
            writer.WriteAttributeString("x", X.ToString());
            writer.WriteAttributeString("y", Y.ToString());

            if (OnEvent != null)
            {
                writer.WriteStartElement("On");
                foreach (var cmd in OnEvent)
                {
                    cmd.Save(writer);
                }
                writer.WriteEndElement();
            }

            if (OffEvent != null)
            {
                writer.WriteStartElement("Off");
                foreach (var cmd in OffEvent)
                {
                    cmd.Save(writer);
                }
                writer.WriteEndElement();
            }

            if (SelectEvent != null)
            {
                writer.WriteStartElement("Select");
                foreach (var cmd in SelectEvent)
                {
                    cmd.Save(writer);
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }

    public class SceneSoundCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Sound; } }

        public SoundInfo SoundInfo { get; private set; }

        public static SceneSoundCommandInfo FromXml(XElement node, string basePath)
        {
            var info = new SceneSoundCommandInfo();

            info.SoundInfo = SoundInfo.FromXml(node, basePath);

            return info;
        }

        public override void Save(XmlTextWriter writer)
        {
            SoundInfo.Save(writer);
        }
    }

    public class SceneNextCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Next; } }

        public HandlerTransfer NextHandler { get; private set; }

        public static SceneNextCommandInfo FromXml(XElement node)
        {
            var info = new SceneNextCommandInfo();

            info.NextHandler = HandlerTransfer.FromXml(node);

            return info;
        }

        public override void Save(XmlTextWriter writer)
        {
            NextHandler.Save(writer);
        }
    }

    public class SceneCallCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Call; } }

        public string Name { get; private set; }

        public static SceneCallCommandInfo FromXml(XElement node)
        {
            var info = new SceneCallCommandInfo();

            info.Name = node.Value;

            return info;
        }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Call");
            writer.WriteValue(this.Name);
            writer.WriteEndElement();
        }
    }

    public class SceneEffectCommandInfo : SceneCommandInfo
    {
        public string GeneratedName { get; private set; }
        public string EntityName { get; private set; }
        public XElement EffectNode { get; private set; }

        public static SceneEffectCommandInfo FromXml(XElement node)
        {
            var info = new SceneEffectCommandInfo();

            info.GeneratedName = Guid.NewGuid().ToString();
            info.EntityName = node.RequireAttribute("entity").Value;
            info.EffectNode = node;

            return info;
        }

        public override SceneCommands Type
        {
            get { return SceneCommands.Effect; }
        }

        public override void Save(XmlTextWriter writer)
        {
            throw new NotImplementedException();
        }
    }

    public class SceneBindingInfo
    {
        public string Source { get; set; }
        public string Target { get; set; }

        public static SceneBindingInfo FromXml(XElement node)
        {
            var info = new SceneBindingInfo();
            info.Source = node.RequireAttribute("source").Value;
            info.Target = node.RequireAttribute("target").Value;
            return info;
        }

        public void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Bind");
            writer.WriteAttributeString("source", Source);
            writer.WriteAttributeString("target", Target);
        }
    }
}
