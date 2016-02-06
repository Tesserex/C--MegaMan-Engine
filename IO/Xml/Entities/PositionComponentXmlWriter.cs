using System;
using System.Xml;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Entities
{
    internal class PositionComponentXmlWriter : IComponentXmlWriter
    {
        public Type ComponentType
        {
            get { return typeof(PositionComponentInfo); }
        }

        public void Write(IComponentInfo info, XmlWriter writer)
        {
            var pos = (PositionComponentInfo)info;

            if (pos.PersistOffscreen)
            {
                writer.WriteStartElement("Position");
                writer.WriteAttributeString("persistoffscreen", pos.PersistOffscreen.ToString());
                writer.WriteEndElement();
            }
        }
    }
}
