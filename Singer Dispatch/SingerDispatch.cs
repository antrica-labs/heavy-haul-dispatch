namespace SingerDispatch
{
    partial class Company
    {
        partial void OnCreated()
        {
            if (IsVisible == null)
            {
                IsVisible = true;
            }

            if (EquifaxComplete == null)
            {                
                EquifaxComplete = false;
            }
        }

        public string Alias
        {
            get
            {
                return OperatingAs != null ? OperatingAs : Name;
            }
        }
    }

    partial class Employee
    {
        public string Name
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

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

    partial class Quote : System.ICloneable
    {
        public string FriendlyName
        {
            get
            {
                return string.Format("#{0} rev {1}", Number, Revision);
            }
        }

        public string NumberAndRev
        {
            get
            {
                return Number + " : " + Revision;
            }
        }

        public object Clone()
        {
            var copy = new Quote();

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

            foreach (var commodity in QuoteCommodities)
            {
                copy.QuoteCommodities.Add((QuoteCommodity)commodity.Clone());
            }

            foreach (var supplement in QuoteSupplements)
            {
                copy.QuoteSupplements.Add((QuoteSupplement)supplement.Clone());
            }

            foreach (var item in StorageItems)
            {
                copy.StorageItems.Add((StorageItem)item.Clone());
            }

            return copy;
        }

        public Job ToJob()
        {
            var job = new Job();

            // Fill the job properties with as many of the applicable quote properties as possible
            job.Quote = this;
            job.CompanyID = CompanyID;
            job.Company = Company;
            job.CareOfCompanyID = CareOfCompanyID;
            job.Company1 = Company1;
            job.StartDate = StartDate;
            job.EndDate = EndDate;
            job.Description = Description;

            foreach (var commodity in QuoteCommodities)
            {
                job.JobCommodities.Add(commodity.ToJobCommodity());
            }

            return job;
        }
    }

    partial class Commodity
    {
        partial void OnCreated()
        {
            if (WeightEstimated == null)
            {
                WeightEstimated = false;
            }

            if (SizeEstimated == null)
            {
                SizeEstimated = false;
            }
        }
    }

    partial class QuoteCommodity : System.ICloneable
    {
        partial void OnCreated()
        {
            if (WeightEstimated == null)
            {
                WeightEstimated = false;
            }

            if (SizeEstimated == null)
            {
                SizeEstimated = false;
            }
        }

        public object Clone()
        {
            var copy = new QuoteCommodity();

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

        public JobCommodity ToJobCommodity()
        {
            var jc = new JobCommodity();

            jc.OriginalCommodityID = OriginalCommodityID;
            jc.Name = Name;
            jc.Value = Value;
            jc.Serial = Serial;
            jc.Unit = Unit;
            jc.Owner = Owner;
            jc.LastLocation = LastLocation;
            jc.LastAddress = LastAddress;
            jc.Length = Length;
            jc.Width = Width;
            jc.Height = Height;
            jc.Weight = Weight;
            jc.SizeEstimated = SizeEstimated;
            jc.WeightEstimated = WeightEstimated;
            jc.Quantity = Quantity;
            jc.CostPerItem = CostPerItem;
            jc.Notes = Notes;
            jc.LoadAddress = DepartureAddress;
            jc.LoadSiteName = DepartureSiteName;
            jc.UnloadAddress = ArrivalAddress;
            jc.UnloadSiteName = ArrivalSiteName;

            return jc;
        }
    }

    partial class JobCommodity
    {
        partial void OnCreated()
        {
            if (WeightEstimated == null)
            {
                WeightEstimated = false;
            }

            if (SizeEstimated == null)
            {
                SizeEstimated = false;
            }
        }
    }

    partial class QuoteSupplement : System.ICloneable
    {
        public object Clone()
        {
            var copy = new QuoteSupplement();

            copy.QuoteID = QuoteID;
            copy.Name = Name;
            copy.Details = Details;
            copy.BillingTypeID = BillingTypeID;
            copy.Quantity = Quantity;
            copy.CostPerItem = CostPerItem;

            return copy;
        }
    }

    partial class StorageItem : System.ICloneable
    {
        public object Clone()
        {
            var copy = new StorageItem();

            copy.QuoteID = QuoteID;
            copy.Details = Details;
            copy.CommodityID = CommodityID;
            copy.BillingTypeID = BillingTypeID;
            copy.Quantity = Quantity;
            copy.CostPerItem = CostPerItem;

            return copy;
        }
    }

    partial class Rate
    {
        public decimal? Hourly { get; set; }
        public decimal? Adjusted { get; set; }
    }

    partial class Load
    {
        partial void  OnCreated()
        {
 	        if (WeightEstimated == null)
            {
                WeightEstimated = false;
            }
        }

        public string FriendlyName
        {
            get
            {
                if (Equipment != null)
                    return "Unit: " + Equipment.UnitNumber;
                else
                    return "Unit: (undefined)";
            }
        }
    }
}
