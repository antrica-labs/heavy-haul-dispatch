using System;
using System.Text;

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

            foreach (var item in QuoteNotes)
            {
                copy.QuoteNotes.Add(item.Duplicate());
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
            jc.LoadLocation = DepartureSiteName;
            jc.LoadAddress = DepartureAddress;
            jc.UnloadLocation = ArrivalSiteName;
            jc.UnloadAddress = ArrivalAddress;

            return jc;
        }
    }

    partial class Condition
    {
        partial void OnCreated()        
        {
            AutoInclude = AutoInclude ?? false;   
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

    partial class QuoteNote
    {
        public QuoteNote Duplicate()
        {           
            return new QuoteNote { Note = this.Note };
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

            cp.Name = Name;
            cp.CareOfCompany = CareOfCompany;
            cp.Company = Company;
            cp.Description = Description;
            cp.Employee = Employee;
            cp.EndDate = cp.EndDate;
            cp.Quote = Quote;
            cp.StartDate = StartDate;
            cp.EndDate = EndDate;
            cp.Status = Status;
            
            foreach (var item in JobCommodities)
                cp.JobCommodities.Add(item.Duplicate());

            foreach (var item in Loads)
                cp.Loads.Add(item.Duplicate());

            foreach (var item in Permits)
                cp.Permits.Add(item.Duplicate());

            foreach (var item in ThirdPartyServices)
                cp.ThirdPartyServices.Add(item.Duplicate());

            foreach (var item in Dispatches)
                cp.Dispatches.Add(item.Duplicate());

            foreach (var item in ReferenceNumbers)
                cp.ReferenceNumbers.Add(item.Duplicate());

            return cp;
        }
    }

    partial class JobReferenceNumber
    {
        public JobReferenceNumber Duplicate()
        {
            return new JobReferenceNumber { Field = Field, Value = Value };
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
            copy.LoadLocation = LoadLocation;
            copy.LoadBy = LoadBy;
            copy.LoadContact = copy.LoadContact;
            copy.LoadMethod = LoadMethod;
            copy.LoadDate = LoadDate;
            copy.LoadInstructions = LoadInstructions;
            copy.LoadRoute = LoadRoute;
            copy.LoadOrientation = LoadOrientation;
            copy.UnloadLocation = UnloadLocation;
            copy.UnloadBy = UnloadBy;
            copy.UnloadContact = UnloadContact;
            copy.UnloadMethod = UnloadMethod;
            copy.UnloadDate = UnloadDate;
            copy.UnloadInstructions = UnloadInstructions;
            copy.UnloadRoute = UnloadRoute;
            copy.LoadOrientation = LoadOrientation;
            copy.Notes = Notes;

            return copy;
        }
    }

    partial class ThirdPartyService
    {
        public ThirdPartyService Duplicate()
        {
            var copy = new ThirdPartyService();

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
                        
            cp.IssuingCompany = IssuingCompany;
            cp.PermitType = PermitType;
            cp.Reference = Reference;
            cp.Conditions = Conditions;
            cp.Cost = Cost;

            return cp;
        }
    }

    partial class Dispatch
    {
        public Dispatch Duplicate()
        {
            var cp = new Dispatch();

            cp.MeetingTime = cp.MeetingTime;            
            cp.Employee = Employee;
            cp.Equipment = Equipment;
            cp.Description = Description;
            cp.Schedule = Schedule;
            cp.DepartingUnits = DepartingUnits;
            cp.DepartingLocation = DepartingLocation;
            cp.Notes = Notes;
            //cp.Rate = Rate;

            foreach (var item in OutOfProvinceTravels)
                cp.OutOfProvinceTravels.Add(item.Duplicate());

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
        }

        public void Notify(string property)
        {
            SendPropertyChanging();
            SendPropertyChanged(property);
        }

        public Load Duplicate()
        {
            var cp = new Load();

            cp.Schedule = Schedule;
            cp.Equipment = Equipment;
            cp.Rate = Rate;
            cp.Season = Season;
            cp.TrailerCombination = TrailerCombination;
            cp.Info = Info;
            cp.StartDate = StartDate;
            cp.EndDate = EndDate;
            cp.Ban = Ban;            
            cp.Notes = Notes;
            cp.EWeightSteer = EWeightSteer;
            cp.EWeightDrive = EWeightDrive;
            cp.EWeightGroup1 = EWeightGroup1;
            cp.EWeightGroup2 = EWeightGroup2;
            cp.EWeightGroup3 = EWeightGroup3;
            cp.EWeightGroup4 = EWeightGroup4;
            cp.EWeightGroup5 = EWeightGroup5;
            cp.EWeightGroup6 = EWeightGroup6;
            cp.EWeightGroup7 = EWeightGroup7;
            cp.EWeightGroup8 = EWeightGroup8;
            cp.EWeightGroup9 = EWeightGroup9;
            cp.EWeightGroup10 = EWeightGroup10;
            cp.SWeightSteer = SWeightSteer;
            cp.SWeightDrive = SWeightDrive;
            cp.SWeightGroup1 = SWeightGroup1;
            cp.SWeightGroup2 = SWeightGroup2;
            cp.SWeightGroup3 = SWeightGroup3;
            cp.SWeightGroup4 = SWeightGroup4;
            cp.SWeightGroup5 = SWeightGroup5;
            cp.SWeightGroup6 = SWeightGroup6;
            cp.SWeightGroup7 = SWeightGroup7;
            cp.SWeightGroup8 = SWeightGroup8;
            cp.SWeightGroup9 = SWeightGroup9;
            cp.SWeightGroup10 = SWeightGroup10;            
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

        public static string PrintCommodityList(Load load)
        {
            var list = load.JobCommodities;
            var output = new StringBuilder();

            for (var i = 0; i < list.Count; i++)
            {
                output.Append(list[i].Name);

                if ((i + 1) != list.Count)
                    output.Append(", ");
            }

            return output.ToString();
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
            {
                var line = item.Duplicate();

                foreach (var extra in item.Extras)
                    line.Extras.Add(extra.Duplicate());

                copy.InvoiceLineItems.Add(line);
            }
            
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
            copy.Hours = Hours;
            copy.Cost = Cost;

            return copy;
        }
    }

    partial class OutOfProvinceTravel
    {
        public OutOfProvinceTravel Duplicate()
        {
            return new OutOfProvinceTravel { ProvinceOrState = ProvinceOrState, Distance = Distance };
        }
    }
}
