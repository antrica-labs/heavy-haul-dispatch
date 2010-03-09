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
            foreach (var c in job.JobCommodities.Where(c => c.ID != 0))
            {
                context.JobCommodities.DeleteOnSubmit(c);
            }

            foreach (var d in job.Dispatches.Where(d => d.ID != 0))
            {
                context.Dispatches.DeleteOnSubmit(d);
            }
            
            foreach (var l in job.Loads.Where(l => l.ID != 0))
            {
                context.Loads.DeleteOnSubmit(l);
            }

            foreach (var t in job.ThirdPartyServices.Where(t => t.ID != 0))
            {
                context.ThirdPartyServices.DeleteOnSubmit(t);
            }

            foreach (var p in job.Permits.Where(p => p.ID != 0))
            {
                context.Permits.DeleteOnSubmit(p);
            }

            if (job.ID != 0)
                context.Jobs.DeleteOnSubmit(job);
            
        }

        public static void PrepareEntityDelete(Invoice invoice, SingerDispatchDataContext context)
        {
            foreach (var item in invoice.ReferenceNumbers.Where(item => item.ID != 0))
            {
                context.ReferenceNumbers.DeleteOnSubmit(item);
            }

            foreach (var item in invoice.InvoiceLineItems.Where(item => item.ID != 0))
            {
                context.InvoiceLineItems.DeleteOnSubmit(item);
            }

            foreach (var item in invoice.InvoiceExtras.Where(item => item.ID != 0))
            {
                context.InvoiceExtras.DeleteOnSubmit(item);
            }

            if (invoice.ID != 0)
                context.Invoices.DeleteOnSubmit(invoice);
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
