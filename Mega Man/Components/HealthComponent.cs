using System;
using MegaMan.Common.Entities;

namespace MegaMan.Engine
{
    public class HealthComponent : Component
    {
        // alive is set to true if health ever goes above zero
        private bool alive = false;

        private float maxHealth;
        private float health;
        private HealthMeter meter;
        private int flashtime;
        private int flashing;
        private DamageMessage damageMSG;

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
            HealthComponent copy = new HealthComponent
            {
                StartHealth = this.StartHealth,
                maxHealth = this.maxHealth,
                flashtime = this.flashtime,
                meter = this.meter
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
            flashing = 0;
        }

        #region Hit Code
        public bool Hurt()
        {
            if (Hit)
            {
                if (!Engine.Instance.NoDamage) Health -= damageMSG.Damage;
                flashing = flashtime;
                Hit = false;

                return true;
            }
            return false;
        }

        public bool AlwaysTrue() { return true; }

        /// <summary>
        /// State is one where Mega Man cannot be hurt.
        /// </summary>
        public void CancelHurt()
        {
            Hit = false;
        }

        public override void Message(IGameMessage msg)
        {
            if (msg is DamageMessage && flashing == 0)
            {
                if (Engine.Instance.Invincible && Parent.Name == "Player") return;

                damageMSG = (DamageMessage)msg;

                Hit = true;
            }
            else if (msg is HealMessage)
            {
                HealMessage heal = (HealMessage)msg;

                Health += heal.Health;
            }
        }
        #endregion

        protected override void Update()
        {
            if (flashing > 0)
            {
                flashing--;
                SpriteComponent spr = Parent.GetComponent<SpriteComponent>();
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
