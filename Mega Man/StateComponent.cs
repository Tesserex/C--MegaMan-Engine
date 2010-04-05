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
        IPositioned pos,
        IMovement mov,
        SpriteComponent spr,
        InputComponent inp,
        CollisionComponent col,
        LadderComponent lad,
        TimerComponent timer,
        HealthComponent health,
        int statetime,
        int lifetime,
        float playerdx,
        float playerdy,
        bool gravflip,
        double random
    );

    public delegate bool Condition(GameEntity entity);
    public delegate void Effect(GameEntity entity);

    public class StateComponent : Component
    {
        public string CurrentState { get; private set; }
        private Dictionary<string, State> states;

        private int stateframes = 0;
        private int lifetime = 0;

        private Dictionary<string, object> dirDict;

        private ParameterExpression posParam, moveParam, sprParam, inputParam, collParam,
            ladderParam, timerParam, stParam, lifeParam, healthParam, playerXParam, playerYParam, gravParam, randParam;

        public event Action<string> StateChanged;

        public StateComponent()
        {
            states = new Dictionary<string, State>();
            posParam = Expression.Parameter(typeof(IPositioned), "Position");
            moveParam = Expression.Parameter(typeof(IMovement), "Movement");
            sprParam = Expression.Parameter(typeof(SpriteComponent), "Sprite");
            inputParam = Expression.Parameter(typeof(InputComponent), "Input");
            collParam = Expression.Parameter(typeof(CollisionComponent), "Collision");
            ladderParam = Expression.Parameter(typeof(LadderComponent), "Ladder");
            timerParam = Expression.Parameter(typeof(TimerComponent), "Timer");
            healthParam = Expression.Parameter(typeof(HealthComponent), "Health");
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

            string old = CurrentState;
            states[CurrentState].CheckTriggers(this, this.Parent);
            if (old != CurrentState)
            {
                states[CurrentState].Initialize(this.Parent);
                stateframes = 0;

                if (StateChanged != null) StateChanged(CurrentState);
            }
        }

        public override void RegisterDependencies(Component component)
        {
        }

        public void LoadStateXml(XElement stateNode)
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
                        if (child.Attribute("mode") != null && child.Attribute("mode").Value == "Repeat") state.AddLogic(LoadXmlEffect(child));
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
                PositionComponent pos = (PositionComponent)entity.GetComponent(typeof(PositionComponent));
                return split(
                pos,
                (IMovement)entity.GetComponent(typeof(MovementComponent)),
                (SpriteComponent)entity.GetComponent(typeof(SpriteComponent)),
                (InputComponent)entity.GetComponent(typeof(InputComponent)),
                (CollisionComponent)entity.GetComponent(typeof(CollisionComponent)),
                (LadderComponent)entity.GetComponent(typeof(LadderComponent)),
                (TimerComponent)entity.GetComponent(typeof(TimerComponent)),
                (HealthComponent)entity.GetComponent(typeof(HealthComponent)),
                ((StateComponent)entity.GetComponent(typeof(StateComponent))).stateframes,
                ((StateComponent)entity.GetComponent(typeof(StateComponent))).lifetime,
                Math.Abs(Game.CurrentGame.CurrentMap.PlayerPos.Position.X - pos.Position.X),
                Math.Abs(Game.CurrentGame.CurrentMap.PlayerPos.Position.Y - pos.Position.Y),
                Game.CurrentGame.GravityFlip,
                Program.rand.NextDouble()
                );
            });
        }

        public Condition ParseCondition(string conditionString)
        {
            LambdaExpression lambda = DynamicExpression.ParseLambda(
                new[] { posParam, moveParam, sprParam, inputParam, collParam, ladderParam, timerParam, healthParam, stParam, lifeParam, playerXParam, playerYParam, gravParam, randParam },
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

        private Effect LoadXmlEffect(XElement node)
        {
            Effect effect = (e) => { };

            switch (node.Name.LocalName)
            {
                case "State":
                    string newstate = node.Value;
                    effect = (entity) =>
                    {
                        StateComponent state = (StateComponent)entity.GetComponent(typeof(StateComponent));
                        if (state != null) state.CurrentState = newstate;
                    };
                    break;

                case "Spawn":
                    string name = node.Attribute("name").Value;
                    string statename = "Start";
                    if (node.Attribute("state") != null) statename = node.Attribute("state").Value;
                    effect = (entity) =>
                    {
                        GameEntity spawn = entity.Spawn(name);
                        if (spawn == null) return;
                        StateMessage msg = new StateMessage(entity, statename);
                        spawn.SendMessage(msg);
                    };
                    break;

                case "Die":
                    effect = (entity) => { entity.Stop(); };
                    break;

                case "Movement":
                    Parent.AddComponent(new MovementComponent());
                    effect = MovementComponent.LoadMovementEffect(node);
                    break;

                case "Ladder":
                    effect = LadderComponent.LoadLadderEffect(node);
                    break;

                case "Sprite":
                    effect = LoadSpriteEffect(node);
                    break;

                case "Position":
                    effect = PositionComponent.LoadPositionAction(node);
                    break;

                case "Sound":
                    string soundname = node.Attribute("name").Value;
                    bool playing = true;
                    XAttribute playAttr = node.Attribute("playing");
                    if (playAttr != null)
                    {
                        if (!bool.TryParse(playAttr.Value, out playing)) throw new EntityXmlException(playAttr, "Playing attribute must be a boolean (true or false).");
                    }
                    effect = (entity) =>
                    {
                        SoundMessage msg = new SoundMessage(entity, soundname, playing);
                        entity.SendMessage(msg);
                    };
                    break;

                case "Collision":
                    Parent.AddComponent(new CollisionComponent());
                    effect = CollisionComponent.LoadCollisionEffect(node);
                    break;

                case "Timer":
                    Parent.AddComponent(new TimerComponent());
                    effect = TimerComponent.LoadXmlEffect(node);
                    break;

                case "Weapon":
                    effect = WeaponComponent.LoadEffect(node);
                    break;
            }
            return effect;
        }

        private Effect LoadSpriteEffect(XElement node)
        {
            Effect action = new Effect((entity) => { });
            foreach (XElement prop in node.Elements())
            {
                switch (prop.Name.LocalName)
                {
                    case "Name":
                        string spritename = prop.Value;
                        action += (entity) => {
                            SpriteComponent spritecomp = (SpriteComponent)entity.GetComponent(typeof(SpriteComponent));
                            spritecomp.ChangeSprite(spritename);
                        };
                        break;
                        
                    case "Playing":
                        bool play;
                        if (!bool.TryParse(prop.Value, out play)) throw new EntityXmlException(prop, "Playing tag must be a valid bool (true or false).");
                        action += (entity) => {
                            SpriteComponent spritecomp = (SpriteComponent)entity.GetComponent(typeof(SpriteComponent));
                            spritecomp.Playing = play;
                        };
                        break;

                    case "Visible":
                        bool vis;
                        if (!bool.TryParse(prop.Value, out vis)) throw new EntityXmlException(prop, "Visible tag must be a valid bool (true or false).");
                        action += (entity) =>
                        {
                            SpriteComponent spritecomp = (SpriteComponent)entity.GetComponent(typeof(SpriteComponent));
                            spritecomp.Visible = vis;
                        };
                        break;

                    case "Group":
                        string group = prop.Value;
                        action += (entity) =>
                        {
                            SpriteComponent spritecomp = (SpriteComponent)entity.GetComponent(typeof(SpriteComponent));
                            spritecomp.ChangeGroup(group);
                        };
                        break;
                }
            }
            return action;
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
