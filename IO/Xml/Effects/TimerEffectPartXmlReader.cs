using System;
using System.Linq;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class TimerEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Timer";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new TimerEffectPartInfo() {
                Start = partNode.Elements("Start").Select(s => s.Value).ToList(),
                Reset = partNode.Elements("Reset").Select(s => s.Value).ToList(),
                Delete = partNode.Elements("Delete").Select(s => s.Value).ToList()
            };
        }
    }
}
