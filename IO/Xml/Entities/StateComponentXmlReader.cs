using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Entities.Effects;
using MegaMan.IO.Xml.Effects;

namespace MegaMan.IO.Xml.Entities
{
    internal class StateComponentXmlReader : IComponentXmlReader
    {
        private readonly TriggerXmlReader _triggerReader;
        private readonly EffectXmlReader _effectReader;

        public StateComponentXmlReader(TriggerXmlReader triggerReader, EffectXmlReader effectReader)
        {
            _triggerReader = triggerReader;
            _effectReader = effectReader;
        }

        public string NodeName
        {
            get { return null; }
        }

        public IComponentInfo Load(XElement node, Project project)
        {
            var comp = new StateComponentInfo();
            foreach (var state in node.Elements("State"))
            {
                var stateInfo = ReadState(state);
                comp.States.Add(stateInfo);
            }

            foreach (var triggerInfo in node.Elements("Trigger"))
            {
                var statesNode = triggerInfo.Element("States");
                var states = statesNode != null ? statesNode.Value.Split(',').Select(s => s.Trim()).ToList() : null;

                var trigger = _triggerReader.Load(triggerInfo);

                if (trigger.Priority == null)
                    trigger.Priority = ((IXmlLineInfo)triggerInfo).LineNumber;

                comp.Triggers.Add(new MultiStateTriggerInfo() {
                    States = states,
                    Trigger = trigger
                });
            }

            return comp;
        }

        private StateInfo ReadState(XElement stateNode)
        {
            var info = new StateInfo();
            info.Name = stateNode.RequireAttribute("name").Value;

            var logic = new List<IEffectPartInfo>();
            var init = new List<IEffectPartInfo>();

            foreach (var child in stateNode.Elements())
            {
                switch (child.Name.LocalName)
                {
                    case "Trigger":
                        var t = _triggerReader.Load(child);

                        if (t.Priority == null)
                            t.Priority = ((IXmlLineInfo)child).LineNumber;

                        info.Triggers.Add(t);
                        break;

                    case "Initialize":
                        init.AddRange(child.Elements().Select(e => _effectReader.LoadPart(e)));
                        break;

                    case "Logic":
                        logic.AddRange(child.Elements().Select(e => _effectReader.LoadPart(e)));
                        break;

                    default:
                        var mode = child.TryAttribute<string>("mode");
                        if (mode != null && mode.ToUpper() == "REPEAT")
                            logic.Add(_effectReader.LoadPart(child));
                        else
                            init.Add(_effectReader.LoadPart(child));
                        break;
                }
            }

            info.Initializer = new EffectInfo() { Parts = init };
            info.Logic = new EffectInfo() { Parts = logic };

            return info;
        }
    }
}
