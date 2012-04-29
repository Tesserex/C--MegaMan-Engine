using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MegaMan.Common;

namespace MegaMan.LevelEditor
{
    public partial class StageSelectEdit : Form
    {
        private Project project;
        private StageSelect stageSelect;
        private Bitmap background;

        public StageSelectEdit(ProjectEditor editor)
        {
            project = editor.Project;

            InitializeComponent();
            this.stageSelect = project.StageSelects.FirstOrDefault();
            this.preview.Image = new Bitmap(project.ScreenWidth, project.ScreenHeight);

            bossX.Value = stageSelect.BossSpacingHorizontal;
            bossY.Value = stageSelect.BossSpacingVertical;

            comboSlot.SelectedIndex = -1;
            foreach (var stage in project.Stages) comboStages.Items.Add(stage.Name);

            if (stageSelect.Background != null)
            {
                textBackground.Text = stageSelect.Background.Absolute;
                try
                {
                    this.background = (Bitmap)Image.FromFile(stageSelect.Background.Absolute);
                    this.background.SetResolution(this.preview.Image.HorizontalResolution, this.preview.Image.VerticalResolution);
                }
                catch
                {
                    this.textBackground.Text = "";
                }
            }

            if (stageSelect.Music != null)
            {
                
            }

            if (stageSelect.ChangeSound != null)
            {
                if (stageSelect.ChangeSound.Type == AudioType.Wav)
                {
                    textSound.Text = stageSelect.ChangeSound.Path.Absolute;
                }
            }

            ReDraw();
        }

        private void ReDraw()
        {
            using (Graphics g = Graphics.FromImage(preview.Image))
            {
                g.Clear(Color.Black);
                if (background != null) g.DrawImage(background, 0, 0);

                if (stageSelect.BossFrame != null)
                {
                    int mid_x = project.ScreenWidth / 2 - stageSelect.BossFrame.Width / 2;
                    int mid_y = project.ScreenHeight / 2 - stageSelect.BossFrame.Height / 2 + stageSelect.BossOffset;

                    int space_x = stageSelect.BossSpacingHorizontal + stageSelect.BossFrame.Width;
                    int space_y = stageSelect.BossSpacingVertical + stageSelect.BossFrame.Height;

                    stageSelect.BossFrame.Draw(g, mid_x - space_x, mid_y - space_y);
                    stageSelect.BossFrame.Draw(g, mid_x, mid_y - space_y);
                    stageSelect.BossFrame.Draw(g, mid_x + space_x, mid_y - space_y);
                    stageSelect.BossFrame.Draw(g, mid_x - space_x, mid_y);
                    stageSelect.BossFrame.Draw(g, mid_x, mid_y);
                    stageSelect.BossFrame.Draw(g, mid_x + space_x, mid_y);
                    stageSelect.BossFrame.Draw(g, mid_x - space_x, mid_y + space_y);
                    stageSelect.BossFrame.Draw(g, mid_x, mid_y + space_y);
                    stageSelect.BossFrame.Draw(g, mid_x + space_x, mid_y + space_y);
                }
            }
            this.preview.Refresh();
        }

        private void backgroundBrowse_Click(object sender, EventArgs e)
        {
            var browse = new OpenFileDialog();
            browse.Filter = "Images (png, gif, bmp, jpg)|*.png;*.gif;*.bmp;*.jpg";
            var result = browse.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    this.background = (Bitmap)Image.FromFile(browse.FileName);
                    this.background.SetResolution(this.preview.Image.HorizontalResolution, this.preview.Image.VerticalResolution);

                    stageSelect.Background = FilePath.FromAbsolute(browse.FileName, project.BaseDir);
                }
                catch
                {
                    MessageBox.Show("Sorry, that image could not be loaded.", "CME Project Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            this.textBackground.Text = browse.FileName;
            ReDraw();
        }

        private void frameBrowse_Click(object sender, EventArgs e)
        {
            var editor = new SpriteEditor(this.project);
            if (stageSelect.BossFrame != null) editor.Sprite = stageSelect.BossFrame;
            editor.SpriteChange += () =>
            {
                stageSelect.BossFrame = editor.Sprite;
            };
            editor.FormClosed += (s, ev) =>
            {
                ReDraw();
            };
            editor.Show();
        }

        private void soundBrowse_Click(object sender, EventArgs e)
        {
            var browse = new OpenFileDialog();
            browse.Filter = "Music (wav, mp3, ogg)|*.wav;*.mp3;*.ogg";
            var result = browse.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.textSound.Text = browse.FileName;
                stageSelect.ChangeSound.Type = AudioType.Wav;
                stageSelect.ChangeSound.Path = FilePath.FromAbsolute(browse.FileName, project.BaseDir);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bossX_ValueChanged(object sender, EventArgs e)
        {
            stageSelect.BossSpacingHorizontal = (int)bossX.Value;
            this.ReDraw();
        }

        private void bossY_ValueChanged(object sender, EventArgs e)
        {
            stageSelect.BossSpacingVertical = (int)bossY.Value;
            this.ReDraw();
        }

        private void comboSlot_SelectedIndexChanged(object sender, EventArgs e)
        {
            BossInfo info = BossAtSlot(comboSlot.SelectedIndex);

            if (info.PortraitPath != null && info.PortraitPath.Relative != "") textPortrait.Text = info.PortraitPath.Absolute;
            else textPortrait.Text = "";

            textBossName.Text = info.Name;

            if (info.NextHandler == null) comboStages.SelectedIndex = -1;
            else comboStages.SelectedIndex = comboStages.Items.IndexOf(info.NextHandler.Name);
        }

        private void comboStages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboStages.SelectedItem != null)
            {
                BossAtSlot(comboSlot.SelectedIndex).NextHandler = new HandlerTransfer
                    {
                        Type = HandlerType.Stage,
                        Name = comboStages.SelectedItem.ToString()
                    };
            }
        }

        private void portraitBrowse_Click(object sender, EventArgs e)
        {
            var browse = new OpenFileDialog();
            browse.Filter = "Images (png, gif, bmp, jpg)|*.png;*.gif;*.bmp;*.jpg";
            var result = browse.ShowDialog();
            if (result == DialogResult.OK)
            {
                BossAtSlot(comboSlot.SelectedIndex).PortraitPath = FilePath.FromAbsolute(browse.FileName, this.project.BaseDir);
                this.textPortrait.Text = browse.FileName;
            }
        }

        private void textBossName_TextChanged(object sender, EventArgs e)
        {
            if (comboStages.SelectedItem != null)
            {
                BossAtSlot(comboSlot.SelectedIndex).Name = textBossName.Text;
            }
        }

        private void bossOffset_ValueChanged(object sender, EventArgs e)
        {
            stageSelect.BossOffset = (int)bossOffset.Value;
            this.ReDraw();
        }

        private BossInfo BossAtSlot(int slot)
        {
            foreach (var info in stageSelect.Bosses)
            {
                if (info.Slot == slot) return info;
            }
            // find the next available one
            foreach (var info in stageSelect.Bosses)
            {
                if (info.Slot == -1)
                {
                    info.Slot = slot;
                    return info;
                }
            }
            BossInfo boss = new BossInfo();
            boss.Slot = slot;
            stageSelect.AddBoss(boss);
            return boss;
        }
    }
}
