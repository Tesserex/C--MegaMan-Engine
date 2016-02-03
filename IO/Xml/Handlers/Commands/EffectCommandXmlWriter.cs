using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class EffectCommandXmlWriter : ICommandXmlWriter
    {
        public Type CommandType
        {
            get
            {
                return typeof(SceneEffectCommandInfo);
            }
        }

        public void Write(SceneCommandInfo info, XmlWriter writer)
        {
            var effect = (SceneEffectCommandInfo)info;

            writer.WriteStartElement("Effect");
            if (effect.EntityId != null)
                writer.WriteAttributeString("entity", effect.EntityId);

            throw new System.Exception("Can't write scene effects right now.");
            writer.WriteEndElement();
        }
    }
}
