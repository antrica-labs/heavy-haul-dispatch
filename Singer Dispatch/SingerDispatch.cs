using System;
using System.Linq;

namespace SingerDispatch
{
    partial class QuoteCommodity
    {
    }

    partial class User
    {
        public string Name
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }

    partial class Contact
    {
        public string Name
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }

    partial class Quote : ICloneable
    {
        public string NumberAndRev
        {
            get
            {
                return Number + " : " + Revision;
            }
        }

        public object Clone()
        {
            Quote copy = new Quote();

            copy.CompanyID = CompanyID;
            copy.Company = Company;
            copy.Number = Number;
            copy.Revision = Revision;
            copy.CareOfCompanyID = CareOfCompanyID;
            copy.Company1 = Company1;
            copy.Description = Description;
            copy.CreationDate = CreationDate;
            copy.ExpirationDate = ExpirationDate;
            copy.QuotedByUserID = QuotedByUserID;
            copy.User = User;
            copy.Price = Price;

            foreach (QuoteCommodity commodity in QuoteCommodities)
            {
                copy.QuoteCommodities.Add((QuoteCommodity)commodity.Clone());
            }

            foreach (QuoteSupplement supplement in QuoteSupplements)
            {
                copy.QuoteSupplements.Add((QuoteSupplement)supplement.Clone());
            }

            return copy;
        }

        public Job ToJob()
        {
            Job job = new Job();

            // Fill the job properties with as many of the applicable quote properties as possible
            job.CompanyID = CompanyID;
            job.Company = Company;
            job.CareOfCompanyID = CareOfCompanyID;
            job.Company1 = Company1;
            job.StartDate = StartDate;
            job.EndDate = EndDate;
            job.Description = Description;
            
            return job;            
        }
    }

    partial class QuoteCommodity : ICloneable
    {
        public object Clone()
        {
            QuoteCommodity copy = new QuoteCommodity();

            copy.QuoteID = QuoteID;
            copy.OriginalCommodityID = OriginalCommodityID;
            copy.DepartureSiteName = DepartureSiteName;
            copy.DepartureAddress = DepartureAddress;
            copy.ArrivalSiteName = ArrivalSiteName;
            copy.ArrivalAddress = ArrivalAddress;
            copy.Name = Name;
            copy.Value = Value;
            copy.Serial = Serial;
            copy.Unit = Unit;
            copy.Owner = Owner;
            copy.LastLocation = LastLocation;
            copy.LastAddress = LastAddress;
            copy.Length = Length;
            copy.Width = Width;
            copy.Height = Height;
            copy.Weight = Weight;
            copy.SizeEstimated = SizeEstimated;
            copy.WeightEstimated = WeightEstimated;
            copy.Quantity = Quantity;
            copy.CostPerItem = CostPerItem;
            copy.Notes = Notes;
            
            return copy;
        }        
    }

    partial class QuoteSupplement : ICloneable
    {
        public object Clone()
        {
            QuoteSupplement copy = new QuoteSupplement();

            copy.QuoteID = QuoteID;
            copy.Name = Name;
            copy.Details = Details;
            copy.BillingTypeID = BillingTypeID;
            copy.Quantity = Quantity;
            copy.CostPerItem = CostPerItem;

            return copy;
        }
    }

}
