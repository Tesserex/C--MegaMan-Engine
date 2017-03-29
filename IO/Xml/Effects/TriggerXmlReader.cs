using System.Xml.Linq;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Effects
{
    internal class TriggerXmlReader
    {
        private readonly EffectXmlReader _effectReader;

        public TriggerXmlReader(EffectXmlReader effectReader)
        {
            _effectReader = effectReader;
        }

        public TriggerInfo Load(XElement triggerNode)
        {
            string conditionString;
            if (triggerNode.Attribute("condition") != null)
                conditionString = triggerNode.GetAttribute<string>("condition");
            else
                conditionString = triggerNode.Element("Condition").Value;

            var effectNode = triggerNode.Element("Effect");
            var effect = _effectReader.Load(effectNode);

            var elseNode = triggerNode.Element("Else");
            var elseEffect = (elseNode != null) ? _effectReader.Load(elseNode) : null;

            var priority = triggerNode.TryAttribute<int?>("priority");

            return new TriggerInfo() {
                Condition = conditionString,
                Effect = effect,
                Else = elseEffect,
                Priority = priority
            };
        }
    }
}
