using System.Linq;
using System;

namespace SingerDispatch.Database
{
    public class EntityHelper
    {
        public static void PrepareEntityDelete(Quote quote, SingerDispatchDataContext context)
        {
            if (quote.Jobs.Count > 0)
                throw new Exception("One or more jobs rely on this quote, so it cannot be deleted.");

            foreach (var c in quote.QuoteCommodities.Where(c => c.ID != 0))
            {
                context.QuoteCommodities.DeleteOnSubmit(c);
            }

            foreach (var s in quote.QuoteSupplements.Where(s => s.ID != 0))
            {
                context.QuoteSupplements.DeleteOnSubmit(s);
            }

            foreach (var item in quote.QuoteStorageItems.Where(item => item.ID != 0))
            {
                context.QuoteStorageItems.DeleteOnSubmit(item);
            }

            foreach (var item in quote.QuoteConditions.Where(item => item.ID != 0))
            {
                context.QuoteConditions.DeleteOnSubmit(item);
            }

            if (quote.ID != 0)
            {
                context.Quotes.DeleteOnSubmit(quote);
            }
            
        }

        public static void PrepareEntityDelete(Job job, SingerDispatchDataContext context)
        {
            if (job.Invoices.Count > 0)
                throw new Exception("This job has already been invoiced and can no longer be deleted.");

            foreach (var c in job.JobCommodities.Where(c => c.ID != 0))
            {
                context.JobCommodities.DeleteOnSubmit(c);
            }

            foreach (var l in job.Loads.Where(l => l.ID != 0))
            {
                PrepareEntityDelete(l, context);
            }

            if (job.ID != 0)
                context.Jobs.DeleteOnSubmit(job);
        }

        public static void PrepareEntityDelete(Invoice invoice, SingerDispatchDataContext context)
        {
            foreach (var item in invoice.ReferenceNumbers.Where(item => item.ID != 0))
            {
                context.InvoiceReferenceNumbers.DeleteOnSubmit(item);
            }

            foreach (var item in invoice.InvoiceLineItems.Where(item => item.ID != 0))
            {
                foreach (var extra in item.Extras.Where(extra => extra.ID != 0))
                {
                    context.InvoiceExtras.DeleteOnSubmit(extra);
                }

                context.InvoiceLineItems.DeleteOnSubmit(item);
            }
            
            if (invoice.ID != 0)
                context.Invoices.DeleteOnSubmit(invoice);
        }

        public static void PrepareEntityDelete(Load load, SingerDispatchDataContext context)
        {
            foreach (var item in load.Dispatches.Where(item => item.ID != 0))
                context.Dispatches.DeleteOnSubmit(item);

            foreach (var item in load.LoadedCommodities.Where(item => item.ID != 0))
                context.LoadedCommodities.DeleteOnSubmit(item);

            foreach (var item in load.Permits.Where(item => item.ID != 0))
                context.Permits.DeleteOnSubmit(item);

            foreach (var item in load.ThirdPartyServices.Where(item => item.ID != 0))
                context.ThirdPartyServices.DeleteOnSubmit(item);

            if (load.ID != 0)
                context.Loads.DeleteOnSubmit(load);
        }


        public static void SaveAsQuoteRevision(Quote quote, SingerDispatchDataContext context)
        {
            var revision = (from q in context.Quotes where q.Number == quote.Number select q.Revision).Max() + 1;

            quote.Revision = revision ?? 0;

            context.SubmitChanges();
        }

        public static void SaveAsNewQuote(Quote quote, SingerDispatchDataContext context)
        {
            var number = (from q in context.Quotes select q.Number).Max() + 1;

            quote.Revision = 0;
            quote.Number = number ?? 10001;

            context.SubmitChanges();
        }

        public static void SaveAsNewJob(Job job, SingerDispatchDataContext context)
        {
            var number = (from j in context.Jobs select j.Number).Max() + 1;

            job.Number = number ?? 10001;

            context.SubmitChanges();
        }

        public static void SaveAsNewLoad(Load load, SingerDispatchDataContext context)
        {
            if (load.Job == null)
                throw new Exception("Load must be assigned to a job");

            var number = (from l in context.Loads where l.Job == load.Job select l.Number).Max() + 1;

            load.Number = number ?? 1;
            
            context.SubmitChanges();
        }

        public static void SaveAsNewDispatch(Dispatch dispatch, SingerDispatchDataContext context)
        {
            var number = (from d in context.Dispatches where d.Load == dispatch.Load select d.Number).Max() + 1;

            dispatch.Number = number ?? 1;

            context.SubmitChanges();
        }

        public static void SaveAsNewInvoice(Invoice invoice, SingerDispatchDataContext context)
        {
            var number = (from i in context.Invoices select i.Number).Max() + 1;

            invoice.Revision = 0;
            invoice.Number = number ?? 10001;

            context.SubmitChanges();
        }

        public static void SaveAsInvoiceRevision(Invoice invoice, SingerDispatchDataContext context)
        {
            var revision = (from i in context.Invoices where i.Number == invoice.Number select i.Revision).Max() + 1;

            invoice.Revision = revision ?? 0;

            context.SubmitChanges();
        }
    }
}
