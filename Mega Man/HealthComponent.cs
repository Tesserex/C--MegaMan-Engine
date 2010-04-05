using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Mega_Man
{
    public class HealthComponent : Component
    {
        private float maxHealth;
        private float health;
        private HealthMeter meter;
        private int flashtime;
        private int flashing;

        public float Health { get { return health; } }
        public float MaxHealth { get { return maxHealth; } }
        public bool Hit { get; private set; }

        void Instance_GameCleanup()
        {
            if (health <= 0) Parent.Die();
        }

        public override Component Clone()
        {
            HealthComponent copy = new HealthComponent();
            copy.maxHealth = this.maxHealth;
            copy.health = this.health;
            copy.flashtime = this.flashtime;

            // if it has a meter, it's intended to only have one instance on the screen
            // so a shallow copy should suffice
            copy.meter = this.meter;
            if (copy.meter != null) copy.meter.Reset();

            return copy;
        }

        public override void Start()
        {
            Engine.Instance.GameThink += Update;
            Engine.Instance.GameCleanup += new Action(Instance_GameCleanup);
            if (meter != null) meter.StartHandler();
        }

        public override void Stop()
        {
            Engine.Instance.GameThink -= Update;
            Engine.Instance.GameCleanup -= new Action(Instance_GameCleanup);
            if (meter != null) meter.Value = 0;
        }

        public override void Message(IGameMessage msg)
        {
            if (msg is DamageMessage && flashing == 0)
            {
                if (Engine.Instance.Invincible && this.Parent == Game.CurrentGame.CurrentMap.Player) return;

                DamageMessage damage = (DamageMessage)msg;

                // here is where you should check for exceptions like resistance or weakness

                health -= damage.Damage;
                if (meter != null) meter.Value = health;

                Hit = true;
                flashing = flashtime;
            }
            else if (msg is HealMessage)
            {
                HealMessage heal = (HealMessage)msg;

                health += heal.Health;
                if (health > maxHealth) health = maxHealth;
                if (meter != null) meter.Value = health;
            }
        }

        protected override void Update()
        {
            Hit = false;

            if (flashing > 0)
            {
                flashing--;
                SpriteComponent spr = (SpriteComponent)Parent.GetComponent(typeof(SpriteComponent));
                if (spr != null) spr.Visible = (flashing % 3 != 1);
            }
        }

        public override void RegisterDependencies(Component component)
        {
            
        }

        public void LoadXml(XElement xml)
        {
            XElement maxNode = xml.Element("Max");
            if (maxNode != null)
            {
                if (!float.TryParse(maxNode.Value, out maxHealth)) throw new EntityXmlException(maxNode, "Health maximum was not a valid number.");
                health = maxHealth;
            }

            XElement meterNode = xml.Element("Meter");
            if (meterNode != null)
            {
                this.meter = new HealthMeter();
                this.meter.LoadXml(meterNode);
                this.meter.MaxValue = this.maxHealth;
            }

            XElement flashNode = xml.Element("Flash");
            if (flashNode != null)
            {
                if (!int.TryParse(flashNode.Value, out flashtime)) throw new EntityXmlException(flashNode, "Health flash time was not a valid number.");
            }
        }
    }
}
