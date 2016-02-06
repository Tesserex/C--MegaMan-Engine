using System.Collections.Generic;
using System.Linq;
using System;
using MegaMan.Common.Entities;

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
            public int? Palette;
        }

        private List<WeaponInfo> weapons = new List<WeaponInfo>();
        private int current;

        public event Action<string, int, int> AmmoChanged;
        private IGameplayContainer _container;

        public string CurrentWeapon { get { return weapons[current].Name; } }

        public int Ammo(string weapon)
        {
            var info = weapons.SingleOrDefault(w => w.Name == weapon);
            if (info != null)
            {
                if (info.Palette == 0) return (int)(Parent.GetComponent<HealthComponent>()).Health;
                return info.Ammo;
            }
            return 0;
        }

        public int MaxAmmo(string weapon)
        {
            var info = weapons.SingleOrDefault(w => w.Name == weapon);
            if (info != null)
            {
                if (info.Palette == 0) return (int)(Parent.GetComponent<HealthComponent>()).MaxHealth;
                return info.Max;
            }
            return 0;
        }

        public void SetWeapon(string name)
        {
            var weapon = weapons.SingleOrDefault(w => w.Name == name);
            if (weapon != null)
            {
                if (weapons[current].Meter != null)
                {
                    weapons[current].Meter.Stop();
                }

                current = weapon.Index;
                ApplyCurrent();
            }
        }

        public override Component Clone()
        {
            WeaponComponent copy = new WeaponComponent {weapons = weapons, current = 0};
            return copy;
        }

        public override void Start(IGameplayContainer container)
        {
            _container = container;
        }

        public override void Stop(IGameplayContainer container)
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
                weapons[current].Meter.Stop();
            }

            current++;
            if (current >= weapons.Count) current = 0;

            ApplyCurrent();
        }

        public void RotateBackward()
        {
            if (weapons[current].Meter != null)
            {
                weapons[current].Meter.Stop();
            }

            current--;
            if (current < 0) current = weapons.Count - 1;

            ApplyCurrent();
        }

        private void ApplyCurrent()
        {
            if (weapons[current].Palette.HasValue)
            {
                SpriteComponent sprites = Parent.GetComponent<SpriteComponent>();
                if (sprites != null)
                {
                    sprites.ChangePalette(weapons[current].Palette.Value);
                }
            }

            if (weapons[current].Meter != null)
            {
                weapons[current].Meter.Start(_container);
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

            if (AmmoChanged != null)
            {
                AmmoChanged(weapons[current].Name, weapons[current].Ammo, weapons[current].Max);
            }
        }

        public void AddWeapon(string name, string entity, int ammo, int usage, HealthMeter meter, int? palette)
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
                Palette = palette,
                Index = weapons.Count
            };

            weapons.Add(weapon);
        }

        public void LoadInfo(WeaponComponentInfo info)
        {
            foreach (var weapon in info.Weapons)
            {
                HealthMeter meter = null;
                if (weapon.Meter != null)
                    meter = HealthMeter.Create(weapon.Meter, true);

                AddWeapon(weapon.Name, weapon.EntityName, weapon.Ammo ?? -1, weapon.Usage ?? 1, meter, weapon.Palette);
            }
        }
    }
}
