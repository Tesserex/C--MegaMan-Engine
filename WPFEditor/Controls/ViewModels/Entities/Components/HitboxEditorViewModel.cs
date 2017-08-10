using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Geometry;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls.ViewModels.Entities.Components
{
    public class HitboxEditorViewModel : ViewModelBase
    {
        private ProjectDocument project;
        private Sprite sprite;
        private HitBoxInfo hitbox;

        public void ChangeProject(ProjectDocument project)
        {
            this.project = project;
            ChangeHitbox(null);
            OnPropertyChanged("TileProperties");
        }

        public void ChangeSprite(Sprite sprite)
        {
            this.sprite = sprite;
            OnPropertyChanged("SpriteTop");
            OnPropertyChanged("SpriteLeft");
            OnPropertyChanged("SpriteWidth");
            OnPropertyChanged("SpriteHeight");
        }

        public void ChangeHitbox(HitBoxInfo hitbox)
        {
            this.hitbox = hitbox;
            OnPropertyChanged("Name");
            OnPropertyChanged("SelectedPropertiesName");
            OnPropertyChanged("ContactDamage");
            OnPropertyChanged("Environment");
            OnPropertyChanged("PushAway");
            OnPropertyChanged("Left");
            OnPropertyChanged("Top");
            OnPropertyChanged("Width");
            OnPropertyChanged("Height");
            OnPropertyChanged("ZoomLeft");
            OnPropertyChanged("ZoomTop");
            OnPropertyChanged("ZoomWidth");
            OnPropertyChanged("ZoomHeight");
        }

        public string Name
        {
            get { return hitbox == null ? "" : hitbox.Name; }
            set
            {
                if (hitbox == null) return;
                hitbox.Name = value;
                OnPropertyChanged();
            }
        }

        public string SelectedPropertiesName
        {
            get { return hitbox == null ? "" : hitbox.PropertiesName; }
            set
            {
                if (hitbox == null) return;
                hitbox.PropertiesName = value;
                OnPropertyChanged();
            }
        }

        public float ContactDamage
        {
            get { return hitbox == null ? 0 : hitbox.ContactDamage; }
            set
            {
                if (hitbox == null) return;
                hitbox.ContactDamage = value;
                OnPropertyChanged();
            }
        }

        public bool Environment
        {
            get { return hitbox == null ? false : hitbox.Environment; }
            set
            {
                if (hitbox == null) return;
                hitbox.Environment = value;
                OnPropertyChanged();
            }
        }

        public bool PushAway
        {
            get { return hitbox == null ? false : hitbox.PushAway; }
            set
            {
                if (hitbox == null) return;
                hitbox.PushAway = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<string> TileProperties
        {
            get
            {
                return this.project.Project.EntityProperties.Keys;
            }
        }

        public float Left
        {
            get { return hitbox == null ? 0 : hitbox.Box.Left; }
            set
            {
                if (hitbox == null) return;
                hitbox.Box = new RectangleF(value, hitbox.Box.Top, hitbox.Box.Width, hitbox.Box.Height);
                OnPropertyChanged("Left");
                OnPropertyChanged("ZoomLeft");
            }
        }

        public float Top
        {
            get { return hitbox == null ? 0 : hitbox.Box.Top; }
            set
            {
                if (hitbox == null) return;
                hitbox.Box = new RectangleF(hitbox.Box.X, value, hitbox.Box.Width, hitbox.Box.Height);
                OnPropertyChanged("Top");
                OnPropertyChanged("ZoomTop");
            }
        }

        public float Width
        {
            get { return hitbox == null ? 0 : hitbox.Box.Width; }
            set
            {
                if (hitbox == null) return;
                hitbox.Box = new RectangleF(hitbox.Box.X, hitbox.Box.Y, value, hitbox.Box.Height);
                OnPropertyChanged("Width");
                OnPropertyChanged("ZoomWidth");
            }
        }

        public float Height
        {
            get { return hitbox == null ? 0 : hitbox.Box.Height; }
            set
            {
                if (hitbox == null) return;
                hitbox.Box = new RectangleF(hitbox.Box.X, hitbox.Box.Y, hitbox.Box.Width, value);
                OnPropertyChanged("Height");
                OnPropertyChanged("ZoomHeight");
            }
        }

        private int zoom;
        public int Zoom
        {
            get { return zoom; }
            set
            {
                zoom = value;
                OnPropertyChanged();
                OnPropertyChanged("ZoomLeft");
                OnPropertyChanged("ZoomTop");
                OnPropertyChanged("ZoomWidth");
                OnPropertyChanged("ZoomHeight");
                OnPropertyChanged("SpriteTop");
                OnPropertyChanged("SpriteLeft");
                OnPropertyChanged("SpriteWidth");
                OnPropertyChanged("SpriteHeight");
            }
        }

        public float ZoomTop { get { return Zoom * (Top); } }
        public float ZoomLeft { get { return Zoom * Left; } }
        public float ZoomWidth { get { return Zoom * Width; } }
        public float ZoomHeight { get { return Zoom * Height; } }

        public int SpriteTop { get { return Zoom * sprite.HotSpot.Y; } }
        public int SpriteLeft { get { return Zoom * sprite.HotSpot.X; } }
        public int SpriteWidth { get { return Zoom * sprite.Width; } }
        public int SpriteHeight { get { return Zoom * sprite.Height; } }
    }
}
