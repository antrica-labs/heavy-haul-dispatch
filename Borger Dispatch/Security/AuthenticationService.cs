using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace SingerDispatch.Security
{
    class AuthenticationService
    {
        public static string GetCurrentUserName()
        {
            var name = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            return name.Split(new char[] { '\\' })[1];
        }

        public static bool IsInAD(string username)
        {
            var search = new DirectorySearcher();

            search.Filter = String.Format("(SAMAccountName={0})", username);
            search.PropertiesToLoad.Add("CN");

            try
            {
                var result = search.FindOne();

                return result != null;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static List<string> GetADGroups(string username)
        {
            var search = new DirectorySearcher();

            search.Filter = String.Format("(SAMAccountName={0})", username);
            search.PropertiesToLoad.Add("memberOf");

            try
            {
                var result = search.FindOne();

                var groups = new List<string>();

                if (result != null)
                {
                    groups.AddRange(result.Properties["memberOf"].Cast<string>());
                }

                return groups;
            }
            catch (Exception e)
            {
                return new List<string>();
            }
        }
    }
}
