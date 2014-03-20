using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMan.Editor.Bll
{
    public interface IProjectFileStructure
    {
        FilePath CreateStagePath(string stageName);
    }
}
