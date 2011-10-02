using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.Engine
{
    public class WeaponComponent : Component
    {
        private class WeaponInfo
        {
            public int Index;
            public string Name;
            public string Entity;
            public int Ammo;
            public int Max;
            public int Usage;
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
            WeaponComponent copy = new WeaponComponent {weapons = weapons, current = 0};
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
            SpriteComponent sprites = Parent.GetComponent<SpriteComponent>();
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
                    Parent.Spawn(weapons[current].Entity);
                    if (weapons[current].Ammo > 0)
                    {
                        AddAmmo(-1 * weapons[current].Usage);
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

        public void AddWeapon(string name, string entity, int ammo, int usage, HealthMeter meter, string spritegroup)
        {
            if (weapons.Any(info => info.Name == name))
            {
                return;
            }
            WeaponInfo weapon = new WeaponInfo
            {
                Name = name,
                Entity = entity,
                Ammo = ammo,
                Max = ammo,
                Usage = usage,
                Meter = meter,
                SpriteGroup = spritegroup,
                Index = weapons.Count
            };

            weapons.Add(weapon);
        }

        public override void LoadXml(XElement node)
        {
            foreach (XElement weapon in node.Elements("Weapon"))
            {
                string name = weapon.RequireAttribute("name").Value;

                string entity = weapon.RequireAttribute("entity").Value;

                int ammo;
                if (!weapon.TryInteger("ammo", out ammo)) ammo = -1;

                int usage;
                if (!weapon.TryInteger("usage", out usage)) usage = 1;

                string spritegroup = "Default";
                XAttribute sprite = weapon.Attribute("pallete");
                if (sprite != null) spritegroup = sprite.Value;

                HealthMeter meter = null;
                XElement meterNode = weapon.Element("Meter");
                if (meterNode != null)
                {
                    meter = HealthMeter.Create(meterNode, true);

                    meter.MaxValue = ammo;
                    meter.Reset();
                }

                AddWeapon(name, entity, ammo, usage, meter, spritegroup);
            }
        }

        public static Effect ParseEffect(XElement node)
        {
            Effect effect = e => { };
            if (node.Value == "Shoot")
            {
                effect = entity =>
                {
                    WeaponComponent weaponComponent = entity.GetComponent<WeaponComponent>();
                    if (weaponComponent != null) weaponComponent.Shoot();
                };
            }
            else if (node.Value == "RotateForward")
            {
                effect = entity =>
                {
                    WeaponComponent weaponComponent = entity.GetComponent<WeaponComponent>();
                    if (weaponComponent != null) weaponComponent.RotateForward();
                };
            }
            else
            {
                XElement ammoNode = node.Element("Ammo");
                if (ammoNode != null)
                {
                    int val = int.Parse(ammoNode.RequireAttribute("val").Value);
                    effect = entity =>
                    {
                        WeaponComponent weaponComponent = entity.GetComponent<WeaponComponent>();
                        if (weaponComponent != null) weaponComponent.AddAmmo(val);
                    };
                }
            }
            return effect;
        }
    }
}
