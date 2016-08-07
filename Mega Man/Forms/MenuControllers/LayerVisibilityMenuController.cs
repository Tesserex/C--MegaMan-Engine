using System;
using System.Windows.Forms;
using MegaMan.Engine.Forms.Settings;

namespace MegaMan.Engine.Forms.MenuControllers
{
    public class LayerVisibilityMenuController : IMenuController
    {
        private readonly ToolStripMenuItem menuItem;
        private readonly Layers layer;

        public LayerVisibilityMenuController(ToolStripMenuItem menuItem, Layers layer)
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
            if (layer == Layers.Background)
                Set(settings.Debug.Layers.Background);
            else if (layer == Layers.Sprite1)
                Set(settings.Debug.Layers.Sprites1);
            else if (layer == Layers.Sprite2)
                Set(settings.Debug.Layers.Sprites2);
            else if (layer == Layers.Sprite3)
                Set(settings.Debug.Layers.Sprites3);
            else if (layer == Layers.Sprite4)
                Set(settings.Debug.Layers.Sprites4);
            else if (layer == Layers.Foreground)
                Set(settings.Debug.Layers.Foreground);
        }
    }
}
