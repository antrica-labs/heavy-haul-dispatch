using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFAutoCompleteBox.Provider;
using System.Collections.ObjectModel;

namespace SingerDispatch.Database.CompleteProviders
{
    class JobNumAndNameACProvider : IAutoCompleteDataProvider
    {
        IEnumerable<Job> Jobs;

        public JobNumAndNameACProvider(IEnumerable<Job> jobs)
        {
            Jobs = jobs;
        }

        public IEnumerable<object> GetItems(string textPattern)
        {
            var jobs = from j in Jobs where (j.Number != null && j.Number.ToString().Contains(textPattern)) || (j.Name != null && j.Name.ToUpper().Contains(textPattern.ToUpper())) orderby j.Number select j;

            return jobs;
        }
    }
}
