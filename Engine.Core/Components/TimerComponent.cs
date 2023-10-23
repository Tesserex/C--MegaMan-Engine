using System.Collections.Generic;

namespace MegaMan.Engine
{
    public class TimerComponent : Component
    {
        public Dictionary<string, int> Timers { get; private set; }

        public TimerComponent()
        {
            Timers = new Dictionary<string, int>();
        }

        public override Component Clone()
        {
            return new TimerComponent();
        }

        public override void Start(IGameplayContainer container)
        {
            Timers.Clear();
            container.GameThink += Update;
        }

        public override void Stop(IGameplayContainer container)
        {
            Timers.Clear();
            container.GameThink -= Update;
        }

        public override void Message(IGameMessage msg)
        {
            
        }

        protected override void Update()
        {
            if (Parent.Paused) return;
            var update = new Dictionary<string, int>();
            foreach (var name in Timers.Keys)
            {
                update[name] = Timers[name] + 1;
            }
            Timers = update;
        }

        public override void RegisterDependencies(Component component)
        {
            
        }

        public bool Exists(string name) { return Timers.ContainsKey(name); }

        public int Value(string name)
        {
            if (Timers.ContainsKey(name)) return Timers[name];
            return 0;
        }
    }
}
