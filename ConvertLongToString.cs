using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BlueBirdKassa
{
    [ValueConversion(typeof(long), typeof(String))]
    public class ConvertLongToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            long data = (long)value;
            string result = " ";
            if (data > 0)
            {
                result = data.ToString();
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value as string;
            long result;
            if (long.TryParse(strValue, out result))
            {
                return result;
            }
            return System.Windows.DependencyProperty.UnsetValue;
        }
    }
}
