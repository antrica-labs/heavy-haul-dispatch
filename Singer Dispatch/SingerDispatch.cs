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

    partial class Quote
    {
        public string FriendlyName
        {
            get
            {
                return string.Format("{0} rev {1}", Number, Revision);
            }
        }

        public string NumberAndRev
        {
            get
            {
                return Number + " : " + Revision;
            }
        }

        public Quote Duplicate()
        {
            var copy = new Quote();

            copy.CompanyID = CompanyID;
            copy.Company = Company;
            copy.Number = Number;
            copy.Revision = Revision;
            copy.CareOfCompanyID = CareOfCompanyID;
            copy.CareOfCompany = CareOfCompany;
            copy.Description = Description;
            copy.CreationDate = CreationDate;
            copy.ExpirationDate = ExpirationDate;
            copy.EmployeeID = EmployeeID;
            copy.Employee = Employee;
            copy.Price = Price;

            foreach (var commodity in QuoteCommodities)
            {
                copy.QuoteCommodities.Add((QuoteCommodity)commodity.Duplicate());
            }

            foreach (var supplement in QuoteSupplements)
            {
                copy.QuoteSupplements.Add((QuoteSupplement)supplement.Duplicate());
            }

            foreach (var item in StorageItems)
            {
                copy.StorageItems.Add((StorageItem)item.Duplicate());
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
            job.CareOfCompany = CareOfCompany;
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

    partial class QuoteCommodity
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

        public QuoteCommodity Duplicate()
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
            copy.Cost = Cost;
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
            jc.Cost = Cost;
            jc.Notes = Notes;
            jc.LoadAddress = DepartureAddress;
            jc.LoadSiteName = DepartureSiteName;
            jc.UnloadAddress = ArrivalAddress;
            jc.UnloadSiteName = ArrivalSiteName;

            return jc;
        }
    }

    partial class QuoteSupplement
    {
        public QuoteSupplement Duplicate()
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

    partial class StorageItem 
    {
        public StorageItem Duplicate()
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

    partial class Job
    {
        public Job Duplicate()
        {
            var cp = new Job();

            cp.CareOfCompanyID = CareOfCompanyID;
            cp.CompanyID = CompanyID;
            cp.Description = Description;
            cp.EmployeeID = EmployeeID;
            cp.EndDate = cp.EndDate;
            cp.QuoteID = QuoteID;
            cp.StartDate = StartDate;
            cp.StatusTypeID = StatusTypeID;

            return null;
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

        public JobCommodity Duplicate()
        {
            var copy = new JobCommodity();

            copy.JobID = JobID;
            copy.LoadID = LoadID;
            copy.OriginalCommodityID = OriginalCommodityID;
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
            copy.Cost = Cost;
            copy.LoadSiteName = LoadSiteName;
            copy.LoadAddress = LoadAddress;
            copy.LoadBy = LoadBy;
            copy.LoadMethodID = LoadMethodID;
            copy.LoadDate = LoadDate;
            copy.LoadInstructions = LoadInstructions;
            copy.LoadRoute = LoadRoute;
            copy.UnloadSiteName = UnloadSiteName;
            copy.UnloadAddress = UnloadAddress;
            copy.UnloadBy = UnloadBy;
            copy.UnloadMethodID = UnloadMethodID;
            copy.UnloadDate = UnloadDate;
            copy.UnloadInstructions = UnloadInstructions;
            copy.UnloadRoute = UnloadRoute;
            copy.Notes = Notes;

            return copy;
        }
    }

    partial class ThirdPartyService
    {
        public ThirdPartyService Duplicate()
        {
            var copy = new ThirdPartyService();

            copy.JobID = JobID;
            copy.LoadID = LoadID;
            copy.CompanyID = CompanyID;
            copy.ServiceTypeID = ServiceTypeID;
            copy.ContactID = ContactID;
            copy.ServiceDate = ServiceDate;
            copy.ServiceTime = ServiceTime;
            copy.Location = Location;
            copy.Reference = Reference;
            copy.Notes = Notes;

            return copy;
        }
    }

    partial class Permit
    {
        public Permit Duplicate()
        {
            var cp = new Permit();
            
            cp.JobID = JobID;
            cp.LoadID = LoadID;
            cp.Issuer = Issuer;
            cp.PermitType = PermitType;            
            cp.Conditions = Conditions;
            cp.Cost = Cost;
            cp.PermitDate = PermitDate;
            cp.PermitTime = PermitTime;

            return cp;
        }
    }

    partial class Dispatch
    {
        public Dispatch Duplicate()
        {
            var cp = new Dispatch();

            cp.JobID = JobID;
            cp.LoadID = LoadID;
            cp.EmployeeID = EmployeeID;
            cp.EquipmentID = EquipmentID;
            cp.Description = Description;
            cp.Notes = Notes;
            cp.RateID = RateID;

            return cp;
        }
    }

    partial class Load
    {
        partial void OnCreated()
        {
 	        if (WeightEstimated == null)
            {
                WeightEstimated = false;
            }
        }

        public Load Duplicate()
        {
            var cp = new Load();
            
            cp.JobID = JobID;
            cp.EquipmentID = EquipmentID;
            cp.RateID = RateID;
            cp.SeasonID = SeasonID;
            cp.TrailerCombinationID = TrailerCombinationID;
            cp.Info = Info;
            cp.StartDate = StartDate;
            cp.EndDate = EndDate;
            cp.Ban = Ban;
            cp.ServiceDescription = ServiceDescription;
            cp.Notes = Notes;
            cp.WeightSteer = WeightSteer;
            cp.WeightDrive = WeightDrive;
            cp.WeightGroup1 = WeightGroup1;
            cp.WeightGroup2 = WeightGroup2;
            cp.WeightGroup3 = WeightGroup3;
            cp.WeightGroup4 = WeightGroup4;
            cp.WeightGroup5 = WeightGroup5;
            cp.WeightGroup6 = WeightGroup6;
            cp.WeightGroup7 = WeightGroup7;
            cp.WeightGroup8 = WeightGroup8;
            cp.WeightGroup9 = WeightGroup9;
            cp.WeightGroup10 = WeightGroup10;
            cp.WeightEstimated = WeightEstimated;
            cp.GrossWeight = GrossWeight;
            cp.LoadedLength = LoadedLength;
            cp.LoadedWidth = LoadedWidth;
            cp.LoadedHeight = LoadedHeight;

            return cp;
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
