using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Utils
{
    public class MeasurementFormater
    {
        public const string UMillimetres = "mm";
        public const string UMetres = "m";
        public const string UCentimetres = "cm";
        public const string UKilometres = "km";
        public const string UKilograms = "kg";
        public const string UInches = "\"";
        public const string UFeet = "'";
        public const string UMiles = "mi";
        public const string UPounds = "lbs";

        private const double CM_PER_INCH = 2.54;
        private const double LBS_PER_KG = 2.20462262;
        private const int M_PER_KM = 1000;
        private const int CM_PER_M = 100;
        private const int MM_PER_CM = 10;
        private const int INCHES_PER_MILE = 63360;

        private const string METRIC_L_SIGFIG = "0.##";
        private const string METRIC_W_SIGFIG = "0";
        private const string INCHES_SIGFIG = "0";
        private const string FEET_SIGFIG = "0";
        private const string MILES_SIGFIG = "0.##";
        private const string POUNDS_SIGFIG = "0.##";

        public static string FromMetres(double m, string unit)
        {
            switch (unit)
            {
                case UMillimetres:
                    return MetresToMillimetres(m);
                case UMetres:
                    return MetresToMetres(m);                    
                case UCentimetres:
                    return MetresToCentimetres(m);
                case UKilometres:
                    return MetresToKilometres(m);
                case UInches:
                    return MetresToInches(m);
                case UFeet:
                    return MetresToFeet(m);
                case UMiles:
                    return MetresToMiles(m);
            }

            throw new Exception(string.Format("Unable to convert centimetres to unit: {0}", unit));
        }

        public static string FromKilograms(double kg, string unit)
        {
            switch (unit)
            {
                case UKilograms:
                    return KiogramsToKilograms(kg);
                case UPounds:
                    return KilogramsToPounds(kg);
            }

            throw new Exception(string.Format("Unable to convert kilograms to unit: {0}", unit));
        }

        public static string MetresToMillimetres(double m)
        {
            double mm = (m * CM_PER_M) * MM_PER_CM;

            return string.Format("{0}{1}", mm.ToString(METRIC_L_SIGFIG), UMillimetres);
        }

        public static string MetresToCentimetres(double m)
        {
            double cm = m * CM_PER_M;

            return string.Format("{0}{1}", cm.ToString(METRIC_L_SIGFIG), UCentimetres);
        }

        public static string MetresToMetres(double m)
        {
            return string.Format("{0}{1}", m.ToString(METRIC_L_SIGFIG), UMetres);
        }

        public static string MetresToKilometres(double m)
        {
            double km = m / M_PER_KM;

            return string.Format("{0}{1}", km.ToString(METRIC_L_SIGFIG), UKilometres);
        }

        public static string MetresToInches(double m)
        {
            double inches = (m * CM_PER_M) / CM_PER_INCH;

            return string.Format("{0}{1}", inches.ToString(INCHES_SIGFIG), UInches);
        }    

        public static string MetresToFeet(double m)
        {
            double feet, inches;

            inches = (m * CM_PER_M) / CM_PER_INCH;
            
            feet = Math.Floor(inches / 12);
            inches = inches % 12;

            var replacements = new object[4];

            replacements[0] = feet.ToString(FEET_SIGFIG);
            replacements[1] = UFeet;
            replacements[2] = inches.ToString(INCHES_SIGFIG);
            replacements[3] = UInches;

            return string.Format("{0}{1}{2}{3}", replacements);
        }

        public static string MetresToMiles(double m)
        {
            double inches;
            double miles;

            inches = (m * CM_PER_M) / CM_PER_INCH;
            miles = inches / INCHES_PER_MILE;

            return string.Format("{0}{1}", miles.ToString(MILES_SIGFIG), UMiles);
        }

        public static string KilogramsToPounds(double kg)
        {
            double pounds;

            pounds = kg * LBS_PER_KG;

            return string.Format("{0}{1}", pounds.ToString(POUNDS_SIGFIG), UPounds);
        }

        public static string KiogramsToKilograms(double kg)
        {
            return string.Format("{0}{1}", kg.ToString(METRIC_W_SIGFIG), UKilograms);
        }
    }
}
