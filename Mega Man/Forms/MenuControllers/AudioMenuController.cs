using System;
using System.Windows.Forms;
using MegaMan.Engine.Forms.Settings;

namespace MegaMan.Engine.Forms.MenuControllers
{
    public class AudioMenuController : IMenuController
    {
        private readonly ToolStripMenuItem menuItem;
        private readonly int channel;

        public AudioMenuController(ToolStripMenuItem menuItem, int channel)
        {
            this.menuItem = menuItem;
            this.channel = channel;

            menuItem.Click += MenuItem_Click;
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            Set(!this.menuItem.Checked);
        }

        public void LoadSettings(Setting settings)
        {
            if (channel == 1)
                Set(settings.Audio.Square1);
            else if (channel == 2)
                Set(settings.Audio.Square2);
            else if (channel == 3)
                Set(settings.Audio.Triangle);
            else if (channel == 4)
                Set(settings.Audio.Noise);
        }

        public void Set(bool value)
        {
            this.menuItem.Checked = value;

            if (channel == 1)
                Engine.Instance.SoundSystem.SquareOne = value;
            else if (channel == 2)
                Engine.Instance.SoundSystem.SquareTwo = value;
            else if (channel == 3)
                Engine.Instance.SoundSystem.Triangle = value;
            else if (channel == 4)
                Engine.Instance.SoundSystem.Noise = value;
        }

        public void SaveSettings(Setting settings)
        {
            if (settings.Audio == null)
                settings.Audio = new LastAudio();

            if (channel == 1)
                settings.Audio.Square1 = this.menuItem.Checked;
            else if (channel == 2)
                settings.Audio.Square2 = this.menuItem.Checked;
            else if (channel == 3)
                settings.Audio.Triangle = this.menuItem.Checked;
            else if (channel == 4)
                settings.Audio.Noise = this.menuItem.Checked;
        }
    }
}
