using MegaMan.Editor.Bll;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MegaMan.Editor.Bll
{
    public class StageChangedEventArgs : EventArgs
    {
        public StageDocument Stage { get; private set; }

        public StageChangedEventArgs(StageDocument stage)
        {
            Stage = stage;
        }
    }

    public interface IStageSelector
    {
        StageDocument Stage { get; }

        void ChangeStage(string stageName);

        event EventHandler<StageChangedEventArgs> StageChanged;
    }
}
