using System;
using MegaMan.Common;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class SpriteViewModel : ViewModelBase
    {
        public Sprite Sprite { get; private set; }

        public SpriteViewModel(Sprite sprite)
        {
            if (sprite == null)
                throw new ArgumentNullException("sprite");

            Sprite = sprite;
        }

        public string Name
        {
            get { return Sprite.Name; }
            set
            {
                Sprite.Name = value;
                OnPropertyChanged();
            }
        }

        public int CurrentIndex
        {
            get
            {
                return Sprite.CurrentIndex;
            }
            set
            {
                Sprite.CurrentIndex = value;
                OnPropertyChanged();
            }
        }

        public int Count { get { return Sprite.Count; } }

        public SpriteFrame CurrentFrame
        {
            get
            {
                return Sprite.CurrentFrame;
            }
        }

        public FilePath SheetPath { get { return Sprite.SheetPath; } }

        public bool Playing { get { return Sprite.Playing; } }

        public int Width
        {
            get
            {
                return Sprite.Width;
            }
            set
            {
                Sprite.Width = value;
                OnPropertyChanged();
            }
        }

        public int Height
        {
            get
            {
                return Sprite.Height;
            }
            set
            {
                Sprite.Height = value;
                OnPropertyChanged();
            }
        }

        public void InsertFrame(int index)
        {
            Sprite.InsertFrame(index);
            OnPropertyChanged("Count");
        }

        public void Remove(SpriteFrame frame)
        {
            Sprite.Remove(frame);
            OnPropertyChanged("Count");
        }

        public void Play()
        {
            Sprite.Play();
        }

        public void Pause()
        {
            Sprite.Pause();
        }
    }
}
