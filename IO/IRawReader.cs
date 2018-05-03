using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Common;

namespace MegaMan.IO
{
    public interface IRawReader : IGameFileReader
    {
        byte[] GetRawData(FilePath path);
    }
}
