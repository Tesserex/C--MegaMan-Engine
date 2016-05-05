using System;
using System.Linq;
using System.Xml;
using MegaMan.Common.Entities;
using MegaMan.IO.Xml.Effects;

namespace MegaMan.IO.Xml.Entities
{
    internal class StateComponentXmlWriter : IComponentXmlWriter
    {
        private readonly TriggerXmlWriter _triggerWriter;

        public Type ComponentType { get { return typeof(StateComponentInfo); } }

        public void Write(IComponentInfo info, XmlWriter writer)
        {
            var stateComponent = (StateComponentInfo)info;

            foreach (var state in stateComponent.States)
                WriteState(state, writer);

            foreach (var trigger in stateComponent.Triggers.OrderBy(t => t.Trigger.Priority))
                _triggerWriter.WriteMulti(trigger, writer);
        }

        private void WriteState(StateInfo state, XmlWriter writer)
        {
            writer.WriteStartElement("State");
            writer.WriteAttributeString("name", state.Name);

            foreach (var trigger in state.Triggers.OrderBy(t => t.Priority))
                _triggerWriter.Write(trigger, writer);

            writer.WriteEndElement();
        }
    }
}
