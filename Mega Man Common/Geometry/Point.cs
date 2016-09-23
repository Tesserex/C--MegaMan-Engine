
namespace MegaMan.Common.Geometry
{
    public struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Offset(int x, int y)
        {
            X += x;
            Y += y;
        }

        public override bool Equals(object obj)
        {
            return (obj is Point) && (this == (Point)obj);
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return (p1.X == p2.X) && (p1.Y == p2.Y);
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return !(p1 == p2);
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public static explicit operator Point(PointF p)
        {
            return new Point((int)p.X, (int)p.Y);
        }

        public static Point Empty = new Point(0, 0);
    }
}
