using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace SingerDispatch.Security
{
    class AuthenticationService
    {
        public string GetCurrentUserName()
        {
            var name = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            return name.Split(new char[] { '\\' })[1];
        }

        public bool IsInAD(string username)
        {
            var search = new DirectorySearcher();

            search.Filter = String.Format("(SAMAccountName={0})", username);
            search.PropertiesToLoad.Add("CN");

            var result = search.FindOne();

            return result != null;
        }

        public List<string> GetADGroups(string username)
        {
            var search = new DirectorySearcher();

            search.Filter = String.Format("(SAMAccountName={0})", username);
            search.PropertiesToLoad.Add("memberOf");

            var result = search.FindOne();

            var groups = new List<string>();

            if (result != null)
            {
                groups.AddRange(result.Properties["memberOf"].Cast<string>());
            }

            return groups;
        }
    }
}
