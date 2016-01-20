using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BlueBirdKassa
{
    [ValueConversion(typeof(Decimal), typeof(String))]
    public class ConvertDecimalToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Decimal data = (Decimal)value;
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
            Decimal result;
            if (Decimal.TryParse(strValue, out result))
            {
                return result;
            }
            return System.Windows.DependencyProperty.UnsetValue;
        }
    }
}
