using alclib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alcwin
{
    static class DisplayConvert
    {
        public static string Current(int c)
        {
            return Current(LogConvert.Current(c));
        }

        public static string Current(double c)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:f0}", c * 1000);
        }

        public static string Voltage(int b, int c)
        {
            if (b >= 0 && b < Data.BatTypes.Length)
                return Voltage(c * Data.BatTypes[b].Voltage);
            return null;
        }

        public static string Voltage(int v)
        {
            return Voltage(LogConvert.Voltage(v));
        }

        public static string Voltage(double v)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:f1}", v);
        }

        public static string Capacity(int c)
        {
            return Capacity(LogConvert.Capacity(c));
        }

        public static string Capacity(double c)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:f0}", c * 1000);
        }

        public static string Function(int f)
        {
            if (f>=0 && f<Data.Functions.Length)
                return Data.Functions[f];
            return null;
        }

        public static string BatType(int b)
        {
            if (b >= 0 && b < Data.BatTypes.Length)
                return Data.BatTypes[b].Caption;
            return null;
        }
    }
}
