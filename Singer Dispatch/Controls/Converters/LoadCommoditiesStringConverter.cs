using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Data.Linq;

namespace SingerDispatch.Controls
{
    class LoadCommoditiesStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {   try
            {
                var list = (EntitySet<JobCommodity>)value;
                var output = new StringBuilder();

                for (var i = 0; i < list.Count; i++)
                {
                    output.Append(list[i].Name);

                    if ((i + 1) != list.Count)
                        output.Append(", ");
                }

                return output.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
