using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
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

        public ICommand AddResistCommand { get; private set; }
        public ICommand DeleteResistCommand { get; private set; }

        public HitboxEditorViewModel()
        {
            AddResistCommand = new RelayCommand(AddResist, x => hitbox != null);
            DeleteResistCommand = new RelayCommand(DeleteResist, x => hitbox != null);

            AddResistName = "";
            AddResistValue = 1;
            OnPropertyChanged("AddResistName");
            OnPropertyChanged("AddResistValue");
        }

        private void AddResist(object obj)
        {
            hitbox.Resistance.Add(AddResistName, AddResistValue);
            AddResistName = "";
            AddResistValue = 1;
            OnPropertyChanged("AddResistName");
            OnPropertyChanged("AddResistValue");
            OnPropertyChanged("Resistance");
        }

        private void DeleteResist(object obj)
        {
            var resist = (ResistanceModel)obj;
            hitbox.Resistance.Remove(resist.Name);
            OnPropertyChanged("Resistance");
        }

        public void ChangeProject(ProjectDocument project)
        {
            this.project = project;
            ChangeHitbox(null);
            OnPropertyChanged("TileProperties");
            OnPropertyChanged("WeaponsList");
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
            OnPropertyChanged("Resistance");
            OnPropertyChanged("Left");
            OnPropertyChanged("Top");
            OnPropertyChanged("Width");
            OnPropertyChanged("Height");
            OnPropertyChanged("ZoomLeft");
            OnPropertyChanged("ZoomTop");
            OnPropertyChanged("ZoomWidth");
            OnPropertyChanged("ZoomHeight");
            OnPropertyChanged("WeaponsList");
        }

        public string Name
        {
            get { return hitbox == null ? "" : hitbox.Name; }
            set
            {
                if (hitbox == null) return;
                hitbox.Name = value;
                project.Dirty = true;
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
                project.Dirty = true;
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
                project.Dirty = true;
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
                project.Dirty = true;
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
                project.Dirty = true;
                OnPropertyChanged();
            }
        }

        public IEnumerable<string> TileProperties
        {
            get
            {
                if (project == null)
                {
                    yield return "";
                }
                else
                {
                    yield return "";
                    foreach (var k in project.Project.EntityProperties.Keys)
                    {
                        yield return k;
                    }
                }
            }
        }

        public IEnumerable<ResistanceModel> Resistance
        {
            get
            {
                if (hitbox == null) return Enumerable.Empty<ResistanceModel>();
                return hitbox.Resistance
                    .Select(r => new ResistanceModel(hitbox, project) {
                        Name = r.Key
                    }).OrderBy(p => p.Name)
                    .ToList();
            }
        }

        public IEnumerable<string> WeaponsList
        {
            get
            {
                if (project == null) return Enumerable.Empty<string>();

                var names = project.Entities
                    .Where(e => e.EditorData != null && e.EditorData.IsProjectile)
                    .Select(e => e.Name)
                    .ToList();

                names.Insert(0, "ALL");

                return names;
            }
        }

        public int Left
        {
            get { return hitbox == null ? 0 : (int)hitbox.Box.Left; }
            set
            {
                if (hitbox == null) return;
                hitbox.Box = new Rectangle(value, hitbox.Box.Top, hitbox.Box.Width, hitbox.Box.Height);
                project.Dirty = true;
                OnPropertyChanged("Left");
                OnPropertyChanged("ZoomLeft");
            }
        }

        public int Top
        {
            get { return hitbox == null ? 0 : (int)hitbox.Box.Top; }
            set
            {
                if (hitbox == null) return;
                hitbox.Box = new Rectangle(hitbox.Box.X, value, hitbox.Box.Width, hitbox.Box.Height);
                project.Dirty = true;
                OnPropertyChanged("Top");
                OnPropertyChanged("ZoomTop");
            }
        }

        public int Width
        {
            get { return hitbox == null ? 0 : (int)hitbox.Box.Width; }
            set
            {
                if (hitbox == null) return;
                hitbox.Box = new Rectangle(hitbox.Box.X, hitbox.Box.Y, value, hitbox.Box.Height);
                project.Dirty = true;
                OnPropertyChanged("Width");
                OnPropertyChanged("ZoomWidth");
            }
        }

        public int Height
        {
            get { return hitbox == null ? 0 : (int)hitbox.Box.Height; }
            set
            {
                if (hitbox == null) return;
                hitbox.Box = new Rectangle(hitbox.Box.X, hitbox.Box.Y, hitbox.Box.Width, value);
                project.Dirty = true;
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

        public int ZoomTop { get { return Zoom * (Top); } set { Top = value / Zoom; } }
        public int ZoomLeft { get { return Zoom * Left; } set { Left = value / Zoom; } }
        public int ZoomWidth { get { return Zoom * Width; } set { Width = value / Zoom; } }
        public int ZoomHeight { get { return Zoom * Height; } set { Height = value / Zoom; } }

        public int SpriteTop { get { return sprite == null ? 0 : Zoom * sprite.HotSpot.Y; } }
        public int SpriteLeft { get { return sprite == null ? 0 : Zoom * sprite.HotSpot.X; } }
        public int SpriteWidth { get { return sprite == null ? 0 : Zoom * sprite.Width; } }
        public int SpriteHeight { get { return sprite == null ? 0 : Zoom * sprite.Height; } }

        public string AddResistName
        {
            get; set;
        }

        public float AddResistValue
        {
            get; set;
        }

        public class ResistanceModel
        {
            private readonly HitBoxInfo hitbox;
            private readonly ProjectDocument project;
            private string name;

            public ResistanceModel(HitBoxInfo hitbox, ProjectDocument project)
            {
                this.hitbox = hitbox;
                this.project = project;
            }

            public string Name
            {
                get { return name; }
                set
                {
                    if (name != null)
                    {
                        var val = hitbox.Resistance[name];
                        hitbox.Resistance.Remove(name);
                        hitbox.Resistance[value] = val;
                        project.Dirty = true;
                    }

                    name = value;
                }
            }

            public float Value
            {
                get { return hitbox.Resistance[name]; }
                set
                {
                    hitbox.Resistance[name] = value;
                    project.Dirty = true;
                }
            }
        }
    }
}
