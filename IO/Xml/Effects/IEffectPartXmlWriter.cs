using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal interface IEffectPartXmlWriter
    {
        Type EffectPartType { get; }
        void Write(IEffectPartInfo info, XmlWriter writer);
    }
}
