using System.Collections.Generic;
using System.Linq;
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
            Effect effect = e => { };

            effect = node.Elements("Start")
                .Select(createNode => createNode.Value)
                .Aggregate(effect, (current, timerName) => current + (entity =>
                {
                    string name = timerName;
                    TimerComponent timer = entity.GetComponent<TimerComponent>();
                    if (timer != null)
                        timer.timers[name] = 0;
                }));

            effect = node.Elements("Reset")
                .Select(resetNode => resetNode.Value)
                .Aggregate(effect, (current, timerName) => current + (entity =>
                {
                    string name = timerName;
                    TimerComponent timer = entity.GetComponent<TimerComponent>();
                    if (timer != null && timer.timers.ContainsKey(name))
                        timer.timers[name] = 0;
                }));

            return node.Elements("Delete")
                .Select(deleteNode => deleteNode.Value)
                .Aggregate(effect, (current, timerName) => current + (entity =>
                {
                    TimerComponent timer = entity.GetComponent<TimerComponent>();
                    if (timer != null)
                        timer.timers.Remove(timerName);
                }));
        }
    }
}
