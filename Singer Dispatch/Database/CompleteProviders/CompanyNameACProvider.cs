using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFAutoCompleteBox.Provider;

namespace SingerDispatch.Database.CompleteProviders
{
    class CompanyNameACProvider : IAutoCompleteDataProvider
    {
        SingerDispatchDataContext Database { get; set; }

        public CompanyNameACProvider(SingerDispatchDataContext database)
        {
            Database = database;
        }

        public IEnumerable<object> GetItems(string textPattern)
        {
            if (textPattern.Length < 3) return null; // Don't bother returning results until more than 2 characters are entered.

            var companies = from c in Database.Companies where c.Name.Contains(textPattern) orderby c.Name select c;

            return companies;
        }
    }
}
