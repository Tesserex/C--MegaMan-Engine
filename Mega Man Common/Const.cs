using System.Globalization;

namespace MegaMan.Common
{
    public static class Const
    {
        static private readonly CultureInfo NumberFormat = CultureInfo.InvariantCulture;

        public static bool TryParse(this string s, out int result)
        {
            bool returnValue = int.TryParse(s, NumberStyles.Integer, NumberFormat, out result);

            if (returnValue == false) return int.TryParse(s.Replace(",", "."), NumberStyles.Integer, NumberFormat, out result);
            return returnValue;
        }

        public static bool TryParse(this string s, out float result)
        {
            bool returnValue = float.TryParse(s, NumberStyles.Float, NumberFormat, out result);

            if (returnValue == false) return float.TryParse(s.Replace(",","."), NumberStyles.Float, NumberFormat, out result);
            return returnValue;
        }

        public static bool TryParse(this string s, out double result)
        {
            bool returnValue = double.TryParse(s, NumberStyles.Float, NumberFormat, out result);

            if (returnValue == false) return double.TryParse(s.Replace(",", "."), NumberStyles.Float, NumberFormat, out result);
            return returnValue;
        }
    }

    public enum AudioType : byte
    {
        Wav,
        NSF,
        Unknown
    }
}
