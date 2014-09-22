//-----------------------------------------------------------------------
// <copyright file="LafiPhoneFormatProvider.cs" company="LostAndFoundIdentity">
// Copyright (c) 2007 LostAndFoundIdentity.com | All rights reserved.
// </copyright>
// <author>Dmitry Kazantsev</author>
//-----------------------------------------------------------------------
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

[assembly: System.CLSCompliant(true)]
namespace SingerDispatch.Utils
{
    /// <summary>
    /// Represents implementation class LafiPhoneFormatProvider that is implements IFormatProvider and ICustomFormatter
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Lafi", Justification = "'Lafi' stands for Lost And Found Identity")]
    public class LafiPhoneFormatProvider : ICustomFormatter, IFormatProvider
    {
        /// <summary>
        /// Regular expression "formula" designed to catch phone number extentions
        /// </summary>
        private const string ExtensionFormula = "((\\s{1,2})?(e|ext|ex|extn|extension|x)(\\.)?(\\s{1,2})?)(\\d+)";

        /// <summary>
        /// Gets type of the format
        /// </summary>
        /// <param name="formatType">Format in question</param>
        /// <returns>Type of the format in question</returns>
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Formats provided string with a pre-defined template
        /// </summary>
        /// <param name="format">Name of the format presented as {0:x}, where 'x' can be 'a', 'c', 'd', 'de', 'e', or 's' </param>
        /// <param name="arg">Value to be formatted</param>
        /// <param name="formatProvider">The format provider class</param>
        /// <returns>Formatted string</returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            // Convert argument to a string.
            string result = arg.ToString();

            switch (format.ToUpperInvariant())
            {
                case null:
                    {
                        return result;
                    }

                case "A":
                    {
                        return FormatPhone(result, "-");
                    }

                case "C":
                    {
                        return FormatPhone(result, string.Empty);
                    }

                case "D": // Default
                    {
                        return FormatPhone(result);   
                    }

                case "DE": // Default + Extension
                    {
                        if (HasExtension(result))
                        {
                            string extension = GetExtension(result);
                            string phone = SubstructExtension(result);
                            phone = FormatPhone(phone);
                            phone = string.Format(CultureInfo.CurrentCulture, "{0} Ext. {1}", phone, extension);
                            return phone;
                        }

                        return FormatPhone(result);
                    }

                case "E": // Extension
                    {
                        if (HasExtension(result))
                        {
                            string extension = GetExtension(result);
                            string phone = SubstructExtension(result);
                            phone = FormatPhone(phone, "-");
                            phone = string.Format(CultureInfo.CurrentCulture, "{0} Ext. {1}", phone, extension);
                            return phone;
                        }

                        return FormatPhone(result, "-");
                    }

                case "S": // Space
                    {
                        return FormatPhone(result, " ");
                    }

                default:
                    {
                        throw new FormatException("'" + format + "' is not a supported format type.");
                    }
            }
        }

        /// <summary>
        /// Formats string representation of North American telephone number;  Inserts provided separator
        /// </summary>
        /// <param name="value">String containing North American telephone number</param>
        /// <param name="separator">String containing separator character</param>
        /// <returns>xxx.xxxx or xxx.xxx.xxxx or x.xxx.xxx.xxxx</returns>
        private static string FormatPhone(string value, string separator)
        {
            string tempString = GetNumericValue(value);
            string countryCode = string.Empty;
            string areaCode = string.Empty;
            string firstThree = string.Empty;
            string lastFour = string.Empty;

            switch (tempString.Length)
            {
                case 7: //// nnn.nnnn
                    {
                        firstThree = tempString.Substring(0, 3);
                        lastFour = tempString.Substring(3, 4);
                        return string.Format(CultureInfo.CurrentCulture, "{0}{2}{1}", firstThree, lastFour, separator);
                    }

                case 10: //// nnn.nnn.nnnn
                    {
                        areaCode = tempString.Substring(0, 3);
                        firstThree = tempString.Substring(3, 3);
                        lastFour = tempString.Substring(6, 4);
                        return string.Format(CultureInfo.CurrentCulture, "{0}{3}{1}{3}{2}", areaCode, firstThree, lastFour, separator);
                    }

                case 11: //// n.nnn.nnn.nnnn
                    {
                        countryCode = tempString.Substring(0, 1);
                        areaCode = tempString.Substring(1, 3);
                        firstThree = tempString.Substring(4, 3);
                        lastFour = tempString.Substring(7, 4);
                        return string.Format(CultureInfo.CurrentCulture, "{0}{4}{1}{4}{2}{4}{3}", countryCode, areaCode, firstThree, lastFour, separator);
                    }

                default:
                    {
                        return value;
                    }
            }
        }

        /// <summary>
        /// Formats a string representing a North American phone number in the "default" format
        /// </summary>
        /// <param name="value">The "phone number" to be formatted</param>
        /// <returns>Formatted phone number</returns>
        private static string FormatPhone(string value)
        {
            string tempString = GetNumericValue(value);
            string countryCode = string.Empty;
            string areaCode = string.Empty;
            string firstThree = string.Empty;
            string lastFour = string.Empty;

            switch (tempString.Length)
            {
                case 7: //// nnn-nnnn
                    {
                        firstThree = tempString.Substring(0, 3);
                        lastFour = tempString.Substring(3, 4);
                        return string.Format(CultureInfo.CurrentCulture, "{0}-{1}", firstThree, lastFour);
                    }

                case 10: //// (nnn) nnn-nnnn
                    {
                        areaCode = tempString.Substring(0, 3);
                        firstThree = tempString.Substring(3, 3);
                        lastFour = tempString.Substring(6, 4);
                        return string.Format(CultureInfo.CurrentCulture, "({0}) {1}-{2}", areaCode, firstThree, lastFour);
                    }

                case 11: //// +n (nnn) nnn-nnnn
                    {
                        countryCode = tempString.Substring(0, 1);
                        areaCode = tempString.Substring(1, 3);
                        firstThree = tempString.Substring(4, 3);
                        lastFour = tempString.Substring(7, 4);
                        return string.Format(CultureInfo.CurrentCulture, "+{0} ({1}) {2}-{3}", countryCode, areaCode, firstThree, lastFour);
                    }

                default:
                    {
                        return value;
                    }
            }
        }

        /// <summary>
        /// Strips all non-numerical characters form provided string
        /// </summary>
        /// <param name="value">String contaning North American telephone number</param>
        /// <returns>Numerical values of provided string</returns>
        private static string GetNumericValue(string value)
        {
            Regex notNumerical = new Regex("[\\D]");
            foreach (Match match in notNumerical.Matches(value))
            {
                value = value.Replace(match.Value, string.Empty);
            }

            return value;
        }

        /// <summary>
        /// Determines whether provided string containes a phone "extension" and returns numerical value of that extention
        /// </summary>
        /// <param name="value">Phone number with expension</param>
        /// <returns>Telephone number extension</returns>
        private static string GetExtension(string value)
        {
            Regex extension = new Regex(ExtensionFormula, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            MatchCollection matches = extension.Matches(value);

            if (0 == matches.Count || string.IsNullOrEmpty(matches[0].Groups[6].Value))
            {
                return string.Empty;
            }

            return matches[0].Groups[6].Value;
        }

        /// <summary>
        /// Determines whether provided string containes a phone "extension" and returns numerical value of that extention
        /// </summary>
        /// <param name="value">Phone number with expension</param>
        /// <returns>Telephone number extension</returns>
        private static string SubstructExtension(string value)
        {
            Regex extension = new Regex(ExtensionFormula, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            value = extension.Replace(value, string.Empty);
            return value;
        }

        /// <summary>
        /// Determines whether the string in question contains the phone number with extension or not
        /// </summary>
        /// <param name="value">The phone number in question</param>
        /// <returns>'True' when extension is found and 'false' when it is not found</returns>
        private static bool HasExtension(string value)
        {
            Regex extension = new Regex(ExtensionFormula, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            MatchCollection matches = extension.Matches(value);

            if (0 == matches.Count)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
