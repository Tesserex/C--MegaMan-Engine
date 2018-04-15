using System;
using MegaMan.Common;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Services;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class SpriteViewModel : ViewModelBase
    {
        private SpriteAnimator animator;

        public Sprite Sprite { get; private set; }

        public SpriteModel Model { get; private set; }

        public SpriteViewModel(Sprite sprite)
        {
            if (sprite == null)
                throw new ArgumentNullException("sprite");

            Sprite = sprite;
            Model = new SpriteModel(Sprite);
            animator = new SpriteAnimator(sprite);
            TickWeakEventManager.AddHandler(Tick);
        }

        public SpriteViewModel(SpriteModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            Sprite = model.Sprite;
            Model = model;
            animator = new SpriteAnimator(Sprite);
            TickWeakEventManager.AddHandler(Tick);
        }

        private void Tick(object sender, EventArgs e)
        {
            animator.Update();
            OnPropertyChanged("CurrentIndex");
            OnPropertyChanged("CurrentFrame");
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
                return animator.CurrentIndex;
            }
            set
            {
                animator.CurrentIndex = value;
                OnPropertyChanged();
            }
        }

        public int Count { get { return Sprite.Count; } }

        public SpriteFrame CurrentFrame
        {
            get
            {
                return animator.CurrentFrame;
            }
        }

        public FilePath SheetPath { get { return Sprite.SheetPath; } }

        public bool Playing { get { return animator.Playing; } }

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

        public void Play()
        {
            animator.Play();
        }

        public void Pause()
        {
            animator.Pause();
        }
    }
}
