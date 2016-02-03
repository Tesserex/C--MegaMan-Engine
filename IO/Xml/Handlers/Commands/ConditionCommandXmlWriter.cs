using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class ConditionCommandXmlWriter : ICommandXmlWriter
    {
        public Type CommandType
        {
            get
            {
                return typeof(SceneConditionCommandInfo);
            }
        }

        public void Write(SceneCommandInfo info, XmlWriter writer)
        {
            var cmd = (SceneConditionCommandInfo)info;

            writer.WriteStartElement("Condition");

            if (cmd.ConditionEntity != null)
                writer.WriteAttributeString("entity", cmd.ConditionEntity);

            writer.WriteAttributeString("condition", cmd.ConditionExpression);

            foreach (var c in cmd.Commands)
            {
                Write(c, writer);
            }

            writer.WriteEndElement();
        }
    }
}
