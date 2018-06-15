using System;
using MegaMan.Common.Entities;

namespace MegaMan.Engine
{
    public class HealthComponent : Component
    {
        // alive is set to true if health ever goes above zero
        private bool alive;

        private float maxHealth;
        private float health;
        private HealthMeter meter;
        private int flashtime;
        private int flashing;
        private bool clearHitNextFrame;

        public float Health
        {
            get { return health; }
            set
            {
                health = value;
                if (health > maxHealth) health = maxHealth;

                if (health > 0)
                {
                    alive = true;
                }

                if (meter != null)
                {
                    meter.Value = health;
                }

                if (HealthChanged != null)
                {
                    HealthChanged(health, maxHealth);
                }
            }
        }

        public float StartHealth { get; private set; }
        public float MaxHealth { get { return maxHealth; } }
        public bool Hit { get; private set; }

        public event Action<float, float> HealthChanged;

        private void Instance_GameCleanup()
        {
            if (alive && Health <= 0) Parent.Die();
        }

        public override Component Clone()
        {
            var copy = new HealthComponent
            {
                StartHealth = StartHealth,
                maxHealth = maxHealth,
                flashtime = flashtime,
                meter = meter
            };

            // if it has a meter, it's intended to only have one instance on the screen
            // so a shallow copy should suffice
            if (copy.meter != null) copy.meter.Reset();

            return copy;
        }

        public override void Start(IGameplayContainer container)
        {
            container.GameThink += Update;
            container.GameCleanup += Instance_GameCleanup;
            Health = StartHealth;

            if (meter != null)
            {
                meter.Start(container);
            }
        }

        public void DelayFill(int frames)
        {
            if (meter != null) meter.DelayedFill(frames);
        }

        public override void Stop(IGameplayContainer container)
        {
            container.GameThink -= Update;
            container.GameCleanup -= Instance_GameCleanup;
            if (meter != null)
            {
                meter.Stop();
            }
            Hit = false;
            flashing = 0;
        }

        public override void Message(IGameMessage msg)
        {
            if (msg is DamageMessage && flashing == 0)
            {
                if (Engine.Instance.Invincible && Parent.Name == "Player") return;

                var damage = (DamageMessage)msg;
                if (!Engine.Instance.NoDamage)
                    Health -= damage.Damage;

                Hit = true;
                clearHitNextFrame = false;
                flashing = flashtime;
            }
            else if (msg is HealMessage)
            {
                var heal = (HealMessage)msg;

                Health += heal.Health;
            }
        }

        protected override void Update()
        {
            if (clearHitNextFrame)
                Hit = false;
            else
                clearHitNextFrame = true;
            
            if (flashing > 0)
            {
                flashing--;
                var spr = Parent.GetComponent<SpriteComponent>();
                if (spr != null) spr.Visible = (flashing % 3 != 1);
            }
        }

        public override void RegisterDependencies(Component component)
        {
            
        }

        public void LoadInfo(HealthComponentInfo info)
        {
            maxHealth = info.Max;
            StartHealth = info.StartValue ?? info.Max;
            flashtime = info.FlashFrames;

            if (info.Meter != null)
            {
                meter = HealthMeter.Create(info.Meter, true);
                meter.MaxValue = maxHealth;
                meter.IsPlayer = (Parent.Name == "Player");
            }
        }

        // this exists for the sake of dynamic expressions,
        // which can't do assignment through operators
        public void Add(float val)
        {
            Health += val;
        }

        public void Reset()
        {
            Health = MaxHealth;
        }
    }
}
