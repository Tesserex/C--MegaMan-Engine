using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Common;
using MegaMan.Common.Geometry;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class SpriteFrameViewModel : ViewModelBase
    {
        public int Index { get; private set; }

        public Rectangle SheetLocation { get { return spriteFrame.SheetLocation; } }
        
        private SpriteFrame spriteFrame;

        public SpriteFrameViewModel(SpriteFrame spriteFrame, int index)
        {
            this.spriteFrame = spriteFrame;
            Index = index;
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
