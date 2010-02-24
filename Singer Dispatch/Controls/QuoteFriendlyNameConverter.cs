using System;
using System.Windows.Data;

namespace SingerDispatch.Controls
{
    class QuoteFriendlyNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if ((int)values[0] != 0)
                    return String.Format("{0} rev. {1}", values[0], values[1]);
                
                return "--";
            }
            catch
            {
                return "";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        
    }
}
