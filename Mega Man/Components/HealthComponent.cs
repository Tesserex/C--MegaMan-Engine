using System.Xml.Linq;
using MegaMan.Common;
using System;
using MegaMan.IO.Xml;

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

        public float Health
        {
            get { return health; }
            private set
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

        public override void Start()
        {
            Parent.Container.GameThink += Update;
            Parent.Container.GameCleanup += Instance_GameCleanup;
            Health = StartHealth;

            if (meter != null)
            {
                meter.Start(Parent.Container);
            }
        }

        public void DelayFill(int frames)
        {
            if (meter != null) meter.DelayedFill(frames);
        }

        public override void Stop()
        {
            Parent.Container.GameThink -= Update;
            Parent.Container.GameCleanup -= Instance_GameCleanup;
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

                DamageMessage damage = (DamageMessage)msg;

                Health -= damage.Damage;

                Hit = true;
                flashing = flashtime;
            }
            else if (msg is HealMessage)
            {
                HealMessage heal = (HealMessage)msg;

                Health += heal.Health;
            }
        }

        protected override void Update()
        {
            Hit = false;

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

        public override void LoadXml(XElement node)
        {
            XElement maxNode = node.Element("Max");
            if (maxNode != null)
            {
                maxHealth = maxNode.GetValue<float>();
            }

            StartHealth = node.TryAttribute<float>("startValue", MaxHealth);

            XElement meterNode = node.Element("Meter");
            if (meterNode != null)
            {
                var meterInfo = HandlerXmlReader.LoadMeter(meterNode, Game.CurrentGame.BasePath);
                meter = HealthMeter.Create(meterInfo, true);
                meter.MaxValue = maxHealth;
                meter.IsPlayer = (Parent.Name == "Player");
            }

            XElement flashNode = node.Element("Flash");
            if (flashNode != null)
            {
                flashtime = flashNode.TryValue<int>();
            }
        }

        public static Effect ParseEffect(XElement effectNode)
        {
            if (effectNode.Attribute("change") != null)
            {
                float changeval = effectNode.TryAttribute<float>("change");
                return entity =>
                {
                    entity.GetComponent<HealthComponent>().Health += changeval;
                };
            }
            return entity => { };
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
