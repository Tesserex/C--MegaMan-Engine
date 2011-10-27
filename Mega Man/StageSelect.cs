using System;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using MegaMan.Common;
using System.Xml;

namespace MegaMan.Engine
{
    /// <summary>
    /// It's a stage select screen, just like it says.
    /// </summary>
    public class StageSelect : IHandleGameEvents
    {
        private class BossSlot
        {
            public string firstname;
            public string lastname;
            public Image portrait;
            public Texture2D texture;
            public string stage;
            public bool alive;
            public Point location;
            public string scene;
        }

        private readonly MegaMan.Common.StageSelect stageSelectInfo;
        private readonly Music musicStageSelect;
        private readonly string changeSound;
        private readonly Texture2D backgroundTexture;
        private readonly Sprite bossFrameOn;
        private readonly Sprite bossFrameOff;
        private readonly BossSlot[] bosses;
        private int selectedIndex;

        public HandlerTransfer NextHandler { get; private set; }

        public event Action End;

        public StageSelect(MegaMan.Common.StageSelect stageSelectInfo)
        {
            this.stageSelectInfo = stageSelectInfo;

            bosses = new BossSlot[8];
            for (int i = 0; i < 8; i++) bosses[i] = new BossSlot();

            bossFrameOn = new Sprite(stageSelectInfo.BossFrame);
            bossFrameOn.SetTexture(Engine.Instance.GraphicsDevice, stageSelectInfo.BossFrame.SheetPath.Absolute);
            bossFrameOff = new Sprite(bossFrameOn);

            bossFrameOn.Play();

            int portraitWidth = bossFrameOn.Width;
            int portraitHeight = bossFrameOn.Height;

            int middleX = (Game.CurrentGame.PixelsAcross - portraitWidth) / 2;
            int middleY = (Game.CurrentGame.PixelsDown - portraitHeight) / 2 + stageSelectInfo.BossOffset;

            int lowerX = middleX - portraitWidth - stageSelectInfo.BossSpacingHorizontal;
            int lowerY = middleY - portraitHeight - stageSelectInfo.BossSpacingVertical;
            int upperX = middleX + portraitWidth + stageSelectInfo.BossSpacingHorizontal;
            int upperY = middleY + portraitHeight + stageSelectInfo.BossSpacingVertical;

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

            if (stageSelectInfo.Music != null)
            {
                if (stageSelectInfo.Music.Type == AudioType.Wav)
                {
                    musicStageSelect = Engine.Instance.SoundSystem.LoadMusic(stageSelectInfo.Music.IntroPath.Absolute, stageSelectInfo.Music.LoopPath.Absolute, 1);
                }
            }

            if (stageSelectInfo.ChangeSound != null) changeSound = Engine.Instance.SoundSystem.EffectFromInfo(stageSelectInfo.ChangeSound);

            StreamReader sr = new StreamReader(stageSelectInfo.Background.Absolute);
            backgroundTexture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, sr.BaseStream);

            foreach (BossInfo boss in stageSelectInfo.Bosses)
            {
                LoadBoss(boss);
            }

            FontSystem.LoadFont("Boss", Path.Combine(Game.CurrentGame.BasePath, "images\\font_boss.png"), 8, 0);
        }

        private void LoadBoss(BossInfo boss)
        {
            int slot = boss.Slot;

            if (boss.Name != null)
            {
                string[] names = boss.Name.Split(' ');
                if (names.Length > 0) bosses[slot].firstname = names[0];
                if (names.Length > 1) bosses[slot].lastname = names[1];
            }

            if (boss.PortraitPath != null)
            {
                StreamReader sr = new StreamReader(boss.PortraitPath.Absolute);
                bosses[slot].portrait = Image.FromFile(boss.PortraitPath.Absolute);
                bosses[slot].texture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, sr.BaseStream);
            }

            bosses[slot].stage = boss.Stage;
            bosses[slot].alive = true;
            bosses[slot].scene = boss.Scene;
        }

        #region IHandleGameEvents Members

        public void StartHandler()
        {
            Engine.Instance.GameInputReceived += GameInputReceived;
            Engine.Instance.GameLogicTick += GameTick;
            Engine.Instance.GameRender += GameRender;

            if (stageSelectInfo.Music.Type == AudioType.NSF) Engine.Instance.SoundSystem.PlayMusicNSF((uint)stageSelectInfo.Music.NsfTrack);
            else if (musicStageSelect != null) musicStageSelect.Play();
        }

        public void StopHandler()
        {
            Engine.Instance.GameInputReceived -= GameInputReceived;
            Engine.Instance.GameLogicTick -= GameTick;
            Engine.Instance.GameRender -= GameRender;

            if (stageSelectInfo.Music.Type == AudioType.NSF) Engine.Instance.SoundSystem.StopMusicNsf();
            else if (musicStageSelect != null) musicStageSelect.Stop();
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
                SelectStage();
                return;
            }
            if (selectedIndex != old && changeSound != null) Engine.Instance.SoundSystem.PlaySfx(changeSound);
        }

        private void GameTick(GameTickEventArgs e)
        {
            bossFrameOn.Update();
        }

        private void GameRender(GameRenderEventArgs e)
        {
            if (Engine.Instance.Background) e.Layers.BackgroundBatch.Draw(backgroundTexture, new Microsoft.Xna.Framework.Vector2(0, 0), e.OpacityColor);

            BossSlot boss;
            for (int i = 0; i < 8; i++)
            {
                boss = bosses[i];
                if (selectedIndex == i)
                {
                    if (Engine.Instance.SpritesOne) bossFrameOn.DrawXna(e.Layers.SpritesBatch[0], e.OpacityColor, boss.location.X, boss.location.Y);
                }
                else
                {
                    if (Engine.Instance.SpritesOne) bossFrameOff.DrawXna(e.Layers.SpritesBatch[0], e.OpacityColor, boss.location.X, boss.location.Y);
                }

                if (boss.alive && boss.portrait != null)
                {
                    if (Engine.Instance.SpritesOne) e.Layers.SpritesBatch[0].Draw(boss.texture, new Microsoft.Xna.Framework.Vector2(boss.location.X + 7, boss.location.Y + 7), e.OpacityColor);
                }

                if (boss.firstname != null)
                {
                    if (Engine.Instance.SpritesThree) FontSystem.Draw(e.Layers.SpritesBatch[2], "Boss", boss.firstname, new PointF(boss.location.X, boss.location.Y + 48));
                }
                if (boss.lastname != null)
                {
                    if (Engine.Instance.SpritesThree) FontSystem.Draw(e.Layers.SpritesBatch[2], "Boss", boss.lastname, new PointF(boss.location.X + (44 - boss.lastname.Length * 7), boss.location.Y + 56));
                }
            }
        }

        private void SelectStage()
        {
            if (End != null && bosses[selectedIndex].stage != null)
            {
                NextHandler = new HandlerTransfer();
                if (bosses[selectedIndex].scene != null)
                {
                    NextHandler.Type = HandlerType.Scene;
                    NextHandler.Name = bosses[selectedIndex].scene;
                }
                else
                {
                    NextHandler.Type = HandlerType.Map;
                    NextHandler.Name = bosses[selectedIndex].stage;
                }

                End();
            }
        }

        #endregion
    }
}
