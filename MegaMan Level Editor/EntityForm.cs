using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace MegaMan.LevelEditor
{
    public partial class EntityForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public event Action<Entity> EntityChanged;

        public EntityForm()
        {
            InitializeComponent();
            Program.AnimateTick += Animate;
        }

        private void Animate()
        {
            var entity = entityList.SelectedItem as Entity;
            if (entity != null && entityPreview.Image != null)
            {
                DrawPreview();
            }
        }

        private void DrawPreview()
        {
            var entity = entityList.SelectedItem as Entity;
            using (var g = Graphics.FromImage(entityPreview.Image))
            {
                g.Clear(Color.Transparent);
                if (entity.MainSprite == null)
                {
                    g.DrawImage(Properties.Resources.nosprite, 0, 0);
                }
                else
                {
                    entity.MainSprite.Draw(g, entity.MainSprite.HotSpot.X, entity.MainSprite.HotSpot.Y);
                }
            }
            entityPreview.Refresh();
        }

        public void Deselect()
        {
            if (entityPreview.Image != null) entityPreview.Image.Dispose();
            entityPreview.Image = null;
        }

        public void LoadEntities(ProjectEditor project)
        {
            entityList.DataSource = project.Entities.ToList();
            entityList.DisplayMember = "Name";
        }

        public void Unload()
        {
            entityList.DataSource = null;
        }

        private void entityList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var entity = entityList.SelectedItem as Entity;
            if (entity != null)
            {
                if (entityPreview.Image != null) entityPreview.Image.Dispose();

                if (entity.MainSprite == null)
                {
                    entityPreview.Image = new Bitmap(16, 16);
                }
                else
                {
                    entityPreview.Image = new Bitmap(entity.MainSprite.Width + 10, entity.MainSprite.Height + 10);
                }

                DrawPreview();

                entityPreview.Refresh();
                if (EntityChanged != null) EntityChanged(entity);
            }
        }
    }
}
