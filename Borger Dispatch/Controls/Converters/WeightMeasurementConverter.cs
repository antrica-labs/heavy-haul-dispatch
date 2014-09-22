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
            if (values.Length != 2 || !(values[0] is Double)) return "";

            var kg = (Double)values[0];
            var imperial = false;            
            string unit;

            if (kg == 0.0) return "";
            if (values[1] is Boolean) imperial = (Boolean)values[1];

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
            var measurement = (string)value;

            measurement = measurement.Trim();

            try
            {
                if (measurement.Length == 0)
                {
                    result[0] = null;
                } 
                else if (measurement.EndsWith(MeasurementFormater.UPounds))
                {
                    double pounds = Double.Parse(measurement.Replace(MeasurementFormater.UPounds, ""));

                    result[0] = pounds * 0.45359237;
                }
                else if (measurement.EndsWith(MeasurementFormater.UKilograms))
                {
                    result[0] = Double.Parse(measurement.Replace(MeasurementFormater.UKilograms, "")); ;
                }
                else
                {   
                    var kg = Double.Parse(measurement);

                    // No unit was entered, so check what the users preference is
                    var settings = new UserSettings();
                    if (!settings.MetricMeasurements)
                        kg *= 0.45359237;

                    result[0] = kg;
                }
            }
            catch
            {
                result[0] = DependencyProperty.UnsetValue;
            }
            
            return result;
        }
    }
}
