using System;
using System.Collections.Generic;

namespace MegaMan.Engine
{
    public class VarsComponent : Component
    {
        private Dictionary<string, string> _vars = new Dictionary<string,string>();

        public string Get(string name)
        {
            return _vars.ContainsKey(name) ? _vars[name] : String.Empty;
        }

        public void Set(string name, string value)
        {
            _vars[name] = value;
        }

        public override Component Clone()
        {
            return new VarsComponent();
        }

        public override void Start(IGameplayContainer container)
        {
            _vars.Clear();
        }

        public override void Stop(IGameplayContainer container)
        {
            _vars.Clear();
        }

        public override void Message(IGameMessage msg)
        {   
        }

        protected override void Update()
        {
        }

        public override void RegisterDependencies(Component component)
        {
        }
    }
}
