using System.Globalization;

namespace MegaMan.Common
{
    public static class Const
    {
        public static readonly NumberFormatInfo NumberFormat = new NumberFormatInfo()
        {
            NumberDecimalSeparator = "."
        };

        public static bool TryParse(this string s, out int result)
        {
            return int.TryParse(s, NumberStyles.Integer, NumberFormat, out result);
        }

        public static bool TryParse(this string s, out float result)
        {
            return float.TryParse(s, NumberStyles.Float, NumberFormat, out result);
        }

        public static bool TryParse(this string s, out double result)
        {
            return double.TryParse(s, NumberStyles.Float, NumberFormat, out result);
        }
    }

    public enum AudioType : byte
    {
        Wav,
        NSF,
        Unknown
    }
}
