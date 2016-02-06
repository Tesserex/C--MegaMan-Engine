using System;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Entities
{
    internal interface IComponentXmlReader
    {
        string NodeName { get; }
        IComponentInfo Load(XElement node, Project project);
    }
}
