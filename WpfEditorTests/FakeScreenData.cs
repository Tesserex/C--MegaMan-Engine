using System;
using System.Collections.Generic;
using MegaMan.Common;
using MegaMan.Editor.Bll.Algorithms;

namespace MegaMan.Editor.Tests
{
    public class FakeScreenData : IScreenData
    {
        private readonly IList<Join> joins;

        public IEnumerable<Join> Joins { get { return joins; } }

        public FakeScreenData()
        {
            this.joins = new List<Join>();
            TileSize = 16;
            Height = 14;
            Width = 16;
        }

        public string Name { get; set; }

        public int TileSize { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public void AddJoin(Join join)
        {
            this.joins.Add(join);
        }

        public void SeverAllJoins()
        {
            this.joins.Clear();
        }
    }
}
