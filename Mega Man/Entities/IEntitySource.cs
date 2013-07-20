using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.Entities
{
    public interface IEntitySource
    {
        GameEntity GetOriginalEntity(string name);
        TileProperties GetProperties(string name);
    }
}
