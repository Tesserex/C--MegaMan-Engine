using System;
using System.Windows.Forms;
using System.Drawing;
using MegaMan.Common;

namespace MegaMan.LevelEditor
{
    public sealed partial class EntityButton : Panel
    {
        private int lastFrame = -1;
        private readonly Sprite sprite;
        private readonly PictureBox spritePict;

        public Entity Entity { get; private set; }

        public EntityButton(Entity entity)
        {
            InitializeComponent();

            Entity = entity;
            sprite = entity.MainSprite;

            spritePict = new PictureBox {Height = sprite.Height, Width = sprite.Width};

            BackColor = Color.Transparent;
            Width = spritePict.Width + 8;
            Height = spritePict.Height + 8;
            Controls.Add(spritePict);
            spritePict.Top = 4;
            spritePict.Left = 4;

            spritePict.Image = sprite[0].CutTile;
            spritePict.Refresh();

            Program.AnimateTick += Program_FrameTick;
            spritePict.Click += spritePict_Click;
        }

        void spritePict_Click(object sender, EventArgs e)
        {
            InvokeOnClick(this, e);
        }

        private void Program_FrameTick()
        {
            if (sprite.CurrentFrame == lastFrame) return;
            lastFrame = sprite.CurrentFrame;

            spritePict.Image = sprite[sprite.CurrentFrame].CutTile;
            spritePict.Refresh();
        }
    }
}
