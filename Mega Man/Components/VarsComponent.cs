using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.Engine
{
    class VarsComponent : Component
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

        public override void LoadXml(XElement xmlNode)
        {
        }
    }
}
