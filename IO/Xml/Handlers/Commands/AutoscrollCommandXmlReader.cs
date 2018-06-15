using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class AutoscrollCommandXmlReader : ICommandXmlReader
    {
        public IEnumerable<string> NodeName
        {
            get
            {
                yield return "Autoscroll";
            }
        }

        public SceneCommandInfo Load(XElement node, string basePath)
        {
            var info = new SceneAutoscrollCommandInfo();

            info.Speed = node.TryAttribute<double>("speed", 1);
            info.StartX = node.TryAttribute("startX", 128);

            return info;
        }
    }
}
