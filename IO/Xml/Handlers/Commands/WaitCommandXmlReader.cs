using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class WaitCommandXmlReader : ICommandXmlReader
    {
        public IEnumerable<string> NodeName
        {
            get
            {
                yield return "WaitForInput";
            }
        }

        public SceneCommandInfo Load(XElement node, string basePath)
        {
            return new SceneWaitCommandInfo();
        }
    }
}
