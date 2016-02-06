using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands {
    internal interface ICommandXmlWriter {
        Type CommandType { get; }
        void Write(SceneCommandInfo info, XmlWriter writer);
    }
}
