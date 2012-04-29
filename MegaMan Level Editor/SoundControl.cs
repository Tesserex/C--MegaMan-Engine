using System;
using System.Windows.Forms;
using MegaMan.Common;

namespace MegaMan.LevelEditor
{
    public partial class SoundControl : UserControl
    {
        private AudioType type = AudioType.Unknown;

        public SoundControl()
        {
            InitializeComponent();
        }

        public SoundInfo GetInfo(string basePath)
        {
            SoundInfo info = new SoundInfo {Type = type};
            if (type == AudioType.NSF)
            {
                info.NsfTrack = (int)trackNumeric.Value;
            }
            else if (type == AudioType.Wav)
            {
                info.Path = FilePath.FromAbsolute(pathText.Text, basePath);
            }
            info.Priority = (byte)priorityNumeric.Value;
            return info;
        }

        private void typeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (typeCombo.SelectedIndex == 0)
            {
                type = AudioType.NSF;
                nsfPanel.Visible = true;
                wavPanel.Visible = false;
            }
            else
            {
                type = AudioType.Wav;
                nsfPanel.Visible = false;
                wavPanel.Visible = true;
            }
        }
    }
}
