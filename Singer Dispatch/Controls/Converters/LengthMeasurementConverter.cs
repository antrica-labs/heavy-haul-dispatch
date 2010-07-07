using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using SingerDispatch.Utils;
using System.IO;

namespace SingerDispatch.Controls
{
    class LengthMeasurementConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is Double) || !(values[1] is Boolean)) return "";

            Double measurement = (Double)values[0]; // Always in meters
            Boolean useImperial = (Boolean)values[1];
            string unit;

            if (measurement == 0.0) return "";

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
            double meters;
            var measurement = (string)value;

            measurement = measurement.Trim();

            if (measurement.EndsWith(MeasurementFormater.UMetres))
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
            else // Assume they are entering with no units and want meters
            {
                meters = Double.Parse(measurement);
            }
            
            var result = new object[1];
            
            result[0] = meters;            

            return result;                        
        }      
    }
}
