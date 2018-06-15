using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal interface IEffectPartXmlReader
    {
        string NodeName { get; }
        IEffectPartInfo Load(XElement partNode);
    }
}
