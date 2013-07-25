using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.Entities
{
    public interface ITilePropertiesSource
    {
        TileProperties GetProperties(string name);
    }
}
