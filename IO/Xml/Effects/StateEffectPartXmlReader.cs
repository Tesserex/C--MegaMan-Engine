using System;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class StateEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "State";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new StateEffectPartInfo() {
                Name = partNode.Value
            };
        }
    }
}
