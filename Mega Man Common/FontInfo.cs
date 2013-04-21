using MegaMan.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common
{
    public class FontInfo
    {
        private Dictionary<char, Point> chars = new Dictionary<char,Point>();

        public int CharWidth { get; set; }
        public bool CaseSensitive { get; set; }
        public FilePath ImagePath { get; set; }

        public Point? this[char p]
        {
            get
            {
                if (!chars.ContainsKey(p))
                    return null;

                return chars[p];
            }
        }

        public void AddLine(int x, int y, string lineText)
        {
            if (!CaseSensitive)
            {
                lineText = lineText.ToUpper();
            }

            var lineChars = lineText.ToCharArray();

            for (int i = 0; i < lineChars.Length; i++)
            {
                var c = lineChars[i];

                chars.Add(c, new Point(x + i * CharWidth, y));
            }
        }
    }
}
