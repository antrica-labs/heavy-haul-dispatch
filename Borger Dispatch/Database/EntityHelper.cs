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

            foreach (var item in quote.QuoteInclusions.Where(item => item.ID != 0))
            {
                context.QuoteInclusions.DeleteOnSubmit(item);
            }

            foreach (var item in quote.QuoteNotes.Where(item => item.ID != 0))
            {
                context.QuoteNotes.DeleteOnSubmit(item);
            }

            if (quote.ID != 0)
            {
                context.Quotes.DeleteOnSubmit(quote);

                try
                {
                    var number = context.QuoteNumbers.Where(n => n.Number == quote.Number).First();

                    context.QuoteNumbers.DeleteOnSubmit(number);
                }
                catch
                { }
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

            foreach (var r in job.ReferenceNumbers.Where(r => r.ID != 0))
            {
                context.JobReferenceNumbers.DeleteOnSubmit(r);
            }

            if (job.ID != 0)
            {
                context.Jobs.DeleteOnSubmit(job);

                try
                {
                    var number = context.JobNumbers.Where(n => n.Number == job.Number).First();

                    context.JobNumbers.DeleteOnSubmit(number);    
                }
                catch
                { }
            }
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
            context.LoadedCommodities.DeleteAllOnSubmit(load.LoadedCommodities.Where(item => item.ID != 0));
            context.Dispatches.DeleteAllOnSubmit(load.Dispatches.Where(item => item.ID != 0));
            context.Permits.DeleteAllOnSubmit(load.Permits.Where(item => item.ID != 0));
            context.ThirdPartyServices.DeleteAllOnSubmit(load.ThirdPartyServices.Where(item => item.ID != 0));
            context.ExtraEquipment.DeleteAllOnSubmit(load.ExtraEquipment.Where(item => item.ID != 0));
            context.LoadReferenceNumbers.DeleteAllOnSubmit(load.ReferenceNumbers.Where(item => item.ID != 0));

            if (load.ID != 0)
                context.Loads.DeleteOnSubmit(load);
        }


        public static void SaveAsQuoteRevision(Quote quote, SingerDispatchDataContext context)
        {
            var revision = (from q in context.Quotes where q.Number == quote.Number select q.Revision).Max() + 1;

            quote.Revision = revision ?? 0;

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

        public static void SaveAsNewStorageItem(StorageItem item, SingerDispatchDataContext context)
        {
            var number = (from i in context.StorageItems where i.Job == item.Job select i.Number).Max() + 1;

            item.Number = number ?? 1;

            context.SubmitChanges();
        }

        public static void SaveAsNewInvoice(Invoice invoice, SingerDispatchDataContext context)
        {
            var number = (from i in context.Invoices select i.Number).Max() + 1;

            invoice.Revision = 0;
            invoice.Number = number ?? 12001;

            context.SubmitChanges();
        }

        public static void SaveAsInvoiceRevision(Invoice invoice, SingerDispatchDataContext context)
        {
            var revision = (from i in context.Invoices where i.Number == invoice.Number select i.Revision).Max() + 1;

            invoice.Revision = revision ?? 1;

            context.SubmitChanges();
        }

        public static void SuggestQuoteNumber(string number, SingerDispatchDataContext context)
        {
            RecordQuoteNumber(number, context);
        }

        private static void RecordQuoteNumber(string number, SingerDispatchDataContext context)
        {
            try
            {
                var proposal = new QuoteNumber() { Number = number };

                context.QuoteNumbers.InsertOnSubmit(proposal);
                context.SubmitChanges();
            }
            catch
            {
                context.RevertChanges();

                throw new InvalidOperationException(string.Format("Number {0} is already in use", number));
            }
        }

        public static void SuggestJobNumber(int number, SingerDispatchDataContext context)
        {
            int max;

            try
            {
                max = Convert.ToInt32(SingerConfigs.GetConfig("MaxUserDefinedJobNumber"));
            }
            catch
            {
                max = Int32.MaxValue;
            }

            if (number >= max)
                throw new ArgumentOutOfRangeException("Job number", string.Format("Specified value is above the max of {0}", max));

            RecordJobNumber(number, context);
        }

        private static void RecordJobNumber(int number, SingerDispatchDataContext context)
        {
            try
            {
                var proposal = new JobNumber() { Number = number };

                context.JobNumbers.InsertOnSubmit(proposal);
                context.SubmitChanges();
            }
            catch
            {
                context.RevertChanges();

                throw new InvalidOperationException(string.Format("Number {0} is already in use", number));
            } 
        }

        public static int GenerateJobNumber(SingerDispatchDataContext context)
        {
            int next;
            int min = Convert.ToInt32(SingerConfigs.GetConfig("MaxUserDefinedJobNumber") ?? "0");

            try
            {
                next = (from j in context.JobNumbers select j.Number).Max() + 1;
            }
            catch (InvalidOperationException e)
            {
                next = min;
            }

            next = Math.Max(min, next);

            RecordJobNumber(next, context);
            
            return next;            
        }

        /*
        public static long GenerateQuoteNumber(SingerDispatchDataContext context)
        {
            var number = (from j in context.Quotes select j.Number).Max() ?? 0;

            var year = number / 1000;
            var count = number % 1000;

            if (number == 0 || year != (DateTime.Now.Year % 100))
                number = (DateTime.Now.Year % 100) * 1000 + 1;
            else
                number = (DateTime.Now.Year % 100) * 1000 + count + 1;

            return number;
        }
        */
    }
}
