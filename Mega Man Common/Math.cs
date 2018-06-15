using System;
using System.Globalization;

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

    public struct PointF
    {
        public static readonly PointF Empty = default(PointF);
        private float x;
        private float y;

        public bool IsEmpty
        {
            get
            {
                return x == 0f && y == 0f;
            }
        }

        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public PointF(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator PointF(Point p)
        {
            return new PointF(p.X, p.Y);
        }

        public static bool operator ==(PointF left, PointF right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(PointF left, PointF right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PointF))
            {
                return false;
            }
            var pointF = (PointF)obj;
            return pointF.X == X && pointF.Y == Y && pointF.GetType().Equals(GetType());
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{X={0}, Y={1}}}", x, y);
        }
    }

    public struct Rectangle
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RectangleF))
            {
                return false;
            }
            var rectangleF = (RectangleF)obj;
            return rectangleF.X == X && rectangleF.Y == Y && rectangleF.Width == Width && rectangleF.Height == Height;
        }

        public override int GetHashCode()
        {
            return (int)((uint)X ^ ((uint)Y << 13 | (uint)Y >> 19) ^ ((uint)Width << 26 | (uint)Width >> 6) ^ ((uint)Height << 7 | (uint)Height >> 25));
        }

        public static bool operator ==(Rectangle left, Rectangle right)
        {
            return left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height;
        }

        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return !(left == right);
        }

        public void Intersect(Rectangle rect)
        {
            var rectangle = Intersect(rect, this);
            X = rectangle.X;
            Y = rectangle.Y;
            Width = rectangle.Width;
            Height = rectangle.Height;
        }

        public static Rectangle Intersect(Rectangle a, Rectangle b)
        {
            var num = Math.Max(a.X, b.X);
            var num2 = Math.Min(a.X + a.Width, b.X + b.Width);
            var num3 = Math.Max(a.Y, b.Y);
            var num4 = Math.Min(a.Y + a.Height, b.Y + b.Height);
            if (num2 >= num && num4 >= num3)
            {
                return new Rectangle(num, num3, num2 - num, num4 - num3);
            }
            return Empty;
        }

        public static Rectangle Union(Rectangle a, Rectangle b)
        {
            var num = Math.Min(a.X, b.X);
            var num2 = Math.Max(a.X + a.Width, b.X + b.Width);
            var num3 = Math.Min(a.Y, b.Y);
            var num4 = Math.Max(a.Y + a.Height, b.Y + b.Height);
            return new Rectangle(num, num3, num2 - num, num4 - num3);
        }

        public bool Contains(int x, int y)
        {
            return X <= x && x < X + Width && Y <= y && y < Y + Height;
        }

        public bool Contains(Point pt)
        {
            return Contains(pt.X, pt.Y);
        }

        public bool Contains(Rectangle rect)
        {
            return X <= rect.X && rect.X + rect.Width <= X + Width && Y <= rect.Y && rect.Y + rect.Height <= Y + Height;
        }

        public static explicit operator Rectangle(RectangleF p)
        {
            return new Rectangle((int)p.X, (int)p.Y, (int)p.Width, (int)p.Height);
        }

        public static Rectangle Empty = new Rectangle(0, 0, 0, 0);
    }

    public struct RectangleF
    {
        public static readonly RectangleF Empty = default(RectangleF);
        private float x;
        private float y;
        private float width;
        private float height;
        
        public PointF Location
        {
            get
            {
                return new PointF(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        
        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }
        
        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public float Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        public float Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        public float Left
        {
            get
            {
                return X;
            }
        }

        public float Top
        {
            get
            {
                return Y;
            }
        }

        public float Right
        {
            get
            {
                return X + Width;
            }
        }

        public float Bottom
        {
            get
            {
                return Y + Height;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return Width <= 0f || Height <= 0f;
            }
        }

        public RectangleF(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public static RectangleF FromLTRB(float left, float top, float right, float bottom)
        {
            return new RectangleF(left, top, right - left, bottom - top);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RectangleF))
            {
                return false;
            }
            var rectangleF = (RectangleF)obj;
            return rectangleF.X == X && rectangleF.Y == Y && rectangleF.Width == Width && rectangleF.Height == Height;
        }

        public static bool operator ==(RectangleF left, RectangleF right)
        {
            return left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height;
        }

        public static bool operator !=(RectangleF left, RectangleF right)
        {
            return !(left == right);
        }

        public bool Contains(float x, float y)
        {
            return X <= x && x < X + Width && Y <= y && y < Y + Height;
        }

        public bool Contains(PointF pt)
        {
            return Contains(pt.X, pt.Y);
        }

        public bool Contains(RectangleF rect)
        {
            return X <= rect.X && rect.X + rect.Width <= X + Width && Y <= rect.Y && rect.Y + rect.Height <= Y + Height;
        }

        public override int GetHashCode()
        {
            return (int)((uint)X ^ ((uint)Y << 13 | (uint)Y >> 19) ^ ((uint)Width << 26 | (uint)Width >> 6) ^ ((uint)Height << 7 | (uint)Height >> 25));
        }

        public void Inflate(float x, float y)
        {
            X -= x;
            Y -= y;
            Width += 2f * x;
            Height += 2f * y;
        }

        public static RectangleF Inflate(RectangleF rect, float x, float y)
        {
            var result = rect;
            result.Inflate(x, y);
            return result;
        }

        public void Intersect(RectangleF rect)
        {
            var rectangleF = Intersect(rect, this);
            X = rectangleF.X;
            Y = rectangleF.Y;
            Width = rectangleF.Width;
            Height = rectangleF.Height;
        }

        public static RectangleF Intersect(RectangleF a, RectangleF b)
        {
            var num = Math.Max(a.X, b.X);
            var num2 = Math.Min(a.X + a.Width, b.X + b.Width);
            var num3 = Math.Max(a.Y, b.Y);
            var num4 = Math.Min(a.Y + a.Height, b.Y + b.Height);
            if (num2 >= num && num4 >= num3)
            {
                return new RectangleF(num, num3, num2 - num, num4 - num3);
            }
            return Empty;
        }

        public bool IntersectsWith(RectangleF rect)
        {
            return rect.X < X + Width && X < rect.X + rect.Width && rect.Y < Y + Height && Y < rect.Y + rect.Height;
        }

        public static RectangleF Union(RectangleF a, RectangleF b)
        {
            var num = Math.Min(a.X, b.X);
            var num2 = Math.Max(a.X + a.Width, b.X + b.Width);
            var num3 = Math.Min(a.Y, b.Y);
            var num4 = Math.Max(a.Y + a.Height, b.Y + b.Height);
            return new RectangleF(num, num3, num2 - num, num4 - num3);
        }

        public void Offset(PointF pos)
        {
            Offset(pos.X, pos.Y);
        }

        public void Offset(float x, float y)
        {
            X += x;
            Y += y;
        }
        
        public static implicit operator RectangleF(Rectangle r)
        {
            return new RectangleF(r.X, r.Y, r.Width, r.Height);
        }

        public override string ToString()
        {
            return string.Concat("{X=", X.ToString(CultureInfo.CurrentCulture), ",Y=", Y.ToString(CultureInfo.CurrentCulture), ",Width=", Width.ToString(CultureInfo.CurrentCulture), ",Height=", Height.ToString(CultureInfo.CurrentCulture), "}");
        }
    }
}
