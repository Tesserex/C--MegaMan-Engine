using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class NextCommandXmlReader : ICommandXmlReader
    {
        private readonly HandlerTransferXmlReader transferReader;

        public NextCommandXmlReader(HandlerTransferXmlReader transferReader)
        {
            this.transferReader = transferReader;
        }

        public IEnumerable<string> NodeName
        {
            get
            {
                yield return "Next";
            }
        }

        public SceneCommandInfo Load(XElement node, string basePath)
        {
            var info = new SceneNextCommandInfo();

            info.NextHandler = transferReader.Load(node);

            return info;
        }
    }
}
