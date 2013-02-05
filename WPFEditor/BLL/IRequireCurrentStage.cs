using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Editor.Bll
{
    public interface IRequireCurrentStage
    {
        void SetStage(StageDocument stage);
    }
}
