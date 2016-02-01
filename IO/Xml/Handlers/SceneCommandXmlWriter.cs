using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers
{
    internal class SceneCommandXmlWriter
    {
        private readonly SceneBindingXmlWriter _bindingWriter;
        private readonly EntityPlacementXmlWriter _entityWriter;
        private readonly SoundXmlWriter _soundWriter;
        private readonly HandlerTransferXmlWriter _transferWriter;

        public SceneCommandXmlWriter(SceneBindingXmlWriter bindingWriter, EntityPlacementXmlWriter entityWriter, SoundXmlWriter soundWriter, HandlerTransferXmlWriter transferWriter)
        {
            _bindingWriter = bindingWriter;
            _entityWriter = entityWriter;
            _soundWriter = soundWriter;
            _transferWriter = transferWriter;
        }

        public void Write(SceneCommandInfo info, XmlWriter writer)
        {

        }

        public void WritePlayCommand(ScenePlayCommandInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("PlayMusic");
            writer.WriteAttributeString("track", info.Track.ToString());
            writer.WriteEndElement();
        }

        public void WriteStopCommand(SceneStopMusicCommandInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("StopMusic");
            writer.WriteEndElement();
        }

        public void WriteAddCommand(SceneAddCommandInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("Add");
            if (!string.IsNullOrEmpty(info.Name)) writer.WriteAttributeString("name", info.Name);
            writer.WriteAttributeString("object", info.Object);
            writer.WriteAttributeString("x", info.X.ToString());
            writer.WriteAttributeString("y", info.Y.ToString());
            writer.WriteEndElement();
        }

        public void WriteRemoveCommand(SceneRemoveCommandInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("Remove");
            writer.WriteAttributeString("name", info.Name);
            writer.WriteEndElement();
        }

        public void WriteEntityCommand(SceneEntityCommandInfo info, XmlWriter writer)
        {
            _entityWriter.Write(info.Placement, writer);
        }

        public void WriteTextCommand(SceneTextCommandInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("Text");
            if (!string.IsNullOrEmpty("Font")) writer.WriteAttributeString("font", info.Font);
            if (!string.IsNullOrEmpty(info.Name)) writer.WriteAttributeString("name", info.Name);
            writer.WriteAttributeString("content", info.Content);
            if (info.Speed != null) writer.WriteAttributeString("speed", info.Speed.Value.ToString());
            writer.WriteAttributeString("x", info.X.ToString());
            writer.WriteAttributeString("y", info.Y.ToString());

            if (info.Binding != null)
                _bindingWriter.Write(info.Binding, writer);

            writer.WriteEndElement();
        }

        public void WriteFillCommand(SceneFillCommandInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("Fill");
            if (!string.IsNullOrEmpty(info.Name)) writer.WriteAttributeString("name", info.Name);
            writer.WriteAttributeString("color", info.Red.ToString() + "," + info.Green.ToString() + "," + info.Blue.ToString());
            writer.WriteAttributeString("x", info.X.ToString());
            writer.WriteAttributeString("y", info.Y.ToString());
            writer.WriteAttributeString("width", info.Width.ToString());
            writer.WriteAttributeString("height", info.Height.ToString());
            writer.WriteAttributeString("layer", info.Layer.ToString());
            writer.WriteEndElement();
        }

        public void WriteFillMoveCommand(SceneFillMoveCommandInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("FillMove");
            if (!string.IsNullOrEmpty(info.Name)) writer.WriteAttributeString("name", info.Name);
            writer.WriteAttributeString("x", info.X.ToString());
            writer.WriteAttributeString("y", info.Y.ToString());
            writer.WriteAttributeString("width", info.Width.ToString());
            writer.WriteAttributeString("height", info.Height.ToString());
            writer.WriteAttributeString("duration", info.Duration.ToString());
            writer.WriteEndElement();
        }

        public void WriteMoveCommand(SceneMoveCommandInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("Move");
            if (!string.IsNullOrEmpty(info.Name)) writer.WriteAttributeString("name", info.Name);
            writer.WriteAttributeString("x", info.X.ToString());
            writer.WriteAttributeString("y", info.Y.ToString());
            if (info.Duration > 0) writer.WriteAttributeString("duration", info.Duration.ToString());
            writer.WriteEndElement();
        }

        public void WriteMenuOptionCommand(MenuOptionCommandInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("Option");

            if (info.Name != null)
                writer.WriteAttributeString("name", info.Name);

            writer.WriteAttributeString("x", info.X.ToString());
            writer.WriteAttributeString("y", info.Y.ToString());

            if (info.OnEvent != null)
            {
                writer.WriteStartElement("On");
                foreach (var cmd in info.OnEvent)
                {
                    Write(cmd, writer);
                }
                writer.WriteEndElement();
            }

            if (info.OffEvent != null)
            {
                writer.WriteStartElement("Off");
                foreach (var cmd in info.OffEvent)
                {
                    Write(cmd, writer);
                }
                writer.WriteEndElement();
            }

            if (info.SelectEvent != null)
            {
                writer.WriteStartElement("Select");
                foreach (var cmd in info.SelectEvent)
                {
                    Write(cmd, writer);
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public void WriteSoundCommand(SceneSoundCommandInfo info, XmlWriter writer)
        {
            _soundWriter.Write(info.SoundInfo, writer);
        }

        public void WriteNextCommand(SceneNextCommandInfo info, XmlWriter writer)
        {
            _transferWriter.Write(info.NextHandler, writer);
        }

        public void WriteCallCommand(SceneCallCommandInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("Call");
            writer.WriteValue(info.Name);
            writer.WriteEndElement();
        }

        public void WriteEffectCommand(SceneEffectCommandInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("Effect");
            if (info.EntityId != null)
                writer.WriteAttributeString("entity", info.EntityId);

            throw new System.Exception("Can't write scene effects right now.");
            writer.WriteEndElement();
        }

        public void WriteConditionCommand(SceneConditionCommandInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("Condition");

            if (info.ConditionEntity != null)
                writer.WriteAttributeString("entity", info.ConditionEntity);

            writer.WriteAttributeString("condition", info.ConditionExpression);

            foreach (var cmd in info.Commands)
            {
                Write(cmd, writer);
            }

            writer.WriteEndElement();
        }

        public void WriteWaitCommand(SceneWaitCommandInfo info, XmlWriter writer)
        {
            writer.WriteElementString("WaitForInput", "");
        }

        public void WriteAutoscrollCommand(SceneAutoscrollCommandInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("Autoscroll");

            writer.WriteAttributeString("speed", info.Speed.ToString());
            writer.WriteAttributeString("startX", info.StartX.ToString());

            writer.WriteEndElement();
        }
    }
}
