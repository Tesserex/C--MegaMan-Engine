using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal interface ICommandXmlReader
    {
        IEnumerable<string> NodeName { get; }
        SceneCommandInfo Load(XElement node, string basePath);
    }
}
