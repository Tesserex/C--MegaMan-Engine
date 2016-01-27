using System;
using System.Xml;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Entities
{
    internal class InputComponentXmlWriter : IComponentXmlWriter
    {
        public Type ComponentType
        {
            get { return typeof(InputComponentInfo); }
        }

        public void Write(IComponentInfo info, XmlWriter writer)
        {
            writer.WriteElementString("Input", "");
        }
    }
}
