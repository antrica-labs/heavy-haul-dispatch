using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using SingerDispatch.Utils;
using System.IO;

namespace SingerDispatch.Controls
{
    class DistanceStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is Double) || !(values[1] is Boolean)) return "";

            Double measurement = (Double)values[0]; // Always in meters
            Boolean useImperial = (Boolean)values[1];
            string unit;

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
            bool isImperial;
            var measurement = (string)value;

            measurement = measurement.Trim();

            if (measurement.EndsWith(MeasurementFormater.UMetres))
            {
                meters = Double.Parse(measurement.Replace(MeasurementFormater.UMetres, ""));
                isImperial = false;
            }
            else if (measurement.EndsWith(MeasurementFormater.UCentimetres))
            {
                meters = Double.Parse(measurement.Replace(MeasurementFormater.UCentimetres, "")) / 100;                
                isImperial = false;
            }
            else if (measurement.Contains(MeasurementFormater.UFeet) || measurement.Contains(MeasurementFormater.UInches))
            {
                measurement = measurement.Replace(MeasurementFormater.UInches, "").Replace(MeasurementFormater.UFeet, "-");

                if (measurement.EndsWith("-"))
                    measurement += "0";

                var tokens = measurement.Split('-');
                int inches;

                if (tokens.Length == 2)                
                {
                    inches = Int32.Parse(tokens[0]) * 12 + Int32.Parse(tokens[1]);
                }
                else
                {
                    inches = Int32.Parse(tokens[0]);
                }

                meters = inches / 39.37;
                isImperial = true;
            }
            else // Assume they are entering with no units and want meters
            {
                meters = Double.Parse(measurement);
                isImperial = false;
            }
            
            var result = new object[2];
            
            result[0] = meters;
            result[1] = isImperial;

            return result;                        
        }      
    }
}
