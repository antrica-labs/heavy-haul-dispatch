using System;
using System.Windows.Data;

namespace SingerDispatch.Controls
{
    class QuoteFriendlyNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2 || !(values[0] is int) || !(values[1] is int)) return "";
 
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
