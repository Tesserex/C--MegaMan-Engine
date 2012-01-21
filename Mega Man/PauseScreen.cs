using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using MegaMan.Common;

namespace MegaMan.Engine
{
    public class PauseScreen : IHandleGameEvents
    {
        private readonly string pauseSound;
        private readonly string changeSound;
        private readonly Texture2D backgroundTexture;
        private readonly Dictionary<string, IMenuSelectable> selectables;
        private string selectedName;
        private IGameplayContainer container;

        private Point currentPos;

        private WeaponComponent playerWeapons;

        private readonly Point livesPos;
        private readonly bool showLives;

        public event Action<HandlerTransfer> End;

        public PauseScreen(MegaMan.Common.PauseScreen pauseInfo, IGameplayContainer container)
        {
            this.container = container;
            selectables = new Dictionary<string, IMenuSelectable>();

            this.playerWeapons = container.Player.GetComponent<WeaponComponent>();

            if (pauseInfo.ChangeSound != null) changeSound = Engine.Instance.SoundSystem.EffectFromInfo(pauseInfo.ChangeSound);
            if (pauseInfo.PauseSound != null) pauseSound = Engine.Instance.SoundSystem.EffectFromInfo(pauseInfo.PauseSound);

            Image.FromFile(pauseInfo.Background.Absolute);
            StreamReader sr = new StreamReader(pauseInfo.Background.Absolute);
            backgroundTexture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, sr.BaseStream);

            foreach (var weaponInfo in pauseInfo.Weapons)
            {
                var item = new MenuWeapon(weaponInfo, container);
                selectables.Add(item.Name, item);
            }

            foreach (var inventory in pauseInfo.Inventory)
            {
                var item = new MenuInventory(inventory, container);
                selectables.Add(item.Name, item);
            }

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

        #region IHandleGameEvents Members

        public void StartHandler()
        {
            Engine.Instance.GameInputReceived += GameInputReceived;
            Engine.Instance.GameRender += GameRender;

            selectedName = playerWeapons.CurrentWeapon;

            foreach (var info in selectables.Values)
            {
                if (info.Name == selectedName)
                {
                    currentPos = info.Location;
                    break;
                }
            }

            foreach (var item in selectables.Values)
            {
                item.Init();
            }
        }

        public void StopHandler()
        {
            Engine.Instance.GameInputReceived -= GameInputReceived;
            Engine.Instance.GameRender -= GameRender;
        }

        public void GameInputReceived(GameInputEventArgs e)
        {
            if (!e.Pressed) return;

            string next = selectedName;
            Point nextPos = currentPos;
            int min = int.MaxValue;

            if (e.Input == GameInput.Start)
            {
                bool close = selectables[selectedName].Select();
                if (close && End != null) End(null);
            }
            else if (e.Input == GameInput.Down)
            {
                foreach (var info in selectables.Values)
                {
                    if (info.Name == selectedName) continue;

                    int ydist = info.Location.Y - currentPos.Y;
                    if (ydist == 0) continue;

                    if (ydist < 0) ydist += Game.CurrentGame.PixelsDown;    // wrapping around bottom

                    // weight x distance worse than y distance
                    int dist = 2 * Math.Abs(info.Location.X - currentPos.X) + ydist;
                    if (dist < min)
                    {
                        min = dist;
                        next = info.Name;
                        nextPos = info.Location;
                    }
                }
            }
            else if (e.Input == GameInput.Up)
            {
                foreach (var info in selectables.Values)
                {
                    if (info.Name == selectedName) continue;

                    int ydist = currentPos.Y - info.Location.Y;
                    if (ydist == 0) continue;

                    if (ydist < 0) ydist += Game.CurrentGame.PixelsDown;    // wrapping around bottom

                    // weight x distance worse than y distance
                    int dist = 2 * Math.Abs(info.Location.X - currentPos.X) + ydist;
                    if (dist < min)
                    {
                        min = dist;
                        next = info.Name;
                        nextPos = info.Location;
                    }
                }
            }
            else if (e.Input == GameInput.Right)
            {
                foreach (var info in selectables.Values)
                {
                    if (info.Name == selectedName) continue;

                    int xdist = info.Location.X - currentPos.X;
                    if (xdist == 0) continue;

                    if (xdist < 0) xdist += Game.CurrentGame.PixelsAcross;    // wrapping around bottom

                    int dist = 2 * Math.Abs(info.Location.Y - currentPos.Y) + xdist;
                    if (dist < min)
                    {
                        min = dist;
                        next = info.Name;
                        nextPos = info.Location;
                    }
                }
            }
            else if (e.Input == GameInput.Left)
            {
                foreach (var info in selectables.Values)
                {
                    if (info.Name == selectedName) continue;

                    int xdist = currentPos.X - info.Location.X;
                    if (xdist == 0) continue;

                    if (xdist < 0) xdist += Game.CurrentGame.PixelsAcross;    // wrapping around bottom

                    int dist = 2 * Math.Abs(info.Location.Y - currentPos.Y) + xdist;
                    if (dist < min)
                    {
                        min = dist;
                        next = info.Name;
                        nextPos = info.Location;
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

            foreach (var info in selectables.Values)
            {
                info.Draw(e.Layers.ForegroundBatch, e.OpacityColor, info.Name == selectedName);
            }

            if (showLives)
            {
                FontSystem.Draw(e.Layers.ForegroundBatch, "Big", Game.CurrentGame.Player.Lives.ToString("D2"), livesPos);
            }
        }

        #endregion
    }
}
