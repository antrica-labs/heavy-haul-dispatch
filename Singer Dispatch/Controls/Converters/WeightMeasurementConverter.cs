using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using SingerDispatch.Utils;
using System.Windows;

namespace SingerDispatch.Controls
{
    class WeightMeasurementConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is Double) || !(values[1] is Boolean)) return "";

            Double kg = (Double)values[0];
            Boolean imperial = (Boolean)values[1];
            string unit;

            if (kg == 0.0) return "";

            if (imperial == true)
            {
                unit = MeasurementFormater.UPounds;
            }
            else
            {
                unit = MeasurementFormater.UKilograms;
            }

            return MeasurementFormater.FromKilograms(kg, unit);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = new object[1];
            double kg;
            var measurement = (string)value;

            measurement = measurement.Trim();

            try
            {
                if (measurement.Length == 0)
                {
                    kg = 0;
                }
                if (measurement.EndsWith(MeasurementFormater.UPounds))
                {
                    double pounds = Double.Parse(measurement.Replace(MeasurementFormater.UPounds, ""));

                    kg = pounds * 0.45359237;
                }
                else
                {
                    kg = Double.Parse(measurement.Replace(MeasurementFormater.UKilograms, ""));
                }

                result[0] = kg;
            }
            catch
            {
                result[0] = DependencyProperty.UnsetValue;
            }
            
            return result;
        }
    }
}
