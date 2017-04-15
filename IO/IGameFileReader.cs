using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.IO.DataSources;

namespace MegaMan.IO
{
    public interface IGameFileReader
    {
        void Init(IDataSource dataSource);
    }
}
