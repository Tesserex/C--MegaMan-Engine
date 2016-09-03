using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class SoundCommandXmlReader : ICommandXmlReader
    {
        public IEnumerable<string> NodeName
        {
            get
            {
                yield return "Sound";
            }
        }

        public SceneCommandInfo Load(XElement node, string basePath)
        {
            var info = new SceneSoundCommandInfo();

            info.SoundInfo = new SoundInfo { Name = node.RequireAttribute("name").Value };

            return info;
        }
    }
}
