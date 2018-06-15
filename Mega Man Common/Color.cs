namespace MegaMan.Common
{
    public struct Color
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color(byte r, byte g, byte b) : this(r, g, b, 255) { }

        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color(float r, float g, float b) : this(r, g, b, 1) { }

        public Color(float r, float g, float b, float a)
        {
            R = (byte)(r * 255);
            G = (byte)(g * 255);
            B = (byte)(b * 255);
            A = (byte)(a * 255);
        }
    }
}
