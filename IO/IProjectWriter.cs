using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common;

namespace MegaMan.IO
{
    public interface IProjectWriter
    {
        void Save(Project project);
    }
}
