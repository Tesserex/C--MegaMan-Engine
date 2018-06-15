using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class HandlerCommandXmlReader
    {
        public List<SceneCommandInfo> LoadCommands(XElement node, string basePath)
        {
            return node.Elements().Select(x => Load(x, basePath)).Where(x => x != null).ToList();
        }

        public SceneCommandInfo Load(XElement node, string basePath)
        {
            if (!readers.ContainsKey(node.Name.LocalName))
                return null;

            var reader = readers[node.Name.LocalName];
            return reader.Load(node, basePath);
        }

        private static Dictionary<string, ICommandXmlReader> readers;

        static HandlerCommandXmlReader()
        {
            readers = Extensions.GetImplementersOf<ICommandXmlReader>()
                .SelectMany(x => x.NodeName.Select(n => new { NodeName = n, Reader = x }))
                .ToDictionary(x => x.NodeName, x => x.Reader);
        }
    }
}
