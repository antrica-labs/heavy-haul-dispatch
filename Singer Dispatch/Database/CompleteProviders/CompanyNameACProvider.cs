using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFAutoCompleteBox.Provider;
using System.Collections.ObjectModel;

namespace SingerDispatch.Database.CompleteProviders
{
    class CompanyNameACProvider : IAutoCompleteDataProvider
    {
        ObservableCollection<Company> Companies;

        public CompanyNameACProvider(ObservableCollection<Company> companies)
        {
            Companies = companies;
        }

        public IEnumerable<object> GetItems(string textPattern)
        {
            if (textPattern.Length < 3) return null; // Don't bother returning results until more than 2 characters are entered.

            var companies = from c in Companies where c.Name.ToUpper().Contains(textPattern.ToUpper()) orderby c.Name select c;

            return companies;
        }
    }
}
