using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using SingerDispatch.Utils;

namespace SingerDispatch.Controls
{
    class PhoneNumberConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;
            else
                return string.Format(new LafiPhoneFormatProvider(), SingerConstants.PhoneFormat, (string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;
            else
                return string.Format(new LafiPhoneFormatProvider(), SingerConstants.PhoneFormat, (string)value);
        }
    }
}
