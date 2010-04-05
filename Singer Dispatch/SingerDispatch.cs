using System;

namespace SingerDispatch
{
    partial class SingerDispatchDataContext
    {
        public SingerDispatchDataContext() : base(System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionParameters"].ConnectionString)
        {
        }
    }

    partial class Company
    {
        public int? ArchiveID { get; set; }

        partial void OnCreated()
        {
            IsVisible = IsVisible ?? true;
            EquifaxComplete = EquifaxComplete ?? false;
        }

        public string Alias
        {
            get
            {
                return OperatingAs ?? Name;
            }
        }
    }

    partial class Address
    {
        public int? ArchiveID { get; set; }
    }

    partial class Contact
    {
        public int? ArchiveID { get; set; }

        public string Name
        {
            get
            {
                return FirstName + " " + LastName;
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

    partial class Commodity
    {
        public string NameAndUnit
        {
            get
            {
                var name = Name;

                if (!string.IsNullOrEmpty(Unit))
                    name += " - " + Unit;
                
                return name;
            }
        }

        partial void OnCreated()
        {
            WeightEstimated = WeightEstimated ?? false;
            SizeEstimated = SizeEstimated ?? false;
        }
    }

    partial class Quote
    {
        partial void OnCreated()
        {
            Price = Price ?? 0.00m;
            IsItemizedBilling = IsItemizedBilling ?? false;
        }

        public string FriendlyName
        {
            get
            {
                return string.Format("{0} rev. {1}", Number, Revision);
            }
        }

        public string NumberAndRev
        {
            get
            {
                return String.Format("{0}-{1}", Number, Revision);
            }
        }

        public Quote Duplicate()
        {
            var copy = new Quote();
                        
            copy.Company = Company;
            copy.Number = Number;
            copy.CareOfCompany = CareOfCompany;
            copy.Description = Description;
            copy.StartDate = StartDate;
            copy.EndDate = EndDate;
            copy.CreationDate = CreationDate;
            copy.ExpirationDate = ExpirationDate;
            copy.Employee = Employee;
            copy.Price = Price;

            foreach (var commodity in QuoteCommodities)
            {
                copy.QuoteCommodities.Add(commodity.Duplicate());
            }

            foreach (var supplement in QuoteSupplements)
            {
                copy.QuoteSupplements.Add(supplement.Duplicate());
            }

            foreach (var item in QuoteStorageItems)
            {
                copy.QuoteStorageItems.Add(item.Duplicate());
            }

            foreach (var item in QuoteConditions)
            {
                copy.QuoteConditions.Add(item.Duplicate());
            }

            return copy;
        }

        public Job ToJob()
        {
            var job = new Job();

            // Fill the job properties with as many of the applicable quote properties as possible
            job.Quote = this;
            job.Company = Company;
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
            WeightEstimated = WeightEstimated ?? false;
            SizeEstimated = SizeEstimated ?? false;
        }

        public QuoteCommodity Duplicate()
        {
            var copy = new QuoteCommodity();

            copy.OriginalCommodity = OriginalCommodity;
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

            jc.OriginalCommodity = OriginalCommodity;
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

    partial class QuoteCondition
    {
        public QuoteCondition Duplicate()
        {
            var copy = new QuoteCondition();

            copy.OriginalCondition = OriginalCondition;
            copy.Line = Line;

            return copy;
        }
    }

    partial class QuoteSupplement
    {
        public QuoteSupplement Duplicate()
        {
            var copy = new QuoteSupplement();

            copy.Name = Name;
            copy.Details = Details;
            copy.BillingType = BillingType;
            copy.Quantity = Quantity;
            copy.CostPerItem = CostPerItem;

            return copy;
        }
    }

    partial class QuoteStorageItem
    {
        public QuoteStorageItem Duplicate()
        {
            var copy = new QuoteStorageItem();

            copy.Details = Details;
            copy.Commodity = Commodity;
            copy.BillingType = BillingType;
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

            cp.CareOfCompany = CareOfCompany;
            cp.Company = Company;
            cp.Description = Description;
            cp.Employee = Employee;
            cp.EndDate = cp.EndDate;
            cp.Quote = Quote;
            cp.StartDate = StartDate;
            cp.JobStatusType = JobStatusType;

            return cp;
        }
    }

    partial class JobCommodity
    {
        partial void OnCreated()
        {
            WeightEstimated = WeightEstimated ?? false;
            SizeEstimated = SizeEstimated ?? false;
        }

        public string NameAndUnit
        {
            get
            {
                var name = Name;

                if (!string.IsNullOrEmpty(Unit))
                    name += " - " + Unit;

                return name;
            }
        }

        public JobCommodity Duplicate()
        {
            var copy = new JobCommodity();

            copy.Load = Load;
            copy.OriginalCommodity = OriginalCommodity;
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
            copy.LoadMethod = LoadMethod;
            copy.LoadDate = LoadDate;
            copy.LoadInstructions = LoadInstructions;
            copy.LoadRoute = LoadRoute;
            copy.UnloadSiteName = UnloadSiteName;
            copy.UnloadAddress = UnloadAddress;
            copy.UnloadBy = UnloadBy;
            copy.UnloadMethod = UnloadMethod;
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

            copy.Load = Load;
            copy.Company = Company;
            copy.ServiceType = ServiceType;
            copy.Contact = Contact;
            copy.ServiceDate = ServiceDate;
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

            cp.Load = Load;
            cp.Issuer = Issuer;
            cp.PermitType = PermitType;
            cp.Conditions = Conditions;
            cp.Cost = Cost;
            cp.PermitDate = PermitDate;

            return cp;
        }
    }

    partial class Dispatch
    {
        public Dispatch Duplicate()
        {
            var cp = new Dispatch();

            cp.Load = Load;
            cp.Employee = Employee;
            cp.Equipment = Equipment;
            cp.Description = Description;
            cp.Notes = Notes;
            cp.Rate = Rate;

            return cp;
        }
        
        public string Name
        {
            get
            {
                string dispatchNumber;

                if (Load == null)
                    dispatchNumber = string.Format("{0}-{1:D2}", Job.Number, Number);
                else
                    dispatchNumber = string.Format("{0}-{1:D2}-{2:D2}", Job.Number, Load.Number, Number);

                return dispatchNumber;
            }
        }
    }

    partial class Load
    {
        partial void OnCreated()
        {
            WeightEstimated = WeightEstimated ?? true;
        }

        public void NotifyJobCommodities()
        {
            SendPropertyChanging();
            SendPropertyChanged("JobCommodities");
        }

        public Load Duplicate()
        {
            var cp = new Load();

            cp.Equipment = Equipment;
            cp.Rate = Rate;
            cp.Season = Season;
            cp.TrailerCombination = TrailerCombination;
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

        public string Name
        {
            get
            {
                var name = String.Format("{0:D2}", Number);
                
                return name;
            }
        }
    }

    partial class Condition
    {
        public override string ToString()
        {
            return Line;
        }
    }

    partial class Invoice
    {
        private double? _totalHours;
        private decimal? _totalCost;
        
        public double? TotalHours
        {
            get
            {
                return _totalHours;
            }
            set
            {
                SendPropertyChanging();
                _totalHours = value;
                SendPropertyChanged("TotalHours");
            }
        }

        public decimal? TotalCost
        {
            get
            {
                return _totalCost;
            }
            set
            {
                SendPropertyChanging();
                _totalCost = value;
                SendPropertyChanged("TotalCost");
            }
        }

        partial void OnCreated()
        {
            GSTExempt = GSTExempt ?? false;
        }

        public Invoice Duplicate()
        {
            var copy = new Invoice();

            copy.Number = Number;
            copy.Comment = Comment;
            copy.Contact = Contact;
            copy.HourlyRate = HourlyRate;
            copy.Hours = Hours;
            copy.GSTExempt = GSTExempt;
            copy.InvoiceDate = InvoiceDate;

            foreach (var item in InvoiceLineItems)
                copy.InvoiceLineItems.Add(item.Duplicate());

            foreach (var item in InvoiceExtras)
                copy.InvoiceExtras.Add(item.Duplicate());

            return copy;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", Number, Revision);
        }
    }
    partial class InvoiceLineItem
    {
        public InvoiceLineItem Duplicate()
        {
            var copy = new InvoiceLineItem();

            copy.Description = Description;
            copy.StartDate = StartDate;
            copy.EndDate = EndDate;
            copy.Departure = Departure;
            copy.Destination = Destination;
            copy.Hours = Hours;
            copy.Cost = Cost;

            return copy;
        }        
    }

    partial class InvoiceExtra
    {
        public InvoiceExtra Duplicate()
        {
            var copy = new InvoiceExtra();

            copy.Description = Description;
            copy.StartDate = StartDate;
            copy.EndDate = EndDate;
            copy.Departure = Departure;
            copy.Destination = Destination;
            copy.Hours = Hours;
            copy.Cost = Cost;

            return copy;
        }
    }
}
