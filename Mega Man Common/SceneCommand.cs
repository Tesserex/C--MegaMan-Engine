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
        StopMusic,
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
        Effect,
        Condition,
        WaitForInput
    }

    public abstract class SceneCommandInfo
    {
        public abstract SceneCommands Type { get; }
        public abstract void Save(XmlTextWriter writer);
    }

    public class ScenePlayCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.PlayMusic; } }
        public int Track { get; set; }
        public FilePath IntroPath { get; set; }
        public FilePath LoopPath { get; set; }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("PlayMusic");
            writer.WriteAttributeString("track", Track.ToString());
            writer.WriteEndElement();
        }
    }

    public class SceneStopMusicCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.StopMusic; } }
        public int Track { get; set; }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("StopMusic");
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
        public EntityPlacement Placement { get; set; }

        public override void Save(XmlTextWriter writer)
        {
            this.Placement.Save(writer);
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

        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public List<SceneCommandInfo> OnEvent { get; set; }
        public List<SceneCommandInfo> OffEvent { get; set; }
        public List<SceneCommandInfo> SelectEvent { get; set; }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Option");

            if (Name != null)
            {
                writer.WriteAttributeString("name", Name);
            }

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

        public SoundInfo SoundInfo { get; set; }

        public override void Save(XmlTextWriter writer)
        {
            SoundInfo.Save(writer);
        }
    }

    public class SceneNextCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Next; } }

        public HandlerTransfer NextHandler { get; set; }

        public override void Save(XmlTextWriter writer)
        {
            NextHandler.Save(writer);
        }
    }

    public class SceneCallCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Call; } }

        public string Name { get; set; }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Call");
            writer.WriteValue(this.Name);
            writer.WriteEndElement();
        }
    }

    public class SceneEffectCommandInfo : SceneCommandInfo
    {
        public string GeneratedName { get; set; }
        public string EntityId { get; set; }
        public XElement EffectNode { get; set; }

        public override SceneCommands Type
        {
            get { return SceneCommands.Effect; }
        }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Effect");
            if (EntityId != null)
            {
                writer.WriteAttributeString("entity", EntityId);
            }
            EffectNode.WriteTo(writer);
            writer.WriteEndElement();
        }
    }

    public class SceneConditionCommandInfo : SceneCommandInfo
    {
        public string ConditionExpression { get; set; }
        public string ConditionEntity { get; set; }
        public List<SceneCommandInfo> Commands { get; set; }

        public override SceneCommands Type
        {
            get { return SceneCommands.Condition; }
        }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Condition");

            if (ConditionEntity != null)
            {
                writer.WriteAttributeString("entity", ConditionEntity);
            }

            writer.WriteAttributeString("condition", ConditionExpression);

            foreach (var cmd in Commands)
            {
                cmd.Save(writer);
            }

            writer.WriteEndElement();
        }
    }

    public class SceneWaitCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type
        {
            get { return SceneCommands.WaitForInput; }
        }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteElementString("WaitForInput", "");
        }
    }

    public class SceneBindingInfo
    {
        public string Source { get; set; }
        public string Target { get; set; }

        public void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Bind");
            writer.WriteAttributeString("source", Source);
            writer.WriteAttributeString("target", Target);
        }
    }
}
