using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Mega_Man
{
    public class PauseScreen : IHandleGameEvents
    {
        private class WeaponInfo
        {
            public Image iconOff, iconOn;
            public Texture2D textureOff, textureOn;
            public string name;
            public string entity;
            public Point location;
            public HealthMeter meter;
        }

        private WavEffect pauseSound;
        private WavEffect changeSound;
        private Image background;
        private Texture2D backgroundTexture;
        private List<WeaponInfo> weapons;
        private string selectedName;

        private Point currentPos;

        private WeaponComponent playerWeapons;

        private Font font;
        private Brush brush;

        private Point livesPos;
        private bool showLives;

        public event Action Unpaused;

        public PauseScreen(XElement reader)
        {
            weapons = new List<WeaponInfo>();

            changeSound = Engine.Instance.SoundSystem.EffectFromXml(reader.Element("ChangeSound"));
            pauseSound = Engine.Instance.SoundSystem.EffectFromXml(reader.Element("PauseSound"));

            background = Image.FromFile(System.IO.Path.Combine(Game.CurrentGame.BasePath, reader.Element("Background").Value));
			StreamReader sr = new StreamReader(System.IO.Path.Combine(Game.CurrentGame.BasePath, reader.Element("Background").Value));
			backgroundTexture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, sr.BaseStream);

            foreach (XElement weapon in reader.Elements("Weapon"))
                LoadWeapon(weapon);

            this.font = new Font(FontFamily.GenericMonospace, 12);
            this.brush = new SolidBrush(System.Drawing.Color.FromArgb(240, 236, 220));

            FontSystem.LoadFont("Big", System.IO.Path.Combine(Game.CurrentGame.BasePath, @"images\font.png"), 8, 0);

            XElement livesNode = reader.Element("Lives");
            if (livesNode != null)
            {
                showLives = true;
                int x=0, y=0;
                XAttribute livesXAttr = livesNode.Attribute("x");
                if (livesXAttr != null)
                {
                    if (!int.TryParse(livesXAttr.Value, out x)) throw new EntityXmlException(livesXAttr, "X position for Lives tag must be an integer.");
                }
                XAttribute livesYAttr = livesNode.Attribute("y");
                if (livesYAttr != null)
                {
                    if (!int.TryParse(livesYAttr.Value, out y)) throw new EntityXmlException(livesYAttr, "Y position for Lives tag must be an integer.");
                }
                livesPos = new Point(x, y);
            }
        }

        public void Sound()
        {
            pauseSound.Play();
        }

        private void LoadWeapon(XElement reader)
        {
            WeaponInfo info = new WeaponInfo();
            info.name = reader.Attribute("name").Value;
            info.entity = reader.Attribute("entity").Value;

			String imagePathOff = System.IO.Path.Combine(Game.CurrentGame.BasePath, reader.Attribute("off").Value);
			String imagePathOn = System.IO.Path.Combine(Game.CurrentGame.BasePath, reader.Attribute("on").Value);

            info.iconOff = Image.FromFile(System.IO.Path.Combine(Game.CurrentGame.BasePath, imagePathOff));
            info.iconOn = Image.FromFile(System.IO.Path.Combine(Game.CurrentGame.BasePath, imagePathOn));

			StreamReader srOff = new StreamReader(imagePathOff);
			StreamReader srOn = new StreamReader(imagePathOn);
            info.textureOff = Texture2D.FromStream(Engine.Instance.GraphicsDevice, srOff.BaseStream);
            info.textureOn = Texture2D.FromStream(Engine.Instance.GraphicsDevice, srOn.BaseStream);

            info.location = new Point(int.Parse(reader.Attribute("x").Value), int.Parse(reader.Attribute("y").Value));

            XElement meter = reader.Element("Meter");
            if (meter != null)
            {
                info.meter = HealthMeter.Create(meter, false);
            }

            weapons.Add(info);
        }

        #region IHandleGameEvents Members

        public void StartHandler()
        {
            Engine.Instance.GameInputReceived += new GameInputEventHandler(GameInputReceived);
            Engine.Instance.GameRender += new GameRenderEventHandler(GameRender);
            Game.CurrentGame.AddGameHandler(this);

            playerWeapons = Game.CurrentGame.CurrentMap.Player.GetComponent<WeaponComponent>();
            selectedName = playerWeapons.CurrentWeapon;

            foreach (WeaponInfo info in weapons)
            {
                if (info.entity == selectedName)
                {
                    currentPos = info.location;
                    break;
                }
            }

            foreach (WeaponInfo info in weapons)
            {
                if (info.meter != null)
                {
                    info.meter.Value = playerWeapons.Ammo(info.entity);
                    info.meter.MaxValue = playerWeapons.MaxAmmo(info.entity);
                }
            }
        }

        public void ApplyWeapon()
        {
            playerWeapons.SetWeapon(selectedName);
        }

        public void StopHandler()
        {
            Engine.Instance.GameInputReceived -= new GameInputEventHandler(GameInputReceived);
            Engine.Instance.GameRender -= new GameRenderEventHandler(GameRender);
            Game.CurrentGame.RemoveGameHandler(this);
        }

        public void GameInputReceived(GameInputEventArgs e)
        {
            if (!e.Pressed) return;

            string next = selectedName;
            Point nextPos = currentPos;
            int min = int.MaxValue;

            if (e.Input == GameInput.Start && e.Pressed)
            {
                if (Unpaused != null) Unpaused();
            }

            else if (e.Input == GameInput.Down)
            {
                foreach (WeaponInfo info in weapons)
                {
                    if (info.entity == selectedName) continue;

                    int ydist = info.location.Y - currentPos.Y;
                    if (ydist == 0) continue;

                    if (ydist < 0) ydist += Game.CurrentGame.PixelsDown;    // wrapping around bottom

                    // weight x distance worse than y distance
                    int dist = 2 * Math.Abs(info.location.X - currentPos.X) + ydist;
                    if (dist < min)
                    {
                        min = dist;
                        next = info.entity;
                        nextPos = info.location;
                    }
                }
            }
            else if (e.Input == GameInput.Up)
            {
                foreach (WeaponInfo info in weapons)
                {
                    if (info.entity == selectedName) continue;

                    int ydist = currentPos.Y - info.location.Y;
                    if (ydist == 0) continue;

                    if (ydist < 0) ydist += Game.CurrentGame.PixelsDown;    // wrapping around bottom

                    // weight x distance worse than y distance
                    int dist = 2 * Math.Abs(info.location.X - currentPos.X) + ydist;
                    if (dist < min)
                    {
                        min = dist;
                        next = info.entity;
                        nextPos = info.location;
                    }
                }
            }
            else if (e.Input == GameInput.Right)
            {
                foreach (WeaponInfo info in weapons)
                {
                    if (info.entity == selectedName) continue;

                    int xdist = info.location.X - currentPos.X;
                    if (xdist == 0) continue;

                    if (xdist < 0) xdist += Game.CurrentGame.PixelsAcross;    // wrapping around bottom

                    int dist = 2 * Math.Abs(info.location.Y - currentPos.Y) + xdist;
                    if (dist < min)
                    {
                        min = dist;
                        next = info.entity;
                        nextPos = info.location;
                    }
                }
            }
            else if (e.Input == GameInput.Left)
            {
                foreach (WeaponInfo info in weapons)
                {
                    if (info.entity == selectedName) continue;

                    int xdist = currentPos.X - info.location.X;
                    if (xdist == 0) continue;

                    if (xdist < 0) xdist += Game.CurrentGame.PixelsAcross;    // wrapping around bottom

                    int dist = 2 * Math.Abs(info.location.Y - currentPos.Y) + xdist;
                    if (dist < min)
                    {
                        min = dist;
                        next = info.entity;
                        nextPos = info.location;
                    }
                }
            }

            if (next != selectedName)
            {
                changeSound.Play();
                selectedName = next;
                currentPos = nextPos;
            }
        }

        public void GameRender(GameRenderEventArgs e)
        {
            if (!Engine.Instance.Foreground) return;

            e.Layers.ForegroundBatch.Draw(backgroundTexture, new Microsoft.Xna.Framework.Vector2(0, 0), e.OpacityColor);

            foreach (WeaponInfo info in weapons)
            {
                if (info.entity == selectedName)
                {
                    e.Layers.ForegroundBatch.Draw(info.textureOn, new Microsoft.Xna.Framework.Vector2(info.location.X, info.location.Y), e.OpacityColor);
                }
                else
                {
                    e.Layers.ForegroundBatch.Draw(info.textureOff, new Microsoft.Xna.Framework.Vector2(info.location.X, info.location.Y), e.OpacityColor);
                }

                FontSystem.Draw(e.Layers.ForegroundBatch, "Big", info.name, new PointF(info.location.X + 20, info.location.Y));

                if (info.meter != null)
                {
                    info.meter.Draw(e.Layers.ForegroundBatch);
                }
            }

            if (showLives)
            {
                FontSystem.Draw(e.Layers.ForegroundBatch, "Big", Game.CurrentGame.PlayerLives.ToString("D2"), livesPos);
            }
        }

        #endregion
    }
}
