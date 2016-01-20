using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BlueBirdKassa
{
    [ValueConversion(typeof(int), typeof(String))]
    public class ConvertIntToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int data = (int)value;
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
            int result;
            if (int.TryParse(strValue, out result))
            {
                return result;
            }
            return System.Windows.DependencyProperty.UnsetValue;
        }
    }
}
