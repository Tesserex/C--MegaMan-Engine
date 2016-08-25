using MegaMan.Common;
using MegaMan.Engine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MegaMan.Common.Entities.Effects;
using System.Reflection;
using Ninject;
using MegaMan.Engine.Entities.Effects;

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
            float playerdabsx,
            float playerdabsy,
            bool gravflip,
            double random,
            Player player
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
        WeaponComponent weapon,
        Player player
    );

    public delegate bool Condition(IEntity entity);
    public delegate void Effect(IEntity entity);

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
        private static readonly ParameterExpression playerXAbsParam;
        private static readonly ParameterExpression playerYAbsParam;
        private static readonly ParameterExpression gravParam;
        private static readonly ParameterExpression randParam;
        private static readonly ParameterExpression playerParam;

        private static readonly Dictionary<string, object> dirDict;

        private static readonly Dictionary<string, Effect> storedEffects = new Dictionary<string, Effect>();

        private static readonly Dictionary<Type, IEffectLoader> effectLoaders = new Dictionary<Type, IEffectLoader>();

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
            playerXAbsParam = Expression.Parameter(typeof(float), "PlayerDistAbsX");
            playerYAbsParam = Expression.Parameter(typeof(float), "PlayerDistAbsY");
            gravParam = Expression.Parameter(typeof(bool), "GravityFlip");
            randParam = Expression.Parameter(typeof(double), "Random");
            playerParam = Expression.Parameter(typeof(Player), "Game");

            dirDict = new Dictionary<string, object>
            {
                {"Up", Direction.Up},
                {"Down", Direction.Down},
                {"Left", Direction.Left},
                {"Right", Direction.Right}
            };

            effectLoaders = Assembly.GetAssembly(typeof(IEffectLoader))
                .GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IEffectLoader)) && !t.IsAbstract)
                .Select(t => Injector.Container.Get(t))
                .Cast<IEffectLoader>()
                .ToDictionary(t => t.PartInfoType);
        }

        public static Condition ParseCondition(string conditionString)
        {
            LambdaExpression lambda = System.Linq.Dynamic.DynamicExpression.ParseLambda(
                new[] { posParam, moveParam, sprParam, inputParam, collParam, ladderParam, timerParam, healthParam, weaponParam, stParam, lifeParam, playerXParam, playerYParam, playerXAbsParam, playerYAbsParam, gravParam, randParam, playerParam },
                typeof(SplitCondition),
                typeof(bool),
                conditionString,
                dirDict);
            SplitCondition trigger = (SplitCondition)lambda.Compile();
            Condition condition = CloseCondition(trigger);

            return condition;
        }

        public static Effect LoadTriggerEffect(EffectInfo info)
        {
            Effect effect = LoadEffect(info);
            if (info.Name != null)
                SaveEffect(info.Name, effect);

            return effect;
        }
        
        public static void LoadEffectsList(IEnumerable<EffectInfo> effects)
        {
            foreach (var effectInfo in effects)
            {
                Effect effect = LoadEffect(effectInfo);
                SaveEffect(effectInfo.Name, effect);
            }
        }
        
        public static void SaveEffect(string name, Effect effect)
        {
            storedEffects.Add(name, effect);
        }

        public static Effect GetLateBoundEffect(string name)
        {
            return e => {
                if (storedEffects.ContainsKey(name)) storedEffects[name](e);
            };
        }

        public static Effect GetOrLoadEffect(string name, EffectInfo info)
        {
            if (!storedEffects.ContainsKey(name))
            {
                storedEffects[name] = LoadEffect(info);
            }

            return storedEffects[name];
        }

        private static Effect LoadEffect(EffectInfo info)
        {
            return info.Parts.Aggregate(new Effect(e => { }), (c, part) => c + LoadEffectPart(part));
        }

        public static Effect LoadEffectPart(IEffectPartInfo partInfo)
        {
            var t = partInfo.GetType();
            if (!effectLoaders.ContainsKey(t))
                throw new GameRunException("Unsupported effect type: " + t.Name);

            var loader = effectLoaders[t];
            return loader.Load(partInfo);
        }

        public static Effect CompileEffect(string st)
        {
            LambdaExpression lambda = System.Linq.Dynamic.DynamicExpression.ParseLambda(
                            new[] { posParam, moveParam, sprParam, inputParam, collParam, ladderParam, timerParam, healthParam, stateParam, weaponParam, playerParam },
                            typeof(SplitEffect),
                            null,
                            st,
                            dirDict);
            return CloseEffect((SplitEffect)lambda.Compile());
        }

        // provides a closure around a split condition
        private static Condition CloseCondition(SplitCondition split)
        {
            return entity => {
                if (entity == null)
                {
                    return split(
                        null, null, null, null, null, null, null, null, null, 0, 0, 0, 0, 0, 0,
                        entity != null ? entity.Container.IsGravityFlipped : false,
                        0,
                        Game.CurrentGame.Player
                    );
                }

                PositionComponent pos = entity.GetComponent<PositionComponent>();

                float pdx = 0;
                float pdy = 0;
                float pdxAbs = 0;
                float pdyAbs = 0;
                GameEntity player = entity.Entities.GetEntityById("Player");
                if (player != null)
                {
                    var playerPos = player.GetComponent<PositionComponent>();
                    if (playerPos != null)
                    {
                        pdx = Math.Abs(playerPos.Position.X - pos.Position.X);
                        pdy = Math.Abs(playerPos.Position.Y - pos.Position.Y);
                        pdxAbs = playerPos.Position.X - pos.Position.X;
                        pdyAbs = playerPos.Position.Y - pos.Position.Y;
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
                    pdxAbs,
                    pdyAbs,
                    entity.Container.IsGravityFlipped,
                    (entity.GetComponent<StateComponent>()).FrameRand,
                    Game.CurrentGame.Player
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
                entity.GetComponent<WeaponComponent>(),
                Game.CurrentGame.Player
            );
        }

        public static void Unload()
        {
            storedEffects.Clear();
        }
    }
}
