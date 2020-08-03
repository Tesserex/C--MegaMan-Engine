using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Common;

namespace MegaMan.Editor.Bll.Algorithms
{
    public interface IScreenData
    {
        string Name { get; }
        int TileSize { get; }
        int Height { get; }
        int Width { get; }

        void AddJoin(Join join);
        void SeverAllJoins();
    }
}
