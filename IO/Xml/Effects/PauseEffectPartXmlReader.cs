using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class PauseEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Pause";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new PauseEffectPartInfo();
        }
    }
}
