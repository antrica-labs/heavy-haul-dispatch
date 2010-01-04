using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Database
{
    class EntityHelper
    {
        public static void PrepareEntityDelete(Quote quote, SingerDispatchDataContext context)
        {
            foreach (var c in quote.QuoteCommodities)
            {
                if (c.ID != 0)
                {
                    context.QuoteCommodities.DeleteOnSubmit(c);
                }
            }

            foreach (var s in quote.QuoteSupplements)
            {
                if (s.ID != 0)
                {
                    context.QuoteSupplements.DeleteOnSubmit(s);
                }
            }

            if (quote.ID != 0)
            {
                context.Quotes.DeleteOnSubmit(quote);
            }
        }

        public static void PrepareEntityDelete(Job job, SingerDispatchDataContext context)
        {
            foreach (var c in job.JobCommodities)
            {
                if (c.ID != 0)
                {
                    context.JobCommodities.DeleteOnSubmit(c);
                }            
            }

            foreach (var d in job.Dispatches)
            {
                if (d.ID != 0)
                {
                    context.Dispatches.DeleteOnSubmit(d);
                }
            }
            
            foreach (var l in job.Loads)
            {
                if (l.ID != 0)
                {
                    context.Loads.DeleteOnSubmit(l);
                }
            }

            foreach (var t in job.ThirdPartyServices)
            {
                if (t.ID != 0)
                {
                    context.ThirdPartyServices.DeleteOnSubmit(t);
                }
            }

            foreach (var p in job.Permits)
            {
                if (p.ID != 0)
                {
                    context.Permits.DeleteOnSubmit(p);
                }
            }         
        }

        public static void SaveAsNewQuote(Quote quote, SingerDispatchDataContext context)
        {
            quote.Revision = 0;

            try
            {
                quote.Number = (from q in context.Quotes select q.Number).Max() + 1;
            }
            catch
            {
                quote.Number = 1;
            }

            context.SubmitChanges();
        }

        public static void SaveAsNewJob(Job job, SingerDispatchDataContext context)
        {
            try
            {
                job.Number = (from j in context.Jobs select j.Number).Max() + 1;
            }
            catch
            {
                job.Number = 1;
            }

            context.SubmitChanges();
        }
    }
}
