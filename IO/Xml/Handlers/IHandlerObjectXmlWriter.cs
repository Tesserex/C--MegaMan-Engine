using System;
using System.Xml;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.IO.Xml.Handlers
{
    internal interface IHandlerObjectXmlWriter
    {
        Type ObjectType { get; }
        void Write(IHandlerObjectInfo info, XmlWriter writer);
    }
}
