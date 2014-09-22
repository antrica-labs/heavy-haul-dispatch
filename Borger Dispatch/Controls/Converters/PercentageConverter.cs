using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Text.RegularExpressions;

namespace SingerDispatch.Controls
{
    class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var dec = value as decimal?;

            dec = dec ?? 0m;

            return string.Format("{0:0.##}%", (dec * 100));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var input = value as string;
            var dec = System.Convert.ToDecimal(input.Replace("%", ""));

            return (dec / 100);
        }
    }
}
