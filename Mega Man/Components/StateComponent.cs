using System;
using System.Collections.Generic;
using System.Linq;
using MegaMan.Common.Entities;
using MegaMan.Engine.Entities;

namespace MegaMan.Engine
{
    [System.Diagnostics.DebuggerDisplay("Parent = {Parent.Name}, State: {currentState}, State Time: {StateFrames}")]
    public class StateComponent : Component
    {
        private string currentState;
        private bool stateChanged;
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
        }

        public override Component Clone()
        {
            StateComponent newone = new StateComponent {states = this.states};

            // notice the shallow copy!

            return newone;
        }

        public override void Start(IGameplayContainer container)
        {
            currentState = "Start";
            stateChanged = false;
            StateFrames = 0;
            Lifetime = 0;
            container.GameThink += Update;
            if (states.ContainsKey(currentState)) states[currentState].Initialize(Parent);
        }

        public override void Stop(IGameplayContainer container)
        {
            StateFrames = 0;
            Lifetime = 0;
            container.GameThink -= Update;
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

            states[currentState].CheckTriggers(this, Parent);
            states[currentState].RunLogic(Parent);

            if (stateChanged)
            {
                states[currentState].Initialize(Parent);
                stateChanged = false;

                if (StateChanged != null) StateChanged(currentState);
            }
        }

        public void ChangeState(string stateName)
        {
            if (!states.ContainsKey(currentState)) throw new GameRunException("Entity \"" + Parent.Name + "\" tried to change to state \"" + currentState + "\", which does not exist.");

            currentState = stateName;
            StateFrames = 0;
            stateChanged = true;
        }

        public override void RegisterDependencies(Component component)
        {
        }

        internal void LoadInfo(StateComponentInfo componentInfo)
        {
            foreach (var stateInfo in componentInfo.States)
            {
                var state = new State() { Name = stateInfo.Name };
                foreach (var trigger in stateInfo.Triggers)
                {
                    state.AddTrigger(ParseTrigger(trigger));
                }

                state.SetInitial(EffectParser.LoadTriggerEffect(stateInfo.Initializer));
                state.SetLogic(EffectParser.LoadTriggerEffect(stateInfo.Logic));

                states[state.Name] = state;
            }

            foreach (var triggerInfo in componentInfo.Triggers)
            {
                var trigger = ParseTrigger(triggerInfo.Trigger);

                if (triggerInfo.States != null)
                {
                    foreach (var stateName in triggerInfo.States)
                    {
                        if (!states.ContainsKey(stateName))
                        {
                            State state = new State {Name = stateName};
                            states.Add(stateName, state);
                        }
                        states[stateName].AddTrigger(trigger);
                    }
                }
                else
                {
                    foreach (var state in states.Values)
                    {
                        state.AddTrigger(trigger);
                    }
                }
            }
        }
        
        private Trigger ParseTrigger(TriggerInfo info)
        {
            try
            {
                var condition = EffectParser.ParseCondition(info.Condition);
                var effect = EffectParser.LoadTriggerEffect(info.Effect);
                var elseEffect = (info.Else != null) ? EffectParser.LoadTriggerEffect(info.Else) : null;
                return new Trigger { Condition = condition, Effect = effect, Else = elseEffect, ConditionString = info.Condition, Priority = info.Priority ?? 0 };
            }
            catch (Exception e)
            {
                throw new GameRunException("There was an error parsing a trigger. There may be a syntax error in your condition expression.\n\nThe error message was:\n\n\t" + e.Message);
            }
        }

        private class Trigger
        {
            public string ConditionString;
            public string EffectString;
            public Condition Condition;
            public Effect Effect;
            public Effect Else;
            public int Priority;
        }

        private class State
        {
            public string Name { get; set; }

            private readonly List<Trigger> triggers = new List<Trigger>();
            private Effect initializer = entity => { };
            private Effect logic = entity => { };

            public void Initialize(IEntity entity)
            {
                initializer(entity);
            }

            public void RunLogic(IEntity entity)
            {
                logic(entity);
            }

            public void SetInitial(Effect effect)
            {
                initializer = effect;
            }

            public void AddInitial(Effect func)
            {
                initializer += func;
            }

            public void SetLogic(Effect effect)
            {
                logic = effect;
            }

            public void AddLogic(Effect func)
            {
                logic += func;
            }

            public void AddTrigger(Trigger trigger)
            {
                triggers.Add(trigger);
            }

            public void CheckTriggers(StateComponent statecomp, IEntity entity)
            {
                string state = statecomp.currentState;
                foreach (Trigger trigger in triggers.OrderBy(t => t.Priority))
                {
                    bool result = trigger.Condition(entity);
                    if (result)
                    {
                        trigger.Effect(entity);
                        if (statecomp.currentState != state)
                            break;
                    }
                    else if (trigger.Else != null)
                    {
                        trigger.Else(entity);
                        if (statecomp.currentState != state)
                            break;
                    }
                }
            }
        }
    }
}
