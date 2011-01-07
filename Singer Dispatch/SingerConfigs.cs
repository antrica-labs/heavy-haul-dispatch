using System;
using System.Linq;

namespace SingerDispatch
{
    public class SingerConfigs
    {
        private static readonly SingerDispatchDataContext database = new SingerDispatchDataContext();

        public static SingerDispatchDataContext CommonDataContext
        {
            get
            {
                return database;
            }
        }

        public static Employee OperatingEmployee = null;

        public static string GetConfig(string key)
        {
            string result;

            try
            {
                var config = (from c in CommonDataContext.Configurations where c.Name == key select c).First();

                result = config.Value;
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

        public static string SingerSearchString
        {
            get
            {
                return "Singer Specialized";
            }
        }
        

        public static string PrintedDateFormatString
        {
            get
            {
                return GetConfig("PrintedDateFormatString") ?? "MMMM d, yyyy";
            }
        }

        public static string PrintedTimeFormatString
        {
            get
            {
                return GetConfig("PrintedTimeFormatString") ?? "HH:mm";
            }
        }
        
        public static decimal GST
        {
            get
            {
                var gst = GetConfig("GSTRate") ?? "";
                decimal result;

                try
                {
                    result = Convert.ToDecimal(gst);
                }
                catch (Exception)
                {
                    result = 0.05m;
                }

                return result;
            }
        }

        public static decimal FuelTax
        {
            get
            {
                var tax = GetConfig("FuelTaxRate");
                decimal result;

                try
                {
                    result = Convert.ToDecimal(tax);
                }
                catch (Exception)
                {
                    result = 0.00m;
                }

                return result;
            }

        }

        public static string DefaultRemoveItemMessage
        {
            get
            {
                var message = GetConfig("GenericRemoveItemConfirmation") ?? "Are you sure you want to remove this item?";

                return message;
            }
        }

        public static string PhoneFormat
        {
            get
            {
                return "{0:de}";
            }
        }

        public static double MinLoadHeight
        {
            get
            {
                return 4.15; // In metres
            }
        }

        public static string DefaultDispatchDescription
        {
            get
            {
                return "Supply men and equipment to transport {0}";
            }
        }
    }
}
