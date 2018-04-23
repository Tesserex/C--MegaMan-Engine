using System;
using MegaMan.Common;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Services;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class SpriteViewModel : ViewModelBase
    {
        public Sprite Sprite { get; private set; }

        public SpriteModel Model { get; private set; }

        public SpriteViewModel(Sprite sprite)
        {
            if (sprite == null)
                throw new ArgumentNullException("sprite");

            Sprite = sprite;
            Model = new SpriteModel(Sprite);
        }

        public SpriteViewModel(SpriteModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            Sprite = model.Sprite;
            Model = model;
            TickWeakEventManager.AddHandler(Tick);
        }

        private void Tick(object sender, EventArgs e)
        {
            if (Model.Playing)
            {
                OnPropertyChanged("CurrentIndex");
                OnPropertyChanged("CurrentFrame");
            }
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
                return Model.CurrentIndex;
            }
            set
            {
                Model.CurrentIndex = value;
                OnPropertyChanged();
            }
        }

        public int Count { get { return Sprite.Count; } }

        public SpriteFrame CurrentFrame
        {
            get
            {
                return Model.CurrentFrame;
            }
        }

        public FilePath SheetPath { get { return Sprite.SheetPath; } }

        public bool Playing { get { return Model.Playing; } }

        public void Play()
        {
            Model.Play();
        }

        public void Pause()
        {
            Model.Pause();
        }

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

        public bool Reversed
        {
            get
            {
                return Sprite.Reversed;
            }
            set
            {
                Sprite.Reversed = value;
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
    }
}
