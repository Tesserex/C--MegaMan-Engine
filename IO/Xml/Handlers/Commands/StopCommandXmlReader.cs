using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class StopCommandXmlReader : ICommandXmlReader
    {
        public IEnumerable<string> NodeName
        {
            get
            {
                yield return "StopMusic";
            }
        }

        public SceneCommandInfo Load(XElement node, string basePath)
        {
            return new SceneStopMusicCommandInfo();
        }
    }
}
