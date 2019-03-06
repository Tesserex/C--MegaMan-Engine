using System;
using System.Globalization;

namespace MegaMan.Common.Geometry
{
    public struct RectangleF
    {
        public static readonly RectangleF Empty = default(RectangleF);
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
        
        public float X { get; set; }

        public float Y { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

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
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public static RectangleF FromLtrb(float left, float top, float right, float bottom)
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
