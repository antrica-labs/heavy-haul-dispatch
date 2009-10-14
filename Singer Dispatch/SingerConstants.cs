using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch
{
    class SingerConstants
    {
        private static List<string> contactMethods = new List<string>();
        private static List<string> customerTypes = new List<string>();
        private static List<string> serviceTypes = new List<string>();
        private static SingerDispatchDataContext database = new SingerDispatchDataContext();

        static SingerConstants()
        {
            contactMethods.Add("Email");
            contactMethods.Add("Primary phone");
            contactMethods.Add("Secondary phone");

            customerTypes.Add("Singer Specialized");
            customerTypes.Add("M.E. Signer Enterprise");

            serviceTypes.Add("Tractor");
            serviceTypes.Add("Crane");
            serviceTypes.Add("Pilot Car");
            serviceTypes.Add("Swamper");
        }

        public static List<string> ContactMethods 
        {
            get
            {
                return contactMethods;
            }
                
        }

        public static List<string> CustomerTypes
        {
            get
            {
                return customerTypes;
            }
        }

        public static List<string> ServiceTypes
        {
            get
            {
                return serviceTypes;
            }
        }

        public static SingerDispatchDataContext CommonDataContext
        {
            get
            {
                return database;
            }
        }

        public static string PrintedDateFormatString
        {
            get
            {
                return "MMMM d, yyyy";
            }
        }
    }
}
