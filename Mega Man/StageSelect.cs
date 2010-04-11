using System;
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
            public Image portrait;
            public Texture2D texture;
            public string mapPath;
            public bool alive = false;
            public Point location;
        }

        private int musicStageSelect;
        private int changeSound;
        private Texture2D backgroundTexture;
        private MegaMan.Sprite bossFrameOn, bossFrameOff;
        private BossInfo[] bosses;
        private int selectedIndex;

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

            int middleX = (Game.CurrentGame.PixelsAcross - portraitWidth) / 2;
            int middleY = (Game.CurrentGame.PixelsDown - portraitHeight) / 2;
            int padding = 24;

            int lowerX = middleX - portraitWidth - padding;
            int lowerY = middleY - portraitHeight - padding;
            int upperX = middleX + portraitWidth + padding;
            int upperY = middleY + portraitHeight + padding;

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

            musicStageSelect = Engine.Instance.LoadMusic(null, System.IO.Path.Combine(Game.CurrentGame.BasePath, reader.Element("Music").Value));

            changeSound = Engine.Instance.LoadSoundEffect(System.IO.Path.Combine(Game.CurrentGame.BasePath, reader.Element("ChangeSound").Value), false);
            
            backgroundTexture = Texture2D.FromFile(Engine.Instance.GraphicsDevice, System.IO.Path.Combine(Game.CurrentGame.BasePath, reader.Element("Background").Value));

            foreach (XElement boss in reader.Elements("Boss"))
                LoadBoss(boss);
        }

        private void LoadBoss(XElement reader)
        {
            int slot;
            XAttribute slotAttr = reader.Attribute("slot");
            if (slotAttr == null) throw new EntityXmlException(reader, "Boss must specify a \"slot\" attribute!");
            if (!int.TryParse(slotAttr.Value, out slot) || slot < 0) throw new EntityXmlException(slotAttr, "Slot attribute must be a non-negative integer.");

            XElement portraitNode = reader.Element("Portrait");
            if (portraitNode != null)
            {
                bosses[slot].portrait = Image.FromFile(System.IO.Path.Combine(Game.CurrentGame.BasePath, portraitNode.Value));
                bosses[slot].texture = Texture2D.FromFile(Engine.Instance.GraphicsDevice, System.IO.Path.Combine(Game.CurrentGame.BasePath, portraitNode.Value));
            }

            XElement stageNode = reader.Element("Stage");
            if (stageNode == null) throw new EntityXmlException(reader, "Boss must specify a stage!");
            bosses[slot].mapPath = System.IO.Path.Combine(Game.CurrentGame.BasePath, stageNode.Value);
            bosses[slot].alive = true;
        }

        #region IHandleGameEvents Members

        public void StartHandler()
        {
            Engine.Instance.GameInputReceived += new GameInputEventHandler(GameInputReceived);
            Engine.Instance.GameLogicTick += new GameTickEventHandler(GameTick);
            Engine.Instance.GameRender += new GameRenderEventHandler(GameRender);
            Engine.Instance.PlayMusic(musicStageSelect);
            Game.CurrentGame.AddGameHandler(this);
        }

        public void StopHandler()
        {
            Engine.Instance.GameInputReceived -= new GameInputEventHandler(GameInputReceived);
            Engine.Instance.GameLogicTick -= new GameTickEventHandler(GameTick);
            Engine.Instance.GameRender -= new GameRenderEventHandler(GameRender);
            Engine.Instance.StopMusic(musicStageSelect);
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
                if (MapSelected != null) MapSelected(bosses[selectedIndex].mapPath);
            }
            if (selectedIndex != old) Engine.Instance.PlaySound(changeSound);
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
            }
        }

        #endregion
    }
}
