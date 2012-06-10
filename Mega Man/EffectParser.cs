using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common;
using System.Linq.Expressions;

namespace MegaMan.Engine
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

    public static class EffectParser
    {
        private static readonly ParameterExpression posParam;
        private static readonly ParameterExpression moveParam;
        private static readonly ParameterExpression sprParam;
        private static readonly ParameterExpression inputParam;
        private static readonly ParameterExpression collParam;
        private static readonly ParameterExpression stateParam;
        private static readonly ParameterExpression weaponParam;
        private static readonly ParameterExpression ladderParam;
        private static readonly ParameterExpression timerParam;
        private static readonly ParameterExpression stParam;
        private static readonly ParameterExpression lifeParam;
        private static readonly ParameterExpression healthParam;
        private static readonly ParameterExpression playerXParam;
        private static readonly ParameterExpression playerYParam;
        private static readonly ParameterExpression gravParam;
        private static readonly ParameterExpression randParam;

        private static readonly Dictionary<string, object> dirDict;

        private static readonly Dictionary<string, Effect> storedEffects = new Dictionary<string, Effect>();

        static EffectParser()
        {
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

            dirDict = new Dictionary<string, object>
            {
                {"Up", Direction.Up},
                {"Down", Direction.Down},
                {"Left", Direction.Left},
                {"Right", Direction.Right}
            };
        }

        public static Condition ParseCondition(string conditionString)
        {
            LambdaExpression lambda = System.Linq.Dynamic.DynamicExpression.ParseLambda(
                new[] { posParam, moveParam, sprParam, inputParam, collParam, ladderParam, timerParam, healthParam, weaponParam, stParam, lifeParam, playerXParam, playerYParam, gravParam, randParam },
                typeof(SplitCondition),
                typeof(bool),
                conditionString,
                dirDict);
            SplitCondition trigger = (SplitCondition)lambda.Compile();
            Condition condition = CloseCondition(trigger);

            return condition;
        }

        public static Effect LoadTriggerEffect(XElement effectnode)
        {
            Effect effect = LoadEffect(effectnode);

            // check for name to save
            XAttribute nameAttr = effectnode.Attribute("name");
            if (nameAttr != null)
            {
                EffectParser.SaveEffect(nameAttr.Value, effect);
            }
            return effect;
        }

        public static void LoadEffectsList(XElement element)
        {
            foreach (var effectnode in element.Elements("Function"))
            {
                string name = effectnode.RequireAttribute("name").Value;

                Effect effect = LoadEffect(effectnode);

                EffectParser.SaveEffect(name, effect);
            }
        }

        public static void SaveEffect(string name, Effect effect)
        {
            storedEffects.Add(name, effect);
        }

        public static Effect GetLateBoundEffect(string name)
        {
            return e =>
            {
                if (storedEffects.ContainsKey(name)) storedEffects[name](e);
            };
        }

        public static Effect GetOrLoadEffect(string name, XElement node)
        {
            if (!storedEffects.ContainsKey(name))
            {
                SaveEffect(name, LoadEffect(node));
            }

            return storedEffects[name];
        }

        private static Effect LoadEffect(XElement node)
        {
            return node.Elements().Aggregate(new Effect(e => { }), (current, child) => current + LoadEffectAction(child));
        }

        public static Effect LoadEffectAction(XElement node)
        {
            Effect effect = e => { };

            switch (node.Name.LocalName)
            {
                case "Call":
                    effect = GetLateBoundEffect(node.Value);
                    break;

                case "Spawn":
                    effect = LoadSpawnEffect(node);
                    break;

                case "Remove":
                    effect = entity => { entity.Remove(); };
                    break;

                case "Die":
                    effect = entity => { entity.Die(); };
                    break;
                
                case "AddInventory":
                    string itemName = node.RequireAttribute("item").Value;
                    int quantity;
                    bool hasQuantity = GameXml.TryInteger(node, "quantity", out quantity);

                    if (!hasQuantity)
                        quantity = 1;

                    effect = entity =>
                    {
                        Game.CurrentGame.Player.CollectItem(itemName, quantity);
                    };
                    break;

                case "RemoveInventory":
                    string itemNameUse = node.RequireAttribute("item").Value;
                    int quantityUse;
                    bool hasQuantityUse = GameXml.TryInteger(node, "quantity", out quantityUse);

                    if (!hasQuantityUse)
                        quantityUse = 1;

                    effect = entity =>
                    {
                        Game.CurrentGame.Player.UseItem(itemNameUse, quantityUse);
                    };
                    break;

                case "Lives":
                    int add = int.Parse(node.RequireAttribute("add").Value);
                    effect = entity =>
                    {
                        Game.CurrentGame.Player.Lives += add;
                    };
                    break;

                case "GravityFlip":
                    bool flip = node.GetBool();
                    effect = entity => { Game.CurrentGame.GravityFlip = flip; };
                    break;

                case "Func":
                    effect = entity => { };
                    string[] statements = node.Value.Split(';');
                    foreach (string st in statements.Where(st => !string.IsNullOrEmpty(st.Trim())))
                    {
                        LambdaExpression lambda = System.Linq.Dynamic.DynamicExpression.ParseLambda(
                            new[] { posParam, moveParam, sprParam, inputParam, collParam, ladderParam, timerParam, healthParam, stateParam, weaponParam },
                            typeof(SplitEffect),
                            null,
                            st,
                            dirDict);
                        effect += CloseEffect((SplitEffect)lambda.Compile());
                    }
                    break;

                case "Trigger":
                    string conditionString;
                    if (node.Attribute("condition") != null) conditionString = node.RequireAttribute("condition").Value;
                    else conditionString = node.Element("Condition").Value;

                    Condition condition = ParseCondition(conditionString);
                    Effect triggerEffect = LoadTriggerEffect(node.Element("Effect"));
                    effect += (e) =>
                    {
                        if (condition(e)) triggerEffect(e);
                    };
                    break;

                case "Pause":
                    effect = entity => { entity.Paused = true; };
                    break;

                case "Unpause":
                    effect = entity => { entity.Paused = false; };
                    break;

                case "Next":
                    var transfer = HandlerTransfer.FromXml(node);
                    effect = e => { Game.CurrentGame.ProcessHandler(transfer); };
                    break;

                default:
                    effect = GameEntity.ParseComponentEffect(node);
                    break;
            }
            return effect;
        }

        public static Effect LoadSpawnEffect(XElement node)
        {
            if (node == null) throw new ArgumentNullException("node");

            string name = node.RequireAttribute("name").Value;
            string statename = "Start";
            if (node.Attribute("state") != null) statename = node.Attribute("state").Value;
            XElement posNodeX = node.Element("X");
            XElement posNodeY = node.Element("Y");
            Effect posEff = null;
            if (posNodeX != null)
            {
                posEff = PositionComponent.ParsePositionBehavior(posNodeX, Axis.X);
            }
            if (posNodeY != null) posEff += PositionComponent.ParsePositionBehavior(posNodeY, Axis.Y);
            return entity =>
            {
                GameEntity spawn = entity.Spawn(name);
                if (spawn == null) return;
                StateMessage msg = new StateMessage(entity, statename);
                spawn.SendMessage(msg);
                if (posEff != null) posEff(spawn);
            };
        }

        // provides a closure around a split condition
        private static Condition CloseCondition(SplitCondition split)
        {
            return entity =>
            {
                PositionComponent pos = entity.GetComponent<PositionComponent>();
                
                float pdx = 0;
                float pdy = 0;
                GameEntity player = entity.Screen.GetEntities("Player").SingleOrDefault();
                if (player != null)
                {
                    var playerPos = player.GetComponent<PositionComponent>();
                    if (playerPos != null)
                    {
                        pdx = Math.Abs(playerPos.Position.X - pos.Position.X);
                        pdy = Math.Abs(playerPos.Position.Y - pos.Position.Y);
                    }
                }

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
                    (entity.GetComponent<StateComponent>()).StateFrames,
                    (entity.GetComponent<StateComponent>()).Lifetime,
                    pdx,
                    pdy,
                    Game.CurrentGame.GravityFlip,
                    (entity.GetComponent<StateComponent>()).FrameRand
                    );
            };
        }

        // provides a closure around a split effect
        private static Effect CloseEffect(SplitEffect split)
        {
            return entity => split(
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
        }

        public static void Unload()
        {
            storedEffects.Clear();
        }
    }
}
