using System;
using System.Xml;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Entities
{
    internal interface IComponentXmlWriter
    {
        Type ComponentType { get; }
        void Write(IComponentInfo info, XmlWriter writer);
    }
}
