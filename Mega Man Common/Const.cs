using System.Globalization;

namespace MegaMan.Common
{
    public static class Const
    {
        private static readonly CultureInfo NumberFormat = CultureInfo.InvariantCulture;

        public static bool TryParse(this string s, out int result)
        {
            var returnValue = int.TryParse(s, NumberStyles.Integer, NumberFormat, out result);

            return returnValue || int.TryParse(s.Replace(",", "."), NumberStyles.Integer, NumberFormat, out result);
        }

        public static bool TryParse(this string s, out float result)
        {
            var returnValue = float.TryParse(s, NumberStyles.Float, NumberFormat, out result);

            return returnValue || float.TryParse(s.Replace(",","."), NumberStyles.Float, NumberFormat, out result);
        }

        public static bool TryParse(this string s, out double result)
        {
            var returnValue = double.TryParse(s, NumberStyles.Float, NumberFormat, out result);

            return returnValue || double.TryParse(s.Replace(",", "."), NumberStyles.Float, NumberFormat, out result);
        }
    }

    public enum AudioType : byte
    {
        Wav,
        Nsf,
        Unknown
    }
}
