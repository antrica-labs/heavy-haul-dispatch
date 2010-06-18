using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Utils
{
    public class MeasurementFormater
    {
        public static string U_Millimeters = "mm";
        public static string U_Meters = "m";
        public static string U_Centimeters = "cm";
        public static string U_Kilometers = "km";
        public static string U_Kilograms = "kg";
        public static string U_Inches = "\"";
        public static string U_Feet = "'";
        public static string U_Miles = "mi";
        public static string U_Pounds = "lbs";        

        public const double CM_PER_INCH = 2.54;
        public const double LBS_PER_KG = 2.20462262;
        public const int CM_PER_M = 100;
        public const int MM_PER_CM = 10;        

        public static string FormatCentimeters(double cm, string unit)
        {
            return "";
        }

        public static string CentimetersToFeet(double cm)
        {
            int feet, inches;

            inches = (int)Math.Ceiling(cm / CM_PER_INCH);
            
            feet = inches / 12;
            inches = inches % 12;

            var replacements = new object[4];

            replacements[0] = feet;
            replacements[1] = U_Feet;
            replacements[2] = inches;
            replacements[3] = U_Inches;

            return string.Format("{0}{1}{2}{3}", replacements);
        }

        public static string CentimetersToMillimeters(double cm)
        {
            double mm = cm * MM_PER_CM;

            return string.Format("{0}{1}", mm, U_Millimeters);
        }

        public static string CentimetersToMeters(double cm)
        {
            double meters = cm / CM_PER_M;

            return string.Format("{0}{1}", meters, U_Meters);
        }

        public static string KilogramsToPounds(double kg)
        {
            double pounds;

            pounds = kg * LBS_PER_KG;

            return string.Format("{0}{1}", pounds, U_Pounds);
        }

        public static string KiogramsToKilograms(double kg)
        {
            return string.Format("{0}{1}", kg, U_Kilograms);
        }
    }
}
