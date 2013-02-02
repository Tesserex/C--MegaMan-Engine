using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class TilesetViewModel : IToolProvider
    {
        private IStageSelector _stageSelector;

        public IToolBehavior Tool
        {
            get { throw new NotImplementedException(); }
        }
    }
}
