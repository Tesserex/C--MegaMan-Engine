﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Mega_Man
{
    /// <summary>
    /// It's a stage select screen, just like it says.
    /// </summary>
    public class StageSelect : IHandleGameEvents
    {
        private class BossInfo
        {
            public string firstname;
            public string lastname;
            public Image portrait;
            public Texture2D texture;
            public string stage;
            public bool alive = false;
            public Point location;
        }

        private Music musicStageSelect;
        private SoundEffect changeSound;
        private Texture2D backgroundTexture;
        private MegaMan.Sprite bossFrameOn, bossFrameOff;
        private BossInfo[] bosses;
        private int selectedIndex;

        private int spacingX, spacingY, offsetY;

        public event Action<string> MapSelected;

        public StageSelect(XElement reader)
        {
            bosses = new BossInfo[8];
            for (int i = 0; i < 8; i++) bosses[i] = new BossInfo();

            XElement spriteNode = reader.Element("BossFrame").Element("Sprite");
            XAttribute tileattr = spriteNode.Attribute("tilesheet");
            bossFrameOn = MegaMan.Sprite.FromXml(spriteNode, Game.CurrentGame.BasePath);
            bossFrameOn.SetTexture(Engine.Instance.GraphicsDevice, System.IO.Path.Combine(Game.CurrentGame.BasePath, tileattr.Value));
            bossFrameOff = new MegaMan.Sprite(bossFrameOn);

            bossFrameOn.Play();

            int portraitWidth = bossFrameOn.Width;
            int portraitHeight = bossFrameOn.Height;

            spacingX = spacingY = 24;
            offsetY = 0;

            XElement spaceNode = reader.Element("Spacing");
            if (spaceNode != null)
            {
                XAttribute spacexAttr = spaceNode.Attribute("x");
                if (spacexAttr == null) throw new EntityXmlException(spaceNode, "StageSelect spacing must have an x and y attribute");
                if (!int.TryParse(spacexAttr.Value, out spacingX)) throw new EntityXmlException(spacexAttr, "Spacing attributes must be integers.");
                XAttribute spaceyAttr = spaceNode.Attribute("y");
                if (spaceyAttr == null) throw new EntityXmlException(spaceNode, "StageSelect spacing must have an x and y attribute");
                if (!int.TryParse(spaceyAttr.Value, out spacingY)) throw new EntityXmlException(spaceyAttr, "Spacing attributes must be integers.");
                XAttribute offsetAttr = spaceNode.Attribute("offset");
                if (offsetAttr != null && !int.TryParse(offsetAttr.Value, out offsetY)) throw new EntityXmlException(offsetAttr, "Spacing attributes must be integers.");
            }

            int middleX = (Game.CurrentGame.PixelsAcross - portraitWidth) / 2;
            int middleY = (Game.CurrentGame.PixelsDown - portraitHeight) / 2 + offsetY;

            int lowerX = middleX - portraitWidth - spacingX;
            int lowerY = middleY - portraitHeight - spacingY;
            int upperX = middleX + portraitWidth + spacingX;
            int upperY = middleY + portraitHeight + spacingY;

            bosses[0].location = new Point(lowerX, lowerY);
            bosses[1].location = new Point(middleX, lowerY);
            bosses[2].location = new Point(upperX, lowerY);
            bosses[7].location = new Point(lowerX, middleY);
            //bosses[0].location = new Point(middleX, middleY);
            bosses[3].location = new Point(upperX, middleY);
            bosses[6].location = new Point(lowerX, upperY);
            bosses[5].location = new Point(middleX, upperY);
            bosses[4].location = new Point(upperX, upperY);

            selectedIndex = 0;

            var musicElement = reader.Element("Music");
            if (musicElement != null) musicStageSelect = Engine.Instance.SoundSystem.LoadMusic(null, System.IO.Path.Combine(Game.CurrentGame.BasePath, musicElement.Value), 1);

            var soundElement = reader.Element("ChangeSound");
            if (soundElement != null) changeSound = Engine.Instance.SoundSystem.EffectFromXml(soundElement);
            
            backgroundTexture = Texture2D.FromFile(Engine.Instance.GraphicsDevice, System.IO.Path.Combine(Game.CurrentGame.BasePath, reader.Element("Background").Value));

            foreach (XElement boss in reader.Elements("Boss"))
                LoadBoss(boss);

            FontSystem.LoadFont("Boss", System.IO.Path.Combine(Game.CurrentGame.BasePath, "images\\font_boss.png"), 8, 0);
        }

        private void LoadBoss(XElement reader)
        {
            int slot;
            XAttribute slotAttr = reader.Attribute("slot");
            if (slotAttr == null) return;
            if (!int.TryParse(slotAttr.Value, out slot) || slot < 0) throw new EntityXmlException(slotAttr, "Slot attribute must be a non-negative integer.");

            XAttribute nameNode = reader.Attribute("name");
            if (nameNode != null)
            {
                string[] names = nameNode.Value.Split(' ');
                if (names.Length > 0) bosses[slot].firstname = names[0];
                if (names.Length > 1) bosses[slot].lastname = names[1];
            }

            XAttribute portraitNode = reader.Attribute("portrait");
            if (portraitNode != null)
            {
                bosses[slot].portrait = Image.FromFile(System.IO.Path.Combine(Game.CurrentGame.BasePath, portraitNode.Value));
                bosses[slot].texture = Texture2D.FromFile(Engine.Instance.GraphicsDevice, System.IO.Path.Combine(Game.CurrentGame.BasePath, portraitNode.Value));
            }

            XAttribute stageNode = reader.Attribute("stage");
            if (stageNode == null) return;
            bosses[slot].stage = stageNode.Value;
            bosses[slot].alive = true;
        }

        #region IHandleGameEvents Members

        public void StartHandler()
        {
            Engine.Instance.GameInputReceived += new GameInputEventHandler(GameInputReceived);
            Engine.Instance.GameLogicTick += new GameTickEventHandler(GameTick);
            Engine.Instance.GameRender += new GameRenderEventHandler(GameRender);
            if (musicStageSelect != null) musicStageSelect.Play();
            Game.CurrentGame.AddGameHandler(this);
        }

        public void StopHandler()
        {
            Engine.Instance.GameInputReceived -= new GameInputEventHandler(GameInputReceived);
            Engine.Instance.GameLogicTick -= new GameTickEventHandler(GameTick);
            Engine.Instance.GameRender -= new GameRenderEventHandler(GameRender);
            if (musicStageSelect != null) musicStageSelect.Stop();
            Game.CurrentGame.RemoveGameHandler(this);
        }

        public void GameInputReceived(GameInputEventArgs e)
        {
            if (!e.Pressed) return;

            int old = selectedIndex;
            if (e.Input == GameInput.Left)
            {
                if (selectedIndex == 1 || selectedIndex == 2) selectedIndex--;
                else if (selectedIndex == 4 || selectedIndex == 5) selectedIndex++;
            }
            else if (e.Input == GameInput.Right)
            {
                if (selectedIndex == 5 || selectedIndex == 6) selectedIndex--;
                else if (selectedIndex == 0 || selectedIndex == 1) selectedIndex++;
            }
            else if (e.Input == GameInput.Down)
            {
                if (selectedIndex == 7) selectedIndex--;
                else if (selectedIndex == 0) selectedIndex = 7;
                else if (selectedIndex == 2 || selectedIndex == 3) selectedIndex++;
            }
            else if (e.Input == GameInput.Up)
            {
                if (selectedIndex == 6) selectedIndex++;
                else if (selectedIndex == 7) selectedIndex = 0;
                else if (selectedIndex == 4 || selectedIndex == 3) selectedIndex--;
            }
            else if (e.Input == GameInput.Start)
            {
                if (MapSelected != null && bosses[selectedIndex].stage != null) MapSelected(bosses[selectedIndex].stage);
            }
            if (selectedIndex != old && changeSound != null) changeSound.Play();
        }

        public void GameTick(GameTickEventArgs e)
        {
            bossFrameOn.Update();
        }

        public void GameRender(GameRenderEventArgs e)
        {
            e.Layers.BackgroundBatch.Draw(backgroundTexture, new Microsoft.Xna.Framework.Vector2(0, 0), e.OpacityColor);

            BossInfo boss;
            for (int i = 0; i < 8; i++)
            {
                boss = bosses[i];
                if (selectedIndex == i)
                {
                    bossFrameOn.DrawXna(e.Layers.SpritesBatch[0], e.OpacityColor, boss.location.X, boss.location.Y);
                }
                else
                {
                    bossFrameOff.DrawXna(e.Layers.SpritesBatch[0], e.OpacityColor, boss.location.X, boss.location.Y);
                }

                if (boss.alive && boss.portrait != null)
                {
                    e.Layers.SpritesBatch[0].Draw(boss.texture, new Microsoft.Xna.Framework.Vector2(boss.location.X + 7, boss.location.Y + 7), e.OpacityColor);
                }

                if (boss.firstname != null)
                {
                    FontSystem.Draw(e.Layers.SpritesBatch[2], "Boss", boss.firstname, new PointF(boss.location.X, boss.location.Y + 48));
                }
                if (boss.lastname != null)
                {
                    FontSystem.Draw(e.Layers.SpritesBatch[2], "Boss", boss.lastname, new PointF(boss.location.X + (44 - boss.lastname.Length * 7), boss.location.Y + 56));
                }
            }
        }

        #endregion
    }
}
