using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public float X;
        public float Y;

        public PointF(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode()
        {
            return (int)X ^ (int)Y;
        }

        public override bool Equals(object obj)
        {
            return (obj is PointF) && (this == (PointF)obj);
        }

        public static bool operator ==(PointF p1, PointF p2)
        {
            return (p1.X == p2.X) && (p1.Y == p2.Y);
        }

        public static bool operator !=(PointF p1, PointF p2)
        {
            return !(p1 == p2);
        }

        public static implicit operator PointF(Point p)
        {
            return new PointF((float)p.X, (float)p.Y);
        }

        public static PointF Empty = new Point(0, 0);
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

        public Rectangle(System.Drawing.Rectangle rect)
        {
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
        }
        public static explicit operator Rectangle(RectangleF p)
        {
            return new Rectangle((int)p.X, (int)p.Y, (int)p.Width, (int)p.Height);
        }

        public static Rectangle Empty = new Rectangle(0, 0, 0, 0);
    }

    public struct RectangleF
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public RectangleF(System.Drawing.RectangleF rect)
        {
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
        }

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public static implicit operator RectangleF(Rectangle p)
        {
            return new RectangleF((float)p.X, (float)p.Y, (float)p.Width, (float)p.Height);
        }

        public static RectangleF Empty = new Rectangle(0, 0, 0, 0);
    }
}
