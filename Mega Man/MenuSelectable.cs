using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using MegaMan.Common;
using System.IO;

namespace MegaMan.Engine
{
    public interface IMenuSelectable
    {
        string Name { get; }
        Point Location { get; }
        bool Selectable { get; }
        void Init();
        void Draw(SpriteBatch batch, Microsoft.Xna.Framework.Color color, bool highlight);
        bool Select();
    }

    public class MenuWeapon : IMenuSelectable
    {
        private IGameplayContainer container;

        private Texture2D textureOff, textureOn;
        private string text;
        private string weapon;
        private Point location;
        private HealthMeter meter;

        public string Name { get { return weapon; } }
        public Point Location { get { return location; } }
        public bool Selectable { get { return true; } }

        public MenuWeapon(PauseWeaponInfo info, IGameplayContainer container)
        {
            this.container = container;

            text = info.Name;
            weapon = info.Weapon;

            string imagePathOff = info.IconOff.Absolute;
            string imagePathOn = info.IconOn.Absolute;

            StreamReader srOff = new StreamReader(imagePathOff);
            StreamReader srOn = new StreamReader(imagePathOn);
            textureOff = Texture2D.FromStream(Engine.Instance.GraphicsDevice, srOff.BaseStream);
            textureOn = Texture2D.FromStream(Engine.Instance.GraphicsDevice, srOn.BaseStream);

            location = info.Location;

            if (info.Meter != null)
            {
                meter = HealthMeter.Create(info.Meter, false, container);
            }
        }

        public void Init()
        {
            if (meter == null) return;

            float val = 1f;
            float max = 1f;

            if (container.Player != null)
            {
                var playerWeapons = container.Player.GetComponent<WeaponComponent>();
                val = playerWeapons.Ammo(weapon);
                max = playerWeapons.MaxAmmo(weapon);
            }

            meter.Value = val;
            meter.MaxValue = max;
        }

        public void Draw(SpriteBatch batch, Microsoft.Xna.Framework.Color color, bool selected)
        {
            batch.Draw(selected ? textureOn : textureOff,
                new Microsoft.Xna.Framework.Vector2(Location.X, Location.Y),
                color);

            FontSystem.Draw(batch, "Big", text, new PointF(Location.X + 20, Location.Y));

            if (meter != null)
            {
                meter.Draw(batch);
            }
        }

        public bool Select()
        {
            if (container.Player != null)
            {
                var playerWeapons = container.Player.GetComponent<WeaponComponent>();
                playerWeapons.SetWeapon(weapon);
            }
            return true;
        }
    }

    public class MenuInventory : IMenuSelectable
    {
        private IGameplayContainer container;

        private Texture2D textureOff, textureOn;
        private Point iconLocation, numberLocation;
        private string useFunc;

        public string Name { get; private set; }
        public Point Location { get { return iconLocation; } }
        public bool Selectable { get; private set; }

        public MenuInventory(InventoryInfo info, IGameplayContainer container)
        {
            this.container = container;

            Name = info.Name;
            useFunc = info.UseFunction;

            string imagePathOff = info.IconOff.Absolute;
            string imagePathOn = info.IconOn.Absolute;

            StreamReader srOff = new StreamReader(imagePathOff);
            StreamReader srOn = new StreamReader(imagePathOn);
            textureOff = Texture2D.FromStream(Engine.Instance.GraphicsDevice, srOff.BaseStream);
            textureOn = Texture2D.FromStream(Engine.Instance.GraphicsDevice, srOn.BaseStream);

            iconLocation = info.IconLocation;
            numberLocation = info.NumberLocation;
            Selectable = info.Selectable;
        }

        public void Init()
        {
        }

        public void Draw(SpriteBatch batch, Microsoft.Xna.Framework.Color color, bool highlight)
        {
            batch.Draw(highlight ? textureOn : textureOff,
                new Microsoft.Xna.Framework.Vector2(Location.X, Location.Y),
                color);

            var quantity = Game.CurrentGame.Player.ItemQuantity(Name);
            FontSystem.Draw(batch, "Big", quantity.ToString("D2"), numberLocation);
        }

        public bool Select()
        {
            if (Selectable && useFunc != null && container.Player != null && Game.CurrentGame.Player.UseItem(Name))
            {
                EffectParser.GetEffect(useFunc)(container.Player);
            }
            return false;
        }
    }
}
