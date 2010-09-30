using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace SingerDispatch.Controls
{
    class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var time = (string)value;

            try
            {
                return TimeCleanup(time);
            }
            catch (Exception e)
            {
                return value;
            }
        }

        private string TimeCleanup(string time)
        {
            string result;

            time = time.Trim().ToUpper().Replace(" ", "").Replace(":", "");
            var is12hourPM = time.EndsWith("PM");
            time = time.Replace("PM", "").Replace("AM", "");

            if (time.Length == 3)
                time = "0" + time + "00";
            else if (time.Length == 4)
                time = time + "00";
            else if (time.Length != 6)
                throw new ArgumentException("Time is not valid");

            try
            {
                int hours = System.Convert.ToInt32(time.Substring(0, 2));
                int minutes = System.Convert.ToInt32(time.Substring(2, 2));
                int seconds = System.Convert.ToInt32(time.Substring(4, 2));

                if (is12hourPM)
                    hours += 12;

                result = string.Format("{0:D2}:{1:D2}", hours, minutes);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Time is not valid");
            }

            return result;
        }
    }
}
