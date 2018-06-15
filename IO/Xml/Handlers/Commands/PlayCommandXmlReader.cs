using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class PlayCommandXmlReader : ICommandXmlReader
    {
        public IEnumerable<string> NodeName { get { yield return "PlayMusic"; } }

        public SceneCommandInfo Load(XElement node, string basePath)
        {
            var info = new ScenePlayCommandInfo();

            info.Track = node.TryAttribute<int>("nsftrack", node.TryAttribute<int>("track"));

            var intro = node.Element("Intro");
            var loop = node.Element("Loop");
            info.IntroPath = (intro != null) ? FilePath.FromRelative(intro.Value, basePath) : null;
            info.LoopPath = (loop != null) ? FilePath.FromRelative(loop.Value, basePath) : null;

            return info;
        }
    }
}
