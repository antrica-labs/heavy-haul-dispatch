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
        {
            string output = "";

            try
            {                
                var list = ((EntitySet<JobCommodity>)value).ToList();

                for (var i = 0; i < list.Count(); i++)
                {
                    output += list[i].Name;

                    if ((i + 1) != list.Count())
                        output += ", ";
                }
            }
            catch (Exception)
            {
                output = "";
            }

            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
