using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml
{
    internal class SceneBindingXmlReader
    {
        public SceneBindingInfo Load(XElement node)
        {
            var info = new SceneBindingInfo();
            info.Source = node.RequireAttribute("source").Value;
            info.Target = node.RequireAttribute("target").Value;
            return info;
        }
    }
}
