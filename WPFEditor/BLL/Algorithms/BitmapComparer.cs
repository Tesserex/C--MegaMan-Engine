using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace MegaMan.Editor.Bll.Algorithms
{
    public class BitmapComparer : IEqualityComparer<WriteableBitmap>
    {
        public bool Equals(WriteableBitmap x, WriteableBitmap y)
        {
            var xBytes = x.ToByteArray();
            var yBytes = y.ToByteArray();

            return xBytes.SequenceEqual(yBytes);
        }

        public int GetHashCode(WriteableBitmap obj)
        {
            var array = obj.ToByteArray();

            if (array == null)
            {
                return 0;
            }
            int hash = 17;
            foreach (var b in array)
            {
                hash = hash * 31 + b;
            }
            return hash;
        }
    }
}
