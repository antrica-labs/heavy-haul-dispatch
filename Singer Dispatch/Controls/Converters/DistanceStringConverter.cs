﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using SingerDispatch.Utils;

namespace SingerDispatch.Controls
{
    class DistanceStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is Double) || !(values[1] is Boolean)) return "";

            Double meters = (Double)values[0];
            Boolean imperial = (Boolean)values[1];
            string unit;

            if (imperial == true)
            {
                unit = MeasurementFormater.UFeet;
            }
            else
            {
                unit = MeasurementFormater.UMetres;
            }

            return MeasurementFormater.FromMetres(meters, unit);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
