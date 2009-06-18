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

        static SingerConstants()
        {
            contactMethods.Add("Email");
            contactMethods.Add("Primary phone");
            contactMethods.Add("Secondary phone");

            customerTypes.Add("Singer Specialized");
            customerTypes.Add("M.E. Signer Enterprise");
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
    }
}
