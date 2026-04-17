using alclib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace alcwin
{
    class BatTypeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                int t = (int)value;
                if (t >= 0 && t < LogConvert.BatTypes)
                    return LogConvert.BatType(t);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class VoltageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length==2 && values[0] is int && values[1] is int)
            {
                int t = (int)values[0];
                int c = (int)values[1];
                if (t >= 0 && t < Data.BatTypes.Length)
                {
                    int v = Data.BatTypes[t].Voltage;
                    return LogConvert.Voltage(v * c).ToString("f1") + " V";
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class CapacityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is int && values[1] is int)
            {
                int t = (int)values[0];
                int c = (int)values[1];
                if (t >= 0 && t < Data.BatTypes.Length)
                {
                    return LogConvert.Capacity(c).ToString("f1") + " Ah";
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class VoltageToFloat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
                return (int)value / 1000.0;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class LogTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                int v = (int)value * 5;
                int s = v % 60;
                int m = v / 60 % 60;
                int h = v / 3600;
                return $"{h:d}:{m:d02}:{s:d02}";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
