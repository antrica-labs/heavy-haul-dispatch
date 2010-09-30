using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using SingerDispatch.Utils;
using System.IO;
using System.Windows.Controls;
using System.Windows;

namespace SingerDispatch.Controls
{
    class LengthMeasurementConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is Double)) return "";

            var measurement = (Double)values[0]; // Always in meters
            var useImperial = false;
            string unit;

            if (measurement == 0.0) return "";
            if (values[1] is Boolean) useImperial = (Boolean)values[1];

            if (useImperial == true)
            {
                unit = MeasurementFormater.UFeet;
            }
            else
            {
                unit = MeasurementFormater.UMetres;
            }

            return MeasurementFormater.FromMetres(measurement, unit);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = new object[1];
            double meters;
            var measurement = (string)value;

            measurement = measurement.Trim();

            try
            {
                if (measurement.Length == 0)
                {
                    meters = 0;
                }
                else if (measurement.EndsWith(MeasurementFormater.UMetres))
                {
                    meters = Double.Parse(measurement.Replace(MeasurementFormater.UMetres, ""));
                }
                else if (measurement.EndsWith(MeasurementFormater.UCentimetres))
                {
                    meters = Double.Parse(measurement.Replace(MeasurementFormater.UCentimetres, "")) / 100;
                }
                else if (measurement.Contains(MeasurementFormater.UFeet) || measurement.Contains(MeasurementFormater.UInches))
                {
                    measurement = measurement.Replace(MeasurementFormater.UInches, "").Replace(MeasurementFormater.UFeet, "-");

                    if (measurement.EndsWith("-"))
                        measurement += "0";

                    var tokens = measurement.Split('-');
                    double inches;

                    if (tokens.Length == 2)
                    {
                        inches = Double.Parse(tokens[0]) * 12 + Double.Parse(tokens[1]);
                    }
                    else
                    {
                        inches = Double.Parse(tokens[0]);
                    }

                    meters = inches / 39.37;
                }
                else 
                {
                    meters = Double.Parse(measurement);

                    // They didn't actually enter a unit, so we need to check what their preference is to see if feet was expected.
                    var settings = new UserSettings();
                    if (!settings.MetricMeasurements)
                        meters *= 0.3048;
                }

                result[0] = meters;
            }
            catch 
            {
                result[0] = DependencyProperty.UnsetValue;
            }

            return result;
        }        
    }
}
