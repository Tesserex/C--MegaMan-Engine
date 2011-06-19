using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Mega_Man
{
    public class PauseScreen : IHandleGameEvents
    {
        private class PauseWeapon
        {
            public Texture2D textureOff, textureOn;
            public string name;
            public string entity;
            public Point location;
            public HealthMeter meter;
        }

        private readonly string pauseSound;
        private readonly string changeSound;
        private readonly Texture2D backgroundTexture;
        private readonly List<PauseWeapon> weapons;
        private string selectedName;

        private Point currentPos;

        private WeaponComponent playerWeapons;

        private readonly Point livesPos;
        private readonly bool showLives;

        public event Action Unpaused;

        public PauseScreen(MegaMan.PauseScreen pauseInfo)
        {
            weapons = new List<PauseWeapon>();

            if (pauseInfo.ChangeSound != null) changeSound = Engine.Instance.SoundSystem.EffectFromInfo(pauseInfo.ChangeSound);
            if (pauseInfo.PauseSound != null) pauseSound = Engine.Instance.SoundSystem.EffectFromInfo(pauseInfo.PauseSound);

            Image.FromFile(pauseInfo.Background.Absolute);
            StreamReader sr = new StreamReader(pauseInfo.Background.Absolute);
            backgroundTexture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, sr.BaseStream);

            foreach (var weaponInfo in pauseInfo.Weapons)
                LoadWeapon(weaponInfo);

            FontSystem.LoadFont("Big", Path.Combine(Game.CurrentGame.BasePath, @"images\font.png"), 8, 0);

            if (pauseInfo.LivesPosition != Point.Empty)
            {
                showLives = true;
                livesPos = pauseInfo.LivesPosition;
            }
        }

        public void Sound()
        {
            if (pauseSound != null) Engine.Instance.SoundSystem.PlaySfx(pauseSound);
        }

        private void LoadWeapon(MegaMan.WeaponInfo weapon)
        {
            PauseWeapon info = new PauseWeapon {name = weapon.Name, entity = weapon.Entity};

            string imagePathOff = weapon.IconOff.Absolute;
            string imagePathOn = weapon.IconOn.Absolute;

            Image.FromFile(imagePathOff);
            Image.FromFile(imagePathOn);

            StreamReader srOff = new StreamReader(imagePathOff);
            StreamReader srOn = new StreamReader(imagePathOn);
            info.textureOff = Texture2D.FromStream(Engine.Instance.GraphicsDevice, srOff.BaseStream);
            info.textureOn = Texture2D.FromStream(Engine.Instance.GraphicsDevice, srOn.BaseStream);

            info.location = weapon.Location;

            if (weapon.Meter != null)
            {
                info.meter = HealthMeter.Create(weapon.Meter, false);
            }

            weapons.Add(info);
        }

        #region IHandleGameEvents Members

        public void StartHandler()
        {
            Engine.Instance.GameInputReceived += GameInputReceived;
            Engine.Instance.GameRender += GameRender;
            Game.CurrentGame.AddGameHandler(this);

            playerWeapons = Game.CurrentGame.CurrentMap.Player.GetComponent<WeaponComponent>();
            selectedName = playerWeapons.CurrentWeapon;

            foreach (PauseWeapon info in weapons)
            {
                if (info.entity == selectedName)
                {
                    currentPos = info.location;
                    break;
                }
            }

            foreach (PauseWeapon info in weapons)
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
            Engine.Instance.GameInputReceived -= GameInputReceived;
            Engine.Instance.GameRender -= GameRender;
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
                foreach (PauseWeapon info in weapons)
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
                foreach (PauseWeapon info in weapons)
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
                foreach (PauseWeapon info in weapons)
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
                foreach (PauseWeapon info in weapons)
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
                if (changeSound != null) Engine.Instance.SoundSystem.PlaySfx(changeSound);
                selectedName = next;
                currentPos = nextPos;
            }
        }

        public void GameRender(GameRenderEventArgs e)
        {
            if (!Engine.Instance.Foreground) return;

            e.Layers.ForegroundBatch.Draw(backgroundTexture, new Microsoft.Xna.Framework.Vector2(0, 0), e.OpacityColor);

            foreach (PauseWeapon info in weapons)
            {
                e.Layers.ForegroundBatch.Draw(info.entity == selectedName ? info.textureOn : info.textureOff,
                                              new Microsoft.Xna.Framework.Vector2(info.location.X, info.location.Y),
                                              e.OpacityColor);

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
