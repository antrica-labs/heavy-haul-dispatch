using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;

namespace SingerDispatch.Permissions
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
            DirectorySearcher search = new DirectorySearcher();
            search.Filter = String.Format("(SAMAccountName={0})", username);
            search.PropertiesToLoad.Add("CN");
            SearchResult result = search.FindOne();

            return result != null;
        }

        public List<string> GetADGroups(string username)
        {
            DirectorySearcher search = new DirectorySearcher();
            search.Filter = String.Format("(SAMAccountName={0})", username);
            search.PropertiesToLoad.Add("memberOf");
            SearchResult result = search.FindOne();

            List<string> groups = new List<string>();

            if (result != null)
            {
                foreach (var entry in result.Properties["memberOf"])
                {
                    groups.Add((string)entry);
                }
            }

            return groups;
        }
    }
}
