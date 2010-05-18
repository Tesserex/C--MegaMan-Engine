using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Xml.Linq;

namespace Mega_Man
{
    public delegate bool SplitCondition(
        PositionComponent pos,
        MovementComponent mov,
        SpriteComponent spr,
        InputComponent inp,
        CollisionComponent col,
        LadderComponent lad,
        TimerComponent timer,
        HealthComponent health,
        WeaponComponent weapon,
        int statetime,
        int lifetime,
        float playerdx,
        float playerdy,
        bool gravflip,
        double random
    );

    public delegate void SplitEffect(
        PositionComponent pos,
        MovementComponent mov,
        SpriteComponent spr,
        InputComponent inp,
        CollisionComponent col,
        LadderComponent lad,
        TimerComponent timer,
        HealthComponent health,
        StateComponent state,
        WeaponComponent weapon
    );

    public delegate bool Condition(GameEntity entity);
    public delegate void Effect(GameEntity entity);

    [System.Diagnostics.DebuggerDisplay("Parent = {Parent.Name}, State: {CurrentState}, State Time: {stateframes}")]
    public class StateComponent : Component
    {
        public string CurrentState { get; private set; }
        private Dictionary<string, State> states;

        private int stateframes = 0;
        private int lifetime = 0;
        private double framerand;

        private Dictionary<string, object> dirDict;

        private ParameterExpression posParam, moveParam, sprParam, inputParam, collParam, stateParam, weaponParam,
            ladderParam, timerParam, stParam, lifeParam, healthParam, playerXParam, playerYParam, gravParam, randParam;

        public int StateTime
        {
            get { return stateframes; }
        }

        public event Action<string> StateChanged;

        public StateComponent()
        {
            states = new Dictionary<string, State>();
            posParam = Expression.Parameter(typeof(PositionComponent), "Position");
            moveParam = Expression.Parameter(typeof(MovementComponent), "Movement");
            sprParam = Expression.Parameter(typeof(SpriteComponent), "Sprite");
            inputParam = Expression.Parameter(typeof(InputComponent), "Input");
            collParam = Expression.Parameter(typeof(CollisionComponent), "Collision");
            ladderParam = Expression.Parameter(typeof(LadderComponent), "Ladder");
            timerParam = Expression.Parameter(typeof(TimerComponent), "Timer");
            healthParam = Expression.Parameter(typeof(HealthComponent), "Health");
            stateParam = Expression.Parameter(typeof(StateComponent), "State");
            weaponParam = Expression.Parameter(typeof(WeaponComponent), "Weapon");
            stParam = Expression.Parameter(typeof(int), "StateTime");
            lifeParam = Expression.Parameter(typeof(int), "LifeTime");
            playerXParam = Expression.Parameter(typeof(float), "PlayerDistX");
            playerYParam = Expression.Parameter(typeof(float), "PlayerDistY");
            gravParam = Expression.Parameter(typeof(bool), "GravityFlip");
            randParam = Expression.Parameter(typeof(double), "Random");

            dirDict = new Dictionary<string, object>();
            dirDict.Add("Up", Direction.Up);
            dirDict.Add("Down", Direction.Down);
            dirDict.Add("Left", Direction.Left);
            dirDict.Add("Right", Direction.Right);

            CurrentState = "Start";
        }

        public override Component Clone()
        {
            StateComponent newone = new StateComponent();

            // notice the shallow copy!
            newone.states = this.states;

            return newone;
        }

        public override void Start()
        {
            Engine.Instance.GameThink += Update;
            if (states.ContainsKey(CurrentState)) states[CurrentState].Initialize(this.Parent);
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
                    CurrentState = statemsg.StateName;
                    states[CurrentState].Initialize(this.Parent);
                    stateframes = 0;
                }
            }
        }

        protected override void Update()
        {
            if (Parent.Paused) return;

            lifetime++;

            if (!states.ContainsKey(CurrentState)) return;

            stateframes++;
            framerand = Program.rand.NextDouble();

            string old = CurrentState;
            states[CurrentState].CheckTriggers(this, this.Parent);
            if (old != CurrentState)
            {
                if (!states.ContainsKey(CurrentState)) throw new GameEntityException("Entity \"" + Parent.Name + "\" tried to change to state \"" + CurrentState + "\", which does not exist.");
                states[CurrentState].Initialize(this.Parent);
                stateframes = 0;

                if (StateChanged != null) StateChanged(CurrentState);
            }
        }

        public override void RegisterDependencies(Component component)
        {
        }

        public override void LoadXml(XElement stateNode)
        {
            string name = stateNode.Attribute("name").Value;

            State state;
            if (states.ContainsKey(name))
            {
                state = states[name];
            }
            else
            {
                state = new State();
                state.Name = name;
                this.states[name] = state;
            }

            foreach (XElement child in stateNode.Elements())
            {
                switch (child.Name.LocalName)
                {
                    case "Trigger":
                        AddStateTrigger(state, child);
                        break;

                    default:
                        if (child.Attribute("mode") != null && child.Attribute("mode").Value.ToUpper() == "REPEAT") state.AddLogic(LoadXmlEffect(child));
                        else state.AddInitial(LoadXmlEffect(child));
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
                    if (!this.states.ContainsKey(stateName))
                    {
                        State state = new State();
                        state.Name = stateName;
                        states.Add(stateName, state);
                    }
                    AddStateTrigger(this.states[stateName], trigger);
                }
            }
            else
            {
                foreach (State state in this.states.Values)
                {
                    AddStateTrigger(state, trigger);
                }
            }
        }

        // provides a closure around a split condition
        private Condition CloseCondition(SplitCondition split)
        {
            return new Condition((entity) =>
            {
                PositionComponent pos = entity.GetComponent<PositionComponent>();
                return split(
                pos,
                entity.GetComponent<MovementComponent>(),
                entity.GetComponent<SpriteComponent>(),
                entity.GetComponent<InputComponent>(),
                entity.GetComponent<CollisionComponent>(),
                entity.GetComponent<LadderComponent>(),
                entity.GetComponent<TimerComponent>(),
                entity.GetComponent<HealthComponent>(),
                entity.GetComponent<WeaponComponent>(),
                (entity.GetComponent<StateComponent>()).stateframes,
                (entity.GetComponent<StateComponent>()).lifetime,
                Math.Abs(Game.CurrentGame.CurrentMap.PlayerPos.Position.X - pos.Position.X),
                Math.Abs(Game.CurrentGame.CurrentMap.PlayerPos.Position.Y - pos.Position.Y),
                Game.CurrentGame.GravityFlip,
                (entity.GetComponent<StateComponent>()).framerand
                );
            });
        }

        // provides a closure around a split effect
        private Effect CloseEffect(SplitEffect split)
        {
            return new Effect((entity) =>
            {
                split(
                entity.GetComponent<PositionComponent>(),
                entity.GetComponent<MovementComponent>(),
                entity.GetComponent<SpriteComponent>(),
                entity.GetComponent<InputComponent>(),
                entity.GetComponent<CollisionComponent>(),
                entity.GetComponent<LadderComponent>(),
                entity.GetComponent<TimerComponent>(),
                entity.GetComponent<HealthComponent>(),
                entity.GetComponent<StateComponent>(),
                entity.GetComponent<WeaponComponent>()
                );
            });
        }

        public Condition ParseCondition(string conditionString)
        {
            LambdaExpression lambda = DynamicExpression.ParseLambda(
                new[] { posParam, moveParam, sprParam, inputParam, collParam, ladderParam, timerParam, healthParam, weaponParam, stParam, lifeParam, playerXParam, playerYParam, gravParam, randParam },
                typeof(SplitCondition),
                typeof(bool),
                conditionString,
                dirDict);
            SplitCondition trigger = (SplitCondition)lambda.Compile();
            Condition condition = CloseCondition(trigger);

            return condition;
        }

        private void AddStateTrigger(State state, XElement triggerNode)
        {
            try
            {
                string conditionString;
                if (triggerNode.Attribute("condition") != null) conditionString = triggerNode.Attribute("condition").Value;
                else conditionString = triggerNode.Element("Condition").Value;

                Condition condition = ParseCondition(conditionString);

                Effect effect = LoadTriggerEffect(triggerNode.Element("Effect"));
                state.AddTrigger(condition, effect);
            }
            catch (Exception e)
            {
                throw new EntityXmlException(triggerNode, "There was an error parsing a trigger. There may be a syntax error in your condition expression.\n\nThe error message was:\n\n\t" + e.Message);
            }
        }

        public Effect LoadTriggerEffect(XElement effectnode)
        {
            Effect effect = new Effect((entity) => { });
            foreach (XElement child in effectnode.Elements())
            {
                effect += LoadXmlEffect(child);
            }
            return effect;
        }

        // this is for when <State> appears in an effect, used for changing state
        public override Effect ParseEffect(XElement effectNode)
        {
            string newstate = effectNode.Value;
            return (entity) =>
            {
                StateComponent state = entity.GetComponent<StateComponent>();
                if (state != null) state.CurrentState = newstate;
            };
        }

        private Effect LoadXmlEffect(XElement node)
        {
            Effect effect = (e) => { };

            switch (node.Name.LocalName)
            {
                case "Spawn":
                    effect = GameEntity.LoadSpawnEffect(node);
                    break;

                case "Die":
                    effect = (entity) => { entity.Stop(); };
                    break;

                case "Lives":
                    int add = int.Parse(node.Attribute("add").Value);
                    effect = (entity) =>
                    {
                        Game.CurrentGame.PlayerLives += add;
                    };
                    break;

                case "Func":
                    effect = (entity) => { };
                    string[] statements = node.Value.Split(';');
                    foreach (string st in statements)
                    {
                        if (string.IsNullOrEmpty(st.Trim())) continue;
                        LambdaExpression lambda = DynamicExpression.ParseLambda(
                            new[] { posParam, moveParam, sprParam, inputParam, collParam, ladderParam, timerParam, healthParam, stateParam, weaponParam },
                            typeof(SplitEffect),
                            null,
                            st,
                            dirDict);
                        effect += CloseEffect((SplitEffect)lambda.Compile());
                    }
                    break;

                default:
                    effect = Parent.ParseComponentEffect(node);
                    break;
            }
            return effect;
        }

        private class Trigger
        {
            public Condition Condition;
            public Effect Effect;
        }

        private class State
        {
            public string Name { get; set; }

            private List<Trigger> triggers = new List<Trigger>();
            private Effect initializer = (entity) => { };
            private Effect logic = (entity) => { };

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
                Trigger trigger = new Trigger();
                trigger.Condition = cond;
                trigger.Effect = effect;
                triggers.Add(trigger);
            }

            public void CheckTriggers(StateComponent statecomp, GameEntity entity)
            {
                string state = statecomp.CurrentState;
                foreach (Trigger trigger in triggers)
                {
                    bool result = (bool)trigger.Condition(entity);
                    if (result)
                    {
                        trigger.Effect(entity);
                        if (statecomp.CurrentState != state) break;
                    }
                }
                logic(entity);
            }
        }
    }
}
