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
        void Init();
        void Draw(SpriteBatch batch, Microsoft.Xna.Framework.Color color, bool selected);
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

        public MenuWeapon(PauseWeaponInfo info, IGameplayContainer container)
        {
            this.container = container;

            text = info.Name;
            weapon = info.Weapon;

            string imagePathOff = info.IconOff.Absolute;
            string imagePathOn = info.IconOn.Absolute;

            Image.FromFile(imagePathOff);
            Image.FromFile(imagePathOn);

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
    }
}
