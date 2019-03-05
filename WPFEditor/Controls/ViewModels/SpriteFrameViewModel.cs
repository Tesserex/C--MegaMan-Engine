using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MegaMan.Common;
using MegaMan.Common.Geometry;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class SpriteFrameViewModel : ViewModelBase
    {
        public int Index { get; private set; }

        public Rectangle SheetLocation { get { return spriteFrame.SheetLocation; } }

        public ICommand DeleteCommand { get; private set; }
        
        private SpriteFrame spriteFrame;
        private SpriteViewModel sprite;

        public SpriteFrameViewModel(SpriteFrame spriteFrame, int index, SpriteViewModel sprite)
        {
            this.spriteFrame = spriteFrame;
            this.sprite = sprite;
            Index = index;
            DeleteCommand = new RelayCommand(Delete);
        }

        private void Delete(object obj)
        {
            sprite.Remove(Index);
        }

        public int Duration
        {
            get { return spriteFrame.Duration; }
            set
            {
                spriteFrame.Duration = value;
                OnPropertyChanged(nameof(Duration));
            }
        }

        internal void SetSheetPosition(int x, int y)
        {
            spriteFrame.SetSheetPosition(x, y);
            OnPropertyChanged(nameof(SheetLocation));
        }
    }
}
