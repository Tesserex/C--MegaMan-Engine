using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Mega_Man
{
    public class WeaponComponent : Component
    {
        private class WeaponInfo
        {
            public int Index;
            public string Name;
            public int Ammo;
            public int Max;
            public HealthMeter Meter;
            public string SpriteGroup;
        }

        private List<WeaponInfo> weapons = new List<WeaponInfo>();
        private int current;

        public string CurrentWeapon { get { return weapons[current].Name; } }

        public int Ammo(string weapon)
        {
            foreach (WeaponInfo info in weapons)
            {
                if (info.Name == weapon)
                {
                    if (info.SpriteGroup == "Default") return (int)(Parent.GetComponent<HealthComponent>()).Health;
                    return info.Ammo;
                }
            }
            return 0;
        }

        public int MaxAmmo(string weapon)
        {
            foreach (WeaponInfo info in weapons)
            {
                if (info.Name == weapon)
                {
                    if (info.SpriteGroup == "Default") return (int)(Parent.GetComponent<HealthComponent>()).MaxHealth;
                    return info.Max;
                }
            }
            return 0;
        }

        public void SetWeapon(string entityName)
        {
            foreach (WeaponInfo info in weapons)
            {
                if (info.Name == entityName)
                {
                    if (weapons[current].Meter != null)
                    {
                        weapons[current].Meter.StopHandler();
                    }

                    current = info.Index;
                    ApplyCurrent();
                    return;
                }
            }
        }

        public override Component Clone()
        {
            WeaponComponent copy = new WeaponComponent();
            copy.weapons = this.weapons;
            copy.current = 0;
            return copy;
        }

        public override void Start()
        {
            
        }

        public override void Stop()
        {
            
        }

        public override void Message(IGameMessage msg)
        {
            
        }

        protected override void Update()
        {
            
        }

        public override void RegisterDependencies(Component component)
        {
            
        }

        public void RotateForward()
        {
            if (weapons[current].Meter != null)
            {
                weapons[current].Meter.StopHandler();
            }

            current++;
            if (current >= weapons.Count) current = 0;

            ApplyCurrent();
        }

        public void RotateBackward()
        {
            if (weapons[current].Meter != null)
            {
                weapons[current].Meter.StopHandler();
            }

            current--;
            if (current < 0) current = weapons.Count - 1;

            ApplyCurrent();
        }

        private void ApplyCurrent()
        {
            SpriteComponent sprites = this.Parent.GetComponent<SpriteComponent>();
            if (sprites != null) sprites.ChangeGroup(weapons[current].SpriteGroup);

            if (weapons[current].Meter != null)
            {
                weapons[current].Meter.StartHandler();
            }
        }

        public void Shoot()
        {
            if (weapons.Count > current && current >= 0)
            {
                if (weapons[current].Ammo != 0)
                {
                    Parent.Spawn(weapons[current].Name);
                    if (weapons[current].Ammo > 0)
                    {
                        AddAmmo(-1);
                    }
                }
            }
        }

        public void AddAmmo(int ammo)
        {
            weapons[current].Ammo += ammo;
            if (weapons[current].Ammo < 0) weapons[current].Ammo = 0;
            if (weapons[current].Ammo > weapons[current].Max) weapons[current].Ammo = weapons[current].Max;

            if (weapons[current].Meter != null) weapons[current].Meter.Value = weapons[current].Ammo;
        }

        public void AddWeapon(string name, int ammo, HealthMeter meter, string spritegroup)
        {
            foreach (WeaponInfo info in weapons)
            {
                if (info.Name == name) return;
            }
            WeaponInfo weapon = new WeaponInfo();
            weapon.Name = name;
            weapon.Ammo = ammo;
            weapon.Max = ammo;
            weapon.Meter = meter;
            weapon.SpriteGroup = spritegroup;

            weapon.Index = weapons.Count;
            weapons.Add(weapon);
        }

        public override void LoadXml(XElement node)
        {
            foreach (XElement weapon in node.Elements("Weapon"))
            {
                string name = weapon.Attribute("name").Value;

                XAttribute ammoattr = weapon.Attribute("ammo");
                int ammo = -1;
                if (ammoattr != null) ammo = int.Parse(ammoattr.Value);

                string spritegroup = "Default";
                XAttribute sprite = weapon.Attribute("pallete");
                if (sprite != null) spritegroup = sprite.Value;

                HealthMeter meter = null;
                XElement meterNode = weapon.Element("Meter");
                if (meterNode != null)
                {
                    meter = new HealthMeter();
                    meter.LoadXml(meterNode);

                    meter.MaxValue = ammo;
                    meter.Reset();
                }

                AddWeapon(name, ammo, meter, spritegroup);
            }
        }

        public override Effect ParseEffect(XElement node)
        {
            Effect effect = (e) => { };
            if (node.Value == "Shoot")
            {
                effect = (entity) =>
                {
                    WeaponComponent weapons = entity.GetComponent<WeaponComponent>();
                    if (weapons != null) weapons.Shoot();
                };
            }
            else if (node.Value == "RotateForward")
            {
                effect = (entity) =>
                {
                    WeaponComponent weapons = entity.GetComponent<WeaponComponent>();
                    if (weapons != null) weapons.RotateForward();
                };
            }
            else
            {
                XElement ammoNode = node.Element("Ammo");
                if (ammoNode != null)
                {
                    int val = int.Parse(ammoNode.Attribute("val").Value);
                    effect = (entity) =>
                    {
                        WeaponComponent weapons = entity.GetComponent<WeaponComponent>();
                        if (weapons != null) weapons.AddAmmo(val);
                    };
                }
            }
            return effect;
        }
    }
}
