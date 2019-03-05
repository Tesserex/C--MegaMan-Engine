using System.Globalization;

namespace MegaMan.Common.Geometry
{
    public struct PointF
    {
        public static readonly PointF Empty = default(PointF);
        private float x;
        private float y;

        public bool IsEmpty
        {
            get
            {
                return this.x == 0f && this.y == 0f;
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

        public PointF(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator PointF(Point p)
        {
            return new PointF((float)p.X, (float)p.Y);
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
            PointF pointF = (PointF)obj;
            return pointF.X == this.X && pointF.Y == this.Y && pointF.GetType().Equals(base.GetType());
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{X={0}, Y={1}}}", new object[]
            {
                this.x,
                this.y
            });
        }
    }
}
