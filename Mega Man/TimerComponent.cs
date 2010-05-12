using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Mega_Man
{
    public class TimerComponent : Component
    {
        private Dictionary<string, int> timers = new Dictionary<string,int>();

        public override Component Clone()
        {
            return new TimerComponent();
        }

        public override void Start()
        {
            Engine.Instance.GameThink += Update;
        }

        public override void Stop()
        {
            Engine.Instance.GameThink -= Update;
        }

        public override void Message(IGameMessage msg)
        {
            
        }

        protected override void Update()
        {
            if (Parent.Paused) return;
            Dictionary<string, int> update = new Dictionary<string, int>();
            foreach (string name in timers.Keys)
            {
                update[name] = timers[name] + 1;
            }
            timers = update;
        }

        public override void RegisterDependencies(Component component)
        {
            
        }

        public bool Exists(string name) { return timers.ContainsKey(name); }

        public int Value(string name)
        {
            if (timers.ContainsKey(name)) return timers[name];
            return 0;
        }

        public override void LoadXml(XElement xmlNode)
        {
            // nothing needed
        }

        public override Effect ParseEffect(XElement node)
        {
            Effect effect = (e) => { };

            foreach (XElement createNode in node.Elements("Start"))
            {
                string timerName = createNode.Value;
                effect += (entity) =>
                {
                    string name = timerName;
                    TimerComponent timer = (TimerComponent)entity.GetComponent(typeof(TimerComponent));
                    if (timer != null) timer.timers[name] = 0;
                };
            }

            foreach (XElement resetNode in node.Elements("Reset"))
            {
                string timerName = resetNode.Value;
                effect += (entity) =>
                {
                    string name = timerName;
                    TimerComponent timer = (TimerComponent)entity.GetComponent(typeof(TimerComponent));
                    if (timer != null && timer.timers.ContainsKey(name)) timer.timers[name] = 0;
                };
            }

            foreach (XElement deleteNode in node.Elements("Delete"))
            {
                string timerName = deleteNode.Value;
                effect += (entity) =>
                {
                    string name = timerName;
                    TimerComponent timer = (TimerComponent)entity.GetComponent(typeof(TimerComponent));
                    if (timer != null) timer.timers.Remove(timerName);
                };
            }

            return effect;
        }
    }
}
