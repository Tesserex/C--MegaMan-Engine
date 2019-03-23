using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            Sprite = sprite ?? throw new ArgumentNullException("sprite");
            Model = new SpriteModel(Sprite);
        }

        public SpriteViewModel(SpriteModel model)
        {
            Model = model ?? throw new ArgumentNullException("model");
            Sprite = model.Sprite;
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

        private ObservableCollection<SpriteFrameViewModel> frameModels;
        public ObservableCollection<SpriteFrameViewModel> Frames
        {
            get
            {
                if (frameModels == null)
                {
                    frameModels = new ObservableCollection<SpriteFrameViewModel>(Sprite.Select((f, i) => new SpriteFrameViewModel(f, i, this)));
                }
                return frameModels;
            }
        }

        public SpriteFrameViewModel CurrentFrame
        {
            get
            {
                return Frames[CurrentIndex];
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

        public AnimationStyle AnimStyle
        {
            get
            {
                return Sprite.AnimStyle;
            }
            set
            {
                Sprite.AnimStyle = value;
                OnPropertyChanged();
            }
        }

        public AnimationDirection AnimDirection
        {
            get
            {
                return Sprite.AnimDirection;
            }
            set
            {
                Sprite.AnimDirection = value;
                OnPropertyChanged();
            }
        }

        public void InsertFrame(int index)
        {
            Sprite.InsertFrame(index);
            Frames.Insert(index, new SpriteFrameViewModel(Sprite[index], index, this));
            OnPropertyChanged(nameof(CurrentIndex));
            OnPropertyChanged(nameof(Count));
        }

        public void Remove(int index)
        {
            Sprite.Remove(Sprite[index]);
            Frames.RemoveAt(index);
            OnPropertyChanged(nameof(CurrentIndex));
            OnPropertyChanged(nameof(Count));
        }
    }
}
