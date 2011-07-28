using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.Engine
{
    [System.Diagnostics.DebuggerDisplay("Parent = {Parent.Name}, State: {currentState}, State Time: {StateFrames}")]
    public class StateComponent : Component
    {
        private string currentState;
        private Dictionary<string, State> states;

        public int StateFrames { get; private set; }
        public int Lifetime { get; private set; }
        public double FrameRand { get; private set; }

        public int StateTime
        {
            get { return StateFrames; }
        }

        public event Action<string> StateChanged;

        public StateComponent()
        {
            states = new Dictionary<string, State>();

            currentState = "Start";
        }

        public override Component Clone()
        {
            StateComponent newone = new StateComponent {states = this.states};

            // notice the shallow copy!

            return newone;
        }

        public override void Start()
        {
            Engine.Instance.GameThink += Update;
            if (states.ContainsKey(currentState)) states[currentState].Initialize(Parent);
        }

        public override void Stop()
        {
            Engine.Instance.GameThink -= Update;
        }

        public override void Message(IGameMessage msg)
        {
            StateMessage statemsg = msg as StateMessage;
            if (statemsg != null)
            {
                if (states.ContainsKey(statemsg.StateName))
                {
                    currentState = statemsg.StateName;
                    states[currentState].Initialize(Parent);
                    StateFrames = 0;
                }
            }
        }

        protected override void Update()
        {
            if (Parent.Paused) return;

            Lifetime++;

            if (!states.ContainsKey(currentState)) return;

            StateFrames++;
            FrameRand = Program.rand.NextDouble();

            string old = currentState;
            states[currentState].CheckTriggers(this, Parent);
            if (old != currentState)
            {
                if (!states.ContainsKey(currentState)) throw new GameEntityException("Entity \"" + Parent.Name + "\" tried to change to state \"" + currentState + "\", which does not exist.");
                states[currentState].Initialize(Parent);
                StateFrames = 0;

                if (StateChanged != null) StateChanged(currentState);
            }
        }

        public override void RegisterDependencies(Component component)
        {
        }

        public override void LoadXml(XElement stateNode)
        {
            string name = stateNode.RequireAttribute("name").Value;

            State state;
            if (states.ContainsKey(name))
            {
                state = states[name];
            }
            else
            {
                state = new State {Name = name};
                states[name] = state;
            }

            foreach (XElement child in stateNode.Elements())
            {
                switch (child.Name.LocalName)
                {
                    case "Trigger":
                        AddStateTrigger(state, child);
                        break;

                    default:
                        // make sure the entity has the component we're dealing with
                        // if it's not a component name it will just return null safely
                        Parent.GetOrCreateComponent(child.Name.LocalName);

                        if (child.Attribute("mode") != null && child.RequireAttribute("mode").Value.ToUpper() == "REPEAT") state.AddLogic(EffectParser.LoadXmlEffect(child));
                        else state.AddInitial(EffectParser.LoadXmlEffect(child));
                        break;
                }
            }
        }

        public void LoadStateTrigger(XElement trigger)
        {
            XElement statesNode = trigger.Element("States");

            if (statesNode != null)
            {
                string statesString = statesNode.Value;
                string[] statesArray = statesString.Split(',');

                foreach (string stateString in statesArray)
                {
                    string stateName = stateString.Trim();
                    if (!states.ContainsKey(stateName))
                    {
                        State state = new State {Name = stateName};
                        states.Add(stateName, state);
                    }
                    AddStateTrigger(states[stateName], trigger);
                }
            }
            else
            {
                foreach (State state in states.Values)
                {
                    AddStateTrigger(state, trigger);
                }
            }
        }

        private void AddStateTrigger(State state, XElement triggerNode)
        {
            try
            {
                string conditionString;
                if (triggerNode.Attribute("condition") != null) conditionString = triggerNode.RequireAttribute("condition").Value;
                else conditionString = triggerNode.Element("Condition").Value;

                Condition condition = EffectParser.ParseCondition(conditionString);

                Effect effect = EffectParser.LoadTriggerEffect(triggerNode.Element("Effect"));
                state.AddTrigger(condition, effect);
            }
            catch (Exception e)
            {
                throw new GameXmlException(triggerNode, "There was an error parsing a trigger. There may be a syntax error in your condition expression.\n\nThe error message was:\n\n\t" + e.Message);
            }
        }

        // this is for when <State> appears in an effect, used for changing state
        public static Effect ParseEffect(XElement effectNode)
        {
            string newstate = effectNode.Value;
            return entity =>
            {
                StateComponent state = entity.GetComponent<StateComponent>();
                if (state != null) state.currentState = newstate;
            };
        }

        private class Trigger
        {
            public Condition Condition;
            public Effect Effect;
        }

        private class State
        {
            public string Name { get; set; }

            private readonly List<Trigger> triggers = new List<Trigger>();
            private Effect initializer = entity => { };
            private Effect logic = entity => { };

            public void Initialize(GameEntity entity)
            {
                initializer(entity);
            }

            public void AddInitial(Effect func)
            {
                initializer += func;
            }

            public void AddLogic(Effect func)
            {
                logic += func;
            }

            public void AddTrigger(Condition cond, Effect effect)
            {
                Trigger trigger = new Trigger {Condition = cond, Effect = effect};
                triggers.Add(trigger);
            }

            public void CheckTriggers(StateComponent statecomp, GameEntity entity)
            {
                string state = statecomp.currentState;
                foreach (Trigger trigger in triggers)
                {
                    bool result = trigger.Condition(entity);
                    if (result)
                    {
                        trigger.Effect(entity);
                        if (statecomp.currentState != state) break;
                    }
                }
                statecomp.states[statecomp.currentState].logic(entity);
            }
        }
    }
}
