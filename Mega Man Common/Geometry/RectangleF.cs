﻿using System;
using System.Globalization;

namespace MegaMan.Common.Geometry
{
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
                return new PointF(this.X, this.Y);
            }
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }

        public float X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        public float Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }

        public float Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        public float Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }

        public float Left
        {
            get
            {
                return this.X;
            }
        }

        public float Top
        {
            get
            {
                return this.Y;
            }
        }

        public float Right
        {
            get
            {
                return this.X + this.Width;
            }
        }

        public float Bottom
        {
            get
            {
                return this.Y + this.Height;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return this.Width <= 0f || this.Height <= 0f;
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
            RectangleF rectangleF = (RectangleF)obj;
            return rectangleF.X == this.X && rectangleF.Y == this.Y && rectangleF.Width == this.Width && rectangleF.Height == this.Height;
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
            return this.X <= x && x < this.X + this.Width && this.Y <= y && y < this.Y + this.Height;
        }

        public bool Contains(PointF pt)
        {
            return this.Contains(pt.X, pt.Y);
        }

        public bool Contains(RectangleF rect)
        {
            return this.X <= rect.X && rect.X + rect.Width <= this.X + this.Width && this.Y <= rect.Y && rect.Y + rect.Height <= this.Y + this.Height;
        }

        public override int GetHashCode()
        {
            return (int)((uint)this.X ^ ((uint)this.Y << 13 | (uint)this.Y >> 19) ^ ((uint)this.Width << 26 | (uint)this.Width >> 6) ^ ((uint)this.Height << 7 | (uint)this.Height >> 25));
        }

        public void Inflate(float x, float y)
        {
            this.X -= x;
            this.Y -= y;
            this.Width += 2f * x;
            this.Height += 2f * y;
        }

        public static RectangleF Inflate(RectangleF rect, float x, float y)
        {
            RectangleF result = rect;
            result.Inflate(x, y);
            return result;
        }

        public void Intersect(RectangleF rect)
        {
            RectangleF rectangleF = RectangleF.Intersect(rect, this);
            this.X = rectangleF.X;
            this.Y = rectangleF.Y;
            this.Width = rectangleF.Width;
            this.Height = rectangleF.Height;
        }

        public static RectangleF Intersect(RectangleF a, RectangleF b)
        {
            float num = Math.Max(a.X, b.X);
            float num2 = Math.Min(a.X + a.Width, b.X + b.Width);
            float num3 = Math.Max(a.Y, b.Y);
            float num4 = Math.Min(a.Y + a.Height, b.Y + b.Height);
            if (num2 >= num && num4 >= num3)
            {
                return new RectangleF(num, num3, num2 - num, num4 - num3);
            }
            return RectangleF.Empty;
        }

        public bool IntersectsWith(RectangleF rect)
        {
            return rect.X < this.X + this.Width && this.X < rect.X + rect.Width && rect.Y < this.Y + this.Height && this.Y < rect.Y + rect.Height;
        }

        public static RectangleF Union(RectangleF a, RectangleF b)
        {
            float num = Math.Min(a.X, b.X);
            float num2 = Math.Max(a.X + a.Width, b.X + b.Width);
            float num3 = Math.Min(a.Y, b.Y);
            float num4 = Math.Max(a.Y + a.Height, b.Y + b.Height);
            return new RectangleF(num, num3, num2 - num, num4 - num3);
        }

        public void Offset(PointF pos)
        {
            this.Offset(pos.X, pos.Y);
        }

        public void Offset(float x, float y)
        {
            this.X += x;
            this.Y += y;
        }

        public static implicit operator RectangleF(Rectangle r)
        {
            return new RectangleF((float)r.X, (float)r.Y, (float)r.Width, (float)r.Height);
        }

        public override string ToString()
        {
            return string.Concat(new string[]
            {
                "{X=",
                this.X.ToString(CultureInfo.CurrentCulture),
                ",Y=",
                this.Y.ToString(CultureInfo.CurrentCulture),
                ",Width=",
                this.Width.ToString(CultureInfo.CurrentCulture),
                ",Height=",
                this.Height.ToString(CultureInfo.CurrentCulture),
                "}"
            });
        }
    }
}
