using System;
using System.Windows.Forms;

namespace MegaMan.Engine.Forms.MenuControllers
{
    public class LayerVisibilityMenuController : IMenuController
    {
        private readonly ToolStripMenuItem menuItem;
        private readonly UserSettingsEnums.Layers layer;

        public LayerVisibilityMenuController(ToolStripMenuItem menuItem, UserSettingsEnums.Layers layer)
        {
            this.menuItem = menuItem;
            this.layer = layer;

            menuItem.Click += MenuItem_Click;
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            Set(!this.menuItem.Checked);
        }

        public void Set(bool value)
        {
            this.menuItem.Checked = value;
            Engine.Instance.SetLayerVisibility((int)this.layer, value);
        }

        public void LoadSettings(Setting settings)
        {
            if (layer == UserSettingsEnums.Layers.Background)
                Set(settings.Debug.Layers.Background);
            else if (layer == UserSettingsEnums.Layers.Sprite1)
                Set(settings.Debug.Layers.Sprites1);
            else if (layer == UserSettingsEnums.Layers.Sprite2)
                Set(settings.Debug.Layers.Sprites2);
            else if (layer == UserSettingsEnums.Layers.Sprite3)
                Set(settings.Debug.Layers.Sprites3);
            else if (layer == UserSettingsEnums.Layers.Sprite4)
                Set(settings.Debug.Layers.Sprites4);
            else if (layer == UserSettingsEnums.Layers.Foreground)
                Set(settings.Debug.Layers.Foreground);
        }
    }
}
