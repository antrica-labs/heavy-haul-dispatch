﻿using System.Linq;
using System;

namespace SingerDispatch.Database
{
    public class EntityHelper
    {
        public static void PrepareEntityDelete(Quote quote, SingerDispatchDataContext context)
        {
            if (quote.Jobs.Count > 0)
                throw new Exception("One or more jobs rely on this quote, so it cannot be deleted.");

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

            foreach (var item in quote.StorageItems)
            {
                if (item.ID != 0)
                {
                    context.StorageItems.DeleteOnSubmit(item);
                }
            }

            foreach (var item in quote.QuoteConditions)
            {
                if (item.ID != 0)
                {
                    context.QuoteConditions.DeleteOnSubmit(item);
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

            if (job.ID != 0)
                context.Jobs.DeleteOnSubmit(job);
            
        }

        public static void SaveAsQuoteRevision(Quote quote, SingerDispatchDataContext context)
        {
            var revision = (from q in context.Quotes where q.Number == quote.Number select q.Revision).Max() + 1;

            quote.Revision = (revision != null) ? revision : 0;

            context.SubmitChanges();
        }

        public static void SaveAsNewQuote(Quote quote, SingerDispatchDataContext context)
        {
            var number = (from q in context.Quotes select q.Number).Max() + 1;

            quote.Revision = 0;
            quote.Number = (number != null) ? number : 10001;

            context.SubmitChanges();
        }

        public static void SaveAsNewJob(Job job, SingerDispatchDataContext context)
        {
            var number = (from j in context.Jobs select j.Number).Max() + 1;

            job.Number = (number != null) ? number : 10001;

            context.SubmitChanges();
        }
    }
}
